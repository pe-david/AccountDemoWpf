using System;
using ReactiveDomain.Bus;
using ReactiveDomain.ViewObjects;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Greylock.Common.ViewModels;

namespace AccountDemoWpf
{
    public class MainWindowViewModel : ViewModel
    {
        public ReactiveCommand<Unit, Unit> AddCreditOrDebitCommand;
        public ReactiveCommand<Unit, Unit> CreateAccountCommand;
        private bool accountCreated = false;

        public MainWindowViewModel(
                                  IGeneralBus bus,
                                  EditMode editMode = EditMode.Add) : base(bus, editMode)
        {
            AddCreditOrDebitCommand = ReactiveCommand.Create(
                () => { AccountCreated = true; }, null);

            CreateAccountCommand = ReactiveCommand.Create(
                () => { AccountCreated = true; }, null);

            //CreateAccountCommand = CommandBuilder.FromAction(() => AccountCreated = true);
        }

        public bool AccountCreated
        {
            get => accountCreated;
            private set => this.RaiseAndSetIfChanged(ref accountCreated, value);
        }
    }
}
