using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;

namespace quản_lý_kho.Data
{
    // Simple helper to load data tables from the configured connection string
    internal static class DatabaseHelper
    {
        private static string ConnectionString
        {
            get
            {
                // Try to read connection string from AppDomain config file manually to avoid requiring System.Configuration reference.
                try
                {
                    var configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    if (File.Exists(configPath))
                    {
                        var doc = XDocument.Load(configPath);
                        var add = doc.Root?
                            .Element("connectionStrings")?
                            .Element("add");
                        if (add != null)
                        {
                            var nameAttr = add.Attribute("name");
                            if (nameAttr != null && nameAttr.Value == "DefaultConnection")
                                return add.Attribute("connectionString")?.Value;
                        }
                        // If there are multiple <add> entries, try to find by name
                        var entries = doc.Root?.Element("connectionStrings")?.Elements("add");
                        if (entries != null)
                        {
                            foreach (var e in entries)
                            {
                                var n = e.Attribute("name")?.Value;
                                if (string.Equals(n, "DefaultConnection", StringComparison.OrdinalIgnoreCase))
                                    return e.Attribute("connectionString")?.Value;
                            }
                        }
                    }
                }
                catch
                {
                    // swallow and return null, caller will handle
                }

                return null;
            }
        }

        public static DataTable LoadTable(string sql)
        {
            var dt = new DataTable();
            if (string.IsNullOrEmpty(ConnectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured in App.config.");

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                conn.Open();
                adapter.Fill(dt);
            }

            return dt;
        }

        public static DataTable LoadTable(string sql, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            if (string.IsNullOrEmpty(ConnectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured in App.config.");

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                adapter.Fill(dt);
            }

            return dt;
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured in App.config.");

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataTable LoadTablePaged(string baseSql, int pageIndex, int pageSize, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
                throw new ArgumentException("orderBy must be provided for paging.");

            var offset = (pageIndex - 1) * pageSize;
            var sql = baseSql + $" ORDER BY {orderBy} OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            return LoadTable(sql);
        }
    }
}
