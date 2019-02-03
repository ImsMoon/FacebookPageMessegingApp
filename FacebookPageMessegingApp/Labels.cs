using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
namespace FacebookPageMessegingApp
{
    public partial class LabelManageForm : Form
    {
        public string connectionstring = Properties.Settings.Default.Database1ConnectionString;
            //Properties.Settings.Default.Database1ConnectionString;

        public LabelManageForm()
        {
            InitializeComponent();
        }

        private void LabelManageForm_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Labels");
            
            using (SQLiteConnection con = new SQLiteConnection(connectionstring))
            {
                SQLiteCommand cmd = new SQLiteCommand("Select * FROM LabelTable", con);
                con.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                con.Close();               
            }
            dataGridView1.DataSource = dt;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnCloseLabelBox_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection con = new SQLiteConnection(connectionstring))
            {
                SQLiteCommand cmd = new SQLiteCommand("Delete FROM LabelTable", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString() != "")
                    {
                        SQLiteCommand cmd2 = new SQLiteCommand("Insert into LabelTable (Labels) Values('" + dataGridView1.Rows[i].Cells[0].Value + "')", con);
                        con.Open();
                        cmd2.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            //this.labelTableTableAdapter.Update(this.database1DataSet.LabelTable);
            this.Close();
        }
    }
}
