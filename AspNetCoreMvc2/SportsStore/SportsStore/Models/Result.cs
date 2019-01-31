using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class Result
    {
        //ch15. 
        //The Controller and Action properties will be used to indicate how a request has been processed,
        //and the Data dictionary will be used to store other details about the request produced by the routing system.

        public string Controller { get; set; }

        public string Action { get; set; }

        public IDictionary<string, object> Data { get; } = new Dictionary<string, object>();
    }
}
