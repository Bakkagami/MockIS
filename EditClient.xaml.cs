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
using System.Windows.Shapes;

namespace Final_Project
{
    /// <summary>
    /// Interaction logic for addInventoryItem.xaml
    /// </summary>
    public partial class EditClient : Window
    {
        int clientID;

        public EditClient(DataRowView client)
        {
            this.clientID = int.Parse(client["client_id"].ToString(), Core.GetProvider());
            InitializeComponent();
            this.txtClient.Text = client["name"].ToString();
            this.txtContactName.Text = client["contact_name"].ToString();
            this.txtContactPhone.Text = client["contact_phone"].ToString();
            this.txtContactEmail.Text = client["email_address"].ToString();
            this.txtAddress.Text = client["street_address"].ToString();
            this.txtCity.Text = client["city"].ToString();
            this.txtState.Text = client["state"].ToString();
            this.txtCountry.Text = client["country"].ToString();
            this.txtZipcode.Text = client["zipcode"].ToString();

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
                Core.UpdateClient(clientID, client, contact, contactPhone, contactEmail, address, City, state, country, zipcode);
                
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
