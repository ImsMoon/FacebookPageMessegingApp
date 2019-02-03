using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Facebook;
using FacebookPageMessegingApp.DataModels;
using System.Data.Sql;
using System.Reflection;
using System.Data.SQLite;
using System.Diagnostics;

namespace FacebookPageMessegingApp
{
    public partial class Form1 : Form
    {
        private string Profile_Access_token = null;
        private const string AppId = "464048570771518";
        private const string ExtendedPermissions = "read_page_mailboxes";
        private string _accessToken;
        private readonly DataGridViewLinkColumn btn = new DataGridViewLinkColumn();
        public static string connectionstring = Properties.Settings.Default.Database1ConnectionString;
        //public static string GetFolderPath= Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

        List<ConversationView> CompletedConvList = new List<ConversationView>();/// list for checking new msg

        string CurrPageAccessToken;
        private string User_Profile_Link = "";
        List<ConversationModel> ConvList = new List<ConversationModel>();

        public string noteString { get; set; }
        public string[] rowNumnerfromNeotes = new string[100];
        public Form1()
        {
            InitializeComponent();
            //call dialog prompt

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        private void FbLogin_Click(object sender, EventArgs e)
        {
            progressBar1.Show();
            label3.Text = "Facebook login";
            if (FbLoginbtn.Text == "Logout")
            {
                pagelist.DataSource = null;
                textBox1.Clear();
                pagelist.Text = "Select Facebook Page";
                FbLoginbtn.Text = "Login";
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                linkLabel1.Text = "";
                label2.Hide();
                User_Profile_Link = "";
                Profile_Access_token = null;
                //logout cache clear
                var fb = new FacebookClient();
                var logoutUrl = fb.GetLogoutUrl(new
                {
                    next = "https://www.facebook.com/connect/login_success.html",
                    access_token = _accessToken
                });
                var webBrowser = new WebBrowser();
                //webBrowser.Navigated += (o, args) =>
                //{
                //    if (args.Url.AbsoluteUri == "https://www.facebook.com/connect/login_success.html")
                //       this.Close();
                //};
                webBrowser.Navigate(logoutUrl.AbsoluteUri);
            }
            else
            {
                var FacebookLogin = new FacebookLoginDialog(AppId, ExtendedPermissions);
                FacebookLogin.ShowDialog();
                FbLoginFunction(FacebookLogin.FacebookOAuthResult);
            }
            progressBar1.Hide();
            label3.Text = "";
        }

        private void FbLoginFunction(FacebookOAuthResult facebookOAuthResult)
        {
            if (facebookOAuthResult != null)
            {
                if (facebookOAuthResult.IsSuccess)
                {
                    _accessToken = facebookOAuthResult.AccessToken;
                    Profile_Access_token = _accessToken;
                    var fb = new FacebookClient(facebookOAuthResult.AccessToken);
                    dynamic result = fb.Get("/me");
                    var name = result.name;
                    linkLabel1.Text = name;
                    label2.Show();
                    linkLabel1.Show();

                    dynamic profileLink = fb.Get("/me?fields=link");
                    User_Profile_Link = profileLink.link;
                    
                    // to get page list with id and name
                    dynamic pages = fb.Get("/me?fields=accounts{name,id}");

                    FbPage pg1 = new FbPage();
                    List<FbPage> PageList = new List<FbPage>();
                    pg1.Name = "Select Facebook Page";
                    pg1.Id = "0";

                    if (pages.accounts != null)
                    {
                        var PageNameFromAccount = pages.accounts.data;

                        PageList.Add(pg1);

                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                FbPage pg = new FbPage();
                                pg.Id = PageNameFromAccount[i].id;
                                pg.Name = PageNameFromAccount[i].name;
                                PageList.Add(pg);
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        }
                    }



                    /// var PageNameFromData = PageNameFromAccount[0].name;

                    pagelist.DataSource = PageList;
                    pagelist.ValueMember = "Id";
                    pagelist.DisplayMember = "Name";
                    FbLoginbtn.Text = "Logout";
                }
                else
                {
                    MessageBox.Show(facebookOAuthResult.ErrorDescription);
                }
            }
        }
        private async Task<List<ConversationView>> getconversationlistAsync(string CurrentpageAccesstoken)
        {

            List<ConversationView> AllConversationViewList = new List<ConversationView>();
            //start getting all conversation list of selected page
            var FbClient = new FacebookClient();
            FbClient.AccessToken = CurrentpageAccesstoken;
            CurrPageAccessToken = CurrentpageAccesstoken;
            List<ConversationModel> AllConversationList = new List<ConversationModel>();
            label3.Text = "Loading...(Please wait)";
            progressBar1.Show();
            dynamic AllmessageJson = await FbClient.GetTaskAsync("me?fields=conversations.limit(505){message_count,link,updated_time,snippet,senders,unread_count}");
            var AllMessageData = AllmessageJson.conversations.data;
            label3.Text = "Loading Conversation (1)";
            foreach (var item in AllMessageData)
            {
                ConversationModel singleconversation = new ConversationModel();
                singleconversation.message_count = item.message_count.ToString();
                singleconversation.link = item.link.ToString();
                try
                {
                    singleconversation.snippet = item.snippet.ToString();
                }
                catch
                {
                    singleconversation.snippet = "[sticker/image]";
                }
                singleconversation.updated_time = DateTime.Parse(item.updated_time);
                singleconversation.id = item.id.ToString();
                singleconversation.unread_count = item.unread_count.ToString();
                var senders = item.senders.data;
                List<MessageSender> senderlist = new List<MessageSender>();
                foreach (var senderitem in senders)
                {
                    MessageSender sender2 = new MessageSender();
                    sender2.id = senderitem.id.ToString();
                    sender2.name = senderitem.name.ToString();
                    sender2.email = senderitem.email.ToString();
                    senderlist.Add(sender2);
                }
                singleconversation.messageSenders = senderlist;
                AllConversationList.Add(singleconversation);

            }
            CompletedConvList = AllConversationViewList;
            dynamic next = AllmessageJson.conversations.paging.next;

            for (int c = 0; c < 100; c++)// i < 100 for 32K msg
            {
                try
                {
                    dynamic AllmessageJson2 = await FbClient.GetTaskAsync(next);
                    var AllMessageData2 = AllmessageJson2.data;
                    next = AllmessageJson2.paging.next;
                    int a = 0;

                    a = AllConversationList.Count;
                    label3.Text = "Loading Conversation (" + a + ")";

                    foreach (var item in AllMessageData2)
                    {

                        ConversationModel singleconversation = new ConversationModel();
                        singleconversation.message_count = item.message_count.ToString();
                        singleconversation.link = item.link.ToString();
                        try
                        {
                            singleconversation.snippet = item.snippet.ToString();

                        }
                        catch
                        {
                            singleconversation.snippet = "[sticker/image]";
                        }
                        singleconversation.updated_time = DateTime.Parse(item.updated_time);
                        singleconversation.id = item.id.ToString();
                        singleconversation.unread_count = item.unread_count.ToString();
                        var senders = item.senders.data;
                        List<MessageSender> senderlist = new List<MessageSender>();
                        foreach (var senderitem in senders)
                        {
                            MessageSender sender2 = new MessageSender();
                            sender2.id = senderitem.id.ToString();
                            sender2.name = senderitem.name.ToString();
                            sender2.email = senderitem.email.ToString();
                            senderlist.Add(sender2);
                        }
                        singleconversation.messageSenders = senderlist;
                        AllConversationList.Add(singleconversation);
                    }
                }
                catch
                {
                    break;
                }
            }
            label3.Show();
            label3.Text = "Loading done. Please wait.";

            ConvList = AllConversationList;
            //insert ino database
            foreach (var item in AllConversationList)
            {
                ConversationView conv = new ConversationView();
                conv.message_id = item.id;
                conv.link = item.link;
                conv.message_count = item.message_count;
                conv.unread_count = item.unread_count;
                conv.updated_time = item.updated_time;
                conv.snippet = item.snippet;
                conv.messageSenders = item.messageSenders;
                DataTable dataTable = new DataTable();
                IEnumerable<DataRow> dataRows;

                using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                {
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM MainConversation where message_id='" + item.id + "'", con);
                    con.Open();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    dataTable.Load(reader);
                    dataRows = dataTable.AsEnumerable();
                }
                foreach (var row in dataRows)
                {
                    conv.Labels = conv.Labels + "," + row.ItemArray[1].ToString();
                    conv.Notes = conv.Notes + "," + row.ItemArray[2].ToString();
                    conv.FB_Labels = conv.FB_Labels + "," + row.ItemArray[3].ToString();
                    conv.FB_Notes = conv.FB_Notes + "," + row.ItemArray[4].ToString();
                }
                AllConversationViewList.Add(conv);
            }
            return AllConversationViewList;
        }

