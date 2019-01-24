using ReactiveDomain.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AccountDemoWpf.Messages
{
    public class DebitApplied : DomainEvent
    {
        private static readonly int TypeId = Interlocked.Increment(ref NextMsgId);
        public override int MsgTypeId => TypeId;

        public readonly double Amount;
        public readonly Guid AccountId;

        public DebitApplied(
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
