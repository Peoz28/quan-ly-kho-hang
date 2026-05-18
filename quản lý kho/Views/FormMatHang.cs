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
    public partial class FormMatHang : Form
    {
        private BindingSource _bs = new BindingSource();
        private int _pageIndex = 1;
        private int _pageSize = 10;
        private int _pageCount = 0;
        private string _orderBy = "MaHang";
        private string _baseSql = "SELECT * FROM MatHang";
        
        private string _colMaHang = "MaHang";
        private string _colTenHang = "TenHang";
        private string _colLoaiHang = "LoaiHang";
        private string _colDonViTinh = "DonViTinh";
        private string _colDonGia = "Đơn Giá";
        
        private bool _bindingsInitialized = false;

        public FormMatHang()
        {
            InitializeComponent();
            Load += FormMatHang_Load;
            btnPrev.Click += (s, e) => { if (_pageIndex > 1) { _pageIndex--; LoadPage(); } };
            btnNext.Click += (s, e) => { if (_pageIndex < _pageCount) { _pageIndex++; LoadPage(); } };
            btnReload.Click += (s, e) => LoadPage();
            btnThem.Click += BtnThem_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLuu.Click += BtnLuu_Click;
            btnTimKiem.Click += BtnTimKiem_Click;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void FormMatHang_Load(object sender, EventArgs e)
        {
            try
            {
                // Detect columns first to avoid invalid order by column
                var schema = DatabaseHelper.LoadTable("SELECT TOP(0) * FROM MatHang");
                var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn c in schema.Columns) cols.Add(c.ColumnName);

                string Pick(params string[] candidates)
                {
                    foreach (var c in candidates) if (cols.Contains(c)) return c;
                    return null;
                }

                _colMaHang = Pick("Ma Hàng", "MaHang", "MaMH", "MaMatHang", "ID", "MaSanPham", "MaSP") ?? schema.Columns[0].ColumnName;
                _orderBy = $"[{_colMaHang}]";
                _colTenHang = Pick("Tên Hàng", "TenHang", "TenMH", "TenMatHang", "Ten", "TenSanPham", "TenSP") ?? _colTenHang;
                _colLoaiHang = Pick("Loại hàng", "LoaiHang", "LoaiMH", "MaLoai", "PhanLoai", "NhomHang") ?? _colLoaiHang;
                _colDonViTinh = Pick("Đơn vị tính", "DonViTinh", "DVT", "DonVi") ?? _colDonViTinh;
                _colDonGia = Pick("Đơn Giá", "DonGia", "Gia", "GiaBan", "GiaNhap") ?? _colDonGia;

                LoadPage();
                dgvData.DataSource = _bs;
                
                txtMaHang.DataBindings.Clear();
                txtTenHang.DataBindings.Clear();
                txtLoaiHang.DataBindings.Clear();
                txtDonViTinh.DataBindings.Clear();
                txtDonGia.DataBindings.Clear();

                if (cols.Contains(_colMaHang)) txtMaHang.DataBindings.Add("Text", _bs, _colMaHang, true, DataSourceUpdateMode.OnPropertyChanged);
                if (cols.Contains(_colTenHang)) txtTenHang.DataBindings.Add("Text", _bs, _colTenHang, true, DataSourceUpdateMode.OnPropertyChanged);
                if (cols.Contains(_colLoaiHang)) txtLoaiHang.DataBindings.Add("Text", _bs, _colLoaiHang, true, DataSourceUpdateMode.OnPropertyChanged);
                if (cols.Contains(_colDonViTinh)) txtDonViTinh.DataBindings.Add("Text", _bs, _colDonViTinh, true, DataSourceUpdateMode.OnPropertyChanged);
                if (cols.Contains(_colDonGia)) txtDonGia.DataBindings.Add("Text", _bs, _colDonGia, true, DataSourceUpdateMode.OnPropertyChanged);

                _bindingsInitialized = true;
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
            var totalTable = DatabaseHelper.LoadTable("SELECT COUNT(1) FROM MatHang");
            var total = totalTable.Rows.Count > 0 ? Convert.ToInt32(totalTable.Rows[0][0]) : 0;
            _pageCount = (int)Math.Ceiling(total / (double)_pageSize);
            lblPage.Text = $"Page {_pageIndex}/{Math.Max(1, _pageCount)}";
            btnPrev.Enabled = _pageIndex > 1;
            btnNext.Enabled = _pageIndex < _pageCount;
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            txtMaHang.Text = "";
            txtTenHang.Text = "";
            txtLoaiHang.Text = "";
            txtDonViTinh.Text = "";
            txtDonGia.Text = "";
            txtMaHang.Focus();
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (_bs.Current == null) return;
            var id = ((DataRowView)_bs.Current).Row[_colMaHang].ToString();
            if (MessageBox.Show($"Xóa mặt hàng {id}?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatabaseHelper.ExecuteNonQuery($"DELETE FROM MatHang WHERE [{_colMaHang}] = @id", new SqlParameter("@id", id));
                LoadPage();
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                var id = txtMaHang.Text.Trim();
                if (string.IsNullOrEmpty(id)) return;

                var exists = DatabaseHelper.LoadTable($"SELECT COUNT(1) FROM MatHang WHERE [{_colMaHang}] = @id", new SqlParameter("@id", id));
                bool isUpdate = Convert.ToInt32(exists.Rows[0][0]) > 0;

                var parameters = new List<SqlParameter> { 
                    new SqlParameter("@id", id),
                    new SqlParameter("@ten", txtTenHang.Text),
                    new SqlParameter("@loai", txtLoaiHang.Text),
                    new SqlParameter("@dv", txtDonViTinh.Text),
                    new SqlParameter("@gia", string.IsNullOrWhiteSpace(txtDonGia.Text) ? (object)DBNull.Value : decimal.TryParse(txtDonGia.Text.Replace(",","."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal giaVal) ? (object)giaVal : DBNull.Value)
                };

                if (isUpdate)
                {
                    var sql = $"UPDATE MatHang SET [{_colTenHang}] = @ten, [{_colLoaiHang}] = @loai, [{_colDonViTinh}] = @dv, [{_colDonGia}] = @gia WHERE [{_colMaHang}] = @id";
                    DatabaseHelper.ExecuteNonQuery(sql, parameters.ToArray());
                }
                else
                {
                    var sql = $"INSERT INTO MatHang([{_colMaHang}], [{_colTenHang}], [{_colLoaiHang}], [{_colDonViTinh}], [{_colDonGia}]) VALUES(@id, @ten, @loai, @dv, @gia)";
                    DatabaseHelper.ExecuteNonQuery(sql, parameters.ToArray());
                }
                LoadPage();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            var kw = txtMaHang.Text.Replace("'", "''");
            var sql = _baseSql + $" WHERE [{_colMaHang}] LIKE '%{kw}%' OR [{_colTenHang}] LIKE '%{kw}%'";
            _bs.DataSource = DatabaseHelper.LoadTable(sql);
            btnPrev.Enabled = btnNext.Enabled = false;
        }
    }
}
