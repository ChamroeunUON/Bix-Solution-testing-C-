using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace BizSolution
{
    public partial class Form1 : Form
    {
        private string _name;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception exception)
            {
                MessageBox.Show("" + exception);
            }
        }

        private void LoadData()
        {
            var con = new SqlConnection(ConnectionString.GetStringConnection);
            var query = "SELECT * FROM person";
            con.Open();
            var command = new SqlCommand(query, con);
            var sqlDataReader = command.ExecuteReader();
            if (sqlDataReader.HasRows)
                while (sqlDataReader.Read())
                    dataGridView1.Rows.Add(sqlDataReader["id"].ToString(),
                        sqlDataReader["name"].ToString(),
                        sqlDataReader["dob"].ToString(),
                        sqlDataReader["gender"].ToString(),
                        sqlDataReader["phone"].ToString(),
                        sqlDataReader["addre"].ToString()
                    );
            con.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearControl();
        }

        private void ClearControl()
        {
            txtId.Text = "";
            txtName.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            cboGender.SelectedItem = cboGender.Items.Count - 1;
            txtPhone.Text = "";
            txtAddress.Text = "";
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var con = new SqlConnection(ConnectionString.GetStringConnection);
                const string query = "INSERT INTO person(name,dob,gender,phone,addre) VALUES(@name,@dob,@gender,@phone,@addre)";
                con.Open();
                using (var command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@name", txtName.Text);
                    command.Parameters.AddWithValue("@dob", dateTimePicker1.Value.ToShortDateString());
                    command.Parameters.AddWithValue("@gender", cboGender.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@phone", txtPhone.Text);
                    command.Parameters.AddWithValue("@addre", txtAddress.Text);

                    command.ExecuteNonQuery();
                    command.Dispose();
                    MessageBox.Show(@"Information have been saved");
                    ClearControl();
                    Form1_Load(this, null);
                }

                con.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("" + exception);
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            var id =Convert.ToInt16(dataGridView1.SelectedCells[0].Value.ToString());
            var con = new SqlConnection(ConnectionString.GetStringConnection);
            con.Open();
            var queryUpdate ="UPDATE person SET " +
                             "name=@name," +
                             "dob=@dob," +
                             "gender=@gender," +
                             "phone=@phone," +
                             "addre=@address" +
                             " WHERE id=@id";

            try
            {
                using (var command = new SqlCommand(queryUpdate, con))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@name", txtName.Text);
                    command.Parameters.AddWithValue("@dob", dateTimePicker1.Value.ToShortDateString());
                    command.Parameters.AddWithValue("@gender", cboGender.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@phone", txtPhone.Text);
                    command.Parameters.AddWithValue("@address", txtAddress.Text);
                    var result = command.ExecuteNonQuery();
                    MessageBox.Show(result < 0 ? @"Nothing Update the Information" : @"Information have been updated");
                    command.Dispose();
                    con.Close();
                    dataGridView1.Rows.Clear();
                    ClearControl();
                    LoadData();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Error: " + exception);
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex.Equals(6)) return;
            FillControlBySelectDataGridview();
        }

        private void FillControlBySelectDataGridview()
        {
            try
            {
                txtId.Text = dataGridView1.SelectedCells[0].Value.ToString();
                txtName.Text = dataGridView1.SelectedCells[1].Value.ToString();
                var date = dataGridView1.SelectedCells[2].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(date);
                cboGender.SelectedItem = dataGridView1.SelectedCells[3].Value.ToString();
                txtPhone.Text = dataGridView1.SelectedCells[4].Value.ToString();
                txtAddress.Text = dataGridView1.SelectedCells[5].Value.ToString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Error Message :" + exception);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
               var con = new SqlConnection(ConnectionString.GetStringConnection);
                var queryDelete = "DELETE FROM person WHERE id =@id ";
                con.Open();
                var command = new SqlCommand(queryDelete,con);
                command.Parameters.AddWithValue("@id", Convert.ToInt16(txtId.Text));
                var result = command.ExecuteNonQuery();
                MessageBox.Show(result < 0 ? "Error while delete." : "Delete Successfully.");
                command.Dispose();
                dataGridView1.Rows.Clear();
                LoadData();
                ClearControl();
            }
            catch (Exception exception)
            {
                MessageBox.Show(@"Error :" + exception);
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!e.ColumnIndex.Equals(6)) return;
            var formOrder = new FormOrder
            {   
                GetId=txtId.Text,
                GetNamem = txtName.Text
            };
            formOrder.ShowDialog();
        }
    }
}