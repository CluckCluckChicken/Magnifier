using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public class ScratchComment
    {
        public ScratchComment(int id, string content, ScratchCommentAuthor author, DateTime? datetime_created)
        {
            this.id = id;
            this.content = content;
            this.datetime_created = datetime_created;
            this.author = author;
        }

        public string _id { get; set; }

        public int id { get; set; }

        public string content { get; set; }

        public DateTime? datetime_created { get; set; }

        public ScratchCommentAuthor author { get; set; }
    }
}
