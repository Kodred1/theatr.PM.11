using Guna.UI2.WinForms;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace theatr.PM._11
{
    public partial class spekk : Form
    {
        int selectedPerformanceId = -1;
        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_10";
        public spekk()
        {
            InitializeComponent();
            LoadHalls();
            LoadTimes();
            LoadPerformances();
        }

        private void LoadHalls()
        {
            guna2ComboBox1.Items.Clear();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT id, name FROM theater.halls", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    guna2ComboBox1.Items.Add(new ComboBoxItem(reader.GetString(1), reader.GetInt32(0)));
                }
            }
        }

        private void LoadTimes()
        {
            guna2ComboBox2.Items.Clear();
            guna2ComboBox2.Items.AddRange(new string[]
            {
                "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", 
            });
        }

        private void LoadPerformances()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var adapter = new NpgsqlDataAdapter(
                    @"SELECT p.id, p.name, p.description, p.date_time, h.name as hall 
                      FROM theater.performances p 
                      JOIN theater.halls h ON p.hall_id = h.id ORDER BY p.id", conn);

                var dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string name = guna2TextBox1.Text;
            string description = guna2TextBox2.Text;
            DateTime date = guna2DateTimePicker1.Value.Date;
            string time = guna2ComboBox2.SelectedItem.ToString();
            var hallItem = guna2ComboBox1.SelectedItem as ComboBoxItem;
            DateTime dateTime = DateTime.Parse($"{date:yyyy-MM-dd} {time}");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    @"INSERT INTO theater.performances (name, description, date_time, hall_id)
                      VALUES (@n, @d, @dt, @h)", conn);
                cmd.Parameters.AddWithValue("n", name);
                cmd.Parameters.AddWithValue("d", description);
                cmd.Parameters.AddWithValue("dt", dateTime);
                cmd.Parameters.AddWithValue("h", hallItem.Value);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Спектакль добавлен!");
            LoadPerformances();
            ClearInputs();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (selectedPerformanceId == -1)
            {
                MessageBox.Show("Выберите спектакль для удаления.");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("DELETE FROM theater.performances WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("id", selectedPerformanceId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Спектакль удалён.");
            LoadPerformances();
            ClearInputs();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (selectedPerformanceId == -1 || !ValidateInputs()) return;

            string name = guna2TextBox1.Text;
            string description = guna2TextBox2.Text;
            DateTime date = guna2DateTimePicker1.Value.Date;
            string time = guna2ComboBox2.SelectedItem.ToString();
            var hallItem = guna2ComboBox1.SelectedItem as ComboBoxItem;
            DateTime dateTime = DateTime.Parse($"{date:yyyy-MM-dd} {time}");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    @"UPDATE theater.performances
                      SET name = @n, description = @d, date_time = @dt, hall_id = @h
                      WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("n", name);
                cmd.Parameters.AddWithValue("d", description);
                cmd.Parameters.AddWithValue("dt", dateTime);
                cmd.Parameters.AddWithValue("h", hallItem.Value);
                cmd.Parameters.AddWithValue("id", selectedPerformanceId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Изменения сохранены.");
            LoadPerformances();
            ClearInputs();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            selectedPerformanceId = Convert.ToInt32(row.Cells["id"].Value);
            guna2TextBox1.Text = row.Cells["name"].Value.ToString();
            guna2TextBox2.Text = row.Cells["description"].Value.ToString();

            DateTime dt = Convert.ToDateTime(row.Cells["date_time"].Value);
            guna2DateTimePicker1.Value = dt.Date;
            guna2ComboBox2.SelectedItem = dt.ToString("HH:mm");

            foreach (ComboBoxItem item in guna2ComboBox1.Items)
            {
                if (item.Text == row.Cells["hall"].Value.ToString())
                {
                    guna2ComboBox1.SelectedItem = item;
                    break;
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBox2.Text) ||
                guna2ComboBox1.SelectedItem == null ||
                guna2ComboBox2.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля.");
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            guna2TextBox1.Clear();
            guna2TextBox2.Clear();
            guna2ComboBox1.SelectedIndex = -1;
            guna2ComboBox2.SelectedIndex = -1;
            selectedPerformanceId = -1;
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string name = guna2TextBox1.Text;
            string description = guna2TextBox2.Text;
            DateTime date = guna2DateTimePicker1.Value.Date;
            string time = guna2ComboBox2.SelectedItem.ToString();
            var hallItem = guna2ComboBox1.SelectedItem as ComboBoxItem;
            DateTime dateTime = DateTime.Parse($"{date:yyyy-MM-dd} {time}");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    @"INSERT INTO theater.performances (name, description, date_time, hall_id)
                      VALUES (@n, @d, @dt, @h)", conn);
                cmd.Parameters.AddWithValue("n", name);
                cmd.Parameters.AddWithValue("d", description);
                cmd.Parameters.AddWithValue("dt", dateTime);
                cmd.Parameters.AddWithValue("h", hallItem.Value);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Спектакль добавлен!");
            LoadPerformances();
            ClearInputs();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            cashier cashier = new cashier();
            cashier.Show();
            this.Close();
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

    public class ComboBoxItem
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public ComboBoxItem(string text, int value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString() => Text;
    }
}
