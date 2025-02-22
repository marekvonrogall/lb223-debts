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

        private void ButtonNewExpense_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ButtonDeposit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