        DataTable Pagedt = new DataTable();// dataTabel for datagridview

        private void Datagridload(List<ConversationView> AllConversationViewList)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();


            Pagedt.Columns.Add("Serial");
            Pagedt.Columns.Add("Sender");
            Pagedt.Columns.Add("Message");
            Pagedt.Columns.Add("Updated Time");
            Pagedt.Columns.Add("Count");
            Pagedt.Columns.Add("Unread");
            Pagedt.Columns.Add("Labels");
            Pagedt.Columns.Add("Notes");
            Pagedt.Columns.Add("FB Labels");
            Pagedt.Columns.Add("FB Notes");
            Pagedt.Columns.Add("MessageId");
            Pagedt.Columns.Add("Link");
            Pagedt.Columns.Add("Senderid");


            // adddataRow(AllConversationViewList);

            int serial = AllConversationViewList.Count;
            foreach (var item in AllConversationViewList)
            {
                Pagedt.Rows.Add(serial, item.messageSenders[0].name, item.snippet, item.updated_time, item.message_count, item.unread_count, item.Labels, item.Notes, item.FB_Labels, item.FB_Notes, item.message_id, item.link, item.messageSenders[0].id);
                btn.Text = serial.ToString();
                serial--;

            }
            dataGridView1.DataSource = Pagedt;
            dataGridView1.Columns["Link"].Visible = false;
            dataGridView1.Columns["Senderid"].Visible = false;
            dataGridView1.Columns["MessageId"].Visible = false;
            dataGridView1.Columns["Updated Time"].DefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            //adddataRow(AllConversationViewList);
            int i = 0, gridviewrowno = dataGridView1.Rows.Count;
            for (i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                //link creation
                DataGridViewLinkCell updatecell = new DataGridViewLinkCell();
                updatecell.Value = AllConversationViewList[i].updated_time;
                dataGridView1.Rows[i].Cells[3] = updatecell;
            }
        }

        public void adddataRow(List<ConversationView> AllConversationViewList)
        {
            int serial = AllConversationViewList.Count;
            foreach (var item in AllConversationViewList)
            {
                dataGridView1.Rows.Add(serial, item.messageSenders[0].name, item.snippet, item.updated_time, item.message_count, item.unread_count, item.Labels, item.Notes, item.FB_Labels, item.FB_Notes, item.message_id);
                btn.Text = serial.ToString();
                serial--;

            }
        }

