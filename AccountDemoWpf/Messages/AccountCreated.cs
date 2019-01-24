using ReactiveDomain.Messaging;
using System;
using System.Threading;

namespace AccountDemoWpf.Messages
{
    public class AccountCreated : DomainEvent
    {
        private static readonly int TypeId = Interlocked.Increment(ref NextMsgId);
        public override int MsgTypeId => TypeId;

        public readonly Guid AccountId;
        public readonly string Name;
        public readonly double Balance;

        public AccountCreated(
            Guid accountId,
            string name,
            double balance,
            Guid correlationId,
            Guid sourceId) : base(correlationId, sourceId)
        {
            AccountId = accountId;
            Name = name;
        }
    }
}
