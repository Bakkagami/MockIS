using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace Final_Project
{

    static class Core
    {
        private static string msgError = "Error";
        private static NpgsqlConnection conn;
        private static NpgsqlDataAdapter da;
        private static DataSet ds = new DataSet();
        private static readonly IFormatProvider provider = System.Globalization.CultureInfo.InvariantCulture.NumberFormat; 

        public static bool ConnectDB(string username, string password)
        {
            // connect to our database using the user's login credentials
            try
            {
                string connString = string.Format(GetProvider(), "Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    "68.20.15.125", "5432", username, password, "MockIS");

                conn = new NpgsqlConnection(connString);
                conn.Open();                
            }
            catch (NpgsqlException msg)
            {
                _ = MessageBox.Show(msg.Message,
                                    msgError,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }

            if(conn.State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void AddOrder(object orderType, object client, object broker, decimal price, SelectItems itemList, string comment)
        {
            // create the order
            using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO orders (order_id, client_id, broker_id, order_type, comments, price) VALUES(NEXTVAL('seq_order_id'), :client, :broker, :orderType, :comment, :price)", conn))
            {
               
                cmd.Parameters.Add(new NpgsqlParameter("client", (int)client));
                cmd.Parameters.Add(new NpgsqlParameter("broker", (int)broker));
                cmd.Parameters.Add(new NpgsqlParameter("orderType", ((ComboBoxItem)orderType).Content.ToString()));
                cmd.Parameters.Add(new NpgsqlParameter("comment", comment));
                cmd.Parameters.Add(new NpgsqlParameter("price", price));

                cmd.ExecuteNonQuery();
            }

            // Add the item list for our sale

            int id;
            foreach (DataRowView row in itemList.dgItemList.Items)
            {
                
                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO item_list(order_id, inventory_id) VALUES (CURRVAL('seq_order_id'), :item)", conn))
                {
                    id = (int)row["inventory_id"];
                    cmd.Parameters.Add(new NpgsqlParameter("item", id));

                    cmd.ExecuteNonQuery();
                }

                // update inventory to reflect items are sold
                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE inventory SET sold = 'SOLD' WHERE inventory_id = :item", conn))
                {
                    id = (int)row["inventory_id"];
                    cmd.Parameters.Add(new NpgsqlParameter("item", id));

                    cmd.ExecuteNonQuery();
                }

            }

        }

        internal static void RemoveOrder(DataRowView drv)
        {
            if (drv != null)
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM orders WHERE order_id = :order", conn))
                {
                    var id = drv["order_id"];
                    cmd.Parameters.Add(new NpgsqlParameter("order", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static IEnumerable<DataGridRow> GetDataRows(DataGrid dg)
        {
            var itemSource = dg.ItemsSource as IEnumerable;
            if (null == itemSource) yield return null;
            foreach (var item in itemSource)
            {
                var row = dg.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        internal static void FilterTable(DataGrid dg, string filter, string tableName)
        {
            DataTable dt = new DataTable();
            string sQuery = "SELECT * FROM " + tableName + " WHERE " + filter;
            try
            {
                using (da = new NpgsqlDataAdapter(sQuery, conn))
                {
                    da.Fill(dt);
                    dg.ItemsSource = dt.DefaultView;

                }
            } catch (NpgsqlException e)
            {
                MessageBox.Show("ERROR: Filter function is still in development: \n Please query in format: <parameter> [CONDITION] '<criteria>' \n EXAMPLE: \"broker_id = 1 \n\n" + e.Message);
            }
        }

        internal static DataTable LoadOrders()
        {
            DataTable dt = new DataTable();
            string sQuery = "SELECT * FROM orders";
            using (da = new NpgsqlDataAdapter(sQuery, conn))
            {
                da.Fill(dt);
                return dt;
            }
        }

        public static DataTable UpdateItemList(int saleID)
        {
            DataTable dt = new DataTable();
            string sQuery = "SELECT inventory_id FROM orders WHERE sale_id=" + saleID;
            using (da = new NpgsqlDataAdapter(sQuery, conn))
            {

                da.Fill(dt);
                return dt;
            }
        }

        public static DataTable LoadInventory()
        {        
            DataTable dt = new DataTable();
            string sQuery = "SELECT * FROM inventory WHERE sold IS NULL OR sold != 'SOLD'";
            using (da = new NpgsqlDataAdapter(sQuery, conn)) 
            {
                da.Fill(dt);
                return dt;
            }
        }

        public static DataTable LoadClients()
        {
            DataTable dt = new DataTable();
            string sQuery = "SELECT c.name, c.client_id, c.contact_name, c.contact_phone, c.email_address, c.street_address, z.city, z.state, z.country, z.zipcode FROM clients c, zipcode z WHERE c.zipcode = z.zipcode";
            using (da = new NpgsqlDataAdapter(sQuery, conn))
            {
                da.Fill(dt);
                return dt;
            }
        }    

        public static DataTable LoadEmployees()
        {
            DataTable dt = new DataTable();
            string sQuery = "Select * FROM employee";
            using (da = new NpgsqlDataAdapter(sQuery, conn))
            {
                da.Fill(dt);            
                return dt;
            } 
        }

        public static void AddClient(string client, string contact, string phone, string email, string address, string city, string state, string country, string zipcode)
        {

            // add a new zipcode if it doesn't exist otherwise just continue
            using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO zipcode (zipcode, city, state, country) SELECT :zipcode, :city, :state, :country WHERE NOT EXISTS(SELECT zipcode FROM zipcode WHERE zipcode = :zipcode)", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("zipcode", zipcode));
                cmd.Parameters.Add(new NpgsqlParameter("city", city));
                cmd.Parameters.Add(new NpgsqlParameter("state", state));
                cmd.Parameters.Add(new NpgsqlParameter("country", country));

                cmd.ExecuteNonQuery();
            }

            // add our new client
            using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO clients (client_id, name, contact_phone, contact_name, email_address, street_address, zipcode) VALUES(NEXTVAL('seq_client_id'), :name, :phone, :contact, :email, :address, :zipcode)", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("name", client));
                cmd.Parameters.Add(new NpgsqlParameter("contact", contact));
                cmd.Parameters.Add(new NpgsqlParameter("phone", phone));
                cmd.Parameters.Add(new NpgsqlParameter("email", email));
                cmd.Parameters.Add(new NpgsqlParameter("address", address));
                cmd.Parameters.Add(new NpgsqlParameter("zipcode", zipcode));

                cmd.ExecuteNonQuery();
            }
        }
         
        public static void UpdateClient(int clientID, string client, string contact, string contactPhone, string contactEmail, string address, string city, string state, string country, string zipcode)
        {
            // if zipcode changes ensure that the new zipcode is in out table
            using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO zipcode (zipcode, city, state, country) SELECT :zipcode, :city, :state, :country WHERE NOT EXISTS(SELECT zipcode FROM zipcode WHERE zipcode = :zipcode)", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("zipcode", zipcode));
                cmd.Parameters.Add(new NpgsqlParameter("city", city));
                cmd.Parameters.Add(new NpgsqlParameter("state", state));
                cmd.Parameters.Add(new NpgsqlParameter("country", country));

                cmd.ExecuteNonQuery();
            }

            // update the table entry
            using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE clients SET name = :client,	contact_name = :contact, contact_phone = :phone, email_address = :email, street_address = :address, zipcode = :zipcode WHERE client_id = :id", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("id", clientID));
                cmd.Parameters.Add(new NpgsqlParameter("client", client));
                cmd.Parameters.Add(new NpgsqlParameter("contact", contact));
                cmd.Parameters.Add(new NpgsqlParameter("phone", contactPhone));
                cmd.Parameters.Add(new NpgsqlParameter("email", contactEmail));
                cmd.Parameters.Add(new NpgsqlParameter("address", address));
                cmd.Parameters.Add(new NpgsqlParameter("zipcode", zipcode));

                cmd.ExecuteNonQuery();
            }
        }

        public static void RemoveClient(DataRowView client)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM clients WHERE client_id = :clientID", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("clientID", int.Parse(client["client_id"].ToString(), GetProvider())));

                cmd.ExecuteNonQuery();
            }
        }

        public static void AddItem(string cat, string manu, string mName, string mNumber, int memS, string memT, string testStatus, int storage, string desc)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO inventory (inventory_id, manufacturer_name, model_name, model_number, memory_size, memory_type, test_status, storage_capacity, description, category) Values (nextval('seq_inventory_id'), :make, :mName, :mNumber, :memSize, :memType, :testStatus, :storage, :description, :category);", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("make", manu));
                cmd.Parameters.Add(new NpgsqlParameter("mName", mName));
                cmd.Parameters.Add(new NpgsqlParameter("mNumber", mNumber));
                cmd.Parameters.Add(new NpgsqlParameter("memSize", memS));
                cmd.Parameters.Add(new NpgsqlParameter("memType", memT));
                cmd.Parameters.Add(new NpgsqlParameter("testStatus", testStatus));
                cmd.Parameters.Add(new NpgsqlParameter("storage", storage));
                cmd.Parameters.Add(new NpgsqlParameter("description", desc));
                cmd.Parameters.Add(new NpgsqlParameter("category", cat));

                cmd.ExecuteNonQuery();
            }

        }

        public static void UpdateItem(int itemID, string cat, string manu, string mName, string mNumber, int memS, string memT, string testStatus, int storage, string desc)
        {
            
            using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE inventory SET manufacturer_name = :make, model_name = :mName, model_number = :mNumber, memory_size = :memSize, memory_type = :memType, test_status = :testStatus, storage_capacity = :storage, description = :description, category = :category WHERE inventory_id = :itemID;", conn))

            {
                cmd.Parameters.Add(new NpgsqlParameter("make", manu));
                cmd.Parameters.Add(new NpgsqlParameter("mName", mName));
                cmd.Parameters.Add(new NpgsqlParameter("mNumber", mNumber));
                cmd.Parameters.Add(new NpgsqlParameter("memSize", memS));
                cmd.Parameters.Add(new NpgsqlParameter("memType", memT));
                cmd.Parameters.Add(new NpgsqlParameter("testStatus", testStatus));
                cmd.Parameters.Add(new NpgsqlParameter("storage", storage));
                cmd.Parameters.Add(new NpgsqlParameter("description", desc));
                cmd.Parameters.Add(new NpgsqlParameter("category", cat));

                cmd.Parameters.Add(new NpgsqlParameter("itemID", itemID)); // note: if undefined this will evaluate to 0 (may return first item in the table)

                cmd.ExecuteNonQuery();
            }
        }


        public static void RemoveItem(DataRowView item)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM inventory WHERE inventory_id = :itemID", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("itemID", int.Parse(item["inventory_id"].ToString(), GetProvider())));

                cmd.ExecuteNonQuery();
            }
        }

        public static IFormatProvider GetProvider()
        {
            return provider;
        }

        public static void ShutdownConnection()
        {
            ds.Dispose();
            conn.Close();
        }
    }
}
