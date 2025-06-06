using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Text.Json;

namespace EmployeeManagement.Utilities
{
    public static class FaceRecognitionService
    {
        private static readonly string PythonExecutable = ConfigurationManager.AppSettings["PythonExecutable"] ?? "python";
        private static readonly string ScriptPath = ConfigurationManager.AppSettings["FaceRecognitionScriptPath"] ?? @"Scripts\face_recognition_service.py";
        private static readonly int TimeoutSeconds = int.Parse(ConfigurationManager.AppSettings["FaceRecognitionTimeout"] ?? "30");

        public static async Task<FaceRecognitionServiceResult> RegisterFaceAsync(string employeeCode, string employeeName, string imagePath)
        {
            try
            {
                // Validate input parameters
                if (string.IsNullOrEmpty(employeeCode))
                    return new FaceRecognitionServiceResult { Success = false, Message = "Mã nhân viên không được để trống" };

                if (string.IsNullOrEmpty(employeeName))
                    return new FaceRecognitionServiceResult { Success = false, Message = "Tên nhân viên không được để trống" };

                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                    return new FaceRecognitionServiceResult { Success = false, Message = "File ảnh không tồn tại" };

                // Check if script exists
                if (!File.Exists(ScriptPath))
                {
                    return new FaceRecognitionServiceResult
                    {
                        Success = false,
                        Message = $"Không tìm thấy script Python: {ScriptPath}\nVui lòng kiểm tra đường dẫn trong App.config"
                    };
                }

                // Build arguments
                string arguments = $"\"{ScriptPath}\" register --employee_id \"{employeeCode}\" --employee_name \"{employeeName}\" --image_path \"{imagePath}\"";

                var result = await ExecutePythonScriptAsync(arguments, TimeoutSeconds * 3); // Triple timeout for registration

                if (result.Success && !string.IsNullOrEmpty(result.Output))
                {
                    try
                    {
                        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(result.Output);

                        if (jsonResult.ContainsKey("face_path") && jsonResult["face_path"].ValueKind == JsonValueKind.String)
                        {
                            result.FacePath = jsonResult["face_path"].GetString() ?? "";
                        }

                        // Update success status from JSON
                        if (jsonResult.ContainsKey("success"))
                        {
                            result.Success = jsonResult["success"].GetBoolean();
                        }

                        // Update message from JSON
                        if (jsonResult.ContainsKey("message") && jsonResult["message"].ValueKind == JsonValueKind.String)
                        {
                            result.Message = jsonResult["message"].GetString() ?? result.Message;
                        }
                    }
                    catch (JsonException ex)
                    {
                        result.Success = false;
                        result.Message = $"Lỗi phân tích JSON: {ex.Message}";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new FaceRecognitionServiceResult
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public static async Task<FaceRecognitionResult> RecognizeFromCameraAsync(int timeoutSeconds = 30)
        {
            try
            {
                if (!File.Exists(ScriptPath))
                {
                    return new FaceRecognitionResult
                    {
                        Success = false,
                        Message = $"Không tìm thấy script Python: {ScriptPath}\nVui lòng kiểm tra đường dẫn trong App.config"
                    };
                }

                string arguments = $"\"{ScriptPath}\" recognize_camera --timeout {timeoutSeconds}";
                var result = await ExecutePythonScriptAsync(arguments, timeoutSeconds + 15);

                if (result.Success && !string.IsNullOrEmpty(result.Output))
                {
                    try
                    {
                        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(result.Output);

                        var recognitionResult = new FaceRecognitionResult();

                        // Check success
                        if (jsonResult.ContainsKey("success"))
                        {
                            recognitionResult.Success = jsonResult["success"].GetBoolean();
                        }

                        if (!recognitionResult.Success)
                        {
                            recognitionResult.Message = jsonResult.ContainsKey("message") && jsonResult["message"].ValueKind == JsonValueKind.String
                                ? jsonResult["message"].GetString() ?? "Nhận diện thất bại"
                                : "Nhận diện thất bại";
                            return recognitionResult;
                        }

                        // Parse successful recognition data
                        if (jsonResult.ContainsKey("employee_id") && jsonResult["employee_id"].ValueKind == JsonValueKind.String)
                            recognitionResult.EmployeeId = jsonResult["employee_id"].GetString() ?? "";

                        if (jsonResult.ContainsKey("employee_name") && jsonResult["employee_name"].ValueKind == JsonValueKind.String)
                            recognitionResult.EmployeeName = jsonResult["employee_name"].GetString() ?? "";

                        if (jsonResult.ContainsKey("confidence") && jsonResult["confidence"].ValueKind == JsonValueKind.Number)
                            recognitionResult.Confidence = (decimal)jsonResult["confidence"].GetDouble();

                        if (jsonResult.ContainsKey("timestamp") && jsonResult["timestamp"].ValueKind == JsonValueKind.String)
                        {
                            if (DateTime.TryParse(jsonResult["timestamp"].GetString(), out var timestamp))
                                recognitionResult.Timestamp = timestamp;
                            else
                                recognitionResult.Timestamp = DateTime.Now;
                        }
                        else
                        {
                            recognitionResult.Timestamp = DateTime.Now;
                        }

                        if (jsonResult.ContainsKey("attendance_image") && jsonResult["attendance_image"].ValueKind == JsonValueKind.String)
                            recognitionResult.AttendanceImagePath = jsonResult["attendance_image"].GetString();

                        return recognitionResult;
                    }
                    catch (JsonException ex)
                    {
                        return new FaceRecognitionResult
                        {
                            Success = false,
                            Message = $"Lỗi phân tích kết quả JSON: {ex.Message}\nOutput: {result.Output}"
                        };
                    }
                }

                return new FaceRecognitionResult
                {
                    Success = false,
                    Message = string.IsNullOrEmpty(result.Message) ? "Không nhận diện được khuôn mặt" : result.Message
                };
            }
            catch (Exception ex)
            {
                return new FaceRecognitionResult
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public static async Task<FaceRecognitionServiceResult> GetRegisteredFacesAsync()
        {
            try
            {
                if (!File.Exists(ScriptPath))
                {
                    return new FaceRecognitionServiceResult
                    {
                        Success = false,
                        Message = $"Không tìm thấy script Python: {ScriptPath}"
                    };
                }

                string arguments = $"\"{ScriptPath}\" list";
                var result = await ExecutePythonScriptAsync(arguments, TimeoutSeconds);

                if (result.Success && !string.IsNullOrEmpty(result.Output))
                {
                    try
                    {
                        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(result.Output);

                        var faces = new List<RegisteredFace>();

                        if (jsonResult.ContainsKey("success") && jsonResult["success"].GetBoolean() &&
                            jsonResult.ContainsKey("faces") && jsonResult["faces"].ValueKind == JsonValueKind.Array)
                        {
                            foreach (var faceElement in jsonResult["faces"].EnumerateArray())
                            {
                                if (faceElement.ValueKind == JsonValueKind.Object)
                                {
                                    var faceObj = faceElement.Deserialize<Dictionary<string, JsonElement>>();
                                    if (faceObj != null)
                                    {
                                        var face = new RegisteredFace();

                                        if (faceObj.ContainsKey("employee_id") && faceObj["employee_id"].ValueKind == JsonValueKind.String)
                                            face.EmployeeId = faceObj["employee_id"].GetString() ?? "";

                                        if (faceObj.ContainsKey("employee_name") && faceObj["employee_name"].ValueKind == JsonValueKind.String)
                                            face.EmployeeName = faceObj["employee_name"].GetString() ?? "";

                                        faces.Add(face);
                                    }
                                }
                            }
                        }

                        result.Faces = faces;
                        result.Success = jsonResult.ContainsKey("success") ? jsonResult["success"].GetBoolean() : false;

                        if (jsonResult.ContainsKey("message") && jsonResult["message"].ValueKind == JsonValueKind.String)
                            result.Message = jsonResult["message"].GetString() ?? "";
                    }
                    catch (JsonException ex)
                    {
                        return new FaceRecognitionServiceResult
                        {
                            Success = false,
                            Message = $"Lỗi phân tích danh sách JSON: {ex.Message}"
                        };
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new FaceRecognitionServiceResult
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public static async Task<FaceRecognitionServiceResult> DeleteRegisteredFaceAsync(string employeeId)
        {
            try
            {
                if (string.IsNullOrEmpty(employeeId))
                    return new FaceRecognitionServiceResult { Success = false, Message = "Mã nhân viên không được để trống" };

                if (!File.Exists(ScriptPath))
                {
                    return new FaceRecognitionServiceResult
                    {
                        Success = false,
                        Message = $"Không tìm thấy script Python: {ScriptPath}"
                    };
                }

                string arguments = $"\"{ScriptPath}\" delete --employee_id \"{employeeId}\"";
                var result = await ExecutePythonScriptAsync(arguments, TimeoutSeconds);

                if (result.Success && !string.IsNullOrEmpty(result.Output))
                {
                    try
                    {
                        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(result.Output);

                        if (jsonResult.ContainsKey("success"))
                            result.Success = jsonResult["success"].GetBoolean();

                        if (jsonResult.ContainsKey("message") && jsonResult["message"].ValueKind == JsonValueKind.String)
                            result.Message = jsonResult["message"].GetString() ?? result.Message;
                    }
                    catch (JsonException ex)
                    {
                        result.Success = false;
                        result.Message = $"Lỗi phân tích JSON: {ex.Message}";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new FaceRecognitionServiceResult
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public static SystemReadinessResult CheckSystemReadiness()
        {
            try
            {
                // Check Python executable
                if (!IsPythonAvailable())
                {
                    return new SystemReadinessResult
                    {
                        IsReady = false,
                        ErrorMessage = "Python không được cài đặt hoặc không trong PATH.\n" +
                                     "Vui lòng cài đặt Python 3.8+ và các thư viện cần thiết:\n" +
                                     "pip install opencv-python face-recognition numpy"
                    };
                }

                // Check script file
                if (!File.Exists(ScriptPath))
                {
                    return new SystemReadinessResult
                    {
                        IsReady = false,
                        ErrorMessage = $"Không tìm thấy script Python: {ScriptPath}\n" +
                                     "Vui lòng đảm bảo file face_recognition_service.py tồn tại trong thư mục Scripts"
                    };
                }

                // Check and create required folders
                string facesFolder = ConfigurationManager.AppSettings["FacesFolder"] ?? "faces";
                if (!Directory.Exists(facesFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(facesFolder);
                    }
                    catch (Exception ex)
                    {
                        return new SystemReadinessResult
                        {
                            IsReady = false,
                            ErrorMessage = $"Không thể tạo thư mục faces: {ex.Message}"
                        };
                    }
                }

                string attendanceFolder = ConfigurationManager.AppSettings["AttendanceImagesFolder"] ?? "attendance_images";
                if (!Directory.Exists(attendanceFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(attendanceFolder);
                    }
                    catch (Exception ex)
                    {
                        return new SystemReadinessResult
                        {
                            IsReady = false,
                            ErrorMessage = $"Không thể tạo thư mục attendance_images: {ex.Message}"
                        };
                    }
                }

                // Try to test Python script
                try
                {
                    var testTask = ExecutePythonScriptAsync($"\"{ScriptPath}\" list", 10);
                    var testResult = testTask.GetAwaiter().GetResult();

                    if (!testResult.Success && testResult.Message.Contains("ModuleNotFoundError"))
                    {
                        return new SystemReadinessResult
                        {
                            IsReady = false,
                            ErrorMessage = "Thiếu thư viện Python cần thiết.\n" +
                                         "Vui lòng cài đặt:\n" +
                                         "pip install opencv-python face-recognition numpy Pillow"
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new SystemReadinessResult
                    {
                        IsReady = false,
                        ErrorMessage = $"Lỗi kiểm tra script Python: {ex.Message}"
                    };
                }

                return new SystemReadinessResult
                {
                    IsReady = true,
                    Message = "Hệ thống Face Recognition sẵn sàng"
                };
            }
            catch (Exception ex)
            {
                return new SystemReadinessResult
                {
                    IsReady = false,
                    ErrorMessage = $"Lỗi kiểm tra hệ thống: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Test Python script health
        /// </summary>
        public static async Task<FaceRecognitionServiceResult> TestSystemHealthAsync()
        {
            try
            {
                if (!File.Exists(ScriptPath))
                {
                    return new FaceRecognitionServiceResult
                    {
                        Success = false,
                        Message = $"Không tìm thấy script Python: {ScriptPath}"
                    };
                }

                string arguments = $"\"{ScriptPath}\" health";
                var result = await ExecutePythonScriptAsync(arguments, 15);

                if (result.Success && !string.IsNullOrEmpty(result.Output))
                {
                    try
                    {
                        var jsonResult = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(result.Output);

                        if (jsonResult.ContainsKey("success"))
                            result.Success = jsonResult["success"].GetBoolean();

                        if (jsonResult.ContainsKey("message") && jsonResult["message"].ValueKind == JsonValueKind.String)
                            result.Message = jsonResult["message"].GetString() ?? result.Message;

                        // Add health info to output for display
                        if (jsonResult.ContainsKey("health_info"))
                        {
                            result.Output = JsonSerializer.Serialize(jsonResult["health_info"], new JsonSerializerOptions
                            {
                                WriteIndented = true
                            });
                        }
                    }
                    catch (JsonException ex)
                    {
                        result.Success = false;
                        result.Message = $"Lỗi phân tích health check JSON: {ex.Message}";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new FaceRecognitionServiceResult
                {
                    Success = false,
                    Message = $"Lỗi health check: {ex.Message}"
                };
            }
        }

        private static bool IsPythonAvailable()
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = PythonExecutable;
                    process.StartInfo.Arguments = "--version";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    bool exited = process.WaitForExit(5000); // 5 second timeout

                    if (!exited)
                    {
                        try { process.Kill(); } catch { }
                        return false;
                    }

                    return process.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private static async Task<FaceRecognitionServiceResult> ExecutePythonScriptAsync(string arguments, int timeoutSeconds)
        {
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                using var process = new Process();

                process.StartInfo.FileName = PythonExecutable;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;

                // Set environment variables for proper encoding
                process.StartInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
                process.StartInfo.EnvironmentVariables["PYTHONUNBUFFERED"] = "1";

                var outputBuilder = new System.Text.StringBuilder();
                var errorBuilder = new System.Text.StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for process with cancellation
                var processTask = Task.Run(() =>
                {
                    process.WaitForExit();
                    return process.ExitCode;
                }, cancellationTokenSource.Token);

                try
                {
                    var exitCode = await processTask;

                    string output = outputBuilder.ToString().Trim();
                    string error = errorBuilder.ToString().Trim();

                    // Log for debugging
                    System.Diagnostics.Debug.WriteLine($"Python Exit Code: {exitCode}");
                    System.Diagnostics.Debug.WriteLine($"Python Output: {output}");
                    if (!string.IsNullOrEmpty(error))
                        System.Diagnostics.Debug.WriteLine($"Python Error: {error}");

                    if (exitCode == 0 && !string.IsNullOrEmpty(output))
                    {
                        // Try to validate JSON
                        try
                        {
                            var testParse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(output);

                            return new FaceRecognitionServiceResult
                            {
                                Success = testParse.ContainsKey("success") ? testParse["success"].GetBoolean() : false,
                                Message = testParse.ContainsKey("message") && testParse["message"].ValueKind == JsonValueKind.String
                                    ? testParse["message"].GetString() ?? ""
                                    : "",
                                Output = output
                            };
                        }
                        catch (JsonException)
                        {
                            return new FaceRecognitionServiceResult
                            {
                                Success = false,
                                Message = $"Kết quả không phải JSON hợp lệ: {output}"
                            };
                        }
                    }
                    else
                    {
                        string errorMessage = !string.IsNullOrEmpty(error)
                            ? error
                            : !string.IsNullOrEmpty(output)
                                ? output
                                : $"Process exited with code {exitCode}";

                        return new FaceRecognitionServiceResult
                        {
                            Success = false,
                            Message = errorMessage
                        };
                    }
                }
                catch (OperationCanceledException)
                {
                    try
                    {
                        if (!process.HasExited)
                            process.Kill();
                    }
                    catch { }

                    return new FaceRecognitionServiceResult
                    {
                        Success = false,
                        Message = $"Quá thời gian chờ ({timeoutSeconds}s)"
                    };
                }
            }
            catch (Exception ex)
            {
                return new FaceRecognitionServiceResult
                {
                    Success = false,
                    Message = $"Lỗi thực thi Python: {ex.Message}"
                };
            }
        }
    }

    // Supporting classes
    public class SystemReadinessResult
    {
        public bool IsReady { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class FaceRecognitionServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Output { get; set; } = string.Empty;
        public string FacePath { get; set; } = string.Empty;
        public List<RegisteredFace> Faces { get; set; } = new List<RegisteredFace>();
    }

    public class FaceRecognitionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public DateTime Timestamp { get; set; }
        public string AttendanceImagePath { get; set; } = string.Empty;
    }

    public class RegisteredFace
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
    }

    public class AttendanceCreateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string AttendanceType { get; set; } = string.Empty;
    }
}