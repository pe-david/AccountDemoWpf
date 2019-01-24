using ReactiveDomain.Messaging;
using System;
using System.Threading;

namespace AccountDemoWpf.Messages
{
    public class CreateAccount : Command
    {
        private static readonly int TypeId = Interlocked.Increment(ref NextMsgId);
        public override int MsgTypeId => TypeId;

        public readonly Guid AccountId;
        public readonly string Name;

        public CreateAccount(
            Guid accountId,
            string name,
            Guid correlationId,
            Guid? sourceId) : base(correlationId, sourceId)
        {
            AccountId = accountId;
            Name = name;
        }
    }
}