        private void dataGridView1_CellClickAsync(object sender, DataGridViewCellEventArgs e)
        {
            if (Profile_Access_token != null)
            {
                //Cursor.Current = Cursors.WaitCursor;
                var fb = new FacebookClient();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                string CurrentPageId = pagelist.SelectedValue.ToString();


                if (CurrentPageId != "0" && Profile_Access_token != null)
                {
                    if (e.ColumnIndex == 3)
                    {
                        dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");

                        //on click redirect to browser inbox

                        try
                        {
                            string msg = "https://www.facebook.com" + dataGridView1.CurrentRow.Cells[11].Value.ToString();
                            System.Diagnostics.Process.Start(msg);
                        }
                        catch (Exception)
                        { }

                    }
                    else
                    {
                        //now load the selected row conversation in conversation box
                        string SelectedRow = e.RowIndex.ToString();
                        if (SelectedRow != "-1")
                        {
                            int i = Convert.ToInt32(SelectedRow);
                            textBox1.Clear();
                            textBox1.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value.ToString();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Login First.");
                }
            }
        }

        private void loadsingleconversation(string CurrentpageAccesstoken, string message_id)
        {
            var FbClient = new FacebookClient();
            FbClient.AccessToken = CurrentpageAccesstoken;
            dynamic messages = FbClient.Get(message_id + "?fields=messages{from,to,message,created_time,sticker,attachments{image_data}}");
            var messagesfromjson = messages.messages.data;
            //load each message into list object 
            List<MessageModel> FullConversation = new List<MessageModel>();

            foreach (var item in messagesfromjson)
            {
                MessageModel singlemessage = new MessageModel();
                singlemessage.fromname = item.from.name.ToString();
                singlemessage.fromid = item.from.id.ToString();
                singlemessage.toname = item.to.data[0].name.ToString();
                singlemessage.toid = item.to.data[0].id.ToString();
                singlemessage.messagetext = item.message.ToString();
                singlemessage.created_time = DateTime.Parse(item.created_time);
                try
                {
                    singlemessage.imageurl = item.attachments.data[0].image_data.url.ToString();
                }
                catch
                {
                    singlemessage.imageurl = null;
                }

                FullConversation.Add(singlemessage);
            }
            //now load the conversation into conversation block
            textBox1.Clear();
            for (int i = FullConversation.Count - 1; i >= 0; i--)
            {
                if (FullConversation[i].messagetext == "")
                {
                    textBox1.AppendText(FullConversation[i].fromname + " : " + "[image/sticker]");
                    textBox1.AppendText("\n");
                    textBox1.AppendText("\n");
                }
                else
                {
                    textBox1.AppendText(FullConversation[i].fromname + " : " + FullConversation[i].messagetext);
                    textBox1.AppendText("\n");
                    textBox1.AppendText("\n");
                }
            }
        }
        //this one is load full conversation function
        private void button1_Click(object sender, EventArgs e)
        {
            if (Profile_Access_token != null)
            {
                progressBar1.Show();
                label3.Text = "Loading conversation";
                Cursor.Current = Cursors.WaitCursor;
                var fb = new FacebookClient();
                fb.AccessToken = Profile_Access_token;
                string CurrentPageId = pagelist.SelectedValue.ToString();
                if (CurrentPageId != "0")
                {
                    dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");
                    string CurrentpageAccesstoken = CurrentPageTokenCall.access_token;
                    //List<ConversationView> conversationViews = new List<ConversationView>();
                    //conversationViews = getconversationlistAsync(CurrentpageAccesstoken);

                    string SelectedRow = dataGridView1.CurrentRow.Index.ToString();
                    //int i = Convert.ToInt32(SelectedRow);
                    //load single converssation usng message id
                    //if (i >= 0 && i < conversationViews.Count)
                    string message_id = dataGridView1.CurrentRow.Cells[10].Value.ToString();
                    loadsingleconversation(CurrentpageAccesstoken, message_id);
                }
                progressBar1.Hide();
                label3.Text = "";
                Cursor.Current = Cursors.Default;

            }
            else
            {
                MessageBox.Show("Please Login First.");
            }

        }

        private async void pagelist_SelectedIndexChangedAsync(object sender, EventArgs e)
        {
            var fb = new FacebookClient();
            fb.AccessToken = Profile_Access_token;
            textBox1.Clear();
            //start getting selected page access token
            string CurrentPageId = pagelist.SelectedValue.ToString();
            if (CurrentPageId != "0")
            {

                dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");
                string CurrentpageAccesstoken = CurrentPageTokenCall.access_token;
                List<ConversationView> AllConversationViewList = new List<ConversationView>();
                try
                {
                    AllConversationViewList = await getconversationlistAsync(CurrentpageAccesstoken);
                    Datagridload(AllConversationViewList);
                }
                catch
                {
                }

            }
            else
            {
                textBox1.Clear();
                pagelist.Text = "Select Facebook Page";
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
            }
            label3.Text = "";
            progressBar1.Hide();
            timer1.Start();
        }

        private async void replyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (Profile_Access_token != null)
            {
                string promptValue;
                //call dialog
                if (dataGridView1.SelectedRows.Count >= 4)// Delay time 4+ msg
                {
                    promptValue = ShowreplyDialog("Enter your message :", "Reply", "", "yes");
                }
                else
                {
                    promptValue = ShowreplyDialog("Enter your message :", "Reply", "", "no");
                }
                if (promptValue != "")
                {
                    int ind = promptValue.IndexOf('*');
                    var message = promptValue.Substring(0, ind);
                    var imgfileandsecound = promptValue.Substring(ind + 1);
                    int ind2 = imgfileandsecound.IndexOf('*');
                    var imgfile = imgfileandsecound.Substring(0, ind2);
                    var secound = imgfileandsecound.Substring(ind2 + 1);
                    if (imgfile == "" && message != "")
                    {
                        await replymenustripclick(promptValue);

                    }
                    else if (imgfile != "" && message != "")
                    {

                        await replymenustripclick(promptValue);

                    }
                    else if (imgfile != "" && message == "")
                    {
                        await replymenustripclick(promptValue);

                    }
                    else
                    {

                    }
                }
            }
            timer1.Start();
        }

        private async Task replymenustripclick(string promptValue)
        {
            progressBar1.Show();
            int ind = promptValue.IndexOf('*');
            var message = promptValue.Substring(0, ind);
            var imgfileandsecound = promptValue.Substring(ind + 1);
            int ind2 = imgfileandsecound.IndexOf('*');
            var imgfile = imgfileandsecound.Substring(0, ind2);
            var secound = imgfileandsecound.Substring(ind2 + 1);
            int s = 0;

            try
            {
                s = Convert.ToInt32(secound);
            }
            catch
            {
                s = 0;
            }
            int index = 0;


            if (message != "" && imgfile == "")
            {
                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {
                    index = dataGridView1.SelectedRows[i].Index;
                    dataGridView1.Rows[index].Cells[2].Value = message;
                }
                //
                var fb = new FacebookClient();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                string CurrentPageId = pagelist.SelectedValue.ToString();
                fb.AccessToken = Profile_Access_token;

                //start getting selected page access token
                if (CurrentPageId != "0")
                {
                    dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");
                    index = 0;

                    for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                    {
                        index = dataGridView1.SelectedRows[i].Index;

                        label3.Text = "(" + (i + 1) + "/" + dataGridView1.SelectedRows.Count + ")" + " Sending message to " + dataGridView1.Rows[index].Cells[1].Value.ToString();
                        label3.Invalidate();//// most important to show label
                        System.Threading.Thread.Sleep(3000);
                        label3.Refresh();

                       
                        int a = await sendmessage(dataGridView1.Rows[index].Cells[12].Value.ToString(), dataGridView1.Rows[index].Cells[10].Value.ToString(), message, null);
                        if (a == 1)
                        {
                        }
                        else
                        {
                            dataGridView1.Rows[index].Cells[2].Value = "N/A";
                            dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                        }
                    }

                    progressBar1.Hide();
                    label3.Text = "";

                    MessageBox.Show("Message sent successfully.\n" + "Total " + dataGridView1.SelectedRows.Count + " message sent.");


                }
            }
            else if (message == "" && imgfile != "")
            {
                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {
                    // System.Threading.Thread.Sleep(s * 1000);
                    index = dataGridView1.SelectedRows[i].Index;
                    dataGridView1.Rows[index].Cells[2].Value = message;
                }
                //
                var fb = new FacebookClient();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                string CurrentPageId = pagelist.SelectedValue.ToString();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                if (CurrentPageId != "0")
                {
                    dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");

                    index = 0;
                    for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                    {
                        index = dataGridView1.SelectedRows[i].Index;
                        int a = await sendmessage(dataGridView1.Rows[index].Cells[12].Value.ToString(), dataGridView1.Rows[index].Cells[10].Value.ToString(), "", imgfile);
                        
                        label3.Text = "(" + (i + 1) + "/" + dataGridView1.SelectedRows.Count + ")" + " Sending message to " + dataGridView1.Rows[index].Cells[1].Value.ToString();
                        label3.Invalidate();
                        System.Threading.Thread.Sleep(3000);
                        label3.Refresh();

                        if (a == 1)
                        {

                        }
                        else
                        {
                            dataGridView1.Rows[index].Cells[2].Value = "N/A";
                            dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                        }
                        
                    }

                    progressBar1.Hide();
                    label3.Text = "";
                    MessageBox.Show("Message sent successfully.\n" + "Total " + dataGridView1.SelectedRows.Count + " message sent.");

                }

            }
            else
            {
                progressBar1.Show();
                label3.Text = "Sending text";
                label3.Show();

                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {
                    index = dataGridView1.SelectedRows[i].Index;
                    dataGridView1.Rows[index].Cells[2].Value = message;
                }
                //
                var fb = new FacebookClient();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                string CurrentPageId = pagelist.SelectedValue.ToString();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                if (CurrentPageId != "0")
                {
                    dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");

                    index = 0;
                    for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                    {

                        index = dataGridView1.SelectedRows[i].Index;
                        int a = 0;
                        a = await sendmessage(dataGridView1.Rows[index].Cells[12].Value.ToString(), dataGridView1.Rows[index].Cells[10].Value.ToString(), message, imgfile);

                        label3.Text = "(" + (i + 1) + "/" + dataGridView1.SelectedRows.Count + ")" + " Sending message to " + dataGridView1.Rows[index].Cells[1].Value.ToString();
                        label3.Invalidate();
                        System.Threading.Thread.Sleep(3000);
                        label3.Refresh();

                        //var w = new Label();// { Size = new Size(0, 0) };
                        //w = label3;
                        //w.Text = "Sending message to " + dataGridView1.Rows[index].Cells[1].Value.ToString();
                        //Task.Delay(TimeSpan.FromSeconds(s)).ContinueWith((t) => w.Show(), TaskScheduler.FromCurrentSynchronizationContext());
                        //w.ControlBox = false;
                        //MessageBox.Show(w, "Sending message to " + dataGridView1.Rows[index].Cells[1].Value.ToString());
                        // MessageBox.Show("Sending message to " + dataGridView1.Rows[index].Cells[1].Value.ToString());
                        // msgdeliverymsg(dataGridView1.Rows[index].Cells[1].Value.ToString(), dataGridView1.SelectedRows.Count);
                        // System.Threading.Thread.Sleep(s*1000);
                        if (a == 0)
                        {
                            dataGridView1.Rows[index].Cells[2].Value = "N/A";
                            dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                        }
                        else
                        {
                            
                        }
                    }

                    progressBar1.Hide();
                    label3.Text = "";
                    MessageBox.Show("Message sent successfully.\n" + "Total " + dataGridView1.SelectedRows.Count + " message sent.");
                }

            }
            progressBar1.Hide();
        }

        public static string ShowreplyDialog(string name, string title, string txtboxtxt, string showDelay)
        {
            Form prompt = new Form();
            prompt.MaximizeBox = false;
            prompt.MinimizeBox = false;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;

            prompt.Width = 441;
            prompt.Height = 276;
            prompt.Text = title;

            Button ok = new Button() { Text = "Ok", Left = 338, Top = 16 };
            Button clear = new Button() { Text = "Clear", Left = 338, Top = 45 };
            Label label = new Label() { Text = "", Left = 50, Top = 45, BackColor = Color.Gray };
            label.Width = 200;

            Label label2 = new Label() { Left = 35, Top = 210 };

            Button addimage = new Button() { Text = "Add File", Left = 258, Top = 45 };
            prompt.Controls.Add(ok);
            prompt.Controls.Add(clear);
            prompt.Controls.Add(label);
            prompt.Controls.Add(label2);
            prompt.Controls.Add(addimage);
            string replybox = "";

            if (showDelay == "yes")
            {
                Label textLabel = new Label() { Left = 50, Top = 20, Text = name };
                TextBox inputBox = new TextBox() { Left = 20, Top = 75, Width = 390, Height = 127 };
                TextBox inputBox2 = new TextBox() { Left = 140, Top = 205, Width = 100 };
                inputBox2.Multiline = false;
                inputBox2.Text = "2";

                inputBox.Multiline = true;
                inputBox.Text = txtboxtxt;
                label2.Text = "Enter Seconds";


                addimage.Click += (sender, e) =>
                {
                    var FD = new System.Windows.Forms.OpenFileDialog();
                    FD.Multiselect = true;
                    FD.Filter = "All Files |";
                    if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        label.Text = FD.FileName;
                    }
                };
                //TODO -- Sending delay ...
                ok.Click += (sender, e) =>
                {
                    if(label.Text != "" && label.Text != null)
                    replybox = inputBox.Text.ToString().Trim() + "*" + label.Text + "*" + inputBox2.Text.ToString().Trim();
                    else
                        replybox = inputBox.Text.ToString().Trim() + "*" + inputBox2.Text.ToString().Trim();

                    prompt.Close();
                };
                clear.Click += (sender, e) => { inputBox.Clear(); };

                inputBox2.KeyPress += (object sender, KeyPressEventArgs e) =>
                  {
                      if (!(Char.IsDigit(e.KeyChar) || (e.KeyChar == (char)Keys.Back)))
                          e.Handled = true;
                  };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(inputBox2);
                prompt.ShowDialog();
            }
            else
            {
                Label textLabel = new Label() { Left = 50, Top = 20, Text = name };
                TextBox inputBox = new TextBox() { Left = 20, Top = 75, Width = 390, Height = 127 };

                inputBox.Multiline = true;
                inputBox.Text = txtboxtxt;

                addimage.Click += (sender, e) =>
                {
                    var FD = new System.Windows.Forms.OpenFileDialog();
                    FD.Multiselect = true;
                    FD.Filter = "All Files |";
                    if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        label.Text = FD.FileName;
                    }
                };
                ok.Click += (sender, e) =>
                {
                    replybox = inputBox.Text.ToString().Trim() + "*" + label.Text + "*" + "".Trim();

                    prompt.Close();
                };
                clear.Click += (sender, e) => { inputBox.Clear(); };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.ShowDialog();
            }

