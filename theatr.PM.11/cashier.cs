using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace theatr.PM._11
{
    public partial class cashier: Form
    {
        public cashier()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            actor actor = new actor();
            actor.Show();
            this.Close();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            spekk spekk = new spekk();
            spekk.Show();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
