using Elsa.Server.Api.Endpoints.FunctionDefinitions.Configs;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

                // Lấy đường dẫn đến thư mục của các assembly chuẩn
                var references = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                    .Select(a => MetadataReference.CreateFromFile(a.Location))
                    .ToList();

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
                    else
                    {
                        throw new Exception("Cannot find the specified DLL file.");
                    }
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

                //Delete the temporary files
                if (File.Exists(OutputPath) && File.Exists(pdbPath) && function.IsCompiled && !string.IsNullOrEmpty(SharedClassName))
                {
                    //Save dll and pdb file if the class is shared
                    if (!Directory.Exists("SharedClasses"))
                    {
                        Directory.CreateDirectory("SharedClasses");
                    }

                    string sharedDllPath = Path.Combine("SharedClasses", SharedClassName + ".dll");
                    string sharedPdbPath = Path.Combine("SharedClasses", SharedClassName + ".pdb");

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
            catch (Exception)
            {
                throw;
            }
        }

        public static void ReloadNeededDllFiles()
        {
            FunctionDefinitionConfigs.NeedDllFiles = new List<string>()
            {
                "System.Runtime.dll",
                "System.Threading.Tasks.dll",
                "System.Linq.dll",
                "System.Collections.dll",
                "System.IO.dll"
            };

            if (!Directory.Exists("SharedClasses"))
            {
                return;
            }

            foreach (var item in Directory.GetFiles(Path.GetFullPath("SharedClasses")))
            {
                FunctionDefinitionConfigs.NeedDllFiles.Add(item);
            }
        }
    }
}
