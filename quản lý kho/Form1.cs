using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using quản_lý_kho.Views;

namespace quản_lý_kho
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
            this.Controls.SetChildIndex(this.lblTitle, 0); // Put label behind MDI children
        }

        private void ShowForm<T>() where T : Form, new()
        {
            // If a child of the requested type already exists, activate it and close others
            foreach (Form child in this.MdiChildren)
            {
                if (child is T)
                {
                    // Close any other children so only this one remains
                    foreach (Form other in this.MdiChildren)
                    {
                        if (!ReferenceEquals(other, child))
                            other.Close();
                    }
                    child.Activate();
                    // Ensure label is hidden when a child is active
                    lblTitle.Visible = false;
                    return;
                }
            }

            // Close all existing children so only the newly opened child is visible
            foreach (Form child in this.MdiChildren)
            {
                child.Close();
            }

            T frm = new T();
            frm.MdiParent = this;
            // Hide the center label while a child form is open
            lblTitle.Visible = false;
            // When the child is closed and there are no other children, show the label again
            frm.FormClosed += (s, args) => { if (this.MdiChildren.Length == 0) lblTitle.Visible = true; };
            // Open the child maximized so windows don't overlap
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void phieuNhapKhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm<FormPhieuNhapKho>();
        }

        private void phieuXuatKhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm<FormPhieuXuatKho>();
        }

        private void phieuKiemKeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm<FormPhieuKiemKe>();
        }

        private void timKiemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm<FormTimKiem>();
        }

        private void inBaoCaoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm<FormInBaoCao>();
        }

        private void maHangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm<FormMatHang>();
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
