using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class SelectItems : Window
    {
        

        public SelectItems()
        {
            InitializeComponent();
            try
            {
                cbxItems.ItemsSource = Core.LoadInventory().DefaultView;

                // dynamically add column to the datagrid since we are dynamically populating it later
                DataGridTextColumn makeColumn = new DataGridTextColumn();
                makeColumn.Header = "Make";
                makeColumn.Binding = new Binding("manufacturer_name");
                dgItemList.Columns.Add(makeColumn);

                DataGridTextColumn modelColumn = new DataGridTextColumn();
                modelColumn.Header = "Model";
                modelColumn.Binding = new Binding("model_name");
                dgItemList.Columns.Add(modelColumn);

                DataGridTextColumn mnumColumn = new DataGridTextColumn();
                mnumColumn.Header = "Model Num";
                mnumColumn.Binding = new Binding("model_number");
                dgItemList.Columns.Add(mnumColumn);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (!dgItemList.Items.Contains(cbxItems.SelectedItem))
            {
                dgItemList.Items.Add(cbxItems.SelectedItem);
            }   
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try { dgItemList.Items.Remove(dgItemList.SelectedItem); } catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        // hide this window if the user closes it in case it needs to be opened again
        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.IsVisible)
            {
                e.Cancel = true;
                this.Hide();
            }

        }
    }
}
