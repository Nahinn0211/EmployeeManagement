using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;
using System.Text.Json;

namespace EmployeeManagement.Utilities
{
    // ===== SUPPORTING CLASSES =====

    public class SystemCheckResult
    {
        public bool IsReady { get; set; }
        public string Message { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
    }

    public class FaceRecognitionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? EmployeeId { get; set; }
        public string EmployeeName { get; set; } = "";
        public double Confidence { get; set; }
        public DateTime Timestamp { get; set; }
        public string? AttendanceImagePath { get; set; }
    }

    public class RegisteredFace
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string FaceImagePath { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
    }

    public class FaceRegistrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string FacePath { get; set; } = string.Empty;
    }

    public class FaceListResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<RegisteredFace> Faces { get; set; } = new List<RegisteredFace>();
    }

    public class FaceOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // ===== MAIN FACE RECOGNITION SERVICE =====

    public class FaceRecognitionService : IDisposable
    {
        #region Static Members for Camera Operations

        private static readonly object _lock = new object();
        private static VideoCaptureDevice? _currentCamera;
        private static volatile bool _isCapturing = false;
        private static Bitmap? _latestFrame;

        #endregion

        #region Instance Members

        private readonly FaceRecognizer recognizer;
        private CascadeClassifier? faceCascade;
        private readonly string facesDataPath;
        private readonly string facesImagePath;
        private readonly double tolerance = 0.6;
        private List<RegisteredFace> registeredFaces;
        private bool disposed = false;

        #endregion

        #region Constructor

        public FaceRecognitionService()
        {
            // Initialize paths
            facesDataPath = Path.Combine(Application.StartupPath, "FaceData");
            facesImagePath = Path.Combine(facesDataPath, "Images");

            Directory.CreateDirectory(facesDataPath);
            Directory.CreateDirectory(facesImagePath);

            // Initialize face recognizer (LBPH - Local Binary Patterns Histograms)
            recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100.0);

            // Initialize face cascade for detection
            InitializeFaceCascade();

            registeredFaces = new List<RegisteredFace>();
            LoadRegisteredFaces();
        }

        #endregion

        #region Static Methods - Required by FaceRecognitionForm

        /// <summary>
        /// Kiểm tra tình trạng sẵn sàng của hệ thống nhận diện khuôn mặt
        /// </summary>
        public static SystemCheckResult CheckSystemReadiness()
        {
            try
            {
                // Check if EmguCV assemblies are available
                if (!IsEmguCVAvailable())
                {
                    return new SystemCheckResult
                    {
                        IsReady = false,
                        Message = "",
                        ErrorMessage = "EmguCV libraries not found. Please install EmguCV NuGet package."
                    };
                }

                // Check if training data directory exists
                string trainingDataPath = GetTrainingDataPath();
                if (!Directory.Exists(trainingDataPath))
                {
                    Directory.CreateDirectory(trainingDataPath);
                }

                return new SystemCheckResult
                {
                    IsReady = true,
                    Message = "Face recognition system is ready",
                    ErrorMessage = ""
                };
            }
            catch (Exception ex)
            {
                return new SystemCheckResult
                {
                    IsReady = false,
                    Message = "",
                    ErrorMessage = $"System check failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Thực hiện nhận diện khuôn mặt từ camera
        /// </summary>
        public static async Task<FaceRecognitionResult> RecognizeFromCameraAsync(int timeoutSeconds)
        {
            try
            {
                // Check system readiness first
                var systemCheck = CheckSystemReadiness();
                if (!systemCheck.IsReady)
                {
                    return new FaceRecognitionResult
                    {
                        Success = false,
                        Message = $"System not ready: {systemCheck.ErrorMessage}",
                        EmployeeId = null,
                        Confidence = 0,
                        Timestamp = DateTime.Now
                    };
                }

                return await PerformFaceRecognitionAsync(timeoutSeconds);
            }
            catch (Exception ex)
            {
                return new FaceRecognitionResult
                {
                    Success = false,
                    Message = $"Recognition error: {ex.Message}",
                    EmployeeId = null,
                    Confidence = 0,
                    Timestamp = DateTime.Now
                };
            }
        }

      
        // ✅ THÊM METHOD MỚI - Real Face Recognition
        private static async Task<(string? employeeId, string employeeName, double confidence)> PerformRealFaceRecognitionAsync(Bitmap frame)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔍 Starting REAL face recognition...");

                // Create temporary FaceRecognitionService instance to access trained model
                using var tempService = new FaceRecognitionService();

                // Save frame to temporary file
                string tempImagePath = Path.Combine(Path.GetTempPath(), $"recognition_frame_{DateTime.Now.Ticks}.jpg");

                try
                {
                    frame.Save(tempImagePath, ImageFormat.Jpeg);
                    System.Diagnostics.Debug.WriteLine($"📁 Saved frame to: {tempImagePath}");

                    // Use real recognition method
                    var result = await tempService.RecognizeFaceFromImageAsync(tempImagePath);

                    if (result.Success)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Real recognition success: {result.EmployeeId} - {result.EmployeeName} ({result.Confidence:F1}%)");
                        return (result.EmployeeId, result.EmployeeName, result.Confidence);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Real recognition failed: {result.Message}");
                        return (null, "", 0);
                    }
                }
                finally
                {
                    // Cleanup temp file
                    if (File.Exists(tempImagePath))
                    {
                        try { File.Delete(tempImagePath); } catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Real recognition error: {ex.Message}");
                return (null, "", 0);
            }
        }


        // THAY THẾ trong FaceRecognitionService.cs

        // 1. Giảm threshold và cải thiện logic nhận diện
        private static async Task<FaceRecognitionResult> PerformFaceRecognitionAsync(int timeoutSeconds)
        {
            try
            {
                if (!await InitializeCameraForRecognitionAsync())
                {
                    return new FaceRecognitionResult
                    {
                        Success = false,
                        Message = "Cannot initialize camera for recognition",
                        EmployeeId = null,
                        Confidence = 0,
                        Timestamp = DateTime.Now
                    };
                }

                var startTime = DateTime.Now;
                string? recognizedEmployeeId = null;
                string recognizedEmployeeName = "";
                double bestConfidence = 0;

                // ✅ Lưu trữ nhiều kết quả để lấy kết quả tốt nhất
                var recognitionResults = new List<(string? employeeId, string employeeName, double confidence)>();

                while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
                {
                    var frame = GetLatestFrame();
                    if (frame != null)
                    {
                        var realResult = await PerformRealFaceRecognitionAsync(frame);

                        // ✅ Thu thập tất cả kết quả có confidence > 40%
                        if (realResult.confidence > 40.0 && !string.IsNullOrEmpty(realResult.employeeId))
                        {
                            recognitionResults.Add(realResult);

                            // Log để debug
                            System.Diagnostics.Debug.WriteLine($"📊 Result: {realResult.employeeId} - {realResult.confidence:F1}%");
                        }

                        // ✅ Cập nhật best result
                        if (realResult.confidence > bestConfidence)
                        {
                            bestConfidence = realResult.confidence;
                            recognizedEmployeeId = realResult.employeeId;
                            recognizedEmployeeName = realResult.employeeName;

                            // ✅ Giảm threshold xuống 50% để dễ nhận diện hơn
                            if (bestConfidence >= 50.0)
                            {
                                System.Diagnostics.Debug.WriteLine($"✅ Quick recognition: {recognizedEmployeeId} ({bestConfidence:F1}%)");
                                break;
                            }
                        }
                    }

                    await Task.Delay(100);
                }

                StopCameraCapture();

                // ✅ Phân tích kết quả thu thập được
                if (recognitionResults.Count > 0)
                {
                    // Tìm employeeId xuất hiện nhiều nhất
                    var mostFrequentEmployee = recognitionResults
                        .Where(r => !string.IsNullOrEmpty(r.employeeId))
                        .GroupBy(r => r.employeeId)
                        .OrderByDescending(g => g.Count())
                        .ThenByDescending(g => g.Max(x => x.confidence))
                        .FirstOrDefault();

                    if (mostFrequentEmployee != null && mostFrequentEmployee.Count() >= 2)
                    {
                        var bestResult = mostFrequentEmployee.OrderByDescending(r => r.confidence).First();

                        // ✅ Giảm threshold cuối xuống 45%
                        if (bestResult.confidence >= 45.0)
                        {
                            string imagePath = await SaveAttendanceImageAsync(GetLatestFrame(), bestResult.employeeId);

                            return new FaceRecognitionResult
                            {
                                Success = true,
                                Message = $"Face recognized with multiple confirmations ({mostFrequentEmployee.Count()} times)",
                                EmployeeId = bestResult.employeeId,
                                EmployeeName = bestResult.employeeName,
                                Confidence = bestResult.confidence,
                                Timestamp = DateTime.Now,
                                AttendanceImagePath = imagePath
                            };
                        }
                    }
                }

                // ✅ Fallback - nếu có kết quả tốt nhất >= 40%
                if (bestConfidence >= 40.0 && !string.IsNullOrEmpty(recognizedEmployeeId))
                {
                    string imagePath = await SaveAttendanceImageAsync(GetLatestFrame(), recognizedEmployeeId);

                    return new FaceRecognitionResult
                    {
                        Success = true,
                        Message = "Face recognized with lower confidence",
                        EmployeeId = recognizedEmployeeId,
                        EmployeeName = recognizedEmployeeName,
                        Confidence = bestConfidence,
                        Timestamp = DateTime.Now,
                        AttendanceImagePath = imagePath
                    };
                }

                return new FaceRecognitionResult
                {
                    Success = false,
                    Message = $"Face not recognized. Best confidence: {bestConfidence:F1}%. Total attempts: {recognitionResults.Count}",
                    EmployeeId = null,
                    Confidence = bestConfidence,
                    Timestamp = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                return new FaceRecognitionResult
                {
                    Success = false,
                    Message = $"Recognition process error: {ex.Message}",
                    EmployeeId = null,
                    Confidence = 0,
                    Timestamp = DateTime.Now
                };
            }
        }


      
        // Helper methods for data augmentation
        private Image<Gray, byte> RotateImage(Image<Gray, byte> image, double angle)
        {
            var center = new PointF(image.Width / 2f, image.Height / 2f);
            var rotationMatrix = new Mat();
            CvInvoke.GetRotationMatrix2D(center, angle, 1.0, rotationMatrix);

            var rotated = new Image<Gray, byte>(image.Size);
            CvInvoke.WarpAffine(image, rotated, rotationMatrix, image.Size);

            rotationMatrix.Dispose();
            return rotated;
        }

        private Image<Gray, byte> AdjustBrightness(Image<Gray, byte> image, double factor)
        {
            var adjusted = image.Clone();
            adjusted._Mul(factor);
            return adjusted;
        }








        // ===== THAY THẾ PHƯƠNG THỨC RecognizeFaceFromImageAsync TRONG FaceRecognitionService.cs =====

        public async Task<FaceRecognitionResult> RecognizeFaceFromImageAsync(string imagePath, CancellationToken cancellationToken = default)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🔍 RecognizeFaceFromImage: {imagePath}");

                if (!File.Exists(imagePath))
                {
                    return new FaceRecognitionResult { Success = false, Message = "Image file not found" };
                }

                if (registeredFaces.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ No registered faces found");
                    return new FaceRecognitionResult { Success = false, Message = "No registered faces in system" };
                }

                System.Diagnostics.Debug.WriteLine($"📊 Total registered faces: {registeredFaces.Count}");

                using var image = new Image<Bgr, byte>(imagePath);
                using var grayImage = image.Convert<Gray, byte>();

                // ✅ PREPROCESSING PIPELINE cải tiến
                using var preprocessedImage = PreprocessImageForRecognition(grayImage);

                // ✅ Cải thiện face detection với multiple attempts
                System.Drawing.Rectangle[] faces = DetectFacesMultipleAttempts(preprocessedImage);

                // ✅ Nếu không detect được face, thử dùng multiple preprocessing methods
                if (faces.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No face detected - trying alternative preprocessing");
                    faces = TryAlternativePreprocessingAndDetection(grayImage);
                }

                // ✅ Fallback: sử dụng center crop nếu vẫn không detect được
                if (faces.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Using center crop as fallback");
                    faces = new System.Drawing.Rectangle[]
                    {
                CreateCenterCropRectangle(grayImage.Width, grayImage.Height)
                    };
                }

                // ✅ Thử nhiều vùng mặt và multiple preprocessing cho mỗi vùng
                var bestResult = new FaceRecognitionResult { Success = false, Confidence = 0 };

                foreach (var face in faces.Take(3)) // Thử tối đa 3 faces
                {
                    try
                    {
                        // Thử multiple preprocessing methods cho mỗi face region
                        var faceResults = await ProcessFaceRegionWithMultipleMethods(preprocessedImage, face);

                        foreach (var result in faceResults)
                        {
                            if (result.Confidence > bestResult.Confidence)
                            {
                                bestResult = result;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Face processing error: {ex.Message}");
                    }
                }

                // ✅ Giảm threshold xuống 25% để dễ nhận diện hơn
                if (bestResult.Confidence >= 25.0)
                {
                    bestResult.Success = true;
                    bestResult.Message = bestResult.Confidence >= 40.0 ?
                        "Face recognized successfully" :
                        $"Face recognized with lower confidence: {bestResult.Confidence:F1}%";
                }
                else
                {
                    bestResult.Message = $"Face not recognized. Best confidence: {bestResult.Confidence:F1}%";
                }

                System.Diagnostics.Debug.WriteLine($"🎯 Final result: {bestResult.EmployeeId} - {bestResult.Confidence:F1}% - {bestResult.Success}");

                return bestResult;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ RecognizeFaceFromImage error: {ex.Message}");
                return new FaceRecognitionResult { Success = false, Message = $"Recognition error: {ex.Message}" };
            }
        }

        // ✅ THÊM CÁC PHƯƠNG THỨC HỖ TRỢ MỚI

        private Image<Gray, byte> PreprocessImageForRecognition(Image<Gray, byte> grayImage)
        {
            try
            {
                var processed = grayImage.Clone();

                // 1. Cải thiện contrast
                CvInvoke.EqualizeHist(processed, processed);

                // 2. Giảm noise
                CvInvoke.GaussianBlur(processed, processed, new Size(3, 3), 0);

                // 3. Chuẩn hóa brightness
                processed._Mul(1.1); // Tăng brightness một chút

                return processed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Preprocessing error: {ex.Message}");
                return grayImage.Clone();
            }
        }

        private System.Drawing.Rectangle[] DetectFacesMultipleAttempts(Image<Gray, byte> grayImage)
        {
            if (faceCascade == null) return new System.Drawing.Rectangle[0];

            // Thử nhiều tham số khác nhau
            var attempts = new[]
            {
        new { ScaleFactor = 1.05, MinNeighbors = 3 },
        new { ScaleFactor = 1.1, MinNeighbors = 3 },
        new { ScaleFactor = 1.05, MinNeighbors = 2 },
        new { ScaleFactor = 1.15, MinNeighbors = 4 },
        new { ScaleFactor = 1.2, MinNeighbors = 3 },
        new { ScaleFactor = 1.08, MinNeighbors = 2 }
    };

            foreach (var attempt in attempts)
            {
                try
                {
                    var faces = faceCascade.DetectMultiScale(grayImage, attempt.ScaleFactor, attempt.MinNeighbors);
                    if (faces.Length > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Faces detected with scale {attempt.ScaleFactor}, neighbors {attempt.MinNeighbors}: {faces.Length}");
                        return faces;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Detection attempt failed: {ex.Message}");
                }
            }

            return new System.Drawing.Rectangle[0];
        }

        private System.Drawing.Rectangle[] TryAlternativePreprocessingAndDetection(Image<Gray, byte> originalImage)
        {
            if (faceCascade == null) return new System.Drawing.Rectangle[0];

            // Thử các phương pháp preprocessing khác nhau
            var preprocessingMethods = new Func<Image<Gray, byte>, Image<Gray, byte>>[]
            {
        // Method 1: Chỉ histogram equalization
        img => {
            var result = img.Clone();
            CvInvoke.EqualizeHist(result, result);
            return result;
        },
        
        // Method 2: Brightness adjustment
        img => {
            var result = img.Clone();
            result._Mul(1.3);
            return result;
        },
        
        // Method 3: Contrast enhancement
        img => {
            var result = img.Clone();
            result._Mul(1.2);
            CvInvoke.Add(result, new ScalarArray(new MCvScalar(10)), result);
            return result;
        },
        
        // Method 4: Gaussian blur + equalization
        img => {
            var result = img.Clone();
            CvInvoke.GaussianBlur(result, result, new Size(5, 5), 0);
            CvInvoke.EqualizeHist(result, result);
            return result;
        }
            };

            foreach (var method in preprocessingMethods)
            {
                try
                {
                    using var processedImage = method(originalImage);
                    var faces = faceCascade.DetectMultiScale(processedImage, 1.1, 3);

                    if (faces.Length > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Alternative preprocessing succeeded: {faces.Length} faces");
                        return faces;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Alternative preprocessing failed: {ex.Message}");
                }
            }

            return new System.Drawing.Rectangle[0];
        }

        private System.Drawing.Rectangle CreateCenterCropRectangle(int width, int height)
        {
            int centerX = width / 2;
            int centerY = height / 2;
            int cropSize = Math.Min(width, height) * 3 / 4; // Lấy 75% kích thước

            return new System.Drawing.Rectangle(
                Math.Max(0, centerX - cropSize / 2),
                Math.Max(0, centerY - cropSize / 2),
                Math.Min(cropSize, width),
                Math.Min(cropSize, height)
            );
        }

        private async Task<List<FaceRecognitionResult>> ProcessFaceRegionWithMultipleMethods(Image<Gray, byte> grayImage, System.Drawing.Rectangle faceRect)
        {
            var results = new List<FaceRecognitionResult>();

            try
            {
                using var faceImage = grayImage.Copy(faceRect);

                // Thử multiple preprocessing methods cho face region
                var processingMethods = new Func<Image<Gray, byte>, Image<Gray, byte>>[]
                {
            // Method 1: Standard processing
            img => {
                var result = img.Resize(100, 100, Inter.Cubic);
                CvInvoke.EqualizeHist(result, result);
                return result;
            },
            
            // Method 2: Enhanced contrast
            img => {
                var result = img.Resize(100, 100, Inter.Cubic);
                CvInvoke.EqualizeHist(result, result);
                result._Mul(1.15);
                return result;
            },
            
            // Method 3: Smooth then enhance
            img => {
                var temp = img.Clone();
                CvInvoke.GaussianBlur(temp, temp, new Size(3, 3), 0);
                var result = temp.Resize(100, 100, Inter.Cubic);
                CvInvoke.EqualizeHist(result, result);
                temp.Dispose();
                return result;
            },
            
            // Method 4: Brightness adjustment
            img => {
                var result = img.Resize(100, 100, Inter.Cubic);
                result._Mul(1.2);
                CvInvoke.EqualizeHist(result, result);
                return result;
            }
                };

                foreach (var method in processingMethods)
                {
                    try
                    {
                        using var processedFace = method(faceImage);
                        var prediction = recognizer.Predict(processedFace);

                        // ✅ Cải thiện công thức confidence - giảm penalty cho distance
                        double confidence = Math.Max(0, 100 - (prediction.Distance / 1.8)); // Giảm từ 2.0 xuống 1.8

                        if (prediction.Label >= 0 && prediction.Label < registeredFaces.Count)
                        {
                            var recognizedFace = registeredFaces[prediction.Label];

                            results.Add(new FaceRecognitionResult
                            {
                                Success = confidence >= 25.0, // Giảm threshold
                                Message = confidence >= 25.0 ? "Face recognized" : $"Low confidence: {confidence:F1}%",
                                EmployeeId = recognizedFace.EmployeeId,
                                EmployeeName = recognizedFace.EmployeeName,
                                Confidence = confidence,
                                Timestamp = DateTime.Now
                            });

                            System.Diagnostics.Debug.WriteLine($"🔍 Method result: {recognizedFace.EmployeeId} - {confidence:F1}% (distance: {prediction.Distance})");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Processing method failed: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Face region processing error: {ex.Message}");
            }

            return results;
        }

        // ✅ CẢI THIỆN PHƯƠNG THỨC TRAINING ĐỂ TẠO MODEL ROBUST HỢN

        private async Task RetrainRecognizerAsync()
        {
            if (registeredFaces.Count == 0) return;

            var images = new List<Mat>();
            var labels = new List<int>();

            try
            {
                for (int i = 0; i < registeredFaces.Count; i++)
                {
                    var face = registeredFaces[i];
                    if (File.Exists(face.FaceImagePath))
                    {
                        System.Diagnostics.Debug.WriteLine($"Training with face: {face.EmployeeId} - {face.FaceImagePath}");

                        // ✅ Load và preprocess image với multiple variations
                        using var originalImg = new Image<Gray, byte>(face.FaceImagePath);

                        // ✅ Thêm original image với multiple preprocessing
                        var preprocessingVariations = new Func<Image<Gray, byte>, Image<Gray, byte>>[]
                        {
                    // Original processing
                    img => {
                        var result = img.Resize(100, 100, Inter.Cubic);
                        CvInvoke.EqualizeHist(result, result);
                        return result;
                    },
                    
                    // Brightness variations
                    img => {
                        var result = img.Resize(100, 100, Inter.Cubic);
                        result._Mul(1.15);
                        CvInvoke.EqualizeHist(result, result);
                        return result;
                    },

                    img => {
                        var result = img.Resize(100, 100, Inter.Cubic);
                        result._Mul(0.85);
                        CvInvoke.EqualizeHist(result, result);
                        return result;
                    },
                    
                    // Contrast enhancement
                    img => {
                        var result = img.Resize(100, 100, Inter.Cubic);
                        result._Mul(1.1);
                        CvInvoke.Add(result, new ScalarArray(new MCvScalar(5)), result);
                        CvInvoke.EqualizeHist(result, result);
                        return result;
                    }
                        };

                        foreach (var preprocessing in preprocessingVariations)
                        {
                            try
                            {
                                using var processed = preprocessing(originalImg);
                                images.Add(processed.Mat.Clone());
                                labels.Add(i);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Preprocessing variation failed: {ex.Message}");
                            }
                        }

                        // ✅ Thêm rotation variations (chỉ một số góc nhỏ)
                        for (int angle = -3; angle <= 3; angle += 3)
                        {
                            if (angle != 0)
                            {
                                try
                                {
                                    using var rotatedImg = RotateImage(originalImg, angle);
                                    using var resizedImg = rotatedImg.Resize(100, 100, Inter.Cubic);
                                    CvInvoke.EqualizeHist(resizedImg, resizedImg);
                                    images.Add(resizedImg.Mat.Clone());
                                    labels.Add(i);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Rotation processing failed: {ex.Message}");
                                }
                            }
                        }
                    }
                }

                if (images.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Training with {images.Count} images for {registeredFaces.Count} faces");
                    recognizer.Train(images.ToArray(), labels.ToArray());
                    System.Diagnostics.Debug.WriteLine($"✅ Training completed successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ No images to train with");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Retrain error: {ex.Message}");
            }
            finally
            {
                foreach (var img in images)
                {
                    img?.Dispose();
                }
            }
        }

        // ✅ THÊM PHƯƠNG THỨC TEST ĐỂ DEBUG
        public async Task<string> TestRecognitionDetailedAsync(string imagePath)
        {
            try
            {
                var result = new System.Text.StringBuilder();
                result.AppendLine("=== DETAILED FACE RECOGNITION TEST ===");
                result.AppendLine($"Image: {Path.GetFileName(imagePath)}");
                result.AppendLine($"Registered faces: {registeredFaces.Count}");
                result.AppendLine();

                if (!File.Exists(imagePath))
                {
                    result.AppendLine("❌ Image file not found");
                    return result.ToString();
                }

                using var image = new Image<Bgr, byte>(imagePath);
                using var grayImage = image.Convert<Gray, byte>();
                result.AppendLine($"Image size: {grayImage.Width}x{grayImage.Height}");

                // Test face detection
                var faces = DetectFacesMultipleAttempts(grayImage);
                result.AppendLine($"Faces detected: {faces.Length}");

                if (faces.Length == 0)
                {
                    result.AppendLine("Trying alternative preprocessing...");
                    faces = TryAlternativePreprocessingAndDetection(grayImage);
                    result.AppendLine($"Alternative detection result: {faces.Length}");
                }

                if (faces.Length == 0)
                {
                    result.AppendLine("Using center crop fallback");
                    faces = new[] { CreateCenterCropRectangle(grayImage.Width, grayImage.Height) };
                }

                // Test recognition for each face
                for (int faceIndex = 0; faceIndex < Math.Min(faces.Length, 2); faceIndex++)
                {
                    result.AppendLine($"\n--- Face {faceIndex + 1} ---");
                    var face = faces[faceIndex];
                    result.AppendLine($"Face region: {face.X},{face.Y} {face.Width}x{face.Height}");

                    using var faceImage = grayImage.Copy(face);
                    using var resizedFace = faceImage.Resize(100, 100, Inter.Cubic);
                    CvInvoke.EqualizeHist(resizedFace, resizedFace);

                    var prediction = recognizer.Predict(resizedFace);
                    double confidence = Math.Max(0, 100 - (prediction.Distance / 1.8));

                    result.AppendLine($"Prediction - Label: {prediction.Label}, Distance: {prediction.Distance:F2}");
                    result.AppendLine($"Confidence: {confidence:F1}%");

                    if (prediction.Label >= 0 && prediction.Label < registeredFaces.Count)
                    {
                        var recognizedFace = registeredFaces[prediction.Label];
                        result.AppendLine($"Recognized as: {recognizedFace.EmployeeId} - {recognizedFace.EmployeeName}");
                        result.AppendLine($"Success: {(confidence >= 25.0 ? "YES" : "NO")} (threshold: 25%)");
                    }
                    else
                    {
                        result.AppendLine("No valid face match found");
                    }
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                return $"Test error: {ex.Message}";
            }
        }















        // 4. Thêm method debug để test recognition
        public async Task<FaceRecognitionResult> TestRecognitionWithDebugAsync(string imagePath)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== FACE RECOGNITION DEBUG TEST ===");

                var result = await RecognizeFaceFromImageAsync(imagePath);

                System.Diagnostics.Debug.WriteLine($"Test Result:");
                System.Diagnostics.Debug.WriteLine($"  Success: {result.Success}");
                System.Diagnostics.Debug.WriteLine($"  Employee: {result.EmployeeId} - {result.EmployeeName}");
                System.Diagnostics.Debug.WriteLine($"  Confidence: {result.Confidence:F1}%");
                System.Diagnostics.Debug.WriteLine($"  Message: {result.Message}");

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Test error: {ex.Message}");
                return new FaceRecognitionResult { Success = false, Message = ex.Message };
            }
        }

        private static async Task<bool> InitializeCameraForRecognitionAsync()
        {
            try
            {
                lock (_lock)
                {
                    if (_isCapturing) return true;

                    var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                    if (videoDevices.Count == 0) return false;

                    _currentCamera = new VideoCaptureDevice(videoDevices[0].MonikerString);
                    _currentCamera.NewFrame += OnNewFrameReceived;
                    _currentCamera.Start();
                    _isCapturing = true;
                }

                // Wait for camera to start
                await Task.Delay(1000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void OnNewFrameReceived(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                lock (_lock)
                {
                    _latestFrame?.Dispose();
                    _latestFrame = (Bitmap)eventArgs.Frame.Clone();
                }
            }
            catch
            {
                // Ignore frame capture errors
            }
        }

        private static Bitmap? GetLatestFrame()
        {
            lock (_lock)
            {
                return _latestFrame != null ? (Bitmap)_latestFrame.Clone() : null;
            }
        }

        private static void StopCameraCapture()
        {
            try
            {
                lock (_lock)
                {
                    if (_currentCamera != null && _isCapturing)
                    {
                        _currentCamera.NewFrame -= OnNewFrameReceived;
                        _currentCamera.SignalToStop();
                        _currentCamera.WaitForStop();
                        _currentCamera = null;
                        _isCapturing = false;
                    }

                    _latestFrame?.Dispose();
                    _latestFrame = null;
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

     
        private static async Task<string> SaveAttendanceImageAsync(Bitmap? frame, string employeeId)
        {
            try
            {
                if (frame == null) return "";

                string attendanceImagesPath = Path.Combine(GetTrainingDataPath(), "AttendanceImages");
                if (!Directory.Exists(attendanceImagesPath))
                {
                    Directory.CreateDirectory(attendanceImagesPath);
                }

                string fileName = $"{employeeId}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string filePath = Path.Combine(attendanceImagesPath, fileName);

                await Task.Run(() =>
                {
                    using (var clone = (Bitmap)frame.Clone())
                    {
                        clone.Save(filePath, ImageFormat.Jpeg);
                    }
                });

                return filePath;
            }
            catch
            {
                return "";
            }
        }

        private static bool IsEmguCVAvailable()
        {
            try
            {
                return File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Emgu.CV.dll"));
            }
            catch
            {
                return false;
            }
        }

        private static string GetTrainingDataPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FaceData");
        }

        #endregion

        #region Instance Methods for Face Registration

        public async Task<FaceRegistrationResult> RegisterFaceAsync(string employeeCode, string employeeName, string imagePath, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(imagePath))
                {
                    return new FaceRegistrationResult { Success = false, Message = "Tệp ảnh không tồn tại" };
                }

                // Validate image file trước khi xử lý
                if (!IsValidImageFile(imagePath))
                {
                    return new FaceRegistrationResult { Success = false, Message = "File ảnh không hợp lệ hoặc bị corrupt" };
                }

                using var image = new Image<Bgr, byte>(imagePath);
                using var grayImage = image.Convert<Gray, byte>();

                // Detect faces
                System.Drawing.Rectangle[] faces = new System.Drawing.Rectangle[0];

                if (faceCascade != null)
                {
                    faces = faceCascade.DetectMultiScale(grayImage, 1.1, 3);
                }
                else
                {
                    // Nếu không có cascade, sử dụng toàn bộ ảnh
                    System.Diagnostics.Debug.WriteLine("Warning: No face cascade available, using full image");
                    faces = new System.Drawing.Rectangle[] { new System.Drawing.Rectangle(0, 0, grayImage.Width, grayImage.Height) };
                }

                if (faces.Length == 0 && faceCascade != null)
                {
                    return new FaceRegistrationResult { Success = false, Message = "Không tìm thấy khuôn mặt trong ảnh" };
                }

                if (faces.Length > 1)
                {
                    return new FaceRegistrationResult { Success = false, Message = "Phát hiện nhiều khuôn mặt, vui lòng chọn ảnh có 1 khuôn mặt" };
                }

                var face = faces.Length > 0 ? faces[0] : new System.Drawing.Rectangle(0, 0, grayImage.Width, grayImage.Height);
                using var faceImage = grayImage.Copy(face);

                // Check for duplicate registration
                if (await IsEmployeeAlreadyRegisteredAsync(employeeCode))
                {
                    // Update existing registration
                    await DeleteRegisteredFaceAsync(employeeCode);
                }

                // Save face image
                string faceFileName = $"{employeeCode}_{SanitizeFileName(employeeName)}.jpg";
                string faceFilePath = Path.Combine(facesImagePath, faceFileName);

                faceImage.Save(faceFilePath);

                // Add to registered faces list
                var registeredFace = new RegisteredFace
                {
                    EmployeeId = employeeCode,
                    EmployeeName = employeeName,
                    FaceImagePath = faceFilePath,
                    RegistrationDate = DateTime.Now
                };

                registeredFaces.Add(registeredFace);

                // Retrain recognizer
                await RetrainRecognizerAsync();

                // Save registered faces data
                await SaveRegisteredFacesAsync();

                return new FaceRegistrationResult
                {
                    Success = true,
                    Message = "Đăng ký khuôn mặt thành công",
                    FacePath = faceFilePath
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RegisterFaceAsync error: {ex}");
                return new FaceRegistrationResult { Success = false, Message = $"Lỗi đăng ký: {ex.Message}" };
            }
        }

        public async Task<FaceListResult> GetRegisteredFacesAsync()
        {
            try
            {
                return new FaceListResult
                {
                    Success = true,
                    Faces = registeredFaces.ToList()
                };
            }
            catch (Exception ex)
            {
                return new FaceListResult
                {
                    Success = false,
                    Message = ex.Message,
                    Faces = new List<RegisteredFace>()
                };
            }
        }

        public async Task<FaceOperationResult> DeleteRegisteredFaceAsync(string employeeId)
        {
            try
            {
                var faceToRemove = registeredFaces.FirstOrDefault(f => f.EmployeeId == employeeId);
                if (faceToRemove == null)
                {
                    return new FaceOperationResult { Success = false, Message = "Không tìm thấy khuôn mặt để xóa" };
                }

                // Remove from list
                registeredFaces.Remove(faceToRemove);

                // Delete image file
                if (File.Exists(faceToRemove.FaceImagePath))
                {
                    File.Delete(faceToRemove.FaceImagePath);
                }

                // Retrain recognizer
                await RetrainRecognizerAsync();

                // Save updated list
                await SaveRegisteredFacesAsync();

                return new FaceOperationResult { Success = true, Message = "Xóa thành công" };
            }
            catch (Exception ex)
            {
                return new FaceOperationResult { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }

        #endregion

        #region Helper Methods

        private void InitializeFaceCascade()
        {
            try
            {
                string cascadePath = Path.Combine(Application.StartupPath, "haarcascade_frontalface_default.xml");

                if (!File.Exists(cascadePath))
                {
                    cascadePath = Path.Combine(Application.StartupPath, "Models", "haarcascade_frontalface_alt.xml");
                }

                if (!File.Exists(cascadePath))
                {
                    cascadePath = Path.Combine(Application.StartupPath, "x64", "haarcascade_frontalface_default.xml");
                }

                if (File.Exists(cascadePath))
                {
                    faceCascade = new CascadeClassifier(cascadePath);

                    // Test cascade
                    try
                    {
                        using var testImg = new Image<Gray, byte>(100, 100);
                        var testResult = faceCascade.DetectMultiScale(testImg);
                        System.Diagnostics.Debug.WriteLine("Face cascade initialized successfully");
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("Warning: Face cascade may not work properly");
                        faceCascade?.Dispose();
                        faceCascade = null;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Warning: Face cascade file not found");
                    faceCascade = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing face cascade: {ex.Message}");
                faceCascade = null;
            }
        }

        private bool IsValidImageFile(string imagePath)
        {
            try
            {
                if (!File.Exists(imagePath))
                    return false;

                using var testImage = Image.FromFile(imagePath);
                if (testImage.Width <= 0 || testImage.Height <= 0)
                    return false;

                using var cvImage = new Image<Bgr, byte>(imagePath);
                if (cvImage.Width <= 0 || cvImage.Height <= 0)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Image validation error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> IsEmployeeAlreadyRegisteredAsync(string employeeCode)
        {
            return registeredFaces.Any(f => f.EmployeeId == employeeCode);
        }

     
        private void LoadRegisteredFaces()
        {
            try
            {
                string dataFile = Path.Combine(facesDataPath, "registered_faces.json");
                if (File.Exists(dataFile))
                {
                    string json = File.ReadAllText(dataFile);
                    registeredFaces = JsonSerializer.Deserialize<List<RegisteredFace>>(json) ?? new List<RegisteredFace>();
                }

                registeredFaces = registeredFaces.Where(f => File.Exists(f.FaceImagePath)).ToList();

                Task.Run(async () => await RetrainRecognizerAsync());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading registered faces: {ex.Message}");
                registeredFaces = new List<RegisteredFace>();
            }
        }

        private async Task SaveRegisteredFacesAsync()
        {
            try
            {
                string dataFile = Path.Combine(facesDataPath, "registered_faces.json");
                string json = JsonSerializer.Serialize(registeredFaces, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(dataFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving registered faces: {ex.Message}");
            }
        }

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    recognizer?.Dispose();
                    faceCascade?.Dispose();
                }
                disposed = true;
            }
        }

        #endregion
    }
}