using AccountDemoWpf.Messages;
using ReactiveDomain.Bus;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using ReactiveDomain.Domain;
using ReactiveDomain.EventStore;
using ReactiveDomain.ReadModel;
using ReactiveUI;
using Splat;

namespace AccountDemoWpf
{
    public class AccountRM : ReadModelBase,
                             IHandle<AccountCreated>,
                             IHandle<CreditApplied>,
                             IHandle<DebitApplied>
    {
        private double balance;
        //private readonly ObservableAsPropertyHelper<bool> _accountHasBeenCreated;

        //public bool AccountHasBeenCreated => _accountHasBeenCreated.Value;

        public AccountRM(Guid accountId)
            : base(() =>
                Locator
                    .Current
                    .GetService<IRepository>()
                    .GetListener(
                        $"ImageCache: account-{accountId:N}",
                        true))
        {
            //this.WhenAnyValue(x => x.AccountHasBeenCreated)
            //    .ToProperty(this, x => x.AccountHasBeenCreated);
            EventStream.Subscribe<AccountCreated>(this);
            EventStream.Subscribe<CreditApplied>(this);
            EventStream.Subscribe<DebitApplied>(this);

            Start<Account>(accountId, null,  true);
        }

        public void Handle(AccountCreated message)
        {
            balance = message.Balance;
            Console.WriteLine();
            Console.WriteLine($"Account created: {message.Name}, {message.AccountId}");
            Console.WriteLine($"Initial balance: ${message.Balance:0.00}");
            Console.WriteLine("Enter cr <amount> to enter a credit.");
            Console.WriteLine("Enter db <amount> to enter a debit.");
        }

        public void Handle(CreditApplied message)
        {
            balance += message.Amount;
            Console.WriteLine();
            Console.WriteLine($"CreditApplied: ${message.Amount:0.00}");
            Console.WriteLine($"Balance: ${balance:0.00}");
        }

        public void Handle(DebitApplied message)
        {
            balance -= message.Amount;
            Console.WriteLine();
            Console.WriteLine($"DebitApplied: ${message.Amount:0.00}");
            Console.WriteLine($"Balance: ${balance:0.00}");
        }
    }
}
