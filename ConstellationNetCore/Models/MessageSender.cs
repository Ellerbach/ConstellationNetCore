
using System;
using System.Runtime.CompilerServices;

namespace Constellation
{
    [Serializable]
    public class MessageSender
    {
        public override string ToString() =>
            $"{this.FriendlyName} ({this.Type}<{this.ConnectionId}>)";

        public string ConnectionId { get; set; }

        public string FriendlyName { get; set; }

        public SenderType Type { get; set; }

        [Serializable]
        public enum SenderType
        {
            ConsumerHub,
            ConstellationHub,
            ConsumerHttp,
            ConstellationHttp
        }
    }
}
