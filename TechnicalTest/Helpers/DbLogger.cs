using System;
using System.Data.SqlClient;

namespace TechnicalTest.Helpers
{
    /// <summary>
    /// Menyimpan detail exception ke tabel ErrorLog.
    /// Kegagalan saat logging SENGAJA ditelan (try-catch kosong) supaya
    /// proses logging tidak menyebabkan exception baru / infinite loop
    /// saat exception filter menangani error.
    /// </summary>
    public static class DbLogger
    {
        public static void LogException(Exception ex, string controllerName, string actionName, string username, string requestUrl)
        {
            if (ex == null) return;

            try
            {
                DbHelper.ExecuteNonQuery(
                    "sp_ErrorLog_Insert",
                    new SqlParameter("@ControllerName", (object)controllerName ?? DBNull.Value),
                    new SqlParameter("@ActionName", (object)actionName ?? DBNull.Value),
                    new SqlParameter("@ExceptionMessage", ex.Message ?? string.Empty),
                    new SqlParameter("@StackTrace", (object)ex.StackTrace ?? DBNull.Value),
                    new SqlParameter("@InnerException", (object)(ex.InnerException != null ? ex.InnerException.Message : null) ?? DBNull.Value),
                    new SqlParameter("@Username", (object)username ?? DBNull.Value),
                    new SqlParameter("@RequestUrl", (object)requestUrl ?? DBNull.Value));
            }
            catch
            {
                // Sengaja ditelan. Fallback file-log bisa ditambahkan di sini kalau perlu.
            }
        }
    }
}
