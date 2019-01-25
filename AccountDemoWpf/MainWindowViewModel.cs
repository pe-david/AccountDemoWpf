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
        private bool accountCreated = false;

        public ReactiveCommand<Unit, Unit> AddCreditOrDebitCommand;
        public ReactiveCommand<Unit, Unit> CreateAccountCommand;

        public MainWindowViewModel(
                                  IGeneralBus bus,
                                  EditMode editMode = EditMode.Add) : base(bus, editMode)
        {
            _bus = bus;
            AddCreditOrDebitCommand = ReactiveCommand.Create(
                () => { AccountCreated = true; }, null);

            CreateAccountCommand = ReactiveCommand.Create(
                () =>
                {
                    try
                    {
                        _bus.Fire(new CreateAccount(
                            Bootstrap.NewAccountId,
                            "TheAccount",
                            Guid.NewGuid(),
                            Guid.Empty));

                        AccountCreated = true;
                    }
                    catch (Exception e)
                    {
                        AccountCreated = false;
                    }

                },
                null);

            //CreateAccountCommand = CommandBuilder.FromAction(() => AccountCreated = true);
        }

        public bool AccountCreated
        {
            get => accountCreated;
            private set => this.RaiseAndSetIfChanged(ref accountCreated, value);
        }
    }
}
