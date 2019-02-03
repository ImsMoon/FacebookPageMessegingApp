using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPageMessegingApp.DataModels
{
   public class MessageModel
    {
        public string fromname { get; set; }
        public string fromid { get; set; }
        public string toname { get; set; }
        public string toid { get; set; }
        public string messagetext { get; set; }
        public DateTime created_time { get; set; }
        public string imageurl { get; set; } = null;
    }
}
