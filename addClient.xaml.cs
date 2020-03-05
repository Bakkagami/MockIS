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
using System.Windows.Shapes;

namespace Final_Project
{
    /// <summary>
    /// Interaction logic for addInventoryItem.xaml
    /// </summary>
    public partial class addClient : Window
    {
        public addClient()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // set our variables
                string client = txtClient.Text;
                string contact = txtContactName.Text;
                string contactPhone = txtContactPhone.Text;
                string contactEmail = txtContactEmail.Text;
                string address = txtAddress.Text;
                string City = txtCity.Text;
                string state = txtState.Text;
                string country = txtCountry.Text;
                string zipcode = txtZipcode.Text;
   

                // run our query
                Core.AddClient(client, contact, contactPhone, contactEmail, address, City, state, country, zipcode);
                
                // close the windowand update the datagrid view in inventory window
                this.Close();
            } catch (Exception inputError)
            {
                MessageBox.Show(inputError.Message);
            }

        }

        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
