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
        public ReactiveCommand<Unit, Unit> AddCreditOrDebit;
        private bool accountCreated = false;

        public MainWindowViewModel(
                                  IGeneralBus bus,
                                  EditMode editMode = EditMode.Add) : base(bus, editMode)
        {
            AddCreditOrDebit = ReactiveCommand.Create(
                () => { AccountCreated = true; }, null);
        }

        public bool AccountCreated
        {
            get => accountCreated;
            private set => this.RaiseAndSetIfChanged(ref accountCreated, value);
        }
    }
}