            return (string)replybox;
        }

        private async Task<int> sendmessage(string senderid, string conversationid, string messagetxt, string filename)
        {
            try
            {

                //now post to api
                var fb = new FacebookClient();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                string CurrentPageId = pagelist.SelectedValue.ToString();
                if (CurrentPageId != "0")
                {
                    dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");
                    string CurrentpageAccesstoken = CurrentPageTokenCall.access_token;
                    //make fb as current page client
                    fb.AccessToken = CurrentpageAccesstoken;
                    var to = new Dictionary<string, object>
                                {
                                    {"id", senderid}
                                };

                    if (messagetxt == "" && filename != "")
                    {
                        System.IO.FileInfo File = new System.IO.FileInfo(filename);
                        var imgstrm = File.OpenRead();
                        var file = new FacebookMediaStream
                        {
                            ContentType = "image/jpeg",
                            FileName = filename,
                        }.SetValue(imgstrm);

                        var msgbody = new Dictionary<string, object>
                                {
                                    {"id", conversationid}, //plese enter full conversation ID// eg: t_mid.1395805167639:9ac20dbffcd33a5d13
                                    {"attachments",file},
                                    {"to" , to}
                                };
                        await fb.PostTaskAsync("messages/", msgbody);
                    }
                    else if (messagetxt != "" && filename == null)
                    {
                        var msgbody = new Dictionary<string, object>
                                {
                                    {"id", conversationid}, //plese enter full conversation ID// eg: t_mid.1395805167639:9ac20dbffcd33a5d13
                                    {"message",messagetxt},
                                    {"to" , to}
                                };
                        await fb.PostTaskAsync("messages/", msgbody);
                    }
                    else
                    {
                        System.IO.FileInfo File = new System.IO.FileInfo(filename);
                        var imgstrm = File.OpenRead();
                        var file = new FacebookMediaStream
                        {
                            ContentType = "image/jpeg",
                            FileName = filename,
                        }.SetValue(imgstrm);
                        var msgbody = new Dictionary<string, object>
                                {
                                    {"id", conversationid}, //plese enter full conversation ID// eg: t_mid.1395805167639:9ac20dbffcd33a5d13
                                    {"message",messagetxt},
                                    { "attachments",file},
                                    {"to" , to}
                                };
                        await fb.PostTaskAsync("messages/", msgbody);

                    }

                }

                return 1;
            }
            catch
            {
                return 0;
            }
        }

        


        private void addNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Profile_Access_token != null)
            {
                //call dialog
                try
                {
                    int index = 0;
                    for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                    {
                        index = dataGridView1.SelectedRows[i].Index;

                        string notes = dataGridView1.Rows[index].Cells[7].Value.ToString();
                        dataGridView1.Rows[index].Cells[9].Value = notes;

                        if (!string.IsNullOrEmpty(notes))
                        {
                            string messageid = dataGridView1.Rows[index].Cells[10].Value.ToString();
                            using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                            {
                                //start
                                SQLiteCommand cmd = new SQLiteCommand("select exists(select 1 from MainConversation where message_id = '" + messageid + "');", con);
                                con.Open();
                                SQLiteDataReader reader = cmd.ExecuteReader();
                                string val = reader.GetValue(0).ToString();
                                // cmd.ExecuteNonQuery();
                                if (val == "1")
                                {
                                    SQLiteCommand cmd2 = new SQLiteCommand("update MainConversation set FB_Notes = '" + notes + "' where message_id = '" + messageid + "' ", con);
                                    cmd2.ExecuteNonQuery();
                                }
                                else
                                {
                                    SQLiteCommand cmd3 = new SQLiteCommand("insert into MainConversation(message_id,FB_Notes) values('" + messageid + "','" + notes + "');", con);
                                    cmd3.ExecuteNonQuery();
                                }
                                con.Close();
                                //end
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please add note before adding FB_Notes");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please Login" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please Login First.");
            }

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.SelectAll();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        //chose quick reply 
        private void choseAReplyToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {

            choseAReplyToolStripMenuItem.DropDownItems.Clear();
            List<string> quickreplies = new List<string>();

            using (SQLiteConnection con = new SQLiteConnection(connectionstring))
            {
                SQLiteCommand cmd = new SQLiteCommand("Select * FROM QuickReplyTable", con);
                con.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    quickreplies.Add(reader.GetString(0));
                }
                con.Close();
            }

            foreach (var item in quickreplies)
            {
                choseAReplyToolStripMenuItem.DropDownItems.Add(item);
            }


        }

        //
        private async void choseAReplyToolStripMenuItem_DropDownItemClickedAsync(object sender, ToolStripItemClickedEventArgs e)
        {
            if (Profile_Access_token != null)
            {
                progressBar1.Show();
                label3.Show();

                GridCellOptions.Close();
                int index = 0;
                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {
                    index = dataGridView1.SelectedRows[i].Index;
                    dataGridView1.Rows[index].Cells[2].Value = e.ClickedItem.Text;
                }

                var fb = new FacebookClient();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                string CurrentPageId = pagelist.SelectedValue.ToString();
                fb.AccessToken = Profile_Access_token;
                //start getting selected page access token
                if (CurrentPageId != "0")
                {
                    dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");

                    index = 0;
                    for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                    {
                        index = dataGridView1.SelectedRows[i].Index;
                        await sendmessage(dataGridView1.Rows[index].Cells[12].Value.ToString(), dataGridView1.Rows[index].Cells[10].Value.ToString(), e.ClickedItem.Text, null);
                        //now call messagage sender function with parameter
                    }
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("Message sent successfully.");

                }
                progressBar1.Hide();
                label3.Hide();
            }
            else
            { 
                MessageBox.Show("Please Login First.");
            }

        }

        /// Add label manage
        private void manageLabeleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LabelManageForm it = new LabelManageForm();
            it.ShowDialog();
        }

        // Manage Quick Reply
        private void managesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickReplyManage qmg = new QuickReplyManage();
            qmg.ShowDialog();
        }

        private void addALabelToolStripMenuItem1_MouseHover(object sender, EventArgs e)
        {
            addALabelToolStripMenuItem1.DropDownItems.Clear();
            List<string> AllLabel = new List<string>();

            using (SQLiteConnection con = new SQLiteConnection(connectionstring))
            {
                SQLiteCommand cmd = new SQLiteCommand("Select * FROM LabelTable", con);
                con.Open();
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    AllLabel.Add(reader.GetString(0));
                }
                con.Close();
            }

            foreach (var item in AllLabel)
            {
                addALabelToolStripMenuItem1.DropDownItems.Add(item);
            }
        }

        /// Add to Local Label 
        private void addALabelToolStripMenuItem1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            GridCellOptions.Close();
            int index = 0;
            for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
            {
                index = dataGridView1.SelectedRows[i].Index;
                dataGridView1.Rows[index].Cells[6].Value = e.ClickedItem.Text;
            }
            string label = e.ClickedItem.Text;

            index = 0;
            for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
            {
                index = dataGridView1.SelectedRows[i].Index;
                string messageid = dataGridView1.Rows[index].Cells[10].Value.ToString();

                using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                {
                    //start
                    SQLiteCommand cmd = new SQLiteCommand("select exists(select 1 from MainConversation where message_id = '" + messageid + "');", con);
                    con.Open();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    string val = reader.GetValue(0).ToString();
                    // cmd.ExecuteNonQuery();
                    if (val == "1")
                    {
                        SQLiteCommand cmd2 = new SQLiteCommand("update MainConversation set Labels = '" + label + "' where message_id = '" + messageid + "' ", con);
                        cmd2.ExecuteNonQuery();
                    }
                    else
                    {
                        SQLiteCommand cmd3 = new SQLiteCommand("insert into MainConversation(message_id,Labels) values('" + messageid + "','" + label + "');", con);
                        cmd3.ExecuteNonQuery();
                    }
                    con.Close();
                    //end
                }
            }
            MessageBox.Show("Successfully Label Added!");
        }
        // Local Add Note MenuStriip Click
        private void addNoteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //call dialog
            string promptValue = ShowDialog("Enter your note :", "Add notes", "");
            promptValue.Trim();
            int index = 0;
            for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
            {
                index = dataGridView1.SelectedRows[i].Index;
                dataGridView1.Rows[index].Cells[7].Value = promptValue;
            }
            index = 0;
            for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
            {
                index = dataGridView1.SelectedRows[i].Index;
                string messageid = dataGridView1.Rows[index].Cells[10].Value.ToString();

                using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                {

                    SQLiteCommand cmd = new SQLiteCommand("select exists(select 1 from MainConversation where message_id = '" + messageid + "');", con);
                    con.Open();
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    string val = reader.GetValue(0).ToString();
                    // cmd.ExecuteNonQuery();
                    if (val == "1")
                    {
                        SQLiteCommand cmd2 = new SQLiteCommand("update MainConversation set Notes = '" + promptValue + "' where message_id = '" + messageid + "' ", con);
                        cmd2.ExecuteNonQuery();
                    }
                    else
                    {
                        SQLiteCommand cmd3 = new SQLiteCommand("insert into MainConversation(message_id,Notes) values('" + messageid + "','" + promptValue + "');", con);
                        cmd3.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            MessageBox.Show("Successfully Note Added");

        }

        //edit note
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string selectednote = dataGridView1.CurrentRow.Cells[7].Value.ToString();
                string messageid = dataGridView1.CurrentRow.Cells[10].Value.ToString();
                string promptValue = ShowDialog("Edit note :", "Add notes", selectednote);

                dataGridView1.CurrentRow.Cells[7].Value = promptValue;
                using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                {
                    SQLiteCommand cmd = new SQLiteCommand("update MainConversation set Notes = '" + promptValue + "' where message_id = '" + messageid + "'", con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No Notes found in this row");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        public static string ShowDialog(string name, string title, string txtboxtxt)
        {
            Form prompt = new Form();
            prompt.MaximizeBox = false;
            prompt.MinimizeBox = false;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;

            prompt.Width = 441;
            prompt.Height = 276;
            prompt.Text = title;

            Button ok = new Button() { Text = "Ok", Left = 338, Top = 16 };
            Button clear = new Button() { Text = "Clear", Left = 338, Top = 45 };


            prompt.Controls.Add(ok);
            prompt.Controls.Add(clear);

            Label textLabel = new Label() { Left = 50, Top = 20, Text = name };
            TextBox inputBox = new TextBox() { Left = 20, Top = 75, Width = 390, Height = 147 };
            inputBox.Multiline = true;
            inputBox.Text = txtboxtxt;

            ok.Click += (sender, e) => { prompt.Close(); };
            clear.Click += (sender, e) => { inputBox.Clear(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.ShowDialog();
            return (string)inputBox.Text.ToString();
        }

        //Delete Local Label
        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {
                    index = dataGridView1.SelectedRows[i].Index;
                    dataGridView1.Rows[index].Cells[6].Value = String.Empty; ;
                    string messageid = dataGridView1.Rows[index].Cells[10].Value.ToString();

                    using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                    {
                        SQLiteCommand cmd = new SQLiteCommand("update MainConversation set Labels = '" + "" + "' where message_id = '" + messageid + "'", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                }
                MessageBox.Show("Label Deleted!");

            }
            catch (Exception)
            {
                MessageBox.Show("No label found");
            }
        }

        /// Delete local Notes
        private void deleteToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {
                    index = dataGridView1.SelectedRows[i].Index;
                    dataGridView1.Rows[index].Cells[7].Value = String.Empty; ;
                    string messageid = dataGridView1.Rows[index].Cells[10].Value.ToString();

                    using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                    {
                        SQLiteCommand cmd = new SQLiteCommand("update MainConversation set Notes = '" + "" + "' where message_id = '" + messageid + "'", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                }

            }
            catch (Exception)
            {
                MessageBox.Show("No Notes found");
            }
        }

        private void messageAnalyticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Profile_Access_token))
            {
                MessageBox.Show("Please Login first");
            }
            else
            {
                //Message Analitic form load
                MessageAnaliticsForm maf = new MessageAnaliticsForm(ConvList);
                maf.Show();
            }
        }

        private void profileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (User_Profile_Link == null && linkLabel1.Text != "Name")
            {
                ProfileWebbrowser browser = new ProfileWebbrowser("www.fb.com");
                browser.ShowDialog();
            }
            else if (User_Profile_Link != null)
            {
                ProfileWebbrowser browser = new ProfileWebbrowser(User_Profile_Link);
                browser.ShowDialog();

            }
            else
            {
                MessageBox.Show("Please Login first");
            }
        }

        private void exportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Profile_Access_token))
            {
                MessageBox.Show("Please Login first");
            }
            else
            {
                //Export data into file
                StringBuilder strB = new StringBuilder();
                //create html & table
                strB.AppendLine("<html><body><center><" +
                              "table border='1' cellpadding='0' cellspacing='0'>");
                strB.AppendLine("<tr>");
                //cteate table header
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    strB.AppendLine("<td align='center' valign='middle'>" +
                                   dataGridView1.Columns[i].HeaderText + "</td>");
                }
                //create table body
                strB.AppendLine("<tr>");
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    strB.AppendLine("<tr>");
                    foreach (DataGridViewCell dgvc in dataGridView1.Rows[i].Cells)
                    {
                        if (dgvc.Value != null)
                        {
                            strB.AppendLine("<td align='center'style='color:blue' valign='middle'>" +
                                        dgvc.Value.ToString() + "</td>");
                        }
                        else
                        {
                            strB.AppendLine("<td align='center'style='color:blue' valign='middle'>" +
                                        "" + "</td>");
                        }

                    }
                    strB.AppendLine("</tr>");

                }
                //table footer & end of html file
                strB.AppendLine("</table></center></body></html>");


                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Filter = "Text file(*.html)|*.html";
                sfd.FilterIndex = 1;
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) { return; }
                string dirLocationString = sfd.FileName;
                // program file save location dte hobe
                System.IO.File.WriteAllText(dirLocationString, strB.ToString());
                System.Diagnostics.Process.Start(dirLocationString);
            }
        }

        private void minimizeToSystemTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Form minimize ti tray
            // this.minimizeToSystemTrayToolStripMenuItem
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = true;
            this.Hide();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }


        //private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    string promptValue = registerDialogaftertrial("Enter your license  details:", "Registration", "");
        //}

        public static string registerDialog(string name, string title, string txtboxtxt)
        {
            Form prompt = new Form();
            prompt.MaximizeBox = false;
            prompt.MinimizeBox = false;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;

            prompt.Width = 436;
            prompt.Height = 200;
            prompt.Text = title;

            Button Trialbtn = new Button() { Text = "Trail", Left = 250, Top = 130 };
            Button Registerbtn = new Button() { Text = "Register", Left = 328, Top = 130 };

            prompt.Controls.Add(Trialbtn);
            prompt.Controls.Add(Registerbtn);

            Label textLabel = new Label() { Left = 20, Text = name };
            Label textLabelemail = new Label() { Left = 20, Text = "Email: ", Top = 35, };
            Label textLabelcode = new Label() { Left = 20, Text = "Key: ", Top = 65, };
            TextBox inputBox1 = new TextBox() { Left = 100, Top = 35, Width = 300, };
            TextBox inputBox2 = new TextBox() { Left = 100, Top = 65, Width = 300, };
            inputBox1.Multiline = false;

            //58KFDWrVN07xWYWRnWOsWCuZ4IqJCRZF 1 month
            //fz3sfOHSmkvqO2sdtG74u20d54b7Oq26 for 3 month
            //ZkbdXY0EARQTCuC5LINNY5WiQsm6Gote 6 month
            //84knSHY5Lj7SvVWLSo1x3qItLtBMYORq 1 year

            Trialbtn.Click += (sender, e) =>
            {
                using (SQLiteConnection conc = new SQLiteConnection(Form1.connectionstring))
                {
                    DateTime d = System.DateTime.Now.AddHours(24);
                    string dd = d.ToString();
                    SQLiteCommand cmd = new SQLiteCommand("INSERT INTO License (Email , [Key], SessionEnd) VALUES ('trail', 'trail','" + dd + "')", conc);
                    conc.Open();
                    dynamic aa = cmd.ExecuteNonQuery();
                    //List<LicenseModel> license = reader.AutoMap<LicenseModel>().ToList();
                    conc.Close();

                }
                MessageBox.Show("Your trial period is set 24 hours from now.");
                prompt.Close();
            };

            Registerbtn.Click += (sender, e) =>
              {
                  if (inputBox1.Text != "" && inputBox2.Text != "")
                  {
                      if (inputBox2.Text.Trim() != "58KFDWrVN07xWYWRnWOsWCuZ4IqJCRZF" || inputBox2.Text.Trim() != "fz3sfOHSmkvqO2sdtG74u20d54b7Oq26" || inputBox2.Text.Trim() != "ZkbdXY0EARQTCuC5LINNY5WiQsm6Gote" || inputBox2.Text.Trim() != "84knSHY5Lj7SvVWLSo1x3qItLtBMYORq")
                      {
                          using (SQLiteConnection con = new SQLiteConnection(Form1.connectionstring))
                          {
                              DateTime enddate = System.DateTime.Now;
                              if (inputBox2.Text.Trim() == "58KFDWrVN07xWYWRnWOsWCuZ4IqJCRZF")
                              {
                                  enddate = enddate.AddMonths(1);
                              }
                              else if (inputBox2.Text.Trim() == "fz3sfOHSmkvqO2sdtG74u20d54b7Oq26")
                              {
                                  enddate = enddate.AddMonths(3);
                              }
                              else if (inputBox2.Text.Trim() == "ZkbdXY0EARQTCuC5LINNY5WiQsm6Gote")
                              {
                                  enddate = enddate.AddMonths(6);
                              }
                              else if (inputBox2.Text.Trim() == "84knSHY5Lj7SvVWLSo1x3qItLtBMYORq")
                              {
                                  enddate = enddate.AddYears(1);
                              }
                              string dd = enddate.ToString();
                              SQLiteCommand cmd = new SQLiteCommand("INSERT into License (Email,[Key], SessionEnd) values ('" + inputBox1.Text + "','" + inputBox2.Text + "','" + dd + "')", con);
                              con.Open();
                              cmd.ExecuteNonQuery();
                              //List<LicenseModel> license = reader.AutoMap<LicenseModel>().ToList();
                              con.Close();
                              MessageBox.Show("Thank you for registration.");
                              prompt.Close();
                          }
                          inputBox1.Clear();
                      }
                      else
                      {
                          MessageBox.Show("Wrong activation key.");
                      }

                  }
                  else
                  {
                      MessageBox.Show("Fill the input fields.");
                  }
              };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox1);
            prompt.Controls.Add(inputBox2);
            prompt.Controls.Add(textLabelemail);
            prompt.Controls.Add(textLabelcode);
            prompt.ControlBox = false;
            prompt.ShowDialog();

            return (string)"";
        }


        public static string registerDialogaftertrial(string name, string title, string txtboxtxt)
        {
            string msg = "";
            Form prompt = new Form();
            prompt.MaximizeBox = false;
            prompt.MinimizeBox = false;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;

            prompt.Width = 436;
            prompt.Height = 200;
            prompt.Text = title;

            Button Registerbtn = new Button() { Text = "Register", Left = 328, Top = 130 };
            Button Trialbtn = new Button() { Text = "Cancel", Left = 250, Top = 130 };
            prompt.Controls.Add(Registerbtn);
            prompt.Controls.Add(Trialbtn);

            Label textLabel = new Label() { Left = 20, Text = name };
            Label textLabelemail = new Label() { Left = 20, Text = "Email: ", Top = 35, };
            Label textLabelcode = new Label() { Left = 20, Text = "Key: ", Top = 65, };
            TextBox inputBox1 = new TextBox() { Left = 100, Top = 35, Width = 300, };
            TextBox inputBox2 = new TextBox() { Left = 100, Top = 65, Width = 300, };
            inputBox1.Multiline = false;

            //58KFDWrVN07xWYWRnWOsWCuZ4IqJCRZF 1 month
            //fz3sfOHSmkvqO2sdtG74u20d54b7Oq26 for 3 month
            //ZkbdXY0EARQTCuC5LINNY5WiQsm6Gote 6 month
            //84knSHY5Lj7SvVWLSo1x3qItLtBMYORq 1 year
            Trialbtn.Click += (sender, e) =>
            {
                msg = "close";
                prompt.Close();

            };
            Registerbtn.Click += (sender, e) =>
        {
            if (inputBox1.Text != "" && inputBox2.Text != "")
            {
                if (inputBox2.Text.Trim() != "58KFDWrVN07xWYWRnWOsWCuZ4IqJCRZF" || inputBox2.Text.Trim() != "fz3sfOHSmkvqO2sdtG74u20d54b7Oq26" || inputBox2.Text.Trim() != "ZkbdXY0EARQTCuC5LINNY5WiQsm6Gote" || inputBox2.Text.Trim() != "84knSHY5Lj7SvVWLSo1x3qItLtBMYORq")
                {
                    using (SQLiteConnection con = new SQLiteConnection(Form1.connectionstring))
                    {
                        DateTime enddate = System.DateTime.Now;
                        if (inputBox2.Text.Trim() == "58KFDWrVN07xWYWRnWOsWCuZ4IqJCRZF")
                        {
                            enddate = enddate.AddMonths(1);
                        }
                        else if (inputBox2.Text.Trim() == "fz3sfOHSmkvqO2sdtG74u20d54b7Oq26")
                        {
                            enddate = enddate.AddMonths(3);
                        }
                        else if (inputBox2.Text.Trim() == "ZkbdXY0EARQTCuC5LINNY5WiQsm6Gote")
                        {
                            enddate = enddate.AddMonths(6);
                        }
                        else if (inputBox2.Text.Trim() == "84knSHY5Lj7SvVWLSo1x3qItLtBMYORq")
                        {
                            enddate = enddate.AddYears(1);
                        }
                        string dd = enddate.ToString();
                        SQLiteCommand cmd = new SQLiteCommand("INSERT into License (Email,[Key], SessionEnd) values ('" + inputBox1.Text + "','" + inputBox2.Text + "','" + dd + "')", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        //List<LicenseModel> license = reader.AutoMap<LicenseModel>().ToList();
                        con.Close();
                        MessageBox.Show("Thank you for registration.");
                        msg = "keep";
                        prompt.Close();

                    }

                }
                else
                {
                    MessageBox.Show("Wrong activation key.");
                }

            }
            else
            {
                MessageBox.Show("Fill the input fields.");
            }
        };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox1);
            prompt.Controls.Add(inputBox2);
            prompt.Controls.Add(textLabelemail);
            prompt.Controls.Add(textLabelcode);
            prompt.ControlBox = false;
            prompt.ShowDialog();

            return (string)msg;
        }

        //reply button
        private async void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (Profile_Access_token != null)
            {
                progressBar1.Show();
                label3.Show();
                int index = 0;
                Cursor.Current = Cursors.WaitCursor;
                string CurrentPageId = pagelist.SelectedValue.ToString();
                if (textBox2.Text != "" && CurrentPageId != "0" || label4.Text != "")
                {
                    button3.Text = "Sending..";

                    for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                    {
                        index = dataGridView1.SelectedRows[i].Index;
                        dataGridView1.Rows[index].Cells[2].Value = textBox2.Text;
                    }
                    //
                    var fb = new FacebookClient();
                    fb.AccessToken = Profile_Access_token;
                    //start getting selected page access token
                    dynamic CurrentPageTokenCall = fb.Get(CurrentPageId + "?fields=access_token");
                    string CurrentpageAccesstoken = CurrentPageTokenCall.access_token;

                    index = 0;
                    for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                    {

                        label3.Show();
                        label3.Text = "Sending to " + dataGridView1.Rows[index].Cells[1].Value.ToString();

                        if (label4.Text != "")//image added
                        {
                            index = dataGridView1.SelectedRows[i].Index;
                            await sendmessage(dataGridView1.Rows[index].Cells[12].Value.ToString(), dataGridView1.Rows[index].Cells[10].Value.ToString(), textBox2.Text, label4.Text);
                        }
                        else //image not added
                        {

                            label3.Show();
                            label3.Text = "Sending to " + dataGridView1.Rows[index].Cells[1].Value.ToString();

                            index = dataGridView1.SelectedRows[i].Index;
                            await sendmessage(dataGridView1.Rows[index].Cells[12].Value.ToString(), dataGridView1.Rows[index].Cells[10].Value.ToString(), textBox2.Text, null);
                        }

                        //now call messagage sender function with parameter
                    }
                    label4.Text = "";
                    textBox2.Text = null;
                    loadsingleconversation(CurrentpageAccesstoken, dataGridView1.Rows[index].Cells[12].Value.ToString());
                    Cursor.Current = Cursors.Default;
                    button3.Text = "Reply";
                }

                progressBar1.Hide();
                label3.Hide();
            }
            else
            {
                MessageBox.Show("Please Login First.");
            }
            timer1.Start();
        }

        //add image button
        private void button1_Click_2(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            FD.Multiselect = true;
            FD.Filter = "All Files |";
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                label4.Text = FD.FileName;
            }
        }

        /// FB Label insert in DB and Rows
        private void addALabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {

                    index = dataGridView1.SelectedRows[i].Index;
                    string label = dataGridView1.Rows[index].Cells[6].Value.ToString();
                    if (!string.IsNullOrEmpty(label))
                    {
                        dataGridView1.Rows[index].Cells[8].Value = label;

                        string messageid = dataGridView1.Rows[index].Cells[10].Value.ToString();
                        using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                        {
                            SQLiteCommand cmd = new SQLiteCommand("IF EXISTS(select 1 from MainConversation where MainConversation.message_id='" + messageid + "')  update MainConversation set FB_Labels = '" + label + "' where message_id = '" + messageid + "' ELSE   insert into MainConversation(FB_Labels) values('" + label + "')", con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Insert a Label before ada an FB Label");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please Login");
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {
                    index = dataGridView1.SelectedRows[i].Index;
                    dataGridView1.Rows[index].Cells[8].Value = String.Empty; ;
                    string messageid = dataGridView1.Rows[index].Cells[10].Value.ToString();

                    using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                    {
                        SQLiteCommand cmd = new SQLiteCommand("update MainConversation set FB_Labels = '" + "" + "' where message_id = '" + messageid + "'", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                }

            }
            catch (Exception)
            {
                MessageBox.Show("No Notes found");
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                for (int i = 0; i <= dataGridView1.SelectedRows.Count - 1; i++)
                {
                    index = dataGridView1.SelectedRows[i].Index;
                    dataGridView1.Rows[index].Cells[9].Value = String.Empty; ;
                    string messageid = dataGridView1.Rows[index].Cells[10].Value.ToString();

                    using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                    {
                        SQLiteCommand cmd = new SQLiteCommand("update MainConversation set FB_Notes = '" + "" + "' where message_id = '" + messageid + "'", con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                }

            }
            catch (Exception)
            {
                MessageBox.Show("No Notes found");
            }
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            if (User_Profile_Link == null && linkLabel1.Text != "Name")
            {
                ProfileWebbrowser browser = new ProfileWebbrowser("www.fb.com");
                browser.ShowDialog();
            }
            else if (User_Profile_Link != null)
            {
                ProfileWebbrowser browser = new ProfileWebbrowser(User_Profile_Link);
                browser.ShowDialog();

            }
            else
            {
                MessageBox.Show("Please Login first");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ContactForm contactForm = new ContactForm();
            contactForm.ShowDialog();
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://youtu.be/JwPanaHkOss");
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(SearchBox.Text))
                {
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Sender LIKE '%{0}%' OR 'Updated Time' LIKE '%{0}%' OR Message LIKE '%{0}%' OR Labels LIKE '%{0}%' OR Notes LIKE '%{0}%' OR 'FB Labels' LIKE '%{0}%' OR 'FB Notes' LIKE '%{0}%'", SearchBox.Text);
                }
                else
                {
                    (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Empty;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchBox.Clear();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        // 12th Nov 2018 Auto refresh new msg

        private async void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            try
            {
                List<ConversationView> newconversationview = new List<ConversationView>();
                //start getting all conversation list of selected page
                var FbClient = new FacebookClient();
                FbClient.AccessToken = CurrPageAccessToken;
                List<ConversationModel> newconversation = new List<ConversationModel>();

                dynamic AllmessageJson = await FbClient.GetTaskAsync("me?fields=conversations.limit(10){message_count,link,updated_time,snippet,senders,unread_count}");
                var AllMessageData = AllmessageJson.conversations.data;
                foreach (var item in AllMessageData)
                {
                    ConversationModel singleconversation = new ConversationModel();
                    singleconversation.message_count = item.message_count.ToString();
                    singleconversation.link = item.link.ToString();
                    try
                    {
                        singleconversation.snippet = item.snippet.ToString();
                    }
                    catch
                    {
                        singleconversation.snippet = "[sticker/image]";
                    }
                    singleconversation.updated_time = DateTime.Parse(item.updated_time);
                    singleconversation.id = item.id.ToString();
                    singleconversation.unread_count = item.unread_count.ToString();
                    var senders = item.senders.data;
                    List<MessageSender> senderlist = new List<MessageSender>();
                    foreach (var senderitem in senders)
                    {
                        MessageSender sender2 = new MessageSender();
                        sender2.id = senderitem.id.ToString();
                        sender2.name = senderitem.name.ToString();
                        sender2.email = senderitem.email.ToString();
                        senderlist.Add(sender2);
                    }
                    singleconversation.messageSenders = senderlist;
                    newconversation.Add(singleconversation);
                }

                foreach (var item in newconversation)
                {
                    ConversationView conv = new ConversationView();
                    conv.message_id = item.id;
                    conv.link = item.link;
                    conv.message_count = item.message_count;
                    conv.unread_count = item.unread_count;
                    conv.updated_time = item.updated_time;
                    conv.snippet = item.snippet;
                    conv.messageSenders = item.messageSenders;
                    DataTable dataTable = new DataTable();
                    IEnumerable<DataRow> dataRows;

                    using (SQLiteConnection con = new SQLiteConnection(connectionstring))
                    {
                        SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM MainConversation where message_id='" + item.id + "'", con);
                        con.Open();
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        dataTable.Load(reader);
                        dataRows = dataTable.AsEnumerable();
                    }
                    foreach (var row in dataRows)
                    {
                        conv.Labels = conv.Labels + "," + row.ItemArray[1].ToString();
                        conv.Notes = conv.Notes + "," + row.ItemArray[2].ToString();
                        conv.FB_Labels = conv.FB_Labels + "," + row.ItemArray[3].ToString();
                        conv.FB_Notes = conv.FB_Notes + "," + row.ItemArray[4].ToString();
                    }
                    newconversationview.Add(conv);
                }
                DateTime checkTimig = Convert.ToDateTime(dataGridView1[3, 0].Value);
                newconversationview.RemoveAll(x => x.updated_time <= checkTimig);
                newconversationview.Reverse();

                if (newconversationview[newconversationview.Count-1].updated_time > checkTimig)
                {
                    foreach (var data in newconversationview)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            int rowIndex = 0;
                            try {
                                if (row.Cells[10].Value.ToString().Equals(data.message_id))
                                {
                                    try
                                    {
                                        rowIndex = row.Index;
                                        
                                        dataGridView1.Rows.RemoveAt(rowIndex);
                                        break;
                                    }
                                    catch (Exception)
                                    {
                                        break;
                                    }
                                }
                            }catch(Exception)
                            {

                            }
                        }



                        // Code performence
                        //var timer1 = Stopwatch.StartNew();
                        //int rowIndex = 0;
                        //DataGridViewRow row = dataGridView1.Rows
                        //                    .Cast<DataGridViewRow>()
                        //                    .Where(r => r.Cells[10].Value.ToString().Equals(data.message_id))
                        //                    .First();

                        //rowIndex = row.Index;

                        ///working linq  
                        //var rows = from DataGridViewRow r in dataGridView1.Rows
                        //            where r.Cells[10].Value.ToString().Equals(data.message_id)
                        //            select r;

                        //var rowFound = rows.First();
                        //dataGridView1.Rows.Remove(rowFound);
                        int datarowcount = dataGridView1.RowCount;

                        DataRow newRow = Pagedt.NewRow();
                        newRow["Serial"] = datarowcount+=2;
                        newRow["Sender"] = data.messageSenders[0].name;
                        newRow["Message"] = data.snippet;
                        newRow["Updated Time"] = data.updated_time;
                        newRow["Count"] = data.message_count;
                        newRow["Unread"] = data.unread_count;
                        newRow["Labels"] = data.Labels;
                        newRow["Notes"] = data.Notes;
                        newRow["FB Labels"] = data.FB_Labels;
                        newRow["FB Notes"] = data.FB_Notes;
                        newRow["MessageId"] = data.message_id;
                        newRow["Link"] = data.link;
                        newRow["Senderid"] = data.messageSenders[0].id;

                        Pagedt.Rows.InsertAt(newRow, 0);

                        dataGridView1.Rows[0].DefaultCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
                        // 13th Nov 2018 New and Update Message Shown Correct Serial Number
                        int newLineup = dataGridView1.RowCount;
                        for(int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            dataGridView1[0, i].Value = newLineup;
                            newLineup--;
                        }
                        // 13th Nov 2018 New and Update Message Shown Correct Serial Number
                    }
                }
            }
            catch (Exception)
            {
                //do nothing
            }

            timer1.Start();
        }
        // 12th Nov 2018.... Clear Cache...
        private void clearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear previous login cache ?","Alert!",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("rundll32.exe")
                {
                    Arguments = "InetCpl.cpl,ClearMyTracksByProcess 2",
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                Process.Start(new ProcessStartInfo("rundll32.exe")
                {
                    Arguments = "InetCpl.cpl,ClearMyTracksByProcess 8",
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
        }

        private void autoMessageBtn_Click(object sender, EventArgs e)
        {
            AutoMessageForm ams = new AutoMessageForm();
            ams.ShowDialog();
        }
    }
}
