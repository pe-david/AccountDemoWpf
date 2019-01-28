using System;
using ReactiveDomain.Bus;
using ReactiveDomain.ViewObjects;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using AccountDemoWpf.Messages;
using CenterSpace.NMath.Core;
using ReactiveUI;
using Greylock.Common.ViewModels;

namespace AccountDemoWpf
{
    public class MainWindowViewModel : ViewModel
    {
        private IGeneralBus _bus;
        private bool accountCreated = true;
        private AccountRM _accountRm;
        private Guid _accountId;
        private double _amount;
        private string _creditOrDebitSelection;

        public ReactiveCommand<Unit, Unit> AddCreditOrDebitCommand;
        public ReactiveCommand<Unit, Unit> CreateAccountCommand;

        public MainWindowViewModel(
                                  IGeneralBus bus,
                                  AccountRM accountRm,
                                  Guid accountId,
                                  EditMode editMode = EditMode.Add) : base(bus, editMode)
        {
            _bus = bus;
            _accountRm = accountRm;
            _accountId = accountId;

            //CreateAccountCommand = ReactiveCommand.Create(
            //    () =>
            //    {
            //        try
            //        {
            //            AccountCreated = true;
            //        }
            //        catch (Exception e)
            //        {
            //            AccountCreated = false;
            //        }
            //    },
            //    null);

            AddCreditOrDebitCommand = CommandBuilder.FromAction(
                action: () =>
                {
                    try
                    {
                        FireCreditOrDebit();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                });
        }

        private void FireCreditOrDebit()
        {
            if (CreditOrDebitSelection == "Credit")
            {
                _bus.Fire(new ApplyCredit(
                        _accountId,
                        Amount,
                        Guid.NewGuid(),
                        Guid.Empty),
                    responseTimeout: TimeSpan.FromSeconds(60));
            }
            else if (CreditOrDebitSelection == "Debit")
            {
                _bus.Fire(new ApplyDebit(
                        _accountId,
                        Amount,
                        Guid.NewGuid(),
                        Guid.Empty),
                    responseTimeout: TimeSpan.FromSeconds(60));
            }
            else
            {
                throw new InvalidOperationException("User choice must be either credit or debit.");
            }
        }

        public double Amount
        {
            get => _amount;
            set => this.RaiseAndSetIfChanged(ref _amount, value);
        }

        public bool AccountCreated
        {
            get => accountCreated;
            private set => this.RaiseAndSetIfChanged(ref accountCreated, value);
        }

        public string CreditOrDebitSelection
        {
            get => _creditOrDebitSelection;
            set => this.RaiseAndSetIfChanged(ref _creditOrDebitSelection, value);
        }
    }
}
