using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NU_Solver
{
    public partial class adesboard : Form
    {
        public adesboard()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            upload upload_frm = new upload();
            upload_frm.Show();
            this.Hide();
        }
    }
}
