using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace theatr.PM._11
{
    public partial class auto : Form
    {
        string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_10";

        public auto()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            guna2TextBox2.PasswordChar = '*';
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            guna2TextBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text.Trim();
            string password = guna2TextBox2.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Проверка логина
                    string checkUserQuery = "SELECT COUNT(*) FROM theater.users WHERE username = @username";
                    using (var checkCmd = new NpgsqlCommand(checkUserQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("username", username);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            MessageBox.Show("Пользователь с таким логином не найден");
                            return;
                        }
                    }

                    // Проверка логина и пароля
                    string query = @"
                        SELECT u.username, r.name as role 
                        FROM theater.users u
                        JOIN theater.roles r ON u.role_id = r.id
                        WHERE u.username = @username AND u.password = crypt(@password, u.password);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string role = reader.GetString(1);

                                MessageBox.Show($"Успешный вход! Роль: {role}", "Авторизация");

                                this.Hide();

                                switch (role.ToLower())
                                {
                                    case "админ":
                                    case "admin":
                                        new admin().Show();
                                        break;
                                    case "кассир":
                                    case "cashier":
                                        new cashier().Show();
                                        break;
                                    case "пользователь":
                                    case "user":
                                        new glava().Show();
                                        break;
                                    default:
                                        MessageBox.Show("Неизвестная роль пользователя.");
                                        this.Show();
                                        break;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Неверный пароль", "Ошибка");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}");
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            regi regForm = new regi();
            regForm.Show();
        }
    }
}
