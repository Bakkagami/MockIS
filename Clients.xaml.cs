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
    public partial class Clients : Page
    {
        public Clients()
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

        private void BtnNewClient_Click(object sender, RoutedEventArgs e)
        {
            _ = new addClient().ShowDialog();
        }


        private void BtnRemoveClient_Click(object sender, RoutedEventArgs e)
        {
            var msg = "Are you sure you want to remove the client from the system?";
            var cap = "Remove Client?";
            if (MessageBox.Show(msg, cap, MessageBoxButton.OKCancel ) == MessageBoxResult.OK)
            {
                try { Core.RemoveClient((DataRowView)dgClients.SelectedItem); } catch (Exception ex) { Console.WriteLine(ex.Message); }
                reloadData();
            }
        }

        private void BtnEditClient_Click(object sender, RoutedEventArgs e)
        {
            try { _ = new EditClient((DataRowView)dgClients.SelectedItem).ShowDialog(); } catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // once an item is selected selected enable the edit button
            if (!btnEditClient.IsEnabled) { btnEditClient.IsEnabled = true; }
            DisplayInfo();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try { ((DataView)dgClients.ItemsSource).RowFilter = txtFilter.Text; }
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

        private void DisplayInfo()
        {
            try
            {
                DataRowView client = (DataRowView)dgClients.SelectedItem;
                lblClientName.Content = client["name"];
                lblClientID.Content = "ID: " + client["client_id"];
                lblClientContactName.Content = "Contact: " + client["contact_name"];
                lblClientContactPhone.Content = "Phone: " + client["contact_phone"];
                lblClientContactEmail.Content = "Email: " + client["email_address"];
                lblClientStreet.Content = "Address: " + client["street_address"];
                lblClientCityState.Content = "City/State: " + client["City"] + ", " + client["state"];
                lblClientZip.Content = "ZipCode: " + client["zipcode"];
                lblClientCountry.Content = "Country: " + client["country"];
            } catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void reloadData()
        {

            try
            {
                dgClients.UnselectAll(); // avoid throwing error due to selected cell refreshing
                dgClients.ItemsSource = null;
                dgClients.ItemsSource = Core.LoadClients().DefaultView;
                dgClients.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
    }
}
