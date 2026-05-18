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
    public partial class FormPhieuNhapKho : Form
    {
        private BindingSource _bs = new BindingSource();
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private int _pageCount = 0;
        private string _orderBy = "[Mã Phiếu Nhập]";
        // select all columns and detect actual names at runtime to avoid Invalid column name errors
        private string _baseSql = "SELECT * FROM PhieuNhapKho";
        private string _colMaPhieu = "Mã Phiếu Nhập";
        private string _colNgay = "Ngày Nhập";
        private string _colNhaCungCap = "Nhà Cung Cấp";
        private string _colMaNvcNccCandidate1 = "Mã Nhân Viên";
        private string _colMaNvcNccCandidate2 = "MaNhanVien";
        private string _colMaNvcNccCandidate3 = "MaNV";
        private string _colMaNvcNcc = null; // actual column name for supplier employee
        private string _colNguoiNhap = "Người Thực Hiện"; // receiver candidate
        private string _colNguoiNhapAlternate = "NguoiNhapKho";
        private string _colGhiChu = "Ghi Chú";
        private bool _bindingsInitialized = false;

        public FormPhieuNhapKho()
        {
            InitializeComponent();
            Load += FormPhieuNhapKho_Load;
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLuu.Click += BtnLuu_Click;
            btnTimKiem.Click += BtnTimKiem_Click;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            btnReload.Click += (s, e) => LoadPage();
            dgvData.SelectionChanged += DgvData_SelectionChanged;
        }

        private void FormPhieuNhapKho_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPage();
                dgvData.DataSource = _bs;
                // bindings will be initialized by LoadPage once schema is known
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
            // compute page count from total rows
            var totalTable = DatabaseHelper.LoadTable("SELECT COUNT(1) AS C FROM PhieuNhapKho");
            var total = totalTable.Rows.Count > 0 ? Convert.ToInt32(totalTable.Rows[0][0]) : 0;
            _pageCount = (int)Math.Ceiling(total / (double)_pageSize);
            lblPage.Text = $"Page {_pageIndex}/{Math.Max(1, _pageCount)}";
            btnPrev.Enabled = _pageIndex > 1;
            btnNext.Enabled = _pageIndex < _pageCount;
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            if (_pageIndex > 1)
            {
                _pageIndex--;
                LoadPage();
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_pageIndex < _pageCount)
            {
                _pageIndex++;
                LoadPage();
            }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                var kw = txtMaPhieu.Text.Replace("'", "''");
                var whereParts = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrEmpty(_colMaPhieu)) whereParts.Add($"[{_colMaPhieu}] LIKE '%{kw}%'");
                if (!string.IsNullOrEmpty(_colNhaCungCap)) whereParts.Add($"[{_colNhaCungCap}] LIKE '%{kw}%'");
                if (!string.IsNullOrEmpty(_colNguoiNhap)) whereParts.Add($"[{_colNguoiNhap}] LIKE '%{kw}%'");
                var where = whereParts.Count > 0 ? " WHERE " + string.Join(" OR ", whereParts) : string.Empty;
                var sql = _baseSql + where;
                var dt = DatabaseHelper.LoadTable(sql);
                _bs.DataSource = dt;
                lblPage.Text = "Search results";
                btnPrev.Enabled = btnNext.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tìm dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvData_SelectionChanged(object sender, EventArgs e)
        {
            // When selection changes, bindings will update automatically because dgv is bound to _bs
        }

        private DataTable GetCurrentTable()
        {
            if (_bs.DataSource is DataTable dt)
                return dt;

            try
            {
                dt = DatabaseHelper.LoadTable("SELECT TOP(1) * FROM PhieuNhapKho");
                return dt;
            }
            catch
            {
                return null;
            }
        }

        private void InitializeBindingsIfPossible()
        {
            var dt = GetCurrentTable();
            if (dt == null) return;

            var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataColumn c in dt.Columns) cols.Add(c.ColumnName);

            string Pick(params string[] candidates)
            {
                foreach (var c in candidates)
                    if (cols.Contains(c)) return c;
                return cols.Count > 0 ? new List<string>(cols)[0] : null;
            }

            _colMaPhieu = Pick("Mã Phiếu Nhập", "MaPhieuNhap", "MaPhieu", "MaPhieuNhapKho", "ID") ?? _colMaPhieu;
            _colNgay = Pick("Ngày Nhập", "NgayNhapKho", "NgayLap", "NgayNhap", "Ngay") ?? _colNgay;
            _colNhaCungCap = Pick("Nhà Cung Cấp", "NhaCungCapNhap", "NhaCungCap", "NCC") ?? _colNhaCungCap;
            _colMaNvcNcc = Pick("Mã Nhân Viên", "MaNhanVien", "MaNV", _colMaNvcNccCandidate1, _colMaNvcNccCandidate2, _colMaNvcNccCandidate3) ?? null;
            _colNguoiNhap = Pick("Người Thực Hiện", "NguoiThucHienNhap", "NguoiNhapKho", "NguoiThucHien", "NguoiNhap") ?? _colNguoiNhap;
            _colGhiChu = Pick("Ghi Chú", "GhiChuNhapKho", "GhiChu", "GhiChuNK") ?? _colGhiChu;
            _orderBy = $"[{_colMaPhieu}]";

            // clear previous bindings
            txtMaPhieu.DataBindings.Clear();
            txtNgayLap.DataBindings.Clear();
            txtMaNhanVien.DataBindings.Clear();
            txtNguoiNhapKho.DataBindings.Clear();
            txtGhiChu.DataBindings.Clear();

            // bind if present
            if (cols.Contains(_colMaPhieu)) txtMaPhieu.DataBindings.Add("Text", _bs, _colMaPhieu, true, DataSourceUpdateMode.OnPropertyChanged);
            if (cols.Contains(_colNgay)) txtNgayLap.DataBindings.Add("Text", _bs, _colNgay, true, DataSourceUpdateMode.OnPropertyChanged);
            string supplierCol = (!string.IsNullOrEmpty(_colMaNvcNcc) && cols.Contains(_colMaNvcNcc)) ? _colMaNvcNcc : _colNhaCungCap;
            if (!string.IsNullOrEmpty(supplierCol) && cols.Contains(supplierCol)) txtMaNhanVien.DataBindings.Add("Text", _bs, supplierCol, true, DataSourceUpdateMode.OnPropertyChanged);
            if (!string.IsNullOrEmpty(_colNguoiNhap) && cols.Contains(_colNguoiNhap)) txtNguoiNhapKho.DataBindings.Add("Text", _bs, _colNguoiNhap, true, DataSourceUpdateMode.OnPropertyChanged);
            if (cols.Contains(_colGhiChu)) txtGhiChu.DataBindings.Add("Text", _bs, _colGhiChu, true, DataSourceUpdateMode.OnPropertyChanged);

            _bindingsInitialized = true;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            // Clear fields for new entry
            txtMaPhieu.Text = string.Empty;
            txtNgayLap.Text = DateTime.Now.ToShortDateString();
            txtMaNhanVien.Text = string.Empty;
            txtGhiChu.Text = string.Empty;
            txtMaPhieu.Focus();
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            // Prepare for editing - fields already bound
            if (_bs.Current == null) return;
            // nothing special here, changes will be saved on Lưu
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (_bs.Current == null) return;
            var dr = ((DataRowView)_bs.Current).Row;
            var id = dr[_colMaPhieu].ToString();
            if (MessageBox.Show("Bạn có chắc muốn xóa phiếu '" + id + "' ?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var sql = "DELETE FROM PhieuNhapKho WHERE MaPhieuNhap = @id";
                var p = new SqlParameter("@id", id);
                try
                {
                    DatabaseHelper.ExecuteNonQuery(sql, p);
                    LoadPage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                var ma = txtMaPhieu.Text.Trim();
                var ngay = txtNgayLap.Text.Trim();
                var manvNcc = txtMaNhanVien.Text.Trim();
                var nguoiNhapKho = txtNguoiNhapKho.Text.Trim();
                var gh = txtGhiChu.Text.Trim();

                if (string.IsNullOrEmpty(ma))
                {
                    MessageBox.Show("Mã phiếu không được để trống.");
                    return;
                }

                // Check if exists
                var exists = DatabaseHelper.LoadTable($"SELECT COUNT(1) AS C FROM PhieuNhapKho WHERE [{_colMaPhieu}] = @id", new SqlParameter("@id", ma));
                var c = exists.Rows.Count > 0 ? Convert.ToInt32(exists.Rows[0][0]) : 0;
                if (c > 0)
                {
                    // update
                    // build update dynamically based on detected column names
                    var setParts = new System.Collections.Generic.List<string>();
                    var parameters = new System.Collections.Generic.List<SqlParameter>();
                    if (!string.IsNullOrEmpty(_colNgay)) { setParts.Add($"[{_colNgay}] = @ngay"); parameters.Add(new SqlParameter("@ngay", ngay)); }
                    string supCol = !string.IsNullOrEmpty(_colMaNvcNcc) ? _colMaNvcNcc : _colNhaCungCap;
                    if (!string.IsNullOrEmpty(supCol)) { setParts.Add($"[{supCol}] = @sup"); parameters.Add(new SqlParameter("@sup", manvNcc)); }
                    if (!string.IsNullOrEmpty(_colNguoiNhap)) { setParts.Add($"[{_colNguoiNhap}] = @nguoinhap"); parameters.Add(new SqlParameter("@nguoinhap", nguoiNhapKho)); }
                    if (!string.IsNullOrEmpty(_colGhiChu)) { setParts.Add($"[{_colGhiChu}] = @gh"); parameters.Add(new SqlParameter("@gh", gh)); }
                    parameters.Add(new SqlParameter("@id", ma));
                    var sql = $"UPDATE PhieuNhapKho SET {string.Join(", ", setParts)} WHERE [{_colMaPhieu}] = @id";
                    DatabaseHelper.ExecuteNonQuery(sql, parameters.ToArray());
                }
                else
                {
                    // insert - build columns and values dynamically
                    var cols = new System.Collections.Generic.List<string> { _colMaPhieu };
                    var vals = new System.Collections.Generic.List<string> { "@id" };
                    var parameters = new System.Collections.Generic.List<SqlParameter> { new SqlParameter("@id", ma) };
                    if (!string.IsNullOrEmpty(_colNgay)) { cols.Add(_colNgay); vals.Add("@ngay"); parameters.Add(new SqlParameter("@ngay", ngay)); }
                    string supColI = !string.IsNullOrEmpty(_colMaNvcNcc) ? _colMaNvcNcc : _colNhaCungCap;
                    if (!string.IsNullOrEmpty(supColI)) { cols.Add(supColI); vals.Add("@sup"); parameters.Add(new SqlParameter("@sup", manvNcc)); }
                    if (!string.IsNullOrEmpty(_colNguoiNhap)) { cols.Add(_colNguoiNhap); vals.Add("@nguoinhap"); parameters.Add(new SqlParameter("@nguoinhap", nguoiNhapKho)); }
                    if (!string.IsNullOrEmpty(_colGhiChu)) { cols.Add(_colGhiChu); vals.Add("@gh"); parameters.Add(new SqlParameter("@gh", gh)); }

                    var sql = $"INSERT INTO PhieuNhapKho({string.Join(",", cols)}) VALUES({string.Join(",", vals)})";
                    DatabaseHelper.ExecuteNonQuery(sql, parameters.ToArray());
                }

                LoadPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể lưu dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
