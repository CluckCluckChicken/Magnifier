// This class exists solely to deserialize JSON from the Scratch API. Go away.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public class ScratchProject
    {
        public int id;

        public ScratchCommentAuthor author;
    }
}
