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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

namespace Final_Project
{
    /// <summary>
    /// Interaction logic for addInventoryItem.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
        SelectItems itemList = new SelectItems();

        public NewOrder()
        {
            InitializeComponent();
            cbxClient.ItemsSource = Core.LoadClients().DefaultView;
            cbxBroker.ItemsSource = Core.LoadEmployees().DefaultView;
            
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Core.AddOrder(cbxOrderType.SelectedItem, cbxClient.SelectedValue, cbxBroker.SelectedValue, 
                    decimal.Parse(txtPrice.Text, Core.GetProvider()), itemList, txtComments.Text);

                itemList.Close();
                this.Close();
            } catch (Exception ie)
            {
                MessageBox.Show(ie.Message);
            }

        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            itemList.Close();
            this.Close();
        }

        private void BtnSelectItem_Click(object sender, RoutedEventArgs e)
        {
            itemList.ShowDialog();
        }
    }
}
