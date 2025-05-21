using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace EmployeeManagement
{
    public class DatabaseHelper
    {
         private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EmployeeManagement"].ConnectionString;
        }

        // Kiểm tra kết nối
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
                return false;
            }
        }

        // Phương thức lấy dữ liệu
        public DataTable GetData(string query)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy dữ liệu: " + ex.Message);
            }

            return dataTable;
        }

        // Phương thức thực thi câu lệnh (Insert, Update, Delete)
        public bool ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực thi truy vấn: " + ex.Message);
                return false;
            }
        }

        // Phương thức trả về giá trị đơn (vd: COUNT, SUM, ...)
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thực thi truy vấn: " + ex.Message);
                return null;
            }
        }
    }
}