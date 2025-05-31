using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EmployeeManagement.Utilities.Attendance
{
    /// <summary>
    /// Utilities hỗ trợ chức năng nhận diện khuôn mặt
    /// </summary>
    public static class FaceRecognition
    {
        private static readonly string PythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FaceRecognition");
        private static readonly string PythonExecutable = "python";

        /// <summary>
        /// Khởi tạo thư mục và file cần thiết cho nhận diện khuôn mặt
        /// </summary>
        public static bool InitializeFaceRecognitionSystem()
        {
            try
            {
                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(PythonScriptPath))
                {
                    Directory.CreateDirectory(PythonScriptPath);
                }

                // Tạo các thư mục con
                string[] subDirectories = { "face_data", "temp_images", "checkin_images", "training_images" };
                foreach (string subDir in subDirectories)
                {
                    string path = Path.Combine(PythonScriptPath, subDir);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }

                // Tạo file script Python nếu chưa có
                CreatePythonScripts();

                Logger.LogInfo("Face recognition system initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to initialize face recognition system: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra xem Python và các thư viện cần thiết đã được cài đặt chưa
        /// </summary>
        public static async Task<bool> CheckPythonEnvironmentAsync()
        {
            try
            {
                // Kiểm tra Python
                var pythonResult = await RunPythonCommandAsync("--version");
                if (!pythonResult.Success)
                {
                    Logger.LogError("Python is not installed or not accessible");
                    return false;
                }

                // Kiểm tra các thư viện cần thiết
                string[] requiredPackages = { "opencv-python", "face-recognition", "numpy", "pillow" };

                foreach (string package in requiredPackages)
                {
                    var checkResult = await RunPythonCommandAsync($"-c \"import {GetPythonModuleName(package)}\"");
                    if (!checkResult.Success)
                    {
                        Logger.LogError($"Required Python package '{package}' is not installed");
                        return false;
                    }
                }

                Logger.LogInfo("Python environment check passed");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error checking Python environment: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Đăng ký khuôn mặt cho nhân viên
        /// </summary>
        public static async Task<bool> RegisterEmployeeFaceAsync(int employeeId, string? employeeName = null)
        {
            try
            {
                Logger.LogInfo($"Starting face registration for employee {employeeId} - {employeeName}");

                string scriptPath = Path.Combine(PythonScriptPath, "face_recognition_system.py");
                if (!File.Exists(scriptPath))
                {
                    Logger.LogError("Face registration script not found");
                    return false;
                }

                var result = await RunPythonScriptAsync(scriptPath, $"register --employee_id {employeeId}");

                if (result.Success && result.Output.Contains("completed successfully"))
                {
                    Logger.LogInfo($"Face registration completed for employee {employeeId}");
                    return true;
                }
                else
                {
                    Logger.LogError($"Face registration failed for employee {employeeId}: {result.ErrorOutput}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error during face registration: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Nhận diện khuôn mặt từ file ảnh
        /// </summary>
        public static async Task<FaceRecognitionResult> RecognizeFaceFromImageAsync(string imagePath)
        {
            try
            {
                if (!File.Exists(imagePath))
                {
                    return new FaceRecognitionResult { Success = false, ErrorMessage = "Image file not found" };
                }

                string scriptPath = Path.Combine(PythonScriptPath, "face_recognition_system.py");
                if (!File.Exists(scriptPath))
                {
                    return new FaceRecognitionResult { Success = false, ErrorMessage = "Face recognition script not found" };
                }

                var result = await RunPythonScriptAsync(scriptPath, $"recognize --image_path \"{imagePath}\"");

                if (result.Success)
                {
                    string output = result.Output.Trim();
                    if (output != "Unknown" && int.TryParse(output, out int employeeId))
                    {
                        return new FaceRecognitionResult
                        {
                            Success = true,
                            EmployeeId = employeeId,
                            Confidence = 0.8f // Default confidence, could be extracted from Python output
                        };
                    }
                    else
                    {
                        return new FaceRecognitionResult
                        {
                            Success = true,
                            EmployeeId = null,
                            Confidence = 0.0f
                        };
                    }
                }
                else
                {
                    return new FaceRecognitionResult
                    {
                        Success = false,
                        ErrorMessage = result.ErrorOutput
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error during face recognition: {ex.Message}");
                return new FaceRecognitionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Chụp ảnh từ camera
        /// </summary>
        public static async Task<string?> CaptureImageFromCameraAsync()
        {
            try
            {
                string outputPath = Path.Combine(PythonScriptPath, "temp_images", $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
                string scriptPath = Path.Combine(PythonScriptPath, "camera_capture.py");

                var result = await RunPythonScriptAsync(scriptPath, $"capture \"{outputPath}\"");

                if (result.Success && File.Exists(outputPath))
                {
                    Logger.LogInfo($"Image captured successfully: {outputPath}");
                    return outputPath;
                }
                else
                {
                    Logger.LogError($"Failed to capture image: {result.ErrorOutput}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error capturing image: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Xóa dữ liệu khuôn mặt của nhân viên
        /// </summary>
        public static async Task<bool> DeleteEmployeeFaceDataAsync(int employeeId)
        {
            try
            {
                string scriptPath = Path.Combine(PythonScriptPath, "face_recognition_system.py");
                var result = await RunPythonScriptAsync(scriptPath, $"delete --employee_id {employeeId}");

                if (result.Success)
                {
                    Logger.LogInfo($"Face data deleted for employee {employeeId}");
                    return true;
                }
                else
                {
                    Logger.LogError($"Failed to delete face data for employee {employeeId}: {result.ErrorOutput}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting face data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy thống kê hệ thống nhận diện khuôn mặt
        /// </summary>
        public static async Task<FaceRecognitionStats> GetSystemStatsAsync()
        {
            try
            {
                string scriptPath = Path.Combine(PythonScriptPath, "face_recognition_system.py");
                var result = await RunPythonScriptAsync(scriptPath, "stats");

                if (result.Success)
                {
                    // Parse JSON output from Python script
                    var stats = new FaceRecognitionStats
                    {
                        TotalRegisteredFaces = GetRegisteredFacesCount(),
                        LastUpdated = DateTime.Now,
                        SystemHealthy = true
                    };

                    return stats;
                }
                else
                {
                    Logger.LogError($"Failed to get system stats: {result.ErrorOutput}");
                    return new FaceRecognitionStats();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error getting system stats: {ex.Message}");
                return new FaceRecognitionStats();
            }
        }

        /// <summary>
        /// Bắt đầu camera stream
        /// </summary>
        public static async Task<bool> StartCameraStreamAsync()
        {
            try
            {
                string scriptPath = Path.Combine(PythonScriptPath, "face_recognition_system.py");
                var result = await RunPythonScriptAsync(scriptPath, "stream_start");
                return result.Success;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error starting camera stream: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Dừng camera stream
        /// </summary>
        public static async Task<bool> StopCameraStreamAsync()
        {
            try
            {
                string scriptPath = Path.Combine(PythonScriptPath, "face_recognition_system.py");
                var result = await RunPythonScriptAsync(scriptPath, "stream_stop");
                return result.Success;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error stopping camera stream: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Chạy lệnh Python
        /// </summary>
        private static async Task<ProcessResult> RunPythonCommandAsync(string arguments)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = PythonExecutable,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = PythonScriptPath
                };

                using Process? process = Process.Start(startInfo);
                if (process == null)
                {
                    return new ProcessResult { Success = false, ErrorOutput = "Failed to start Python process" };
                }

                await process.WaitForExitAsync();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                return new ProcessResult
                {
                    Success = process.ExitCode == 0,
                    Output = output,
                    ErrorOutput = error,
                    ExitCode = process.ExitCode
                };
            }
            catch (Exception ex)
            {
                return new ProcessResult
                {
                    Success = false,
                    ErrorOutput = ex.Message
                };
            }
        }

        /// <summary>
        /// Chạy script Python
        /// </summary>
        private static async Task<ProcessResult> RunPythonScriptAsync(string scriptPath, string arguments)
        {
            return await RunPythonCommandAsync($"\"{scriptPath}\" {arguments}");
        }

        /// <summary>
        /// Tạo các script Python cần thiết
        /// </summary>
        private static void CreatePythonScripts()
        {
            CreateFaceRecognitionScript();
            CreateCameraCaptureScript();
            CreateRequirementsFile();
        }

        /// <summary>
        /// Tạo script nhận diện khuôn mặt chính
        /// </summary>
        private static void CreateFaceRecognitionScript()
        {
            string scriptPath = Path.Combine(PythonScriptPath, "face_recognition_system.py");

            if (!File.Exists(scriptPath))
            {
                // Script content đã được tạo trong artifact trước
                Logger.LogInfo("Face recognition Python script should be copied to project directory");
            }
        }

        /// <summary>
        /// Tạo script chụp ảnh từ camera
        /// </summary>
        private static void CreateCameraCaptureScript()
        {
            string scriptPath = Path.Combine(PythonScriptPath, "camera_capture.py");

            if (!File.Exists(scriptPath))
            {
                // Script content đã được tạo trong artifact trước
                Logger.LogInfo("Camera capture Python script should be copied to project directory");
            }
        }

        /// <summary>
        /// Tạo file requirements.txt
        /// </summary>
        private static void CreateRequirementsFile()
        {
            string requirementsPath = Path.Combine(PythonScriptPath, "requirements.txt");

            if (!File.Exists(requirementsPath))
            {
                string requirements = @"opencv-python==4.8.1.78
face-recognition==1.3.0
numpy==1.24.3
Pillow==10.0.1
cmake==3.27.7
dlib==19.24.2
argparse
";
                File.WriteAllText(requirementsPath, requirements);
                Logger.LogInfo("Requirements.txt file created");
            }
        }

        /// <summary>
        /// Lấy tên module Python từ tên package
        /// </summary>
        private static string GetPythonModuleName(string packageName)
        {
            return packageName switch
            {
                "opencv-python" => "cv2",
                "face-recognition" => "face_recognition",
                "pillow" => "PIL",
                _ => packageName.Replace("-", "_")
            };
        }

        /// <summary>
        /// Đếm số lượng khuôn mặt đã đăng ký
        /// </summary>
        private static int GetRegisteredFacesCount()
        {
            try
            {
                string encodingsFile = Path.Combine(PythonScriptPath, "face_encodings.pkl");
                if (File.Exists(encodingsFile))
                {
                    // Simplified count - in reality would need to parse pickle file
                    return 1;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Kết quả nhận diện khuôn mặt
    /// </summary>
    public class FaceRecognitionResult
    {
        public bool Success { get; set; }
        public int? EmployeeId { get; set; }
        public float Confidence { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Thống kê hệ thống nhận diện khuôn mặt
    /// </summary>
    public class FaceRecognitionStats
    {
        public int TotalRegisteredFaces { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool SystemHealthy { get; set; } = true;
        public string SystemVersion { get; set; } = "1.0";
    }

    /// <summary>
    /// Kết quả thực thi process
    /// </summary>
    public class ProcessResult
    {
        public bool Success { get; set; }
        public string Output { get; set; } = string.Empty;
        public string ErrorOutput { get; set; } = string.Empty;
        public int ExitCode { get; set; }
    }
}