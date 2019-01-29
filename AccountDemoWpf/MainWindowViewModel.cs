using System;
using System.Collections.Generic;
using System.Management;
using ReactiveDomain.Bus;
using ReactiveDomain.ViewObjects;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using AccountDemoWpf.Messages;
using CenterSpace.NMath.Core;
using ReactiveUI;
using Greylock.Common.ViewModels;

namespace AccountDemoWpf
{
    public class MainWindowViewModel : ViewModel
    {
        private IGeneralBus _bus;
        private AccountRM _accountRm;
        private Guid _accountId;
        private double? _amount;
        private string _creditOrDebitSelection;
        private readonly ObservableAsPropertyHelper<string> _accountUpdateMessage;

        public ReactiveCommand<Unit, Unit> AddCreditOrDebitCommand;

        public MainWindowViewModel(
                                  IGeneralBus bus,
                                  AccountRM accountRm,
                                  Guid accountId,
                                  EditMode editMode = EditMode.Add) : base(bus, editMode)
        {
            _bus = bus;
            _accountRm = accountRm;
            _accountId = accountId;

            Output = new ReactiveList<string>() { "Initial balance: $0.00" };

            AddCreditOrDebitCommand = CommandBuilder.FromAction(
                canExecute: this.WhenAnyValue(x => x.Amount, x => x > 0),
                action: () =>
                {
                    try
                    {
                        FireCreditOrDebit();
                    }
                    catch (Exception e)
                    {
                        Application.Current.Dispatcher.Invoke(() => Output.Add(e.Message));
                    }
                });

            _accountRm
                .AccountUpdateMessage
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(m =>
                {
                    if (m != null)
                    {
                        Output.Add(m);
                    }
                });
        }

        private void FireCreditOrDebit()
        {
            if (!Amount.HasValue || Amount < 0) throw new InvalidOperationException("Amount must be > 0.");

            switch (CreditOrDebitSelection)
            {
                case "Credit":
                    _bus.Fire(new ApplyCredit(
                            _accountId,
                            Amount.Value,
                            Guid.NewGuid(),
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                    break;

                case "Debit":
                    _bus.Fire(new ApplyDebit(
                            _accountId,
                            Amount.Value,
                            Guid.NewGuid(),
                            Guid.Empty),
                        responseTimeout: TimeSpan.FromSeconds(60));
                    break;

                default:
                    throw new InvalidOperationException("User choice must be either credit or debit.");
            }
        }

        public ReactiveList<string> Output { get; set; }

        public double? Amount
        {
            get => _amount;
            set => this.RaiseAndSetIfChanged(ref _amount, value);
        }

        public string CreditOrDebitSelection
        {
            get => _creditOrDebitSelection;
            set => this.RaiseAndSetIfChanged(ref _creditOrDebitSelection, value);
        }
    }
}
