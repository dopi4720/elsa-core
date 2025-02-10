using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Xml.Linq;

public class DynamicRunner
{
    static Assembly? ResolvingHandler(AssemblyLoadContext context, AssemblyName name)
    {
        // Lấy tên assembly đang yêu cầu
        var assemblyName = name.Name;

        // Kiểm tra nếu assembly đã được tải
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (asm.GetName().Name == assemblyName)
            {
                return asm;
            }
        }

        // Tìm và tải assembly từ thư mục chứa các thư viện phụ thuộc (nếu có)
        string dependencyPath = Path.Combine("SharedClasses", $"{assemblyName}.dll");
        if (File.Exists(dependencyPath))
        {
            return Assembly.LoadFrom(dependencyPath);
        }
        return null;
    }
    public static async Task<object?> RunAsync(byte[] dllBytes, byte[] pdbBytes, string className, Dictionary<string, object?> parameterDictionary = null)
    {
        // Tạo AssemblyLoadContext riêng để tải assembly
        var loadContext = new CustomAssemblyLoadContext();
        loadContext.Resolving += ResolvingHandler;

        try
        {
            // Tải assembly chính và pdb vào AssemblyLoadContext
            using var dllStream = new MemoryStream(dllBytes);
            using var pdbStream = pdbBytes != null ? new MemoryStream(pdbBytes) : null;
            var assembly = loadContext.LoadFromStream(dllStream, pdbStream);

            // Tìm class và phương thức "Execute" trong assembly
            Type type = assembly.GetType(className) ?? throw new Exception($"Class '{className}' không tìm thấy trong assembly.");
            MethodInfo method = type.GetMethod("Execute") ?? throw new Exception($"Method 'Execute' không tìm thấy trong class '{className}'.");

            // Kiểm tra nếu phương thức là async và trả về ValueTask
            if (!typeof(ValueTask<object?>).IsAssignableFrom(method.ReturnType))
            {
                throw new Exception("Method 'Execute' phải là async và trả về ValueTask.");
            }

            // Tạo danh sách các tham số để truyền vào phương thức
            var parameterInfos = method.GetParameters();
            var parameterValues = new object?[parameterInfos.Length];

            if (parameterDictionary != null)
            {
                // Khớp tham số dựa trên tên
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    string paramName = parameterInfos[i].Name ?? throw new Exception($"Name of parameter is null ({JsonConvert.SerializeObject(parameterInfos[i], new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })})");
                    if (parameterDictionary.TryGetValue(paramName, out var value))
                    {
                        // Ép kiểu giá trị thành kiểu phù hợp

                        try
                        {
                            // Kiểm tra nếu kiểu là dynamic hoặc object
                            if (parameterInfos[i].ParameterType == typeof(object))
                            {
                                parameterValues[i] = value;
                            }
                            else
                            {
                                parameterValues[i] = Convert.ChangeType(value, parameterInfos[i].ParameterType);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Không thể chuyển đổi tham số '{paramName}' thành kiểu '{parameterInfos[i].ParameterType.Name}': {ex.Message}");
                        }
                    }
                    else if (paramName == "drpContext")
                    {
                        // Xử lý đặc biệt cho tham số "drpContext"
                        string drpContextPath = Path.GetFullPath("DrpSystem.dll");
                        if (File.Exists(drpContextPath))
                        {
                            var depAssembly = Assembly.LoadFrom(drpContextPath);
                            var drpType = depAssembly.GetType("DrpSystem.Models.DrpContext");
                            parameterValues[i] = drpType != null ? Activator.CreateInstance(drpType) : null;

                        }

                        //string DrpContextPath = Path.Combine("DrpSystem.dll");
                        //if (File.Exists(DrpContextPath))
                        //{
                        //    Assembly depAssembly = Assembly.LoadFrom(DrpContextPath);
                        //    foreach (var depType in depAssembly.GetTypes())
                        //    {
                        //        if (depType.Name == "DrpContext")
                        //        {
                        //            parameterValues[i] = Activator.CreateInstance(depType);
                        //            break;
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        // Gán giá trị mặc định dựa trên kiểu tham số
                        Type paramType = parameterInfos[i].ParameterType;
                        if (paramType == typeof(string))
                        {
                            parameterValues[i] = string.Empty;
                        }
                        else if (paramType.IsValueType) // Kiểm tra nếu là kiểu giá trị
                        {
                            parameterValues[i] = Activator.CreateInstance(paramType); // Giá trị mặc định cho kiểu giá trị (0 cho số, false cho bool, etc.)
                        }
                        else
                        {
                            parameterValues[i] = null; // Giá trị mặc định cho kiểu tham chiếu
                        }
                    }
                }
            }

            // Gọi method async
            var result = (ValueTask<object?>)method.Invoke(null, parameterValues)!;

            // Chờ kết quả
            var resultValue = await result;

            GC.Collect();

            return resultValue;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            _ = Task.Run(() =>
            {
                loadContext.Resolving -= ResolvingHandler;
                // Giải phóng AssemblyLoadContext sau khi hoàn thành
                loadContext.Unload();
                GC.Collect();
            });
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
