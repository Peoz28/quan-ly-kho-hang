using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace quản_lý_kho.Views
{
    public partial class FormInBaoCao : Form
    {
        private DataTable _printData = null;  // dữ liệu để in
        private int _printPage = 0;           // trang hiện tại khi in

        // Thông tin báo cáo để in
        private string _tuNgay = "";
        private string _denNgay = "";
        private int _soNhap, _soXuat, _soKiem;
        private decimal _tongNhap, _tongXuat;

        public FormInBaoCao()
        {
            InitializeComponent();
            Load += (s, e) =>
            {
                dtpTuNgay.Value = DateTime.Now.AddMonths(-1);
                dtpDenNgay.Value = DateTime.Now;
                dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            };
            btnXemBaoCao.Click += BtnXemBaoCao_Click;
            btnBaoCaoTongHop.Click += BtnIn_Click;
            btnLamMoi.Click += (s, e) =>
            {
                dtpTuNgay.Value = DateTime.Now.AddMonths(-1);
                dtpDenNgay.Value = DateTime.Now;
                dgvData.DataSource = null;
                _printData = null;
            };
        }

        // ─── Xem báo cáo ────────────────────────────────────────────────────────
        private void BtnXemBaoCao_Click(object sender, EventArgs e)
        {
            try
            {
                var from = dtpTuNgay.Value.ToString("yyyy-MM-dd 00:00:00");
                var to   = dtpDenNgay.Value.ToString("yyyy-MM-dd 23:59:59");

                string DetectDateCol(string tbl) {
                    var s = Data.DatabaseHelper.LoadTable($"SELECT TOP(0) * FROM {tbl}");
                    foreach (DataColumn c in s.Columns) if (c.DataType == typeof(DateTime)) return c.ColumnName;
                    return s.Columns[0].ColumnName;
                }
                string DetectMaCol(string tbl) {
                    var s = Data.DatabaseHelper.LoadTable($"SELECT TOP(0) * FROM {tbl}");
                    foreach (DataColumn c in s.Columns)
                        if (c.ColumnName.ToLower().Contains("mã") || c.ColumnName.ToLower().Contains("ma")) return c.ColumnName;
                    return s.Columns[0].ColumnName;
                }
                string DetectTenCol(string tbl) {
                    var s = Data.DatabaseHelper.LoadTable($"SELECT TOP(0) * FROM {tbl}");
                    foreach (DataColumn c in s.Columns)
                        if (c.ColumnName.ToLower().Contains("tên") || c.ColumnName.ToLower().Contains("ten")) return c.ColumnName;
                    return s.Columns.Count > 1 ? s.Columns[1].ColumnName : s.Columns[0].ColumnName;
                }

                string dN = DetectDateCol("PhieuNhapKho"), mN = DetectMaCol("PhieuNhapKho");
                string dX = DetectDateCol("PhieuXuatKho"), mX = DetectMaCol("PhieuXuatKho");
                string dK = DetectDateCol("PhieuKiemKe");
                string cMaMH = DetectMaCol("MatHang"), cTenMH = DetectTenCol("MatHang");

                _tuNgay  = dtpTuNgay.Value.ToString("dd/MM/yyyy");
                _denNgay = dtpDenNgay.Value.ToString("dd/MM/yyyy");

                _soNhap  = Convert.ToInt32(Data.DatabaseHelper.LoadTable($"SELECT COUNT(*) FROM PhieuNhapKho WHERE [{dN}] BETWEEN '{from}' AND '{to}'").Rows[0][0]);
                _soXuat  = Convert.ToInt32(Data.DatabaseHelper.LoadTable($"SELECT COUNT(*) FROM PhieuXuatKho WHERE [{dX}] BETWEEN '{from}' AND '{to}'").Rows[0][0]);
                _soKiem  = Convert.ToInt32(Data.DatabaseHelper.LoadTable($"SELECT COUNT(*) FROM PhieuKiemKe WHERE [{dK}] BETWEEN '{from}' AND '{to}'").Rows[0][0]);
                _tongNhap = 0; _tongXuat = 0;

                // Build hiển thị lưới
                var dt = new DataTable();
                dt.Columns.Add("Thông tin");
                dt.Columns.Add("Giá trị");

                dt.Rows.Add("=== THÔNG TIN ĐƠN VỊ ===", "");
                dt.Rows.Add("Đơn vị", "KHO ĐIỆN MÁY GIA DỤNG");
                dt.Rows.Add("Thời kỳ", $"{_tuNgay} - {_denNgay}");
                dt.Rows.Add("Ngày lập", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                dt.Rows.Add("", "");

                dt.Rows.Add("=== CHỈ SỐ TRONG KỲ ===", "");
                dt.Rows.Add("Số phiếu nhập", _soNhap);
                dt.Rows.Add("Số phiếu xuất", _soXuat);
                dt.Rows.Add("Số phiếu kiểm kê", _soKiem);
                dt.Rows.Add("Giá trị nhập kho", _tongNhap.ToString("N0") + " VNĐ");
                dt.Rows.Add("Giá trị xuất kho", _tongXuat.ToString("N0") + " VNĐ");
                dt.Rows.Add("Cân đối thu chi", (_tongXuat - _tongNhap).ToString("N0") + " VNĐ");
                dt.Rows.Add("", "");

                dt.Rows.Add("=== DANH SÁCH MẶT HÀNG ===", "");
                var items = Data.DatabaseHelper.LoadTable($"SELECT [{cMaMH}], [{cTenMH}] FROM MatHang");
                foreach (DataRow row in items.Rows)
                    dt.Rows.Add($"[{row[0]}]", row[1]);

                dt.Rows.Add("", "");
                dt.Rows.Add("=== XÁC NHẬN ===", "");
                dt.Rows.Add("Người lập", "Admin");
                dt.Rows.Add("Trạng thái", "Đã ký duyệt");

                _printData = dt;
                dgvData.DataSource = dt;
                dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // ─── In phiếu ───────────────────────────────────────────────────────────
        private void BtnIn_Click(object sender, EventArgs e)
        {
            // Nếu chưa xem báo cáo thì tự tải
            if (_printData == null)
                BtnXemBaoCao_Click(sender, e);

            if (_printData == null) return;

            var pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            pd.DefaultPageSettings.Margins   = new Margins(50, 50, 50, 50);
            _printPage = 0;
            pd.PrintPage += PrintPage_Handler;

            using (var ppd = new PrintPreviewDialog())
            {
                ppd.Document  = pd;
                ppd.Width     = 1000;
                ppd.Height    = 750;
                ppd.Text      = "Xem trước — Báo cáo kho hàng";
                ppd.ShowDialog(this);
            }
        }

        // ─── Vẽ trang in ────────────────────────────────────────────────────────
        private void PrintPage_Handler(object sender, PrintPageEventArgs e)
        {
            var g      = e.Graphics;
            float left = e.MarginBounds.Left;
            float right= e.MarginBounds.Right;
            float width= e.MarginBounds.Width;
            float y    = e.MarginBounds.Top;

            // === HEADER: Tên công ty ===
            var fontCty   = new Font("Arial", 14, FontStyle.Bold);
            var fontSub   = new Font("Arial", 9);
            var fontTitle = new Font("Arial", 12, FontStyle.Bold);
            var fontNorm  = new Font("Arial", 9);
            var fontBold  = new Font("Arial", 9, FontStyle.Bold);
            var fontSmall = new Font("Arial", 8, FontStyle.Italic);

            var blue = Color.FromArgb(0, 70, 127);
            var gray = Color.FromArgb(240, 240, 240);

            // Dòng tiêu đề công ty
            g.DrawString("KHO ĐIỆN MÁY GIA DỤNG", fontCty, new SolidBrush(blue), left, y);
            y += fontCty.GetHeight(g) + 2;
            g.DrawString("Địa chỉ: 123 Đường ABC, TP. HÀ NỘI  |  ĐT: 024.1234.5678", fontSub, Brushes.Gray, left, y);
            y += fontSub.GetHeight(g) + 4;

            // Đường kẻ ngang màu xanh
            using (var pen = new Pen(blue, 1.5f))
                g.DrawLine(pen, left, y, right, y);
            y += 10;

            // Tiêu đề báo cáo
            string titleText = $"BÁO CÁO TỔNG HỢP KHO — Từ {_tuNgay} đến {_denNgay}";
            var titleSize = g.MeasureString(titleText, fontTitle);
            g.DrawString(titleText, fontTitle, new SolidBrush(blue), left + (width - titleSize.Width) / 2, y);
            y += titleSize.Height + 2;

            string dateText = $"Ngày lập: {DateTime.Now:dd/MM/yyyy HH:mm}";
            g.DrawString(dateText, fontSmall, Brushes.Gray, left, y);
            y += fontSmall.GetHeight(g) + 12;

            // === BẢNG DỮ LIỆU ===
            float col1W = width * 0.45f;
            float col2W = width * 0.55f;

            // Header bảng
            var headerRect1 = new RectangleF(left,        y, col1W, 20);
            var headerRect2 = new RectangleF(left + col1W, y, col2W, 20);
            g.FillRectangle(new SolidBrush(blue), headerRect1);
            g.FillRectangle(new SolidBrush(blue), headerRect2);

            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("Thông tin", fontBold, Brushes.White, headerRect1, fmt);
            g.DrawString("Giá trị",   fontBold, Brushes.White, headerRect2, fmt);
            y += 20;

            // Vẽ từng dòng dữ liệu
            bool altRow = false;
            foreach (DataRow row in _printData.Rows)
            {
                string col1 = row[0]?.ToString() ?? "";
                string col2 = row[1]?.ToString() ?? "";

                bool isHeader = col1.StartsWith("===");
                float rowH = isHeader ? 20 : 18;

                var rect1 = new RectangleF(left,         y, col1W, rowH);
                var rect2 = new RectangleF(left + col1W, y, col2W, rowH);

                // Nền dòng
                if (isHeader)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(210, 225, 245)), rect1);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(210, 225, 245)), rect2);
                }
                else if (altRow)
                {
                    g.FillRectangle(new SolidBrush(gray), rect1);
                    g.FillRectangle(new SolidBrush(gray), rect2);
                }

                // Text
                var rowFont = isHeader ? fontBold : fontNorm;
                var fmtLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap };
                g.DrawString(col1, rowFont, Brushes.Black, new RectangleF(rect1.X + 4, rect1.Y, rect1.Width - 4, rect1.Height), fmtLeft);
                g.DrawString(col2, fontNorm, Brushes.Black, new RectangleF(rect2.X + 4, rect2.Y, rect2.Width - 4, rect2.Height), fmtLeft);

                // Đường kẻ ngang
                using (var pen = new Pen(Color.LightGray, 0.5f))
                {
                    g.DrawLine(pen, left, y + rowH, right, y + rowH);
                    g.DrawLine(pen, left, y, left, y + rowH);
                    g.DrawLine(pen, left + col1W, y, left + col1W, y + rowH);
                    g.DrawLine(pen, right, y, right, y + rowH);
                }

                y += rowH;
                if (!isHeader && !string.IsNullOrEmpty(col1)) altRow = !altRow;
            }

            // === TỔNG CỘNG ===
            y += 10;
            var totalBrush = new SolidBrush(blue);
            string totalStr = $"TỔNG CỘNG GIÁ TRỊ NHẬP:  {_tongNhap:N0} VNĐ     |     XUẤT:  {_tongXuat:N0} VNĐ";
            var totalFont = new Font("Arial", 10, FontStyle.Bold);
            var totalSize = g.MeasureString(totalStr, totalFont);
            g.DrawString(totalStr, totalFont, totalBrush, right - totalSize.Width, y);
            y += totalSize.Height + 20;

            // === CHỮ KÝ ===
            float sigX = left + width * 0.6f;
            g.DrawString("Người lập phiếu", fontNorm, Brushes.Black, sigX + 20, y);
            y += fontNorm.GetHeight(g) + 2;
            g.DrawString("(Ký, ghi rõ họ tên)", fontSmall, Brushes.Gray, sigX + 10, y);
            y += 50;
            using (var pen = new Pen(Color.Black, 0.8f))
                g.DrawLine(pen, sigX, y, sigX + 170, y);

            e.HasMorePages = false;
        }
    }
}
