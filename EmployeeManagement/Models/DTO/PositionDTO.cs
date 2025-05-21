using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models.DTO
{
    public class PositionStatisticsDTO
    {
        public int TotalPositions { get; set; }
        public int TotalEmployees { get; set; }
        public decimal LowestSalary { get; set; }
        public decimal HighestSalary { get; set; }
        public decimal AverageSalary { get; set; }
        public string LowestSalaryPositionName { get; set; }
        public string HighestSalaryPositionName { get; set; }
        public string MostPopularPositionName { get; set; }
        public int MostPopularPositionCount { get; set; }
        public string LeastPopularPositionName { get; set; }
        public int LeastPopularPositionCount { get; set; }
        public int UnusedPositionsCount { get; set; }
    }
    /// <summary>
    /// Data Transfer Object cho Position dùng trong hiển thị và truyền dữ liệu
    /// </summary>
    public class PositionDTO
    {
        // Thông tin cơ bản
        public int PositionID { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }

        // Thông tin bổ sung
        public int EmployeeCount { get; set; }

        // Thông tin thời gian
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Tạo PositionDTO từ Position model
        /// </summary>
        /// <param name="position">Position model</param>
        /// <param name="employeeCount">Số lượng nhân viên (optional)</param>
        /// <returns>PositionDTO</returns>
        public static PositionDTO FromPosition(Position position, int? employeeCount = null)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            return new PositionDTO
            {
                PositionID = position.PositionID,
                PositionName = position.PositionName,
                Description = position.Description ?? string.Empty,
                BaseSalary = position.BaseSalary,
                EmployeeCount = employeeCount ?? position.Employees?.Count ?? 0,
                CreatedAt = position.CreatedAt,
                UpdatedAt = position.UpdatedAt
            };
        }

        /// <summary>
        /// Tạo danh sách PositionDTO từ danh sách Position
        /// </summary>
        /// <param name="positions">Danh sách Position</param>
        /// <returns>Danh sách PositionDTO</returns>
        public static List<PositionDTO> FromPositionList(List<Position> positions)
        {
            if (positions == null)
                throw new ArgumentNullException(nameof(positions));

            List<PositionDTO> dtoList = new List<PositionDTO>();

            foreach (var position in positions)
            {
                dtoList.Add(FromPosition(position));
            }

            return dtoList;
        }

        /// <summary>
        /// Tạo Position model từ PositionDTO
        /// </summary>
        /// <returns>Position model</returns>
        public Position ToPosition()
        {
            return new Position
            {
                PositionID = this.PositionID,
                PositionName = this.PositionName,
                Description = string.IsNullOrEmpty(this.Description) ? null : this.Description,
                BaseSalary = this.BaseSalary,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt
            };
        }
    }

    /// <summary>
    /// Data Transfer Object cho hiển thị chức vụ trên DataGridView
    /// </summary>
    public class PositionDisplayModel
    {
        public int PositionID { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public string BaseSalaryFormatted { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Tạo PositionDisplayModel từ PositionDTO
        /// </summary>
        /// <param name="dto">PositionDTO</param>
        /// <returns>PositionDisplayModel</returns>
        public static PositionDisplayModel FromDTO(PositionDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new PositionDisplayModel
            {
                PositionID = dto.PositionID,
                PositionName = dto.PositionName,
                Description = dto.Description,
                BaseSalary = dto.BaseSalary,
                BaseSalaryFormatted = dto.BaseSalary.ToString("N0") + " VNĐ",
                EmployeeCount = dto.EmployeeCount,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        /// <summary>
        /// Tạo danh sách PositionDisplayModel từ danh sách PositionDTO
        /// </summary>
        /// <param name="dtoList">Danh sách PositionDTO</param>
        /// <returns>Danh sách PositionDisplayModel</returns>
        public static List<PositionDisplayModel> FromDTOList(List<PositionDTO> dtoList)
        {
            if (dtoList == null)
                throw new ArgumentNullException(nameof(dtoList));

            List<PositionDisplayModel> displayList = new List<PositionDisplayModel>();

            foreach (var dto in dtoList)
            {
                displayList.Add(FromDTO(dto));
            }

            return displayList;
        }
    }
}