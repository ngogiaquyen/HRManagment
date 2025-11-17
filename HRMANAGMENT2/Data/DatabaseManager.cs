using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace HRManagementApp.Controllers
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
            try
            {
                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                _connectionString = config.GetConnectionString("HRConnection");
                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new InvalidOperationException("Không tìm thấy connection string 'HRConnection' trong appsettings.json.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khởi tạo DatabaseManager: {ex.Message}", ex);
            }
        }

        public DataTable GetDataTable(string query, SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi truy vấn dữ liệu: {ex.Message}", ex);
            }
        }

        public void ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thực thi câu lệnh: {ex.Message}", ex);
            }
        }

        public object ExecuteScalar(string query, SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thực thi câu lệnh scalar: {ex.Message}", ex);
            }
        }

        public string SaveFileToDataFolder(string sourcePath)
        {
            try
            {
                if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
                    return null;

                string targetDirectory = @"C:\data\";
                if (!Directory.Exists(targetDirectory))
                    Directory.CreateDirectory(targetDirectory);

                string fileName = Path.GetFileName(sourcePath);
                string targetPath = Path.Combine(targetDirectory, fileName);

                string srcFull = Path.GetFullPath(sourcePath);
                string dstFull = Path.GetFullPath(targetPath);
                if (string.Equals(srcFull, dstFull, StringComparison.OrdinalIgnoreCase))
                    return targetPath;

                File.Copy(sourcePath, targetPath, true);
                return targetPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu tệp: {ex.Message}", ex);
            }
        }

        public string SaveFileForEmployee(string sourcePath, string employeeName, string logicalDocumentName)
        {
            try
            {
                if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
                    return null;

                string safeEmployee = MakeSafeFolderName(employeeName);
                string baseDir = @"C:\data";
                string employeeDir = Path.Combine(baseDir, safeEmployee);
                if (!Directory.Exists(employeeDir))
                    Directory.CreateDirectory(employeeDir);

                string ext = Path.GetExtension(sourcePath);
                string safeDoc = MakeSafeFileName(logicalDocumentName);
                string fileName = $"{safeDoc}{ext}";
                string targetPath = Path.Combine(employeeDir, fileName);
                string srcFull = Path.GetFullPath(sourcePath);
                string dstFull = Path.GetFullPath(targetPath);
                if (!string.Equals(srcFull, dstFull, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(sourcePath, targetPath, true);
                }
                return targetPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu tệp theo nhân viên: {ex.Message}", ex);
            }
        }

        private static string MakeSafeFolderName(string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            return input?.Trim();
        }

        private static string MakeSafeFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "TaiLieu";
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            return input.Trim();
        }
    }
}