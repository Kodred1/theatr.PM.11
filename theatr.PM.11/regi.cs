using Npgsql;
using System;
using System.Windows.Forms;

namespace theatr.PM._11
{
    public partial class regi : Form
    {
        string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_10";

        public regi()
        {
            InitializeComponent();
        }

        private void regi_Load(object sender, EventArgs e)
        {
            guna2TextBox2.PasswordChar = '*';
            guna2TextBox3.PasswordChar = '*';
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            guna2TextBox2.PasswordChar = checkBox1.Checked ? '\0' : '*';
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            guna2TextBox3.PasswordChar = checkBox2.Checked ? '\0' : '*';
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text.Trim();
            string password = guna2TextBox2.Text;
            string confirmPassword = guna2TextBox3.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        if ((long)checkCmd.ExecuteScalar() > 0)
                        {
                            MessageBox.Show("Логин уже используется. Выберите другой.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Получение ID роли 'user'
                    string getRoleQuery = "SELECT id FROM theater.roles WHERE LOWER(name) = 'user'";
                    int roleId;
                    using (var roleCmd = new NpgsqlCommand(getRoleQuery, conn))
                    {
                        object roleObj = roleCmd.ExecuteScalar();
                        if (roleObj == null)
                        {
                            MessageBox.Show("Роль 'user' не найдена в базе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        roleId = Convert.ToInt32(roleObj);
                    }

                    // Вставка нового пользователя
                    string insertUserQuery = @"
                        INSERT INTO theater.users (username, password, role_id) 
                        VALUES (@username, crypt(@password, gen_salt('bf')), @role_id)";
                    using (var insertCmd = new NpgsqlCommand(insertUserQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("username", username);
                        insertCmd.Parameters.AddWithValue("password", password);
                        insertCmd.Parameters.AddWithValue("role_id", roleId);

                        int rows = insertCmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Переход к авторизации
                            auto loginForm = new auto();
                            loginForm.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка регистрации пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
