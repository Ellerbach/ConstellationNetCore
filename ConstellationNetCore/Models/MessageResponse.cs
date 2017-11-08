using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Constellation.Models
{
    public class MessageResponse
    {
        public MessageScopeResponse[] messageScope { get; set; }
    }

    public class MessageScopeResponse
    {
        public Sender Sender { get; set; }
        public Scope Scope { get; set; }
        public string Key { get; set; }
        public object Data { get; set; }
    }

    public class Sender
    {
        public string ConnectionId { get; set; }
        public int Type { get; set; }
        public string FriendlyName { get; set; }
    }    
    public class Scope
    {        
        public string SagaId { get; set; }
        public int scope { get; set; }
        public string[] Args { get; set; }
    }

}
