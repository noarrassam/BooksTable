using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N01389325Project
{
    public partial class Form1 : Form
    {
        string connString = @"Data Source=DESKTOP-96046EQ\SQLEXPRESS;Initial Catalog=MMABOOKS;User ID=NoorRassam;Password=Humber;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        SqlDataAdapter dataAdapter; //this object here allows us to build the connection between the program and the database
        DataTable table; //table to hold the information so we can fill the datagrid view
        SqlConnection conn; //Declares a varialbles to hold sql Code
        String selectionStatement = "select * from Books"; 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            JcSelect.SelectedIndex = 0; //Select First Item when the form loads.
            dataGridView1.DataSource = bindingSource1;

            //Select all data from Customers Class class
            GetData("Select * from Books");
        }

        private void GetData(string selectCommand)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connString); //pass in the select command and the connection string
                table = new DataTable(); //make a new data table object
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table); //fill the data table 
                bindingSource1.DataSource = table; //set the data source on the binding source to the table
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);//show useful message to the user of the program
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SqlCommand command; //declares a new sql command object
                                //field names in the table
            string insert = @"insert into Books(ID, Title, Price) 
                                    values(@ID, @Title, @Price)"; // Parameters names

            // using allows disposing low level resources 
            using (conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open(); //open the connection 
                    command = new SqlCommand(insert, conn); // Create new sql command object
                    command.Parameters.AddWithValue(@"ID", txtID.Text); //Read value from fields and add it to table
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);
                    command.Parameters.AddWithValue(@"Price", txtPrice.Text);
                    command.ExecuteNonQuery(); //push stuff into the table
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); //if there is something wrong show user a message.
                }
            }
            GetData("Select * from Books");
            dataGridView1.Update(); // redraws the data grid view so the new record is visible on the bottom

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //grab a reference to the current row
            DataGridViewRow row = dataGridView1.CurrentCell.OwningRow;
            //grab the value from the ID of the selected record
            string value = row.Cells["ID"].Value.ToString();
            //grab the value from the Title field.
            string title = row.Cells["Title"].Value.ToString();
            //grab the value from the Price field
            string price = row.Cells["Price"].Value.ToString();
            DialogResult result = MessageBox.Show("Do you really want to delete" + txtTitle + " " + ", record " + value, "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // this is the sql to delete the records from the sql table
            string deleteStatement = @"Delete from Books where Title = '" + title + "'";

            //check weather ther user really wants to delete records
            if (result == DialogResult.Yes)
            {
                using (conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open(); //try to open a connection 
                        SqlCommand comm = new SqlCommand(deleteStatement, conn);
                        comm.ExecuteNonQuery(); // this line accually causes the deletetion to run
                        GetData(selectionStatement); // get the data again
                        dataGridView1.Update(); //redraw the data grid vie with updated information

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message); //runs when something is wring with the connection
                    }
                }
            }
        }

        private void btnSrch_Click(object sender, EventArgs e)
        {
            switch(JcSelect.SelectedItem.ToString())
            {
                case "ID":
                    GetData("select * from Books where lower(ID) like '%" + txtSrch.Text.ToLower() + "%'");
                    break;
                case "Title":
                    GetData("select * from Books where lower(Title) like '%"+ txtSrch.Text.ToLower()+"%'");
                    break;
                case "Price":
                    GetData("select * from Books where lower(Price) like '%" + txtSrch.Text.ToLower() + "%'");
                    break;
            }
        }

        private void btnExport_Click_1(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog()==DialogResult.OK) // Checks whether a user has clicked "OK" button 
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    foreach(DataGridViewRow row in dataGridView1.Rows) // Grab each row in the data grid view
                    {
                        foreach(DataGridViewCell cell in row.Cells) // Grabed row, go through the cells of that row
                        {
                            sw.Write(cell.Value); // This line writes the value into a text file.
                            sw.Write("        "); // Tab
                        }
                        sw.WriteLine(); //This will push the cursor to the next line.
                    }
                    Process.Start("notepad.exe", saveFileDialog1.FileName); //open file in notepad after the file is written to the drive.
                }
            }
        }
    }
}