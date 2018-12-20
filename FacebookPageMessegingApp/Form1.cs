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


namespace FacebookPageMessegingApp
{
    public partial class Form1 : Form
    {
        private const string AppId = "464048570771518";
        private const string ExtendedPermissions = "pages_messaging";
        private string _accessToken;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void FbLogin_Click(object sender, EventArgs e)
        {
            var fbLoginDialog = new FacebookLoginDialog(AppId, ExtendedPermissions);
            fbLoginDialog.ShowDialog();

            DisplayAppropriateMessage(fbLoginDialog.FacebookOAuthResult);
        }
        private void DisplayAppropriateMessage(FacebookOAuthResult facebookOAuthResult)
        {
            if (facebookOAuthResult != null)
            {
                if (facebookOAuthResult.IsSuccess)
                {
                    _accessToken = facebookOAuthResult.AccessToken;
                    var fb = new FacebookClient(facebookOAuthResult.AccessToken);

                    dynamic result = fb.Get("/me");
                    var name = result.name;

                    // to get page list with id and name
                    dynamic pages = fb.Get("/me?fields=accounts{name,id}");
                    var PageNameFromJson = pages.accounts;
                    var PageNameFromAccount = PageNameFromJson.data;
                   /// var PageNameFromData = PageNameFromAccount[0].name;

                    List<Page> pagefromjson = new List<Page>();

                    Page pg = new Page();
                    for(int i=0; i<=0; i++)
                    {
                        try
                        {
                            pg.Id = PageNameFromAccount[i].id;
                            pg.Name = PageNameFromAccount[i].name;
                            pagefromjson.Add(pg);
                        }catch(Exception)
                        {
                            break;
                        }
                    }

                    pagelist.DataSource = pagefromjson;
                    pagelist.ValueMember = "Id";
                    pagelist.DisplayMember = "Name";
                    
               
                    // for .net 3.5
                    //var result = (IDictionary<string, object>)fb.Get("/me");
                    //var name = (string)result["name"];

                    //MessageBox.Show("Hi " + name);
                    //btnLogout.Visible = true;
                }
                else
                {
                    MessageBox.Show(facebookOAuthResult.ErrorDescription);
                }
            }
        }
        //private void btnLogout_Click(object sender, EventArgs e)
        //{
        //    var webBrowser = new WebBrowser();
        //    var fb = new FacebookClient();
        //    var logouUrl = fb.GetLogoutUrl(new { access_token = _accessToken, next = "https://www.facebook.com/connect/login_success.html" });
        //    webBrowser.Navigate(logouUrl);
        //    btnLogout.Visible = false;
        //}

    }
}
