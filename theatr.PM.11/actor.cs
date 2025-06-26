using Guna.UI2.WinForms;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace theatr.PM._11
{
    public partial class actor : Form
    {
        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_10";

        public actor()
        {
            InitializeComponent();
        }

        private void actor_Load(object sender, EventArgs e)
        {
            LoadPerformances();
        }

        private void LoadPerformances()
        {
            guna2ComboBox1.Items.Clear();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT id, name FROM theater.performances ORDER BY name", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var item = new KeyValuePair<string, int>(reader.GetString(1), reader.GetInt32(0));
                    guna2ComboBox1.Items.Add(item);
                }
            }

            guna2ComboBox1.DisplayMember = "Key";
            guna2ComboBox1.ValueMember = "Value";
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string actorName = guna2TextBox1.Text.Trim();
            string role = guna2TextBox2.Text.Trim();
            var selected = (KeyValuePair<string, int>)guna2ComboBox1.SelectedItem;
            int performanceId = selected.Value;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    @"INSERT INTO theater.actors (full_name, performance_id, role_description)
                      VALUES (@name, @perfId, @role)", conn);
                cmd.Parameters.AddWithValue("name", actorName);
                cmd.Parameters.AddWithValue("perfId", performanceId);
                cmd.Parameters.AddWithValue("role", role);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Актёр добавлен.");
            ClearInputs();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text) ||
                string.IsNullOrWhiteSpace(guna2TextBox2.Text) ||
                guna2ComboBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            guna2TextBox1.Clear();
            guna2TextBox2.Clear();
            guna2ComboBox1.SelectedIndex = -1;
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            cashier cashier = new cashier();
            cashier.Show();
            this.Close();
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string actorName = guna2TextBox1.Text.Trim();
            string role = guna2TextBox2.Text.Trim();
            var selected = (KeyValuePair<string, int>)guna2ComboBox1.SelectedItem;
            int performanceId = selected.Value;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    @"INSERT INTO theater.actors (full_name, performance_id, role_description)
                      VALUES (@name, @perfId, @role)", conn);
                cmd.Parameters.AddWithValue("name", actorName);
                cmd.Parameters.AddWithValue("perfId", performanceId);
                cmd.Parameters.AddWithValue("role", role);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Актёр добавлен.");
            ClearInputs();
        }
    }
}
