using ReactiveDomain.Messaging;
using System;
using System.Threading;

namespace AccountDemoWpf.Messages
{
    public class ApplyDebit : Command
    {
        private static readonly int TypeId = Interlocked.Increment(ref NextMsgId);
        public override int MsgTypeId => TypeId;

        public readonly Guid AccountId;
        public readonly double Amount;

        public ApplyDebit(
            Guid accountId,
            double amount,
            Guid correlationId,
            Guid? sourceId) : base(correlationId, sourceId)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountId), "Cannot enter an amount that is < 0.");
            }

            AccountId = accountId;
            Amount = amount;
        }
    }
}
