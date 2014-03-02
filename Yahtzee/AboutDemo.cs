using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Yahtzee
{
    public partial class AboutDemo : Form
    {
        public AboutDemo()
        {
            InitializeComponent();
        }

        private void AboutDemo_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Random_Click(object sender, EventArgs e)
        {
            Main m = new Main();
            m.Show();
            this.Visible = false;
            return;
        }
    }
}
