using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

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
                    return new FaceRecognitionServiceResult { Success = false, Message = $"Không tìm thấy script Python: {ScriptPath}" };

                // Escape arguments for command line
                string safeEmployeeCode = EscapeArgument(employeeCode);
                string safeEmployeeName = EscapeArgument(employeeName);
                string safeImagePath = EscapeArgument(imagePath);

                string arguments = $"\"{ScriptPath}\" register --employee_id \"{safeEmployeeCode}\" --employee_name \"{safeEmployeeName}\" --image_path \"{safeImagePath}\"";

                var result = await ExecutePythonScriptAsync(arguments, TimeoutSeconds * 2); // Double timeout for registration

                if (result.Success)
                {
                    // Parse the result to get face path
                    var jsonResult = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(result.Output);

                    if (jsonResult.ContainsKey("face_path"))
                    {
                        result.FacePath = jsonResult["face_path"].ToString();
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
                        Message = $"Không tìm thấy script Python: {ScriptPath}"
                    };
                }

                string arguments = $"\"{ScriptPath}\" recognize_camera --timeout {timeoutSeconds}";
                var result = await ExecutePythonScriptAsync(arguments, timeoutSeconds + 10);

                if (result.Success)
                {
                    try
                    {
                        var jsonResult = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(result.Output);

                        return new FaceRecognitionResult
                        {
                            Success = true,
                            EmployeeId = jsonResult["employee_id"].ToString(),
                            EmployeeName = jsonResult.ContainsKey("employee_name") ? jsonResult["employee_name"].ToString() : "",
                            Confidence = Convert.ToDecimal(jsonResult["confidence"]),
                            Timestamp = DateTime.Parse(jsonResult["timestamp"].ToString()),
                            AttendanceImagePath = jsonResult.ContainsKey("attendance_image") ? jsonResult["attendance_image"].ToString() : null
                        };
                    }
                    catch (Exception parseEx)
                    {
                        return new FaceRecognitionResult
                        {
                            Success = false,
                            Message = $"Lỗi phân tích kết quả: {parseEx.Message}"
                        };
                    }
                }

                return new FaceRecognitionResult
                {
                    Success = false,
                    Message = result.Message
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

                if (result.Success)
                {
                    try
                    {
                        var jsonResult = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(result.Output);

                        var faces = new List<RegisteredFace>();
                        if (jsonResult.ContainsKey("faces"))
                        {
                            var facesArray = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonResult["faces"].ToString());

                            foreach (var face in facesArray)
                            {
                                faces.Add(new RegisteredFace
                                {
                                    EmployeeId = face["employee_id"].ToString(),
                                    EmployeeName = face["employee_name"].ToString()
                                });
                            }
                        }

                        result.Faces = faces;
                    }
                    catch (Exception parseEx)
                    {
                        return new FaceRecognitionServiceResult
                        {
                            Success = false,
                            Message = $"Lỗi phân tích danh sách: {parseEx.Message}"
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

                string safeEmployeeId = EscapeArgument(employeeId);
                string arguments = $"\"{ScriptPath}\" delete --employee_id \"{safeEmployeeId}\"";

                return await ExecutePythonScriptAsync(arguments, TimeoutSeconds);
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
                        ErrorMessage = "Python không được cài đặt hoặc không trong PATH"
                    };
                }

                // Check script file
                if (!File.Exists(ScriptPath))
                {
                    return new SystemReadinessResult
                    {
                        IsReady = false,
                        ErrorMessage = $"Không tìm thấy script Python: {ScriptPath}"
                    };
                }

                // Check required folders
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
                        process.Kill();
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
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = PythonExecutable;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
                    process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;

                    // Set environment variables
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

                    // Wait for process to complete with cancellation support
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

                        if (exitCode == 0 && !string.IsNullOrEmpty(output))
                        {
                            // Try to parse as JSON to validate
                            try
                            {
                                var testParse = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(output);

                                return new FaceRecognitionServiceResult
                                {
                                    Success = testParse.ContainsKey("success") && testParse["success"].ToString().ToLower() == "true",
                                    Message = testParse.ContainsKey("message") ? testParse["message"].ToString() : "",
                                    Output = output
                                };
                            }
                            catch
                            {
                                return new FaceRecognitionServiceResult
                                {
                                    Success = false,
                                    Message = $"Kết quả không hợp lệ: {output}"
                                };
                            }
                        }
                        else
                        {
                            string errorMessage = !string.IsNullOrEmpty(error) ? error : $"Process exited with code {exitCode}";

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
            }
            catch (Exception ex)
            {
                return new FaceRecognitionServiceResult
                {
                    Success = false,
                    Message = $"Lỗi thực thi: {ex.Message}"
                };
            }
            finally
            {
                cancellationTokenSource?.Dispose();
            }
        }

        private static string EscapeArgument(string argument)
        {
            if (string.IsNullOrEmpty(argument))
                return argument;

            // Escape special characters for command line
            return argument.Replace("\"", "\\\"").Replace("\\", "\\\\");
        }
    }
    public class SystemReadinessResult
    {
        public bool IsReady { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    // Supporting classes
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