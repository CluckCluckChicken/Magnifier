using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Spyglass.Models
{
    public enum Requirement
    {
        id,
        content,
        username
    }

    public class CommentSearchRequirement
    {
        public char prefix { get; set; }

        public Requirement requirement { get; set; }

        public string value { get; set; }
    }
}
