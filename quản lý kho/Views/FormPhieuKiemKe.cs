using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using quản_lý_kho.Data;

namespace quản_lý_kho.Views
{
    public partial class FormPhieuKiemKe : Form
    {
        private BindingSource _bs = new BindingSource();
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private int _pageCount = 0;
        private string _orderBy = "[Mã Hàng]";
        private string _baseSql = "SELECT * FROM PhieuKiemKe";
        
        private string _colMaPhieu = "Mã Hàng";
        private string _colNgay = "Ngày Kiểm Kê";
        private string _colNguoiThucHien = "Người Thực Hiện";
        private string _colKetQua = "Kết Quả";
        private string _colGhiChu = "Ghi Chú";
        
        private bool _bindingsInitialized = false;

        public FormPhieuKiemKe()
        {
            InitializeComponent();
            Load += FormPhieuKiemKe_Load;
            btnPrev.Click += (s, e) => { if (_pageIndex > 1) { _pageIndex--; LoadPage(); } };
            btnNext.Click += (s, e) => { if (_pageIndex < _pageCount) { _pageIndex++; LoadPage(); } };
            btnReload.Click += (s, e) => LoadPage();
            btnThem.Click += BtnThem_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLuu.Click += BtnLuu_Click;
            btnTimKiem.Click += BtnTimKiem_Click;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void FormPhieuKiemKe_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPage();
                dgvData.DataSource = _bs;
                if (!_bindingsInitialized)
                    InitializeBindingsIfPossible();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPage()
        {
            var dt = DatabaseHelper.LoadTablePaged(_baseSql, _pageIndex, _pageSize, _orderBy);
            _bs.DataSource = dt;
            var totalTable = DatabaseHelper.LoadTable("SELECT COUNT(1) FROM PhieuKiemKe");
            var total = totalTable.Rows.Count > 0 ? Convert.ToInt32(totalTable.Rows[0][0]) : 0;
            _pageCount = (int)Math.Ceiling(total / (double)_pageSize);
            lblPage.Text = $"Page {_pageIndex}/{Math.Max(1, _pageCount)}";
            btnPrev.Enabled = _pageIndex > 1;
            btnNext.Enabled = _pageIndex < _pageCount;
        }

        private void InitializeBindingsIfPossible()
        {
            DataTable dt = null;
            if (_bs.DataSource is DataTable d) dt = d;
            else { try { dt = DatabaseHelper.LoadTable("SELECT TOP(1) * FROM PhieuKiemKe"); } catch { return; } }
            if (dt == null) return;

            var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataColumn c in dt.Columns) cols.Add(c.ColumnName);

            string Pick(params string[] candidates)
            {
                foreach (var c in candidates) if (cols.Contains(c)) return c;
                return cols.Count > 0 ? new List<string>(cols)[0] : null;
            }

            _colMaPhieu = Pick("MaKiemKe", "Ma Hàng", "MaPhieu", "ID") ?? _colMaPhieu;
            _colNgay = Pick("Ngày Kiểm Kê", "NgayKiemKeKho", "NgayKiemKe", "NgayLap", "Ngay") ?? _colNgay;
            _colNguoiThucHien = Pick("Người Thực Hiện", "NguoiThucHienKiemKe", "NguoiThucHien", "NguoiKiemKe", "MaNhanVien") ?? _colNguoiThucHien;
            _colKetQua = Pick("Kết Quả", "KetQuaKiemKe", "KetQua") ?? _colKetQua;
            _colGhiChu = Pick("Ghi Chú", "GhiChuKiemKeKho", "GhiChu") ?? _colGhiChu;
            _orderBy = $"[{_colMaPhieu}]";

            txtMaPhieu.DataBindings.Clear();
            txtNgayLap.DataBindings.Clear();
            txtMaNhanVien.DataBindings.Clear();
            txtKetQua.DataBindings.Clear();
            txtGhiChu.DataBindings.Clear();

            if (cols.Contains(_colMaPhieu)) txtMaPhieu.DataBindings.Add("Text", _bs, _colMaPhieu, true, DataSourceUpdateMode.OnPropertyChanged);
            if (cols.Contains(_colNgay)) txtNgayLap.DataBindings.Add("Text", _bs, _colNgay, true, DataSourceUpdateMode.OnPropertyChanged);
            if (cols.Contains(_colNguoiThucHien)) txtMaNhanVien.DataBindings.Add("Text", _bs, _colNguoiThucHien, true, DataSourceUpdateMode.OnPropertyChanged);
            if (cols.Contains(_colKetQua)) txtKetQua.DataBindings.Add("Text", _bs, _colKetQua, true, DataSourceUpdateMode.OnPropertyChanged);
            if (cols.Contains(_colGhiChu)) txtGhiChu.DataBindings.Add("Text", _bs, _colGhiChu, true, DataSourceUpdateMode.OnPropertyChanged);

            _bindingsInitialized = true;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            txtMaPhieu.Text = "";
            txtNgayLap.Text = DateTime.Now.ToShortDateString();
            txtMaNhanVien.Text = "";
            txtKetQua.Text = "";
            txtGhiChu.Text = "";
            txtMaPhieu.Focus();
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (_bs.Current == null) return;
            var id = ((DataRowView)_bs.Current).Row[_colMaPhieu].ToString();
            if (MessageBox.Show($"Xóa phiếu {id}?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseHelper.ExecuteNonQuery($"DELETE FROM PhieuKiemKe WHERE [{_colMaPhieu}] = @id", new SqlParameter("@id", id));
                LoadPage();
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                var id = txtMaPhieu.Text.Trim();
                if (string.IsNullOrEmpty(id)) return;

                var exists = DatabaseHelper.LoadTable($"SELECT COUNT(1) FROM PhieuKiemKe WHERE [{_colMaPhieu}] = @id", new SqlParameter("@id", id));
                bool isUpdate = Convert.ToInt32(exists.Rows[0][0]) > 0;

                var parameters = new List<SqlParameter> { 
                    new SqlParameter("@id", id),
                    new SqlParameter("@ngay", txtNgayLap.Text),
                    new SqlParameter("@nth", txtMaNhanVien.Text),
                    new SqlParameter("@kq", txtKetQua.Text),
                    new SqlParameter("@gh", txtGhiChu.Text)
                };

                if (isUpdate)
                {
                    var sql = $"UPDATE PhieuKiemKe SET [{_colNgay}] = @ngay, [{_colNguoiThucHien}] = @nth, [{_colKetQua}] = @kq, [{_colGhiChu}] = @gh WHERE [{_colMaPhieu}] = @id";
                    DatabaseHelper.ExecuteNonQuery(sql, parameters.ToArray());
                }
                else
                {
                    var sql = $"INSERT INTO PhieuKiemKe([{_colMaPhieu}], [{_colNgay}], [{_colNguoiThucHien}], [{_colKetQua}], [{_colGhiChu}]) VALUES(@id, @ngay, @nth, @kq, @gh)";
                    DatabaseHelper.ExecuteNonQuery(sql, parameters.ToArray());
                }
                LoadPage();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            var kw = txtMaPhieu.Text.Replace("'", "''");
            var sql = _baseSql + $" WHERE [{_colMaPhieu}] LIKE '%{kw}%' OR [{_colNguoiThucHien}] LIKE '%{kw}%'";
            _bs.DataSource = DatabaseHelper.LoadTable(sql);
            btnPrev.Enabled = btnNext.Enabled = false;
        }
    }
}
