using System;
using System.Collections.Generic;
using System.Data;
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

namespace Final_Project
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class EmployeePage : Page
    {
        public EmployeePage()
        {
            InitializeComponent();

            // populate the datagrid
            dgEmployees.ItemsSource = Core.LoadEmployees().DefaultView;
        }

        private void BnHome_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            ((Window)this.Parent).Content = new EmployeePage();
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            ((Window)this.Parent).Content = new Inventory();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            ((Window)this.Parent).Content = new Clients();
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            ((Window)this.Parent).Content = new Orders();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var msg = "Are you sure you want to log out?";
            var caption = "Log Out";

            if (MessageBox.Show(msg, caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Core.ShutdownConnection();
                ((Window)this.Parent).Close();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayEmployee();
        }

        private void DisplayEmployee()
        {
            // display our employee information
            try {
                DataRowView selected = (DataRowView)dgEmployees.SelectedItem;
                selLastName.Content = selected[1];
                selFirstName.Content = selected[2];
                selPhone.Content = selected[3];
                selEmail.Content = selected[4];
                selAddress.Content = selected[5];
                selZipCode.Content = selected[6];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
