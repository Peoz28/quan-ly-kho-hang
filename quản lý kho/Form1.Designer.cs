namespace quản_lý_kho
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.trangChuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nhapLieuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phieuNhapKhoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phieuXuatKhoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.phieuKiemKeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maHangToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timKiemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inBaoCaoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTitle = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trangChuToolStripMenuItem,
            this.nhapLieuToolStripMenuItem,
            this.timKiemToolStripMenuItem,
            this.inBaoCaoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1000, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // trangChuToolStripMenuItem
            // 
            this.trangChuToolStripMenuItem.Name = "trangChuToolStripMenuItem";
            this.trangChuToolStripMenuItem.Size = new System.Drawing.Size(87, 24);
            this.trangChuToolStripMenuItem.Text = "Trang chủ";
            // 
            // nhapLieuToolStripMenuItem
            // 
            this.nhapLieuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.phieuNhapKhoToolStripMenuItem,
            this.phieuXuatKhoToolStripMenuItem,
            this.phieuKiemKeToolStripMenuItem,
            this.maHangToolStripMenuItem});
            this.nhapLieuToolStripMenuItem.Name = "nhapLieuToolStripMenuItem";
            this.nhapLieuToolStripMenuItem.Size = new System.Drawing.Size(87, 24);
            this.nhapLieuToolStripMenuItem.Text = "Nhập liệu";
            // 
            // phieuNhapKhoToolStripMenuItem
            // 
            this.phieuNhapKhoToolStripMenuItem.Name = "phieuNhapKhoToolStripMenuItem";
            this.phieuNhapKhoToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.phieuNhapKhoToolStripMenuItem.Text = "Phiếu nhập kho";
            this.phieuNhapKhoToolStripMenuItem.Click += new System.EventHandler(this.phieuNhapKhoToolStripMenuItem_Click);
            // 
            // phieuXuatKhoToolStripMenuItem
            // 
            this.phieuXuatKhoToolStripMenuItem.Name = "phieuXuatKhoToolStripMenuItem";
            this.phieuXuatKhoToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.phieuXuatKhoToolStripMenuItem.Text = "Phiếu xuất kho";
            this.phieuXuatKhoToolStripMenuItem.Click += new System.EventHandler(this.phieuXuatKhoToolStripMenuItem_Click);
            // 
            // phieuKiemKeToolStripMenuItem
            // 
            this.phieuKiemKeToolStripMenuItem.Name = "phieuKiemKeToolStripMenuItem";
            this.phieuKiemKeToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.phieuKiemKeToolStripMenuItem.Text = "Phiếu kiểm kê";
            this.phieuKiemKeToolStripMenuItem.Click += new System.EventHandler(this.phieuKiemKeToolStripMenuItem_Click);
            // 
            // maHangToolStripMenuItem
            // 
            this.maHangToolStripMenuItem.Name = "maHangToolStripMenuItem";
            this.maHangToolStripMenuItem.Size = new System.Drawing.Size(193, 26);
            this.maHangToolStripMenuItem.Text = "Mã hàng";
            this.maHangToolStripMenuItem.Click += new System.EventHandler(this.maHangToolStripMenuItem_Click);
            // 
            // timKiemToolStripMenuItem
            // 
            this.timKiemToolStripMenuItem.Name = "timKiemToolStripMenuItem";
            this.timKiemToolStripMenuItem.Size = new System.Drawing.Size(84, 24);
            this.timKiemToolStripMenuItem.Text = "Tìm kiếm";
            this.timKiemToolStripMenuItem.Click += new System.EventHandler(this.timKiemToolStripMenuItem_Click);
            // 
            // inBaoCaoToolStripMenuItem
            // 
            this.inBaoCaoToolStripMenuItem.Name = "inBaoCaoToolStripMenuItem";
            this.inBaoCaoToolStripMenuItem.Size = new System.Drawing.Size(93, 24);
            this.inBaoCaoToolStripMenuItem.Text = "In báo cáo";
            this.inBaoCaoToolStripMenuItem.Click += new System.EventHandler(this.inBaoCaoToolStripMenuItem_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Blue;
            this.lblTitle.Location = new System.Drawing.Point(0, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1000, 600);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Quản lý kho hàng điện máy";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 628);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Trang chủ";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem trangChuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nhapLieuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem phieuNhapKhoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem phieuXuatKhoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem phieuKiemKeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem maHangToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timKiemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inBaoCaoToolStripMenuItem;
        private System.Windows.Forms.Label lblTitle;
    }
}

