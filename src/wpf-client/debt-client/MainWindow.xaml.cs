using System.Windows;

namespace debt_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DebtService debtService = new DebtService();
        public MainWindow()
        {
            InitializeComponent();
            UpdateDebt();
        }

        private async void UpdateDebt()
        {
            decimal? debt = await debtService.GetDebt();
            LabelCurrentDebt.Content = debt.HasValue ? $"CHF {debt.Value.ToString("F2")}" : "The API could not be reached!";
            TextBoxInput.Text = string.Empty;
        }

        private async void ButtonNewExpense_Click(object sender, RoutedEventArgs e)
        {
            string message = await debtService.AddDebt(TextBoxInput.Text);
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateDebt();
        }

        private async void ButtonDeposit_Click(object sender, RoutedEventArgs e)
        {
            string message = await debtService.SubtractDebt(TextBoxInput.Text);
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateDebt();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateDebt();
        }
    }
}
