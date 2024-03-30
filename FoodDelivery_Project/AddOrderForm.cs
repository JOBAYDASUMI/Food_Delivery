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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace FoodDelivery_Project
{
    public partial class AddOrderForm : Form
    {
        DataTable dt = new DataTable();
        public AddOrderForm()
        {
            InitializeComponent();
            this.dataGridView1.AutoGenerateColumns = false;
        }
        public MainForm FormToSync { get; set; }
        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void AddOrderForm_Load(object sender, EventArgs e)
        {
            BuildOrderDataTable();
            this.dataGridView1.DataSource = dt;
            LoadCombo();
        }

        private void LoadCombo()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {

                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM FoodItems", con))
                {
                   DataTable dtc =new DataTable();
                    da.Fill(dtc);
                    this.comboBox1.DataSource = dtc;
                }
            }
        }

        private void BuildOrderDataTable()
        {
            dt.Columns.Add(new DataColumn("id", typeof(int)));
            dt.Columns.Add(new DataColumn("name", typeof(string)){ MaxLength=50});
            dt.Columns.Add(new DataColumn("quantity", typeof(int)));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataRow dr = dt.NewRow();
            dr["id"] = comboBox1.SelectedValue;
            dr["name"] = comboBox1.Text;
            dr["quantity"] = numericUpDown1.Value;
            dt.Rows.Add(dr);
            comboBox1.SelectedIndex = -1;
            numericUpDown1.Value = 0;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if( e.ColumnIndex== 3)
            {
                dt.Rows[e.RowIndex].Delete();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            DataTable dtc = dt.GetChanges();
            
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    try
                    {


                        SqlCommand cmdCust = new SqlCommand("InsertCustomer", con, trx);
                        cmdCust.CommandType = CommandType.StoredProcedure;
                        cmdCust.Parameters.AddWithValue("@customername", textBox1.Text);
                        cmdCust.Parameters.AddWithValue("@customeraddress", textBox2.Text);
                        cmdCust.Parameters.AddWithValue("@phone", textBox3.Text);

                        cmdCust.Parameters.Add(new SqlParameter() { ParameterName = "@id", Direction = ParameterDirection.Output, DbType = DbType.Int32 });
                        cmdCust.ExecuteNonQuery();
                        int cid = (int)cmdCust.Parameters["@id"].Value;
                        foreach (DataRow dr in dtc.Rows)
                        {
                            //int pid = dr["PaymentId"];

                            string sql = @"INSERT INTO Orders (orderdate, customerid, itemid, quantity) VALUES (@d, @c, @i, @q)";
                            SqlCommand cmd = new SqlCommand(sql, con, trx);
                            cmd.Parameters.AddWithValue("@d", DateTime.Now.Date);
                            cmd.Parameters.AddWithValue("@c", cid);
                            cmd.Parameters.AddWithValue("@i", dr["id"]);
                            cmd.Parameters.AddWithValue("@q", dr["quantity"]);
                            cmd.ExecuteNonQuery();



                        }
                        trx.Commit();
                        dt.AcceptChanges();
                        MessageBox.Show("Data saved", "Success");
                        
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        
                        dt.Rows.Clear();
                    }
                    catch (Exception ex)
                    {
                        trx.Rollback();
                        MessageBox.Show(ex.Message, "Error");
                    }

                }
                con.Close();
            }
        }

        private void AddOrderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormToSync.FetchData();
        }
    }
}
