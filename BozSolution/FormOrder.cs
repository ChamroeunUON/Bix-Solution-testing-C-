using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using DataGrid = System.Web.UI.WebControls.DataGrid;

namespace BizSolution
{
    public partial class FormOrder : Form
    {
        public FormOrder()
        {
            InitializeComponent();
        }

        private string Product { set; get; }
        private string Price { set; get; }
        private Dictionary<int, Product> _data;
        private ComboBox _comboBox;
        public string GetName { get; set; }
        public string GetId { get; set; }
        public float GetPrice { get; set; }
        public DateTime GetDate { get; set; }
        public int GetLastOrderId {
            get
            {
                var orderId = 0;
               var con = new SqlConnection(ConnectionString.GetStringConnection);
                con.Open();
                const string query = "SELECT TOP 1 * FROM tblorder ORDER BY ID DESC";
                var com = new SqlCommand(query, con);
                var id = com.ExecuteReader();
                if (!id.HasRows) MessageBox.Show(@"No Data");
                while (id.Read())
                {
                    orderId = int.Parse(id["id"].ToString());
                }
                con.Close();
                return orderId;
            }
             }
        private void FormOrder_Load(object sender, EventArgs e)
        {
            txtName.Text = GetName;
            try
            {
                _data = new Dictionary<int, Product>();
                var con = new SqlConnection(ConnectionString.GetStringConnection);
                const string query = "SELECT id,product,price FROM product";
                con.Open();
                var command = new SqlCommand(query, con);
                var result = command.ExecuteReader();
                if (!result.HasRows)
                    MessageBox.Show(@"Don't have product", @"Information", MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk);
                while (result.Read())
                    _data.Add(int.Parse(result["id"].ToString()), new Product
                    {
                        Id = int.Parse(result["id"].ToString()),
                        ProcustName = result["product"].ToString(),
                        Price = float.Parse(result["price"].ToString())
                    });

                var comboProduct = (DataGridViewComboBoxColumn) dataGridView1.Columns["product"];
                foreach (var d in _data.Values) {comboProduct?.Items.Add(d.ProcustName);}
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Error :" + exception);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Get ComboBox Object
            _comboBox = e.Control as ComboBox;

            if (_comboBox == null) return;

            // Avoid attached multiple Event Handler
            _comboBox.SelectedIndexChanged -= comboBox_SelectIndexChange;

            //  Then non Add
            _comboBox.SelectedIndexChanged += comboBox_SelectIndexChange;
        }

        private void comboBox_SelectIndexChange(object sender, EventArgs e)
        {
            try
            {
                var selected = ((ComboBox) sender).SelectedItem.ToString();
                var con = new SqlConnection(ConnectionString.GetStringConnection);
                con.Open();
                const string query = "SELECT id,price FROM product WHERE product=@id";
                var command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@id", selected);
                var result = command.ExecuteReader();
                if (!result.HasRows) MessageBox.Show(@"Could not fild any product");
                while (result.Read())
                {
                    GetPrice = float.Parse(result["price"].ToString());
                    dataGridView1.CurrentRow.Cells[3].Value = result["id"].ToString();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Error :" + exception);
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!e.ColumnIndex.Equals(1)) return;
            var rowIndex = dataGridView1.CurrentCell.RowIndex;
            var qty = float.Parse(dataGridView1.Rows[rowIndex].Cells[1].Value.ToString());
            var amount = GetPrice * qty;
            dataGridView1.Rows[rowIndex].Cells[2].Value = amount.ToString("C");


            try
            {
                float sum1 = 0;
                for (var i = 0; i < dataGridView1.Rows.Count - 1; ++i)
                {
                    var s = Convert.ToSingle(dataGridView1.Rows[i].Cells[2].Value.ToString().Replace("$", ""));
                    sum1 += s;
                }

                txtTotal.Text = sum1.ToString("C");
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Error :" + exception);
            }
        }

        private void txtReceive_KeyUp(object sender, KeyEventArgs e)
        {
            var to = txtTotal.Text.Replace("$", "");
            var receive = float.Parse(txtReceive.Text);
            var total = Convert.ToSingle(to);
            var exchange = receive - total;
            if (exchange > total)
            {
                MessageBox.Show(@"Please check it again may have something went wrong");
                return;
            }
            txtExchange.Text = exchange.ToString("C");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
            try
            {
                var con = new SqlConnection(ConnectionString.GetStringConnection);
                const string queryOrder = "INSERT INTO tblorder VALUES(@money_receive,@total,@person_id,@date,@exchange)";
                con.Open();
                var command = new SqlCommand(queryOrder, con);
                command.Parameters.AddWithValue("@money_receive", Convert.ToSingle(txtReceive.Text.Replace("$", "")));
                command.Parameters.AddWithValue("@total", Convert.ToSingle(txtTotal.Text.Replace("$", "")));
                command.Parameters.AddWithValue("@person_id",int.Parse(GetId));
                command.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                float.TryParse(txtExchange.Text, NumberStyles.Currency,
                 CultureInfo.CurrentCulture.NumberFormat, out var value);
                command.Parameters.AddWithValue("@exchange", value);
                command.ExecuteNonQuery();
//                con.Close();X
                // Add Item into Order Detail
                for (var i = 0; i < dataGridView1.RowCount-1; ++i)
                {
                    try
                    {
                        var id = GetLastOrderId;
                        var qty = int.Parse(dataGridView1.Rows[i].Cells[1].Value.ToString());
                        var sum = Convert.ToSingle(dataGridView1.Rows[i].Cells[1].Value.ToString());
                        var proId = int.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString());

                
                        const string queryDeatil = "INSERT INTO orderdetail VALUES(@proId,@orderId,@qty,@sum)";
                        var commandDetail = new SqlCommand(queryDeatil,con);


                        commandDetail.Parameters.AddWithValue("@proId", proId);
                        commandDetail.Parameters.AddWithValue("@orderId", id);
                        commandDetail.Parameters.AddWithValue("@qty", qty);
                        commandDetail.Parameters.AddWithValue("@sum", sum);
                        command.ExecuteNonQuery();

                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(@"Error :" + exception);
                    }

                }
                
                con.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Error Message:" + exception);
            }
        }
    }
}