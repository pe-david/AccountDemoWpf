using AccountDemoWpf.Messages;
using ReactiveDomain.Bus;
using ReactiveDomain.Domain;
using ReactiveDomain.Messaging;
using System;
using FluentAssertions.Common;

namespace AccountDemoWpf
{
    public class AccountSvc : 
        TransientSubscriber,
        IHandleCommand<CreateAccount>,
        IHandleCommand<ApplyCredit>,
        IHandleCommand<ApplyDebit>
    {
        private readonly IRepository _repo;

        public AccountSvc(IGeneralBus bus, IRepository repo)
            : base(bus)
        {
            _repo = repo;

            bus.Subscribe<CreateAccount>(this);
            bus.Subscribe<ApplyCredit>(this);
            bus.Subscribe<ApplyDebit>(this);
        }


        public CommandResponse Handle(CreateAccount command)
        {
            if (command.AccountId.IsSameOrEqualTo(Guid.Empty))
            {
                throw new InvalidOperationException("Cannot create an account with an empty account id.");
            }

            if (command.CorrelationId.IsSameOrEqualTo(Guid.Empty))
            {
                throw new InvalidOperationException("Cannot create an account with an empty correlation id.");
            }

            if (_repo.TryGetById<Account>(command.AccountId, out var existing))
            {
                throw new InvalidOperationException("Cannot create an account with a duplicate account id.");
            }

            var account = new Account(
                                command.AccountId,
                                command.Name,
                                command.CorrelationId,
                                command.MsgId);

            var commitId = Guid.NewGuid();
            _repo.Save(account, commitId);
            return command.Succeed();
        }

        public CommandResponse Handle(ApplyCredit command)
        {
            if (command.AccountId.IsSameOrEqualTo(Guid.Empty))
            {
                throw new InvalidOperationException("Cannot apply a credit with an empty account id.");
            }

            if (command.CorrelationId.IsSameOrEqualTo(Guid.Empty))
            {
                throw new InvalidOperationException("Cannot apply a credit with an empty correlation id.");
            }

            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
            {
                throw new InvalidOperationException($"ApplyCredit: Account {{{command.AccountId}}} not found.");
            }

            account.ApplyCredit(command.AccountId, command.Amount, command.CorrelationId, command.MsgId);

            _repo.Save(account, Guid.NewGuid());
            return command.Succeed();
        }

        public CommandResponse Handle(ApplyDebit command)
        {
            if (command.AccountId.IsSameOrEqualTo(Guid.Empty))
            {
                throw new InvalidOperationException("Cannot apply a debit with an empty account id.");
            }

            if (command.CorrelationId.IsSameOrEqualTo(Guid.Empty))
            {
                throw new InvalidOperationException("Cannot apply a debit with an empty correlation id.");
            }

            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
            {
                throw new InvalidOperationException($"ApplyDebit: Account {{{command.AccountId}}} not found.");
            }

            account.ApplyDebit(command.AccountId, command.Amount, command.CorrelationId, command.MsgId);

            _repo.Save(account, Guid.NewGuid());
            return command.Succeed();
        }
    }
}
