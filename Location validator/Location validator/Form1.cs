using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Z.Dapper.Plus;

namespace Location_validator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = tableCollection[cboSheet.SelectedItem.ToString()];
            //dataGridView1.DataSource = dt;
            if (dt !=null)
            {
                List<DataLocation> dataLocations = new List<DataLocation>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataLocation dataLocation = new DataLocation();
                    dataLocation.Location_Name = dt.Rows[i]["Location_Name"].ToString();
                    dataLocation.Longitude = dt.Rows[i]["Longitude"].ToString();
                    dataLocation.Latitude = dt.Rows[i]["Latitude"].ToString();
                    dataLocation.Date = dt.Rows[i]["Date"].ToString();
                    dataLocation.Day = dt.Rows[i]["Day"].ToString();
                    dataLocation.Month = dt.Rows[i]["Month"].ToString();
                    dataLocation.Year = dt.Rows[i]["Year"].ToString();
                    dataLocation.Hour = dt.Rows[i]["Hour"].ToString();
                    dataLocation.Minute = dt.Rows[i]["Minute"].ToString();
                    dataLocation.Day_of_Week = dt.Rows[i]["Day_of_Week"].ToString();
                    dataLocation.IP = dt.Rows[i]["IP"].ToString();
                    dataLocations.Add(dataLocation);
                }
                dataLocationBindingSource.DataSource = dataLocations;
            }
        }

        DataTableCollection tableCollection;
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog openFileDialog = new OpenFileDialog() { Filter ="Excel 97-2003 workbook*.xls|Excel workbook|*.xlsx"})
            {
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textFilename.Text = openFileDialog.FileName;
                    using (var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using(IExcelDataReader reader=ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable=(_)=>new ExcelDataTableConfiguration() { UseHeaderRow = true}

                            });
                            tableCollection = result.Tables;
                            cboSheet.Items.Clear();
                            foreach (DataTable table in tableCollection)
                                cboSheet.Items.Add(table.TableName);// add sheet to combobox
                        }
                    }
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = "Data Source =.; Initial Catalog = Location validator; Integrated Security = True";
                DapperPlusManager.Entity<DataLocation>().Table("LocationData");
                List<DataLocation> dataLocations = dataLocationBindingSource.DataSource as List<DataLocation>;
                if(dataLocations !=null)
                {
                    using(IDbConnection db = new SqlConnection(connectionString))
                    {
                        db.BulkInsert(dataLocations);
                    }
                }
                MessageBox.Show("Finita la comedia!!!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
