using AccountDemoWpf.Messages;
using ReactiveDomain.Bus;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using ReactiveDomain.Domain;
using ReactiveDomain.EventStore;
using ReactiveDomain.ReadModel;
using ReactiveDomain.Util;
using ReactiveUI;
using Splat;

namespace AccountDemoWpf
{
    public class AccountRM : ReadModelBase,
                             IHandle<CreditApplied>,
                             IHandle<DebitApplied>
    {
        private double _balance;
        private readonly ReadModelProperty<string> _accountUpdateMessage = new ReadModelProperty<string>(null);
        public IObservable<string> AccountUpdateMessage => _accountUpdateMessage;

        public AccountRM(Guid accountId)
            : base(() =>
                Locator
                    .Current
                    .GetService<IRepository>()
                    .GetListener(
                        $"Doggy: account-{accountId:N}",
                        true))
        {
            _balance = 0;
            EventStream.Subscribe<CreditApplied>(this);
            EventStream.Subscribe<DebitApplied>(this);
            Start<Account>(accountId, null,  true);
        }

        public void Handle(CreditApplied message)
        {
            _balance += message.Amount;
            _accountUpdateMessage.Update($"Credit: ${message.Amount:0.00} - Balance: ${_balance:0.00}");
        }

        public void Handle(DebitApplied message)
        {
            _balance -= message.Amount;
            _accountUpdateMessage.Update($"Debit: ${message.Amount:0.00} - Balance: ${_balance:0.00}");
        }
    }
}
