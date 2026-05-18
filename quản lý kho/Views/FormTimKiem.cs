using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using quản_lý_kho.Data;

namespace quản_lý_kho.Views
{
    public partial class FormTimKiem : Form
    {
        public FormTimKiem()
        {
            InitializeComponent();
            Load += FormTimKiem_Load;
            btnTimKiem.Click += BtnTimKiem_Click;
            btnReload.Click += (s, e) => { txtTuKhoa.Clear(); LoadAllData(); };
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvData.ReadOnly = true;
            dgvData.AllowUserToAddRows = false;
        }

        private void FormTimKiem_Load(object sender, EventArgs e)
        {
            LoadAllData();
            txtTuKhoa.Focus();
        }

        // ─── Detect column names helper ──────────────────────────────────────────

        private static string DetectCol(DataTable schema, params string[] keywords)
        {
            foreach (DataColumn c in schema.Columns)
            {
                string name = c.ColumnName.ToLower();
                foreach (var kw in keywords)
                    if (name.Contains(kw)) return c.ColumnName;
            }
            return null;
        }

        // ─── Build SELECT for each table ─────────────────────────────────────────

        private string BuildNhapSql(string where = "")
        {
            var schema = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM PhieuNhapKho");
            string cMa   = DetectCol(schema, "mã", "ma", "phiếu", "phieu") ?? schema.Columns[0].ColumnName;
            string cNgay = null;
            foreach (DataColumn c in schema.Columns) if (c.DataType == typeof(DateTime)) { cNgay = c.ColumnName; break; }
            if (cNgay == null) cNgay = DetectCol(schema, "ngày", "ngay");
            string cDoiTac = DetectCol(schema, "nhà", "nha", "đối", "doi", "cung") ?? "";
            string cNguoi  = DetectCol(schema, "người", "nguoi") ?? "";
            string cGhiChu = DetectCol(schema, "ghi") ?? "";

            string ngayExpr = string.IsNullOrEmpty(cNgay) ? "NULL" : $"[{cNgay}]";
            string sql = $"SELECT N'Nhập' as [Phân loại], [{cMa}] as [Mã], {ngayExpr} as [Ngày], " +
                         $"[{cDoiTac}] as [Đối tác / Loại hàng], [{cNguoi}] as [Người thực hiện / Tên hàng], " +
                         $"[{cGhiChu}] as [Ghi chú / ĐVT], NULL as [Đơn Giá] FROM PhieuNhapKho";
            if (!string.IsNullOrEmpty(where)) sql += " WHERE " + where;
            return sql;
        }

        private string BuildXuatSql(string where = "")
        {
            var schema = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM PhieuXuatKho");
            string cMa   = DetectCol(schema, "mã", "ma", "phiếu", "phieu") ?? schema.Columns[0].ColumnName;
            string cNgay = null;
            foreach (DataColumn c in schema.Columns) if (c.DataType == typeof(DateTime)) { cNgay = c.ColumnName; break; }
            if (cNgay == null) cNgay = DetectCol(schema, "ngày", "ngay");
            string cDoiTac = DetectCol(schema, "khách", "khach", "đối", "doi") ?? "";
            string cNguoi  = DetectCol(schema, "người", "nguoi") ?? "";
            string cGhiChu = DetectCol(schema, "ghi") ?? "";

            string ngayExpr = string.IsNullOrEmpty(cNgay) ? "NULL" : $"[{cNgay}]";
            string sql = $"SELECT N'Xuất' as [Phân loại], [{cMa}] as [Mã], {ngayExpr} as [Ngày], " +
                         $"[{cDoiTac}] as [Đối tác / Loại hàng], [{cNguoi}] as [Người thực hiện / Tên hàng], " +
                         $"[{cGhiChu}] as [Ghi chú / ĐVT], NULL as [Đơn Giá] FROM PhieuXuatKho";
            if (!string.IsNullOrEmpty(where)) sql += " WHERE " + where;
            return sql;
        }

