using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPageMessegingApp.DataModels
{
    public class AutoMessageModel
    {
        public int Id { get; set; }
        public string TargetMessage { get; set; }
        public string Message { get; set; }
    }

    public class ImagePaths
    {
        public int ImagePathId { get; set; }
        public string ImagePath { get; set; }

        public int AutoMessageId { get; set; }
    }
    public class Keywords
    {
        public int KeywordId { get; set; }
        public string Words { get; set; }

        public int AutoMessageId { get; set; }
    }
}
