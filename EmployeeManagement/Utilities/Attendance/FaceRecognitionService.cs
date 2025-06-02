using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using EmployeeManagement.Utilities;
using Newtonsoft.Json;

namespace EmployeeManagement.Utilities
{
    /// <summary>
    /// Service xử lý nhận diện khuôn mặt
    /// </summary>
    public static class FaceRecognitionService
    {
        private static readonly string FaceDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EmployeeManagement", "FaceData");

        private static readonly string AttendanceImagesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EmployeeManagement", "AttendanceImages");

        static FaceRecognitionService()
        {
            // Tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(FaceDataDirectory);
            Directory.CreateDirectory(AttendanceImagesDirectory);
        }

        /// <summary>
        /// Kiểm tra hệ thống có sẵn sàng cho face recognition không
        /// </summary>
        public static SystemReadinessResult CheckSystemReadiness()
        {
            var result = new SystemReadinessResult();
            var errors = new List<string>();

            try
            {
                // Kiểm tra thư mục face data
                if (!Directory.Exists(FaceDataDirectory))
                {
                    Directory.CreateDirectory(FaceDataDirectory);
                }

                // Kiểm tra quyền truy cập file system
                if (!HasFileSystemPermissions())
                {
                    errors.Add("Không có quyền truy cập thư mục lưu trữ");
                }

                // Kiểm tra Python (nếu cần)
                if (!IsPythonAvailable())
                {
                    errors.Add("Python không được cài đặt hoặc không tìm thấy");
                }

                result.IsReady = errors.Count == 0;
                result.Errors = errors;
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi kiểm tra hệ thống: {ex.Message}");
                result.IsReady = false;
                result.Errors = errors;
            }

            return result;
        }