        private string BuildKiemKeSql(string where = "")
        {
            var schema = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM PhieuKiemKe");
            string cMa   = DetectCol(schema, "mã", "ma", "kiểm", "kiem") ?? schema.Columns[0].ColumnName;
            string cNgay = null;
            foreach (DataColumn c in schema.Columns) if (c.DataType == typeof(DateTime)) { cNgay = c.ColumnName; break; }
            if (cNgay == null) cNgay = DetectCol(schema, "ngày", "ngay");
            string cDoiTac = DetectCol(schema, "kết", "ket", "đối", "doi") ?? "";
            string cNguoi  = DetectCol(schema, "người", "nguoi") ?? "";
            string cGhiChu = DetectCol(schema, "ghi") ?? "";

            string ngayExpr = string.IsNullOrEmpty(cNgay) ? "NULL" : $"[{cNgay}]";
            string sql = $"SELECT N'Kiểm kê' as [Phân loại], [{cMa}] as [Mã], {ngayExpr} as [Ngày], " +
                         $"[{cDoiTac}] as [Đối tác / Loại hàng], [{cNguoi}] as [Người thực hiện / Tên hàng], " +
                         $"[{cGhiChu}] as [Ghi chú / ĐVT], NULL as [Đơn Giá] FROM PhieuKiemKe";
            if (!string.IsNullOrEmpty(where)) sql += " WHERE " + where;
            return sql;
        }

        private string BuildMatHangSql(string where = "")
        {
            var schema = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM MatHang");
            string cMa    = DetectCol(schema, "mã", "ma") ?? schema.Columns[0].ColumnName;
            string cTen   = DetectCol(schema, "tên", "ten") ?? "";
            string cLoai  = DetectCol(schema, "loại", "loai") ?? "";
            string cDvt   = DetectCol(schema, "đơn vị", "don vi", "đơn", "don", "dvt") ?? "";
            string cGia   = DetectCol(schema, "đơn giá", "don gia", "giá", "gia") ?? "";

            string giaExpr = string.IsNullOrEmpty(cGia) ? "NULL" : $"CAST([{cGia}] AS DECIMAL(18,2))";
            string sql = $"SELECT N'Mặt hàng' as [Phân loại], [{cMa}] as [Mã], NULL as [Ngày], " +
                         $"[{cLoai}] as [Đối tác / Loại hàng], [{cTen}] as [Người thực hiện / Tên hàng], " +
                         $"[{cDvt}] as [Ghi chú / ĐVT], {giaExpr} as [Đơn Giá] FROM MatHang";
            if (!string.IsNullOrEmpty(where)) sql += " WHERE " + where;
            return sql;
        }

        // ─── Load all data ────────────────────────────────────────────────────────

