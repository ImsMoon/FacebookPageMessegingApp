using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPageMessegingApp.DataModels
{
   public class ConversationModelDb
    {
        public string message_id { get; set; }
        public string Labels { get; set; }
        public string Notes { get; set; }
        public string FB_Labels { get; set; }
        public string FB_Notes { get; set; }
    }

    public class ConversationView
    {
        public string message_count { get; set; }
        public string link { get; set; }
        public DateTime updated_time { get; set; }
        public string snippet { get; set; }
        public string message_id { get; set; }
        public string unread_count { get; set; }
        public List<MessageSender> messageSenders { get; set; }
        public string Labels { get; set; }
        public string Notes { get; set; }
        public string FB_Labels { get; set; }
        public string FB_Notes { get; set; }
    }
}
