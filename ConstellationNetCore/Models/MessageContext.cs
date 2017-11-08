using System;

namespace Constellation.Package
{
    public class MessageContext
    {
        [ThreadStatic]
        private static MessageContext current;

        internal MessageContext()
        {
        }

        public static MessageContext Current
        {
            get =>
                (current ?? None);
            set
            {
                current = value;
            }
        }

        public bool HasContext =>
            ((this.Scope != null) && (this.Sender != null));

        public bool IsSaga =>
            (this.HasContext && this.Scope.IsSaga);

        public static MessageContext None =>
            new MessageContext();

        public string SagaId
        {
            get
            {
                if (!this.HasContext)
                {
                    return null;
                }
                return this.Scope.SagaId;
            }
        }

        public MessageScope Scope { get; set; }

        public MessageSender Sender { get; set; }
    }
}
