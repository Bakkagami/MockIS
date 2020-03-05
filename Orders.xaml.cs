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
    public partial class Orders : Page
    {
        public Orders()
        {
            InitializeComponent();

            // populate the datagrid
            reloadData();
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

        private void BtnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            _ = new NewOrder().ShowDialog();
        }


        private void BtnRemoveOrder_Click(object sender, RoutedEventArgs e)
        {
            var msg = "Remove Item from order history?";
            var cap = "Delete Order";
            if (MessageBox.Show(msg, cap, MessageBoxButton.OKCancel ) == MessageBoxResult.OK)
            {
                Core.RemoveOrder((DataRowView)dgOrders.SelectedItem);
                reloadData();
            }
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
           // tbd: Edit existing orders
        }


        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try { ((DataView)dgOrders.ItemsSource).RowFilter = txtFilter.Text; }
            catch (Exception filterex)
            
            {
                var msg = "\n NOTE: Filter function is still in development: filter format <column_name> [OPERATOR] '<condition>'";
                MessageBox.Show(filterex.Message + msg);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            reloadData();
        }

        private void reloadData()
        {

            try
            {
                dgOrders.ItemsSource = null;
                dgOrders.ItemsSource = Core.LoadOrders().DefaultView;
                dgOrders.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
    }
}
