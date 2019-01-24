using ReactiveDomain.Messaging;
using System;
using System.Threading;

namespace AccountDemoWpf.Messages
{
    public class CreditApplied : DomainEvent
    {
        private static readonly int TypeId = Interlocked.Increment(ref NextMsgId);
        public override int MsgTypeId => TypeId;

        public readonly double Amount;
        public readonly Guid AccountId;

        public CreditApplied(
            Guid accountId,
            double amount,
            Guid correlationId,
            Guid sourceId) : base(correlationId, sourceId)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}
