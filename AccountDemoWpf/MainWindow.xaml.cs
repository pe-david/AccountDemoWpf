using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EventStore.Projections.Core.Messages;
using ReactiveUI;

namespace AccountDemoWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IViewFor<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(
                d =>
                {
                    d(this.Bind(ViewModel, vm => vm.Amount, v => v.txtAmount.Text));
                    d(this.Bind(ViewModel, vm => vm.CreditOrDebitSelection, v => v.cbCreditOrDebit.Text));
                    d(this.OneWayBind(ViewModel, vm => vm.AccountCreated, v => v.cbCreditOrDebit.IsEnabled));
                    d(this.OneWayBind(ViewModel, vm => vm.AccountCreated, v => v.btnAddCreditOrDebit.IsEnabled));
                    d(this.BindCommand(ViewModel, vm => vm.AddCreditOrDebitCommand, v => v.btnAddCreditOrDebit));

                    cbCreditOrDebit.SelectedIndex = 0;
                });
        }

        public MainWindowViewModel ViewModel
        {
            get => (MainWindowViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel", 
                typeof(MainWindowViewModel),
                typeof(MainWindow),
                new PropertyMetadata(default(MainWindowViewModel)));

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainWindowViewModel) value;
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
