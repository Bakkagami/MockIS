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
    public partial class addInventoryItem : Window
    {
        public addInventoryItem()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // set our variables
                string category = cbxCategory.Text;
                string manu = txtManufacturer.Text;
                string mName = txtmodelName.Text;
                string mNumber = txtmodelNumber.Text;
                // validate to ensure memsize input is an integer
                int memSize = int.Parse(txtmemorySize.Text, Core.GetProvider());
                string memType = txtmemoryType.Text;
                string testStatus = cbxTestStatus.Text; ;
                // validate storage cap as integer
                int storageCap = int.Parse(txtStorage.Text, Core.GetProvider());
                string description = txtDescription.Text;
                //testing
                Console.WriteLine("Adding to inventory - {0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}",category, manu, mName, mNumber, memSize, memType, testStatus, storageCap, description);

                // run our query
                Core.AddItem(category, manu, mName, mNumber, memSize, memType, testStatus, storageCap, description);
                
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
