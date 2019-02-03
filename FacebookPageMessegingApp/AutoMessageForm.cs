using FacebookPageMessegingApp.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacebookPageMessegingApp
{
    public partial class AutoMessageForm : Form
    {
        public AutoMessageForm()
        {
            InitializeComponent();
            GetTargerMessages();
        }
        private List<AutoMessageModel> TargetMessages = new List<AutoMessageModel>();
        private List<string> selectedImages = new List<string>();


        private void GetTargerMessages()
        {
            targetMsgListBox.Items.Clear();

            using (SQLiteConnection con = new SQLiteConnection(Form1.connectionstring))
            {
                SQLiteCommand cmd = new SQLiteCommand("Select * FROM AutoMessageModel", con);
                con.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    AutoMessageModel autoMsg = new AutoMessageModel();
                    autoMsg.Id = reader.GetInt32(0);
                    autoMsg.TargetMessage = reader.GetString(1);
                    autoMsg.Message = reader.GetString(2);
                    TargetMessages.Add(autoMsg);

                }
                con.Close();
            }
            if (TargetMessages.Count > 0)
            {
                targetMsgListBox.DataSource = TargetMessages;
                targetMsgListBox.DisplayMember = "TargetMessage";
                targetMsgListBox.ValueMember = "Id";
                targetMsgListBox.SelectedIndex = -1;
            }
        }


        private void AutoMessageSetupForm_Load(object sender, EventArgs e)
        {

        }

        private void targetMsgListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoMessageModel AutoMsg = targetMsgListBox.SelectedItem as AutoMessageModel;
            if (AutoMsg == null) return;

            targetMessageTextBox.Text = AutoMsg.TargetMessage;
            autoMessageTextBox.Text = AutoMsg.Message;
            GetTotalInfo(AutoMsg.Id);

        }

        private void GetTotalInfo(int id)
        {
            using (SQLiteConnection con = new SQLiteConnection(Form1.connectionstring))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("Select * from Keywords where AutoMessageId ='"+id+"'",con))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    List<Keywords> keywords = new List<Keywords>();
                    keywordListBox.Items.Clear();
                    while (reader.Read())
                    {
                      keywordListBox.Items.Add(reader.GetString(1));
                    }
                }

                using (SQLiteCommand cmd = new SQLiteCommand("Select * from ImagePaths where AutoMessageId ='" + id + "'", con))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    List<string> ImagePaths = new List<string>();
                    while (reader.Read())
                    {
                        ImagePaths.Add(reader.GetString(1));
                    }
                    ImageLoad(ImagePaths.ToArray());
                }
            }
        }


        private void addImgBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select post wise photos",
                Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp"
            };
            if (open.ShowDialog() == DialogResult.OK)
            {
                ImageLoad(open.FileNames);
            }
        }

        private void ImageLoad(string[] loadedimgs)
        {
            imagePanel.Controls.Clear();
            int x = 50;
            int y = 50;
            int maxheight = -1;
            string[] files = loadedimgs;
            addImgBtn.Text = "+" + files.Count();
            foreach (string image in files)
            {
                PictureBox pbx = new PictureBox();
                pbx.Image = Image.FromFile(image);
                pbx.Location = new Point(x, y);
                pbx.SizeMode = PictureBoxSizeMode.StretchImage;
                x += pbx.Width + 10;
                maxheight = Math.Max(pbx.Height, maxheight);
                if (x > this.ClientSize.Width - 100)
                {
                    x = 20;
                    y += maxheight + 10;
                }
                imagePanel.Controls.Add(pbx);
            }
            selectedImages.AddRange(files);
        }

        private void keywordListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MessageBox.Show("Do you want to delete this keyword ?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                (sender as ListBox).Items.Remove(keywordListBox.SelectedItem);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (targetMessageTextBox.Text == "" || autoMessageTextBox.Text == "")
                return;

            string targerMsg = targetMessageTextBox.Text;
            string autoMsg = autoMessageTextBox.Text;

            long lastId = -1;
            List<string> keywords = new List<string>();

            if (keywordListBox.Items.Count > 0)
            {
                keywords = keywordListBox.Items.Cast<String>().ToList();
            }

            using (SQLiteConnection con = new SQLiteConnection(Form1.connectionstring))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("Insert into AutoMessageModel(TargetMessage,Message) values('" + targerMsg + "','" + autoMsg + "'); SELECT last_insert_rowid();", con))
                {
                    lastId = (long)cmd.ExecuteScalar();
                }

                foreach (var keyword in keywords)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("Insert into Keywords(Words,AutoMessageId) values('" + keyword + "','" + lastId + "');", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (var imagepath in selectedImages)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand("Insert into Keywords(ImagePath,AutoMessageId) values('" + imagepath + "','" + lastId + "');", con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                con.Close();
            }
            GetTargerMessages();
        }

        private void addKeywordButton_Click(object sender, EventArgs e)
        {
            if (keywordTextBox.Text == "")
                return;

            string keyword = keywordTextBox.Text;
            keywordListBox.Items.Add(keyword);
        }

        private void keywordTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
            (sender as TextBox).Focus();
        }
    }
}
