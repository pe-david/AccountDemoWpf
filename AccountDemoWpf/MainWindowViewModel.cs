using System;
using ReactiveDomain.Bus;
using ReactiveDomain.ViewObjects;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using AccountDemoWpf.Messages;
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

            AddCreditOrDebitCommand = ReactiveCommand.Create(
                () => { AccountCreated = true; }, null);

            CreateAccountCommand = ReactiveCommand.Create(
                () =>
                {
                    try
                    {
                        AccountCreated = true;
                    }
                    catch (Exception e)
                    {
                        AccountCreated = false;
                    }
                },
                null);

            //CreateAccountCommand = CommandBuilder.FromAction(action: () =>
            //{
            //    try
            //    {
            //        AccountCreated = true;
            //    }
            //    catch (Exception e)
            //    {
            //        AccountCreated = false;
            //    }
            //}, scheduler: RxApp.MainThreadScheduler);
        }

        public bool AccountCreated
        {
            get => accountCreated;
            private set => this.RaiseAndSetIfChanged(ref accountCreated, value);
        }
    }
}
