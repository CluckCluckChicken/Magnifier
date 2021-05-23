using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spyglass.Models
{
    public class AuthenticationResponse
    {
        public AuthenticationResponse(HttpResponseMessage _response, string _token)
        {
            response = _response;
            token = _token;
        }

        public HttpResponseMessage response { get; set; }

        public string token { get; set; }
    }
}
