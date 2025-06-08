//using System;

//namespace EmployeeManagement.Utilities
//{
//    /// <summary>
//    /// Kết quả kiểm tra tình trạng hệ thống Face Recognition
//    /// </summary>
//    namespace EmployeeManagement.Utilities
//    {
//        public class SystemCheckResult
//        {
//            public bool IsReady { get; set; }
//            public string Message { get; set; } = "";
//            public string ErrorMessage { get; set; } = "";
//            public DateTime CheckTime { get; set; } = DateTime.Now;
//            public Exception? Exception { get; set; }

//            public static SystemCheckResult Success(string message = "Hệ thống sẵn sàng")
//            {
//                return new SystemCheckResult
//                {
//                    IsReady = true,
//                    Message = message,
//                    ErrorMessage = "",
//                    CheckTime = DateTime.Now
//                };
//            }

//            public static SystemCheckResult Failure(string errorMessage, Exception? exception = null)
//            {
//                return new SystemCheckResult
//                {
//                    IsReady = false,
//                    Message = "",
//                    ErrorMessage = errorMessage,
//                    Exception = exception,
//                    CheckTime = DateTime.Now
//                };
//            }
//        }
//    }
//}