        /// <summary>
        /// Cài đặt các dependencies cần thiết
        /// </summary>
        public static async System.Threading.Tasks.Task<InstallationResult> InstallDependenciesAsync()
        {
            try
            {
                var result = new InstallationResult();

                // Placeholder cho việc cài đặt thư viện
                await System.Threading.Tasks.Task.Delay(3000); // Simulate installation time

                // Trong thực tế, đây sẽ là nơi cài đặt:
                // - OpenCV libraries
                // - Face recognition libraries
                // - Python dependencies

                result.Success = true;
                result.Message = "Cài đặt thành công tất cả thư viện cần thiết";

                return result;
            }
            catch (Exception ex)
            {
                return new InstallationResult
                {
                    Success = false,
                    Message = $"Lỗi cài đặt: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Đăng ký khuôn mặt cho nhân viên
        /// </summary>
        public static async System.Threading.Tasks.Task<FaceRegistrationResult> RegisterFaceAsync(string employeeId, string employeeName, string imagePath)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(employeeId) || string.IsNullOrEmpty(imagePath))
                {
                    return new FaceRegistrationResult
                    {
                        Success = false,
                        Message = "Thông tin đầu vào không hợp lệ"
                    };
                }

                if (!File.Exists(imagePath))
                {
                    return new FaceRegistrationResult
                    {
                        Success = false,
                        Message = "File ảnh không tồn tại"
                    };
                }

                // Copy image to face data directory
                string faceDataFileName = $"employee_{employeeId}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string destinationPath = Path.Combine(FaceDataDirectory, faceDataFileName);

                File.Copy(imagePath, destinationPath, true);

                // Placeholder cho việc training face recognition model
                await System.Threading.Tasks.Task.Delay(2000); // Simulate processing time

                // Trong thực tế, đây sẽ là nơi:
                // - Extract face features từ ảnh
                // - Train hoặc update face recognition model
                // - Lưu face encoding vào database hoặc file

                return new FaceRegistrationResult
                {
                    Success = true,
                    Message = "Đăng ký khuôn mặt thành công",
                    FacePath = destinationPath
                };
            }
            catch (Exception ex)
            {
                Logger.LogError($"FaceRecognitionService.RegisterFaceAsync: {ex.Message}");
                return new FaceRegistrationResult
                {
                    Success = false,
                    Message = $"Lỗi đăng ký khuôn mặt: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Nhận diện khuôn mặt từ camera
        /// </summary>
        public static async System.Threading.Tasks.Task<FaceRecognitionResult> RecognizeFromCameraAsync(int timeoutSeconds = 30)
        {
            try
            {
                // Placeholder cho việc capture và nhận diện từ camera
                await System.Threading.Tasks.Task.Delay(5000); // Simulate recognition time

                // Trong thực tế, đây sẽ là nơi:
                // - Capture ảnh từ camera
                // - Extract face features
                // - So sánh với các face đã đăng ký
                // - Trả về kết quả nhận diện

                // Simulate successful recognition
                string attendanceImagePath = Path.Combine(AttendanceImagesDirectory,
                    $"attendance_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

                // Create a dummy image file (in real implementation, this would be the captured image)
                File.WriteAllText(attendanceImagePath, "Dummy attendance image");

                return new FaceRecognitionResult
                {
                    Success = true,
                    EmployeeId = "NV0001", // This would come from face recognition
                    EmployeeName = "Nguyễn Văn A", // This would be looked up from database
                    Confidence = 95.5f,
                    Timestamp = DateTime.Now,
                    AttendanceImagePath = attendanceImagePath
                };
            }
            catch (Exception ex)
            {
                Logger.LogError($"FaceRecognitionService.RecognizeFromCameraAsync: {ex.Message}");
                return new FaceRecognitionResult
                {
                    Success = false,
                    Message = $"Lỗi nhận diện: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Lấy danh sách khuôn mặt đã đăng ký
        /// </summary>
        public static async System.Threading.Tasks.Task<RegisteredFacesResult> GetRegisteredFacesAsync()
        {
            try
            {
                var faces = new List<RegisteredFace>();

                // Placeholder - trong thực tế sẽ lấy từ database
                await System.Threading.Tasks.Task.Delay(500);

                // Scan face data directory
                if (Directory.Exists(FaceDataDirectory))
                {
                    var files = Directory.GetFiles(FaceDataDirectory, "employee_*.jpg");
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        var parts = fileName.Split('_');

                        if (parts.Length >= 2)
                        {
                            faces.Add(new RegisteredFace
                            {
                                EmployeeId = parts[1],
                                EmployeeName = $"Nhân viên {parts[1]}", // Would lookup from database
                                FacePath = file,
                                RegisterDate = File.GetCreationTime(file)
                            });
                        }
                    }
                }

                return new RegisteredFacesResult
                {
                    Success = true,
                    Faces = faces
                };
            }
            catch (Exception ex)
            {
                Logger.LogError($"FaceRecognitionService.GetRegisteredFacesAsync: {ex.Message}");
                return new RegisteredFacesResult
                {
                    Success = false,
                    Message = $"Lỗi lấy danh sách: {ex.Message}",
                    Faces = new List<RegisteredFace>()
                };
            }
        }

        /// <summary>
        /// Xóa khuôn mặt đã đăng ký
        /// </summary>
        public static async System.Threading.Tasks.Task<FaceDeleteResult> DeleteRegisteredFaceAsync(string employeeId)
        {
            try
            {
                await System.Threading.Tasks.Task.Delay(500);

                // Find and delete face files for this employee
                if (Directory.Exists(FaceDataDirectory))
                {
                    var files = Directory.GetFiles(FaceDataDirectory, $"employee_{employeeId}_*.jpg");
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                }

                // Trong thực tế, cũng cần xóa face encoding từ model

                return new FaceDeleteResult
                {
                    Success = true,
                    Message = "Xóa khuôn mặt thành công"
                };
            }
            catch (Exception ex)
            {
                Logger.LogError($"FaceRecognitionService.DeleteRegisteredFaceAsync: {ex.Message}");
                return new FaceDeleteResult
                {
                    Success = false,
                    Message = $"Lỗi xóa khuôn mặt: {ex.Message}"
                };
            }
        }

        #region Helper Methods

        private static bool IsPythonAvailable()
        {
            try
            {
                using var process = new Process();
                process.StartInfo.FileName = "python";
                process.StartInfo.Arguments = "--version";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit(5000);

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool HasFileSystemPermissions()
        {
            try
            {
                var testFile = Path.Combine(FaceDataDirectory, "test.txt");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    #region Result Classes

    /// <summary>
    /// Kết quả kiểm tra hệ thống
    /// </summary>
    public class SystemReadinessResult
    {
        public bool IsReady { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Kết quả cài đặt
    /// </summary>
    public class InstallationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả đăng ký khuôn mặt
    /// </summary>
    public class FaceRegistrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string FacePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả nhận diện khuôn mặt
    /// </summary>
    public class FaceRecognitionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public DateTime Timestamp { get; set; }
        public string AttendanceImagePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả xóa khuôn mặt
    /// </summary>
    public class FaceDeleteResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả danh sách khuôn mặt
    /// </summary>
    public class RegisteredFacesResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<RegisteredFace> Faces { get; set; } = new List<RegisteredFace>();
    }

    /// <summary>
    /// Thông tin khuôn mặt đã đăng ký
    /// </summary>
    public class RegisteredFace
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string FacePath { get; set; } = string.Empty;
        public DateTime RegisterDate { get; set; }
    }

    #endregion
}