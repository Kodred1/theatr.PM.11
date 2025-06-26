using Guna.UI2.WinForms;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace theatr.PM._11
{
    public partial class user : Form
    {
        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_10";

        public user()
        {
            InitializeComponent();
        }

        private void user_Load(object sender, EventArgs e)
        {
            // Заполнение списка ролей
            guna2ComboBox1.Items.Clear();
            guna2ComboBox1.Items.Add("admin");
            guna2ComboBox1.Items.Add("cashier");
            guna2ComboBox1.Items.Add("user");
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string login = guna2TextBox1.Text.Trim();
            string password = guna2TextBox2.Text.Trim();
            string roleName = guna2ComboBox1.SelectedItem.ToString();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Получаем id роли
                int roleId = -1;
                using (var roleCmd = new NpgsqlCommand("SELECT id FROM theater.roles WHERE name = @r", conn))
                {
                    roleCmd.Parameters.AddWithValue("r", roleName);
                    var result = roleCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Ошибка: роль не найдена.");
                        return;
                    }
                    roleId = Convert.ToInt32(result);
                }

                // Вставляем пользователя с шифрованием пароля
                var cmd = new NpgsqlCommand(
                    @"INSERT INTO theater.users (username, password, role_id)
                      VALUES (@u, crypt(@p, gen_salt('bf')), @r)", conn);
                cmd.Parameters.AddWithValue("u", login);
                cmd.Parameters.AddWithValue("p", password);
                cmd.Parameters.AddWithValue("r", roleId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Пользователь успешно добавлен.");
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

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string login = guna2TextBox1.Text.Trim();
            string password = guna2TextBox2.Text.Trim();
            string roleName = guna2ComboBox1.SelectedItem.ToString();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // Проверяем, есть ли уже такой логин
                using (var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM theater.users WHERE username = @u", conn))
                {
                    checkCmd.Parameters.AddWithValue("u", login);
                    long count = (long)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.");
                        return;
                    }
                }

                // Получаем id роли
                int roleId = -1;
                using (var roleCmd = new NpgsqlCommand("SELECT id FROM theater.roles WHERE name = @r", conn))
                {
                    roleCmd.Parameters.AddWithValue("r", roleName);
                    var result = roleCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Ошибка: роль не найдена.");
                        return;
                    }
                    roleId = Convert.ToInt32(result);
                }

                // Вставляем пользователя с шифрованием пароля
                var cmd = new NpgsqlCommand(
                    @"INSERT INTO theater.users (username, password, role_id)
              VALUES (@u, crypt(@p, gen_salt('bf')), @r)", conn);
                cmd.Parameters.AddWithValue("u", login);
                cmd.Parameters.AddWithValue("p", password);
                cmd.Parameters.AddWithValue("r", roleId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Пользователь успешно добавлен.");
            ClearInputs();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            admin admin = new admin();
            admin.Show();
            this.Close();
        }
    }
}
