using System.Windows;

namespace debt_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApiService apiService = new ApiService();
        public MainWindow()
        {
            InitializeComponent();
            UpdateDebt();
        }

        private async void UpdateDebt()
        {
            LabelCurrentDebt.Content = await apiService.GetDebt();
        }

        private async void ButtonNewExpense_Click(object sender, RoutedEventArgs e)
        {
            await apiService.AddDebt(decimal.Parse(TextBoxInput.Text));
            UpdateDebt();
        }

        private async void ButtonDeposit_Click(object sender, RoutedEventArgs e)
        {
            await apiService.SubtractDebt(decimal.Parse(TextBoxInput.Text));
            UpdateDebt();
        }
    }
}
