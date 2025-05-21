using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using EmployeeManagement.Utilities;
 
namespace EmployeeManagement.Utilities
{
    public class ExcelExport
    {
        /// <summary>
        /// Xuất dữ liệu ra file Excel
        /// </summary>
        /// <param name="data">Dữ liệu cần xuất</param>
        /// <param name="filePath">Đường dẫn file</param>
        /// <param name="sheetName">Tên sheet</param>
        /// <returns>True nếu thành công</returns>
        public bool ExportToExcel<T>(List<T> data, string filePath, string sheetName = "Sheet1")
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Workbook workbook = null;
            Worksheet worksheet = null;

            try
            {
                // Tạo Excel Application
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                // Tạo Workbook mới
                workbook = excelApp.Workbooks.Add();
                worksheet = (Worksheet)workbook.ActiveSheet;
                worksheet.Name = sheetName;

                if (data == null || data.Count == 0)
                {
                    worksheet.Cells[1, 1] = "Không có dữ liệu";
                    workbook.SaveAs(filePath);
                    return true;
                }

                // Lấy properties của object
                PropertyInfo[] properties = typeof(T).GetProperties();

                // Tạo header
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1] = GetDisplayName(properties[i].Name);
                }

                // Format header
                Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, properties.Length]];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                headerRange.Borders.LineStyle = XlLineStyle.xlContinuous;

                // Thêm dữ liệu
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(data[row]);
                        worksheet.Cells[row + 2, col + 1] = value?.ToString() ?? "";
                    }
                }

                // Auto-fit columns
                worksheet.Columns.AutoFit();

                // Thêm borders cho toàn bộ dữ liệu
                Microsoft.Office.Interop.Excel.Range dataRange = worksheet.Range[
                    worksheet.Cells[1, 1],
                    worksheet.Cells[data.Count + 1, properties.Length]
                ];
                dataRange.Borders.LineStyle = XlLineStyle.xlContinuous;

                // Thêm thông tin footer
                int footerRow = data.Count + 3;
                worksheet.Cells[footerRow, 1] = $"Tổng số bản ghi: {data.Count}";
                worksheet.Cells[footerRow + 1, 1] = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                worksheet.Cells[footerRow + 2, 1] = "Được tạo bởi: Employee Management System";

                // Lưu file
                workbook.SaveAs(filePath);

                Logger.LogInfo($"Xuất Excel thành công: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi xuất Excel: {ex.Message}");
                throw new Exception($"Không thể xuất file Excel: {ex.Message}");
            }
            finally
            {
                // Cleanup
                try
                {
                    if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    if (workbook != null)
                    {
                        workbook.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    }
                    if (excelApp != null)
                    {
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }
                catch { /* Ignore cleanup errors */ }
            }
        }

        /// <summary>
        /// Xuất DataTable ra Excel bằng Interop
        /// </summary>
        public bool ExportDataTableToExcel(System.Data.DataTable dataTable, string filePath, string sheetName = "Sheet1")
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Workbook workbook = null;
            Worksheet worksheet = null;

            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                workbook = excelApp.Workbooks.Add();
                worksheet = (Worksheet)workbook.ActiveSheet;
                worksheet.Name = sheetName;

                // Add headers
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
                }

                // Format header
                Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, dataTable.Columns.Count]];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                // Add data
                for (int row = 0; row < dataTable.Rows.Count; row++)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        worksheet.Cells[row + 2, col + 1] = dataTable.Rows[row][col]?.ToString() ?? "";
                    }
                }

                // Auto-fit columns
                worksheet.Columns.AutoFit();

                // Add footer
                int footerRow = dataTable.Rows.Count + 3;
                worksheet.Cells[footerRow, 1] = $"Tổng số bản ghi: {dataTable.Rows.Count}";
                worksheet.Cells[footerRow + 1, 1] = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";

                // Save file
                workbook.SaveAs(filePath);

                Logger.LogInfo($"Xuất Excel (DataTable) thành công: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi xuất Excel (DataTable): {ex.Message}");
                throw new Exception($"Không thể xuất file Excel: {ex.Message}");
            }
            finally
            {
                try
                {
                    if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    if (workbook != null)
                    {
                        workbook.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    }
                    if (excelApp != null)
                    {
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Tạo file Excel template bằng Interop
        /// </summary>
        public bool CreateTemplate(string[] headers, string filePath, string sheetName = "Template")
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Workbook workbook = null;
            Worksheet worksheet = null;

            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                workbook = excelApp.Workbooks.Add();
                worksheet = (Worksheet)workbook.ActiveSheet;
                worksheet.Name = sheetName;

                // Add headers
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1] = headers[i];
                }

                // Format headers
                Microsoft.Office.Interop.Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, headers.Length]];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);

                // Add sample data row
                worksheet.Cells[2, 1] = "Dữ liệu mẫu...";

                // Auto-fit columns
                worksheet.Columns.AutoFit();

                // Add instructions
                worksheet.Cells[4, 1] = "Hướng dẫn:";
                worksheet.Cells[5, 1] = "1. Nhập dữ liệu từ dòng 2 trở đi";
                worksheet.Cells[6, 1] = "2. Không được xóa hoặc thay đổi header (dòng 1)";
                worksheet.Cells[7, 1] = "3. Lưu file với định dạng .xlsx";

                // Save file
                workbook.SaveAs(filePath);

                Logger.LogInfo($"Tạo template Excel thành công: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Lỗi tạo template Excel: {ex.Message}");
                return false;
            }
            finally
            {
                try
                {
                    if (worksheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    if (workbook != null)
                    {
                        workbook.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    }
                    if (excelApp != null)
                    {
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Chuyển đổi tên property thành tên hiển thị
        /// </summary>
        private string GetDisplayName(string propertyName)
        {
            return propertyName switch
            {
                "ProjectCode" => "Mã dự án",
                "ProjectName" => "Tên dự án",
                "ManagerName" => "Quản lý",
                "StartDate" => "Ngày bắt đầu",
                "EndDate" => "Ngày kết thúc",
                "Budget" => "Ngân sách",
                "Status" => "Trạng thái",
                "CompletionPercentage" => "Tiến độ (%)",
                "TotalTasks" => "Tổng công việc",
                "CompletedTasks" => "CV hoàn thành",
                "TotalEmployees" => "Tổng nhân viên",
                "ActualCost" => "Chi phí thực tế",
                "Notes" => "Ghi chú",
                "IsOverdue" => "Quá hạn",
                "DaysRemaining" => "Ngày còn lại",
                "DaysOverdue" => "Ngày quá hạn",
                "MaDuAn" => "Mã dự án",
                "TenDuAn" => "Tên dự án",
                "QuanLy" => "Quản lý",
                "NgayBatDau" => "Ngày bắt đầu",
                "NgayKetThuc" => "Ngày kết thúc",
                "NganSach" => "Ngân sách",
                "TrangThai" => "Trạng thái",
                "TienDo" => "Tiến độ",
                "TongCongViec" => "Tổng công việc",
                "CongViecHoanThanh" => "CV hoàn thành",
                "TongNhanVien" => "Tổng nhân viên",
                "ChiPhiThucTe" => "Chi phí thực tế",
                "TrangThaiQuaHan" => "Tình trạng",
                _ => propertyName
            };
        }
    }
}