using System;
using AccountDemoWpf.Messages;
using Greylock.Common.Tests.Helpers;
using ReactiveDomain.Bus;
using Xunit;
using Assert = Xunit.Assert;
using TestHelpers = Greylock.Common.Tests.Helpers.Assert;

namespace AccountDemoWpf.Tests
{
    //ReSharper disable once InconsistentNaming
    public sealed class when_manipulating_account :
                        with_account_service,
                        IHandle<AccountCreated>,
                        IHandle<CreditApplied>,
                        IHandle<DebitApplied>
    {
        private readonly Guid validAccountId = Guid.NewGuid();

        protected override void When()
        {

        }

        [Fact]
        public void can_create_account()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                        accountId,
                        "NewAccount",
                         correlationId,
                        null),
                responseTimeout: TimeSpan.FromMilliseconds(1500));

            BusCommands.AssertNext<CreateAccount>(correlationId, out var cmd)
                       .AssertEmpty();

            RepositoryEvents.AssertNext<AccountCreated>(correlationId, out var evt)
                            .AssertEmpty();

            Assert.Equal(accountId, evt.AccountId);
        }

        [Fact]
        public void cannot_create_duplicate_account()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(1500));

            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(
                        new CreateAccount(
                            accountId,
                            "DuplicateAccount",
                            correlationId,
                            null),
                        responseTimeout: TimeSpan.FromMilliseconds(1500));
                });
        }

        [Fact]
        public void cannot_create_account_with_empty_id()
        {
            var accountId = Guid.Empty;
            var correlationId = Guid.NewGuid();

            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(
                        new CreateAccount(
                            accountId,
                            "NewAccount",
                            correlationId,
                            null),
                        responseTimeout: TimeSpan.FromMilliseconds(1500));
                });
        }

        [Fact]
        public void cannot_create_account_with_empty_correlation_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.Empty;

            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(
                        new CreateAccount(
                            accountId,
                            "NewAccount",
                            correlationId,
                            null),
                        responseTimeout: TimeSpan.FromMilliseconds(1500));
                });
        }

        [Fact]
        public void can_apply_credit()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountCredited = 123.45;
            Bus.Fire(new ApplyCredit(
                    accountId,
                    amountCredited,
                    correlationId,
                    Guid.Empty),
                responseTimeout: TimeSpan.FromSeconds(60));

            BusCommands.DequeueNext<CreateAccount>();
            RepositoryEvents.DequeueNext<AccountCreated>();

            BusCommands.AssertNext<ApplyCredit>(correlationId, out var cmd)
                .AssertEmpty();

            RepositoryEvents.AssertNext<CreditApplied>(correlationId, out var evt)
                .AssertEmpty();

            Assert.Equal(amountCredited, evt.Amount);
        }

        [Fact]
        public void cannot_apply_credit_with_empty_account_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            Guid.Empty,
                            amountCredited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_credit_with_empty_correlation_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(
                        new ApplyCredit(
                            accountId,
                            amountCredited,
                            Guid.Empty,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_debit_with_empty_account_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountDebited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyDebit(
                            Guid.Empty,
                            amountDebited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_debit_with_empty_correlation_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(
                        new ApplyDebit(
                            accountId,
                            amountCredited,
                            Guid.Empty,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_debit_with_wrong_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountDebited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            Guid.NewGuid(),
                            amountDebited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_credit_with_wrong_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            Guid.NewGuid(),
                            amountCredited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void can_apply_debit()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amount = 123.45;
            Bus.Fire(new ApplyCredit(
                    accountId,
                    amount,
                    correlationId,
                    Guid.Empty),
                responseTimeout: TimeSpan.FromSeconds(60));

            Bus.Fire(new ApplyDebit(
                    accountId,
                    amount,
                    correlationId,
                    Guid.Empty),
                responseTimeout: TimeSpan.FromSeconds(60));

            BusCommands.DequeueNext<CreateAccount>();
            BusCommands.DequeueNext<ApplyCredit>();
            RepositoryEvents.DequeueNext<AccountCreated>();
            RepositoryEvents.DequeueNext<CreditApplied>();

            BusCommands.AssertNext<ApplyDebit>(correlationId, out var cmd)
                .AssertEmpty();

            RepositoryEvents.AssertNext<DebitApplied>(correlationId, out var evt)
                .AssertEmpty();

            Assert.Equal(amount, evt.Amount);
        }

        [Fact]
        public void cannot_debit_a_negative_amount()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountDebited = -123.45;
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    Bus.Fire(new ApplyDebit(
                            accountId,
                            amountDebited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_credit_a_negative_amount()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountCredited = -123.45;
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            accountId,
                            amountCredited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_debit_with_negative_balance()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountDebited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyDebit(
                            accountId,
                            amountDebited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void debit_fails_when_wrong_account_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountDebited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyDebit(
                            Guid.NewGuid(),
                            amountDebited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void credit_fails_when_wrong_account_id()
        {
            var accountId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            Bus.Fire(
                new CreateAccount(
                    accountId,
                    "NewAccount",
                    correlationId,
                    null),
                responseTimeout: TimeSpan.FromMilliseconds(3000));

            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            Guid.NewGuid(),
                            amountCredited,
                            correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void command_enabled_with_valid_amount()
        {
            var guid = Guid.NewGuid();
            var readModel = new AccountRM(guid);
            var bus = new CommandBus(
                "Main Bus",
                false);

            //var vm = new MainWindowViewModel(Bus, new AccountRM(Guid.NewGuid()), Guid.NewGuid());
            var vm = new MainWindowViewModel(bus, readModel, guid)
            {
                Amount = 10
            };
            TestHelpers.CanExecute(vm.AddCreditOrDebitCommand);
        }

        public void Handle(AccountCreated message)
        {
        }

        public void Handle(CreditApplied message)
        {
        }

        public void Handle(DebitApplied message)
        {
        }

        //public void Dispose()
        //{
        //    Dispose(true);
        //}

        //private bool _disposed = false;
        //protected override void Dispose(bool disposing)
        //{
        //    if (!_disposed)
        //    {
        //        if (disposing)
        //        {
        //        }

        //        _disposed = true;
        //    }

        //    base.Dispose(disposing);
        //}
    }
}
