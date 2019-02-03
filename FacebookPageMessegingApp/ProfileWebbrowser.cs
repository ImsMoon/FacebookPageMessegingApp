using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookPageMessegingApp
{
    public partial class ProfileWebbrowser : Form
    {
        private string link = "";
        public ProfileWebbrowser(string link)
        {
            InitializeComponent();
            this.link = link;
        }

        private void ProfileWebbrowser_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(link))
                webBrowser.Navigate(link);
            else
                MessageBox.Show("Log in first");
        }
    }
}
