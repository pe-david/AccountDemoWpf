using System;
using AccountDemoWpf.Messages;
using ReactiveDomain.Bus;
using ReactiveDomain.Domain;
using ReactiveDomain.Messaging;
using Xunit;

namespace AccountDemoWpf.Tests
{
    //ReSharper disable once InconsistentNaming
    public sealed class when_manipulating_account :
                        with_account_service,
                        IHandle<AccountCreated>,
                        IHandle<CreditApplied>,
                        IHandle<DebitApplied>,
                        IHandleCommand<ApplyCredit>,
                        IHandleCommand<ApplyDebit>
    {
        private Guid _accountId;
        private Guid _correlationId;
        private Account _account;

        protected override void When()
        {
            _accountId = Guid.NewGuid();
            _correlationId = Guid.NewGuid();
            _account = new Account(_accountId, "TestAccount", _correlationId, Guid.NewGuid());
            Repository.Save(_account, Guid.NewGuid());

            _account.ApplyCredit(_accountId, 1000, _correlationId, Guid.NewGuid());
            Repository.Save(_account, Guid.NewGuid());
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

            RepositoryEvents.DequeueNext<AccountCreated>();
            RepositoryEvents.DequeueNext<CreditApplied>();

            RepositoryEvents.AssertNext<AccountCreated>(correlationId, out var evt)
                            .AssertEmpty();

            Assert.Equal(accountId, evt.AccountId);
        }

        [Fact]
        public void cannot_create_duplicate_account()
        {
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(
                        new CreateAccount(
                            _accountId,
                            "DuplicateAccount",
                            _correlationId,
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
            const double amountCredited = 123.45;
            Bus.Fire(new ApplyCredit(
                    _accountId,
                    amountCredited,
                    _correlationId,
                    Guid.Empty),
                responseTimeout: TimeSpan.FromSeconds(60));

            BusCommands.AssertNext<ApplyCredit>(_correlationId, out var cmd)
                .AssertEmpty();

            RepositoryEvents.DequeueNext<AccountCreated>();
            RepositoryEvents.DequeueNext<CreditApplied>();

            RepositoryEvents.AssertNext<CreditApplied>(_correlationId, out var evt)
                .AssertEmpty();

            Assert.Equal(amountCredited, evt.Amount);
        }

        [Fact]
        public void cannot_apply_credit_with_empty_account_id()
        {
            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            Guid.Empty,
                            amountCredited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_credit_with_empty_correlation_id()
        {
            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(
                        new ApplyCredit(
                            _accountId,
                            amountCredited,
                            Guid.Empty,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_debit_with_empty_account_id()
        {
            const double amountDebited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyDebit(
                            Guid.Empty,
                            amountDebited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_debit_with_empty_correlation_id()
        {
            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(
                        new ApplyDebit(
                            _accountId,
                            amountCredited,
                            Guid.Empty,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_debit_with_wrong_id()
        {
            const double amountDebited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            Guid.NewGuid(),
                            amountDebited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_credit_with_wrong_id()
        {
            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            Guid.NewGuid(),
                            amountCredited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void can_apply_debit()
        {
            const double amountDebited = 123.45;
            Bus.Fire(new ApplyDebit(
                    _accountId,
                    amountDebited,
                    _correlationId,
                    Guid.Empty),
                responseTimeout: TimeSpan.FromSeconds(60));

            BusCommands.AssertNext<ApplyDebit>(_correlationId, out var cmd)
                .AssertEmpty();

            RepositoryEvents.DequeueNext<AccountCreated>();
            RepositoryEvents.DequeueNext<CreditApplied>();

            RepositoryEvents.AssertNext<DebitApplied>(_correlationId, out var evt)
                .AssertEmpty();

            Assert.Equal(amountDebited, evt.Amount);
        }

        [Fact]
        public void cannot_debit_a_negative_amount()
        {
            const double amountDebited = -123.45;
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    Bus.Fire(new ApplyDebit(
                            _accountId,
                            amountDebited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_credit_a_negative_amount()
        {
            const double amountCredited = -123.45;
            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            _accountId,
                            amountCredited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void cannot_apply_debit_with_negative_balance()
        {
            const double amountDebited = 2000;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyDebit(
                            _accountId,
                            amountDebited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void debit_fails_when_wrong_account_id()
        {
            const double amountDebited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyDebit(
                            Guid.NewGuid(),
                            amountDebited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void credit_fails_when_wrong_account_id()
        {
            const double amountCredited = 123.45;
            Assert.Throws<CommandException>(
                () =>
                {
                    Bus.Fire(new ApplyCredit(
                            Guid.NewGuid(),
                            amountCredited,
                            _correlationId,
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                });
        }

        [Fact]
        public void command_enabled_with_valid_amount()
        {
            var accountId = Guid.NewGuid();
            var vm = new MainWindowViewModel(Bus, null, accountId);

            var canExecute = true;
            var canExecuteCmd = vm.AddCreditOrDebitCommand.CanExecute;

            using (canExecuteCmd.Subscribe(x => canExecute = x))
            {
                vm.Amount = -10;
                Assert.IsOrBecomesFalse(() => canExecute);

                vm.Amount = 10;
                Assert.IsOrBecomesTrue(() => canExecute);
            }
        }

        [Fact]
        public void dropdown_determines_credit_or_debit()
        {
            var vm = new MainWindowViewModel(Bus, null, _accountId)
            {
                Amount = 100,
                CreditOrDebitSelection = "Debit"
            };

            var test = vm.AddCreditOrDebitCommand.Execute();
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

        public CommandResponse Handle(ApplyCredit command)
        {
            return command.Succeed();
        }

        public CommandResponse Handle(ApplyDebit command)
        {
            return command.Succeed();
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
