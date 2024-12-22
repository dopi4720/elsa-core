using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Elsa.Server.Api.Endpoints.FunctionDefinitions.Configs;

public class DynamicRunner
{
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("Dynamic assembly loading is not compatible with trimming.")]
    public static async Task<object?> RunAsync(byte[] dllBytes, byte[] pdbBytes, string className, Dictionary<string, object?> parameterDictionary = null)
    {
        // Tải assembly từ byte[]
        Assembly assembly = Assembly.Load(dllBytes, pdbBytes);

        // Thiết lập xử lý sự kiện để tải các assembly phụ thuộc
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            // Lấy tên assembly đang yêu cầu
            var assemblyName = new AssemblyName(args.Name).Name;

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
        };

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
                        parameterValues[i] = Convert.ChangeType(value, parameterInfos[i].ParameterType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Không thể chuyển đổi tham số '{paramName}' thành kiểu '{parameterInfos[i].ParameterType.Name}': {ex.Message}");
                    }
                }
                else if (parameterInfos[i].Name == "drpContext")
                {
                    string DrpContextPath = Path.Combine("DrpSystem.dll");
                    if (File.Exists(DrpContextPath))
                    {
                        Assembly depAssembly = Assembly.LoadFrom(DrpContextPath);
                        foreach (var depType in depAssembly.GetTypes())
                        {
                            if (depType.Name == "DrpContext")
                            {
                                parameterValues[i] = Activator.CreateInstance(depType);
                                break;
                            }
                        }
                    }
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
        return await result;
    }
}
