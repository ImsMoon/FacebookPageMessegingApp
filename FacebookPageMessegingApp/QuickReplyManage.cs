using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookPageMessegingApp
{
    public partial class QuickReplyManage : Form
    {
        public string connectionstring = Properties.Settings.Default.Database1ConnectionString;

        public QuickReplyManage()
        {
            InitializeComponent();
        }

        private void QuickReplyManage_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Reply");

            using (SQLiteConnection con = new SQLiteConnection(connectionstring))
            {
                SQLiteCommand cmd = new SQLiteCommand("Select * FROM QuickReplyTable", con);
                con.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                con.Close();
            }
            dataGridView1.DataSource = dt;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SaveReply_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection con = new SQLiteConnection(connectionstring))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("Delete FROM QuickReplyTable", con))
                {
                    cmd.ExecuteNonQuery();
                }
                con.Close();
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString() != "")
                    {
                        SQLiteCommand cmd2 = new SQLiteCommand("Insert into QuickReplyTable (Reply) Values('" + dataGridView1.Rows[i].Cells[0].Value + "')", con);
                        con.Open();
                        cmd2.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            //this.labelTableTableAdapter.Update(this.database1DataSet.LabelTable);
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
