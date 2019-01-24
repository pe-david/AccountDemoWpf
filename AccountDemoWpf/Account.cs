using AccountDemoWpf.Messages;
using EventStore.Common.Utils;
using ReactiveDomain.Domain;
using System;

namespace AccountDemoWpf
{
    public class Account : AggregateBase
    {
        public Account(
            Guid accountId,
            string name,
            Guid correlationId,
            Guid sourceId) : base()
        {
            Ensure.NotEmptyGuid(accountId, "accountId");
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotEmptyGuid(correlationId, "correlationId");
            Ensure.NotEmptyGuid(sourceId, "sourceId");

            RaiseEvent(new AccountCreated(
                accountId,
                name,
                Balance,
                correlationId,
                sourceId));
        }

        public Account()
        {
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            Register<AccountCreated>(Apply);
            Register<CreditApplied>(Apply);
            Register<DebitApplied>(Apply);
        }

        private void Apply(DebitApplied @event)
        {
            Balance -= @event.Amount;
        }

        private void Apply(CreditApplied @event)
        {
            Balance += @event.Amount;
        }

        private void Apply(AccountCreated @event)
        {
            Id = @event.AccountId;
        }

        public double Balance { get; private set; }

        public void ApplyCredit(Guid accountId, double amount, Guid corrId, Guid sourceId)
        {
            Ensure.NotEmptyGuid(accountId, nameof(accountId));
            Ensure.Equal(Id.GetHashCode(), accountId.GetHashCode(), nameof(accountId));

            RaiseEvent(new CreditApplied(
                Id,
                amount,
                corrId,
                sourceId: sourceId));
        }

        public void ApplyDebit(Guid accountId, double amount, Guid corrId, Guid sourceId)
        {
            Ensure.NotEmptyGuid(accountId, nameof(accountId));
            Ensure.Equal(Id.GetHashCode(), accountId.GetHashCode(), nameof(accountId));

            var newBalance = Balance - amount;
            if (newBalance < 0)
            {
                throw new InvalidOperationException("Balance cannot be below 0.");
            }

            RaiseEvent(new DebitApplied(
                Id,
                amount,
                corrId,
                sourceId: sourceId));
        }
    }
}