        private void LoadAllData()
        {
            try
            {
                DataTable finalDt = new DataTable();
                AppendResults(finalDt, BuildNhapSql());
                AppendResults(finalDt, BuildXuatSql());
                AppendResults(finalDt, BuildKiemKeSql());
                AppendResults(finalDt, BuildMatHangSql());
                dgvData.DataSource = finalDt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── Search ───────────────────────────────────────────────────────────────

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                var kw = txtTuKhoa.Text.Trim().Replace("'", "''");
                if (string.IsNullOrEmpty(kw))
                {
                    MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // detect columns per table for WHERE clauses
                var schemaNhap  = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM PhieuNhapKho");
                var schemaXuat  = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM PhieuXuatKho");
                var schemaKiem  = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM PhieuKiemKe");
                var schemaMH    = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM MatHang");

                string cNhapMa    = DetectCol(schemaNhap, "mã", "ma", "phiếu", "phieu") ?? schemaNhap.Columns[0].ColumnName;
                string cNhapNguoi = DetectCol(schemaNhap, "người", "nguoi") ?? "";
                string cNhapDt    = DetectCol(schemaNhap, "nhà", "nha", "đối", "doi", "cung") ?? "";
                string cNhapGhi   = DetectCol(schemaNhap, "ghi") ?? "";

                string cXuatMa    = DetectCol(schemaXuat, "mã", "ma", "phiếu", "phieu") ?? schemaXuat.Columns[0].ColumnName;
                string cXuatNguoi = DetectCol(schemaXuat, "người", "nguoi") ?? "";
                string cXuatDt    = DetectCol(schemaXuat, "khách", "khach", "đối", "doi") ?? "";
                string cXuatGhi   = DetectCol(schemaXuat, "ghi") ?? "";

                string cKiemMa    = DetectCol(schemaKiem, "mã", "ma", "kiểm", "kiem") ?? schemaKiem.Columns[0].ColumnName;
                string cKiemNguoi = DetectCol(schemaKiem, "người", "nguoi") ?? "";
                string cKiemDt    = DetectCol(schemaKiem, "kết", "ket", "đối", "doi") ?? "";
                string cKiemGhi   = DetectCol(schemaKiem, "ghi") ?? "";

                string cMHMa      = DetectCol(schemaMH, "mã", "ma") ?? schemaMH.Columns[0].ColumnName;
                string cMHTen     = DetectCol(schemaMH, "tên", "ten") ?? "";
                string cMHLoai    = DetectCol(schemaMH, "loại", "loai") ?? "";
                string cMHDvt     = DetectCol(schemaMH, "đơn vị", "don vi", "đơn", "don", "dvt") ?? "";
                string cMHGia     = DetectCol(schemaMH, "đơn giá", "don gia", "giá", "gia") ?? "";

                // Dùng từ khóa gốc (kwOrig) để so sánh nhãn Phân loại — không dùng kw đã escape SQL
                var kwOrig = txtTuKhoa.Text.Trim().ToLower();

                // Nếu từ khóa là nhãn Phân loại → lấy toàn bộ bảng đó (không lọc WHERE)
                bool matchNhap = "nhập".StartsWith(kwOrig) || kwOrig.StartsWith("nhap") || kwOrig.StartsWith("nhập");
                bool matchXuat = "xuất".StartsWith(kwOrig) || kwOrig.StartsWith("xuat") || kwOrig.StartsWith("xuất");
                bool matchKiem = "kiểm kê".StartsWith(kwOrig) || kwOrig.StartsWith("kiem") || kwOrig.StartsWith("kiểm");
                bool matchMH   = "mặt hàng".StartsWith(kwOrig) || kwOrig.StartsWith("mat hang") || kwOrig.StartsWith("mặt hàng") || kwOrig == "mặt" || kwOrig == "mat";

                string whereNhap = matchNhap ? "" : $"([{cNhapMa}] LIKE N'%{kw}%' OR [{cNhapNguoi}] LIKE N'%{kw}%' OR [{cNhapDt}] LIKE N'%{kw}%' OR [{cNhapGhi}] LIKE N'%{kw}%')";
                string whereXuat = matchXuat ? "" : $"([{cXuatMa}] LIKE N'%{kw}%' OR [{cXuatNguoi}] LIKE N'%{kw}%' OR [{cXuatDt}] LIKE N'%{kw}%' OR [{cXuatGhi}] LIKE N'%{kw}%')";
                string whereKiem = matchKiem ? "" : $"([{cKiemMa}] LIKE N'%{kw}%' OR [{cKiemNguoi}] LIKE N'%{kw}%' OR [{cKiemDt}] LIKE N'%{kw}%' OR [{cKiemGhi}] LIKE N'%{kw}%')";

                // MatHang: tìm cả cột Đơn Giá (DECIMAL) bằng CAST sang text
                string giaSearch = string.IsNullOrEmpty(cMHGia)
                    ? ""
                    : $" OR CAST([{cMHGia}] AS NVARCHAR(50)) LIKE N'%{kw}%'";
                string whereMH = matchMH ? "" : $"([{cMHMa}] LIKE N'%{kw}%' OR [{cMHTen}] LIKE N'%{kw}%' OR [{cMHLoai}] LIKE N'%{kw}%' OR [{cMHDvt}] LIKE N'%{kw}%'{giaSearch})";

                DataTable finalDt = new DataTable();
                AppendResults(finalDt, BuildNhapSql(whereNhap));
                AppendResults(finalDt, BuildXuatSql(whereXuat));
                AppendResults(finalDt, BuildKiemKeSql(whereKiem));
                AppendResults(finalDt, BuildMatHangSql(whereMH));
                dgvData.DataSource = finalDt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendResults(DataTable target, string sql)
        {
            var dt = DatabaseHelper.LoadTable(sql);
            if (target.Columns.Count == 0)
            {
                foreach (DataColumn col in dt.Columns)
                    target.Columns.Add(col.ColumnName, col.DataType);
            }
            foreach (DataRow row in dt.Rows)
                target.ImportRow(row);
        }
    }
}
