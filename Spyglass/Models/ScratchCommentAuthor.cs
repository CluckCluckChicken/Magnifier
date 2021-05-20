using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public class ScratchCommentAuthor
    {
        public ScratchCommentAuthor(string username, string image)
        {
            this.username = username;
            this.image = image;
        }
        
        public string _id { get; set; }

        public string username { get; set; }

        public string image { get; set; }
    }
}
