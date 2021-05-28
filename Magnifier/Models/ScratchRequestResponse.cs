using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Magnifier.Models
{
    public class ScratchRequestResponse
    {
        public ScratchRequestResponse(HttpResponseMessage _response, ScratchProject _project = null, ScratchComment _comment = null, List<int> _comments = null)
        {
            response = _response;
            statusCode = _response.StatusCode;
            succeeded = _response.IsSuccessStatusCode;

            project = _project;
            comment = _comment;
            comments = _comments;
        }

        public HttpResponseMessage response;
        public HttpStatusCode statusCode;
        public bool succeeded;

        public ScratchProject project;
        public ScratchComment comment;
        public List<int> comments;
    }
}
