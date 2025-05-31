using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.DAL;
using EmployeeManagement.Models;
using EmployeeManagement.Models.DTO;

namespace EmployeeManagement.BLL
{
    /// <summary>
    /// Business Logic Layer cho Position
    /// </summary>
    public class PositionBLL
    {
        private readonly PositionDAL _positionDAL;

        /// <summary>
        /// Khởi tạo với connection string mặc định
        /// </summary>
        public PositionBLL()
        {
            _positionDAL = new PositionDAL();
        }

        /// <summary>
        /// Khởi tạo với connection string chỉ định
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối</param>
        public PositionBLL(string connectionString)
        {
            _positionDAL = new PositionDAL(connectionString);
        }

        /// <summary>
        /// Lấy tất cả chức vụ
        /// </summary>
        /// <returns>Danh sách PositionDTO</returns>
        public List<PositionDTO> GetAllPositions()
        {
            try
            {
                return _positionDAL.GetAllPositions();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách chức vụ: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thông tin của một chức vụ theo ID
        /// </summary>
        /// <param name="positionId">ID của chức vụ</param>
        /// <returns>PositionDTO</returns>
        public PositionDTO GetPositionById(int positionId)
        {
            try
            {
                return _positionDAL.GetPositionById(positionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin chức vụ: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm chức vụ mới
        /// </summary>
        /// <param name="positionDTO">Đối tượng PositionDTO</param>
        /// <returns>ID của chức vụ mới được thêm</returns>
        public int AddPosition(PositionDTO positionDTO)
        {
            try
            {
                // Validation
                ValidatePosition(positionDTO);

                // Thêm vào database
                return _positionDAL.AddPosition(positionDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm chức vụ: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin chức vụ
        /// </summary>
        /// <param name="positionDTO">Đối tượng PositionDTO</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool UpdatePosition(PositionDTO positionDTO)
        {
            try
            {
                // Validation
                ValidatePosition(positionDTO, true);

                // Cập nhật trong database
                return _positionDAL.UpdatePosition(positionDTO);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật chức vụ: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa chức vụ
        /// </summary>
        /// <param name="positionId">ID của chức vụ cần xóa</param>
        /// <returns>true nếu xóa thành công</returns>
        public bool DeletePosition(int positionId)
        {
            try
            {
                // Kiểm tra xem chức vụ có tồn tại không
                var position = _positionDAL.GetPositionById(positionId);
                if (position == null)
                    throw new ArgumentException($"Không tìm thấy chức vụ với ID {positionId}");

                // Xóa trong database (sẽ kiểm tra ràng buộc trong DAL)
                return _positionDAL.DeletePosition(positionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa chức vụ: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm chức vụ theo các tiêu chí
        /// </summary>
        /// <param name="searchText">Văn bản tìm kiếm</param>
        /// <param name="minSalary">Lương cơ bản tối thiểu</param>
        /// <param name="maxSalary">Lương cơ bản tối đa</param>
        /// <returns>Danh sách PositionDTO</returns>
        public List<PositionDTO> SearchPositions(string searchText, decimal? minSalary = null, decimal? maxSalary = null)
        {
            try
            {
                return _positionDAL.SearchPositions(searchText, minSalary, maxSalary);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm chức vụ: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy thống kê về chức vụ
        /// </summary>
        /// <returns>Thống kê chức vụ</returns>
        public PositionStatisticsDTO GetPositionStatistics()
        {
            try
            {
                var positions = _positionDAL.GetAllPositions();

                var lowestSalary = positions.Any() ? positions.Min(p => p.BaseSalary) : 0;
                var highestSalary = positions.Any() ? positions.Max(p => p.BaseSalary) : 0;
                var averageSalary = positions.Any() ? positions.Average(p => p.BaseSalary) : 0;

                var lowestSalaryPosition = positions.FirstOrDefault(p => p.BaseSalary == lowestSalary);
                var highestSalaryPosition = positions.FirstOrDefault(p => p.BaseSalary == highestSalary);

                var totalEmployees = positions.Sum(p => p.EmployeeCount);
                var mostPopularPosition = positions.OrderByDescending(p => p.EmployeeCount).FirstOrDefault();
                var leastPopularPosition = positions.Where(p => p.EmployeeCount > 0).OrderBy(p => p.EmployeeCount).FirstOrDefault();
                var unusedPositions = positions.Count(p => p.EmployeeCount == 0);

                return new PositionStatisticsDTO
                {
                    TotalPositions = positions.Count,
                    TotalEmployees = totalEmployees,
                    LowestSalary = lowestSalary,
                    HighestSalary = highestSalary,
                    AverageSalary = averageSalary,
                    LowestSalaryPositionName = lowestSalaryPosition?.PositionName ?? "Không có",
                    HighestSalaryPositionName = highestSalaryPosition?.PositionName ?? "Không có",
                    MostPopularPositionName = mostPopularPosition?.PositionName ?? "Không có",
                    MostPopularPositionCount = mostPopularPosition?.EmployeeCount ?? 0,
                    LeastPopularPositionName = leastPopularPosition?.PositionName ?? "Không có",
                    LeastPopularPositionCount = leastPopularPosition?.EmployeeCount ?? 0,
                    UnusedPositionsCount = unusedPositions
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê chức vụ: {ex.Message}", ex);
            }
        }

        #region Helper Methods
        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu chức vụ
        /// </summary>
        /// <param name="positionDTO">Đối tượng PositionDTO cần kiểm tra</param>
        /// <param name="isUpdate">true nếu là cập nhật, false nếu là thêm mới</param>
        private void ValidatePosition(PositionDTO positionDTO, bool isUpdate = false)
        {
            if (positionDTO == null)
                throw new ArgumentNullException(nameof(positionDTO), "Dữ liệu chức vụ không được phép null");

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(positionDTO.PositionName))
                throw new ArgumentException("Tên chức vụ không được để trống", nameof(positionDTO.PositionName));

            if (positionDTO.PositionName.Length > 100)
                throw new ArgumentException("Tên chức vụ không được vượt quá 100 ký tự", nameof(positionDTO.PositionName));

            if (positionDTO.Description?.Length > 255)
                throw new ArgumentException("Mô tả chức vụ không được vượt quá 255 ký tự", nameof(positionDTO.Description));

            if (positionDTO.BaseSalary < 0)
                throw new ArgumentException("Lương cơ bản không được âm", nameof(positionDTO.BaseSalary));

            // Kiểm tra trùng tên khi thêm mới
            if (!isUpdate)
            {
                if (_positionDAL.IsPositionNameExists(positionDTO.PositionName))
                    throw new ArgumentException("Tên chức vụ đã tồn tại", nameof(positionDTO.PositionName));
            }
            // Kiểm tra trùng tên khi cập nhật
            else
            {
                if (_positionDAL.IsPositionNameExists(positionDTO.PositionName, positionDTO.PositionID))
                    throw new ArgumentException("Tên chức vụ đã tồn tại", nameof(positionDTO.PositionName));
            }
        }
        #endregion
    }

    /// <summary>
    /// DTO cho thống kê chức vụ
    /// </summary>
    
}