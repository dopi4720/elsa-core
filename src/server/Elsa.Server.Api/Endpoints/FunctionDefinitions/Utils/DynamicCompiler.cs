using Elsa.Server.Api.Endpoints.FunctionDefinitions.Configs;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions.Utils
{
    public class DynamicCompiler
    {
        public static CompiledFunction Compile(string sourceCode, string SharedClassName = "")
        {
            try
            {
                CompiledFunction function = new CompiledFunction();
                string OutputPath = Path.Combine(Path.GetTempPath(), string.IsNullOrEmpty(SharedClassName) ? Path.GetRandomFileName() : SharedClassName + ".dll");
                string pdbPath = Path.ChangeExtension(OutputPath, ".pdb");

                // Tạo một cây cú pháp (syntax tree) từ source code
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

                // Lấy danh sách các lớp trong mã nguồn
                var root = syntaxTree.GetRoot();

                #region Find class name

                var classDeclarations = root.DescendantNodes()
                                            .OfType<ClassDeclarationSyntax>();
                if (!classDeclarations.Any())
                {
                    function.CompileMessage = "No class found in the source code.";
                    return function;
                }

                // Kiểm tra và lấy tên lớp đầu tiên (nếu có)
                string className = classDeclarations.FirstOrDefault()?.Identifier.Text ?? throw new Exception("Not found class name");

                function.ClassName = className;
                #endregion

                #region Check Execute function is exist
                bool IsExecuteFunctionExist = root.DescendantNodes()
                         .OfType<MethodDeclarationSyntax>()
                         .Any(method =>
                             method.Identifier.Text == "Execute" && // Tên phương thức là "Execute"
                             method.Modifiers.Any(SyntaxKind.PublicKeyword) && // Có từ khóa "public"
                             method.Modifiers.Any(SyntaxKind.StaticKeyword) && // Có từ khóa "static"
                             method.Modifiers.Any(SyntaxKind.AsyncKeyword) && // Có từ khóa "async"
                             method.ReturnType is GenericNameSyntax genericName &&
                             genericName.Identifier.Text == "ValueTask" && // Kiểu trả về là ValueTask
                             genericName.TypeArgumentList.Arguments.FirstOrDefault() is NullableTypeSyntax nullableType &&
                             nullableType.ElementType.ToString() == "object" // Giá trị bên trong là "object?"
                         );
                if (!IsExecuteFunctionExist)
                {
                    throw new Exception("Not found Execute Function");
                }
                #endregion

                // Lấy danh sách các assembly từ DependencyContext
                var references = new List<MetadataReference>();

                foreach (var library in DependencyContext.Default.RuntimeLibraries)
                {
                    foreach (var runtimeGroup in library.RuntimeAssemblyGroups)
                    {
                        foreach (var assemblyPath in runtimeGroup.AssetPaths)
                        {
                            // Xây dựng đường dẫn đầy đủ đến assembly
                            var fullPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), assemblyPath);

                            if (File.Exists(fullPath))
                            {
                                references.Add(MetadataReference.CreateFromFile(fullPath));
                            }
                        }
                    }
                }

                // Thêm các tham chiếu từ danh sách đường dẫn DLL được truyền vào
                foreach (var dllPath in FunctionDefinitionConfigs.NeedDllFiles)
                {
                    string RuntimePath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), dllPath);
                    if (File.Exists(dllPath))
                    {
                        references.Add(MetadataReference.CreateFromFile(dllPath));
                    }
                    else if (File.Exists(RuntimePath))
                    {
                        references.Add(MetadataReference.CreateFromFile(RuntimePath));
                    }
                    //else
                    //{
                    //    throw new Exception($"Cannot find the specified DLL file: {dllPath}");
                    //}
                }

                // Tạo một CSharpCompilation từ syntax tree
                CSharpCompilation compilation = CSharpCompilation.Create(
                    Path.GetFileNameWithoutExtension(OutputPath),
                    new[] { syntaxTree },
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                // Thiết lập các tùy chọn để tạo ra file binary và file PDB
                using (var dllStream = new FileStream(OutputPath, FileMode.Create))
                using (var pdbStream = new FileStream(pdbPath, FileMode.Create))
                {
                    EmitResult result = compilation.Emit(dllStream, pdbStream);

                    // Kiểm tra xem việc biên dịch có thành công hay không
                    if (!result.Success)
                    {
                        // Nếu có lỗi biên dịch, xuất lỗi
                        foreach (Diagnostic diagnostic in result.Diagnostics)
                        {
                            if (diagnostic.Severity == DiagnosticSeverity.Error)
                            {
                                function.CompileMessage += diagnostic.GetMessage() + Environment.NewLine;
                            }
                        }
                        function.IsCompiled = false;
                        return function;
                    }
                }

                function.IsCompiled = true;
                function.DllBytes = File.ReadAllBytes(OutputPath);
                function.PdbBytes = File.ReadAllBytes(pdbPath);

                // Tải assembly bằng AssemblyLoadContext
                //var assemblyLoadContext = new CustomAssemblyLoadContext();
                //using (var dllStream = new MemoryStream(function.DllBytes))
                //using (var pdbStream = new MemoryStream(function.PdbBytes))
                //{
                //    var assembly = assemblyLoadContext.LoadFromStream(dllStream, pdbStream);
                //    function.LoadedAssembly = assembly;
                //}

                // Delete temporary files nếu không phải class dùng chung
                if (File.Exists(OutputPath) && File.Exists(pdbPath) && function.IsCompiled && !string.IsNullOrEmpty(SharedClassName))
                {
                    string sharedDllPath = Path.Combine("SharedClasses", SharedClassName + ".dll");
                    string sharedPdbPath = Path.Combine("SharedClasses", SharedClassName + ".pdb");

                    if (!Directory.Exists("SharedClasses"))
                    {
                        Directory.CreateDirectory("SharedClasses");
                    }

                    File.Move(OutputPath, sharedDllPath, true);
                    File.Move(pdbPath, sharedPdbPath, true);
                    ReloadNeededDllFiles();
                }
                else
                {
                    File.Delete(OutputPath);
                    File.Delete(pdbPath);
                }

                return function;
            }
            catch (Exception ex)
            {
                throw new Exception($"Compilation failed: {ex.Message}", ex);
            }
            finally
            {
                GC.Collect();
            }
        }

        public static void ReloadNeededDllFiles(List<string>? CoreDllPath = null)
        {
            FunctionDefinitionConfigs.NeedDllFiles.Clear();

            var tmp = new List<string>()
            {
                "System.Runtime.dll",
                "System.Threading.Tasks.dll",
                "System.Linq.dll",
                "System.Collections.dll",
                "System.IO.dll",
                "DrpSystem.dll",
                "Newtonsoft.Json.dll",
                "Microsoft.EntityFrameworkCore.dll",
            };

            foreach (var dllPath in tmp)
            {
                string RuntimePath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), dllPath);
                if (File.Exists(Path.GetFullPath(dllPath)))
                {
                    FunctionDefinitionConfigs.NeedDllFiles.Add(dllPath);
                }
                else if (File.Exists(RuntimePath))
                {
                    FunctionDefinitionConfigs.NeedDllFiles.Add(RuntimePath);
                }
            }

            if (!Directory.Exists("SharedClasses"))
            {
                Directory.CreateDirectory("SharedClasses");
            }

            foreach (var item in Directory.GetFiles(Path.GetFullPath("SharedClasses")))
            {
                FunctionDefinitionConfigs.NeedDllFiles.Add(item);
            }

            if (CoreDllPath == null || CoreDllPath.Count == 0)
            {
                return;
            }

            foreach (var item in CoreDllPath.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                FunctionDefinitionConfigs.NeedDllFiles.Add(item);
            }
        }
        private class CustomAssemblyLoadContext : AssemblyLoadContext
        {
            public CustomAssemblyLoadContext() : base(isCollectible: true) { }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                // Tìm và tải assembly phụ thuộc
                string dependencyPath = Path.Combine("SharedClasses", $"{assemblyName.Name}.dll");
                if (File.Exists(dependencyPath))
                {
                    return LoadFromAssemblyPath(dependencyPath);
                }
                return null; // Không tải nếu không tìm thấy
            }
        }
    }
}
