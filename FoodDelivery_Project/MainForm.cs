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

namespace FoodDelivery_Project
{
    public partial class MainForm : Form
    {
        BindingSource bsF, bsO, bsC;
        DataSet ds;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            FetchData();
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        public void FetchData()
        {
            ds = new DataSet();
            bsF = new BindingSource();
            bsO = new BindingSource();
            bsC = new BindingSource();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {

                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM FoodItems", con))
                {
                    da.Fill(ds, "FoodItems");

                    da.SelectCommand.CommandText = "SELECT * FROM Orders";
                    da.Fill(ds, "Orders");
                    //Add image column
                    ds.Tables["FoodItems"].Columns.Add(new DataColumn("Img", typeof(byte[])));
                    //Fill Image column in rows
                    foreach (DataRow r in ds.Tables["FoodItems"].Rows)
                    {
                        r["Img"] = File.ReadAllBytes(Path.GetFullPath($@"..\..\Pictures\{r["picture"]}"));
                    }
                    //
                    da.SelectCommand.CommandText = "SELECT * FROM Customers";
                    da.Fill(ds, "Customers");
                    ds.Tables["FoodItems"].PrimaryKey = new DataColumn[] { ds.Tables["FoodItems"].Columns["itemid"] };
                    ds.Tables["Orders"].PrimaryKey = new DataColumn[] { ds.Tables["Orders"].Columns["OrderId"] };
                    ds.Tables["Customers"].PrimaryKey = new DataColumn[] { ds.Tables["Customers"].Columns["CustomerId"] };
                    ds.Relations.Add(new DataRelation("FK_F_O", ds.Tables["FoodItems"].Columns["itemid"], ds.Tables["Orders"].Columns["itemid"]));
                    ds.Relations.Add(new DataRelation("FK_C_O", ds.Tables["Customers"].Columns["CustomerId"], ds.Tables["Orders"].Columns["CustomerId"]));
                    bsF.DataSource = ds;
                    bsF.DataMember = "FoodItems";
                    bsC.DataSource = ds;
                    bsC.DataMember = "Customers";
                    bsO.DataSource = bsC;
                    bsO.DataMember = "FK_C_O";
                    
                    dataGridView1.DataSource = bsO;

                    lblName.DataBindings.Clear();
                    lblPrice.DataBindings.Clear();
                    lblDesc.DataBindings.Clear();
                    pictureBox1.DataBindings.Clear();
                    checkBox1.DataBindings.Clear();

                    lblCName.DataBindings.Clear();
                    lblAddress.DataBindings.Clear();
                    lblPhone.DataBindings.Clear();

                    lblName.DataBindings.Add(new Binding("Text", bsF, "itemname"));
                    lblDesc.DataBindings.Add(new Binding("Text", bsF, "description"));
                    Binding bp = new Binding("Text", bsF, "price", true, DataSourceUpdateMode.OnPropertyChanged);
                    bp.Format += Bp_Format;
                    lblPrice.DataBindings.Add(bp);
                    checkBox1.DataBindings.Add(new Binding("Checked", bsF, "available", true));
                    pictureBox1.DataBindings.Add(new Binding("Image", bsF, "Img", true));

                    lblCName.DataBindings.Add(new Binding("Text", bsC, "customername"));
                    lblAddress.DataBindings.Add(new Binding("Text", bsC, "customeraddress"));
                    lblPhone.DataBindings.Add(new Binding("Text", bsC, "phone"));
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            new AddOrderForm { FormToSync = this }.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(this.bsC.Position<this.bsC.Count-1)
            {
                bsC.MoveNext();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.bsC.Position >0)
            {
                bsC.MovePrevious();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bsC.MoveLast();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bsC.MoveFirst();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Bp_Format(object sender, ConvertEventArgs e)
        {
            decimal v = (decimal)e.Value;
            e.Value = v.ToString("0.00");
        }
    }
}
