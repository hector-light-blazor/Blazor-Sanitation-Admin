using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanitationPortal.Models.Response
{
    public class Response<T>
    {
        public bool Success { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        public List<Error> Errors { get; set; } 
    }

    public class Error 
    {
        public int ErrorCode { get; set; }

        public string Message { get; set; } = string.Empty;

    }
}
