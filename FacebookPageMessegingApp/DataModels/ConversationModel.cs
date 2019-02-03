using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPageMessegingApp.DataModels
{
   public class ConversationModel
    {
       public string message_count { get; set; }
       public string link { get; set; }
       public DateTime updated_time { get; set; }
       public string snippet { get; set; }
       public string id { get; set; }
       public string unread_count { get; set; }
       public List<MessageSender> messageSenders { get; set; }
    }

    public class MessageSender
    {
        public string name { get; set; }
        public string email { get; set; }
        public string id { get; set; }
    }
}
