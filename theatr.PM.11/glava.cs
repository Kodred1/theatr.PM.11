using Npgsql;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace theatr.PM._11
{
    public partial class glava : Form
    {
        string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_10";

        public glava()
        {
            InitializeComponent();
            this.Load += new EventHandler(glava_Load);
        }

        private void glava_Load(object sender, EventArgs e)
        {
            LoadPerformancesToPanel();
        }

        private void LoadPerformancesToPanel()
        {
            panel1.Controls.Clear();
            panel1.AutoScroll = true;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id, name, date_time FROM theater.performances ORDER BY date_time";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    int y = 10;

                    while (reader.Read())
                    {
                        int perfId = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        DateTime date = reader.GetDateTime(2);

                        Panel perfPanel = new Panel
                        {
                            Size = new Size(550, 150),
                            Location = new Point(10, y),
                            BorderStyle = BorderStyle.FixedSingle
                        };

                        PictureBox poster = new PictureBox
                        {
                            Size = new Size(100, 130),
                            Location = new Point(10, 10),
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Image = GetPosterImageByName(name)
                        };

                        Label lblName = new Label
                        {
                            Text = name,
                            Font = new Font("Segoe UI", 12, FontStyle.Bold),
                            Location = new Point(120, 20),
                            AutoSize = true
                        };

                        Label lblDate = new Label
                        {
                            Text = date.ToString("dd.MM.yyyy HH:mm"),
                            Font = new Font("Segoe UI", 10),
                            Location = new Point(120, 60),
                            AutoSize = true
                        };

                        Button btnDetails = new Button
                        {
                            Text = "Подробнее",
                            Location = new Point(120, 90),
                            Tag = perfId,
                            BackColor = Color.Black,
                            ForeColor = Color.White
                        };
                        btnDetails.Click += BtnDetails_Click;

                        Button btnBuyy = new Button
                        {
                            Text = "Купить билет",
                            Location = new Point(240, 90),
                            Tag = new { Id = perfId, Name = name, Date = date },
                            BackColor = Color.DarkGreen,
                            ForeColor = Color.White
                        };
                        btnBuyy.Click += BtnBuyy_Click;

                        perfPanel.Controls.Add(poster);
                        perfPanel.Controls.Add(lblName);
                        perfPanel.Controls.Add(lblDate);
                        perfPanel.Controls.Add(btnDetails);
                        perfPanel.Controls.Add(btnBuyy);

                        panel1.Controls.Add(perfPanel);

                        y += perfPanel.Height + 10;
                    }
                }
            }
        }

        private void BtnBuyy_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            dynamic tag = btn.Tag;

            int perfId = tag.Id;
            string perfName = tag.Name;
            DateTime perfDate = tag.Date;

            // Тут можно подставить реальное название зала из базы, пока заглушка:
            buyy buyForm = new buyy(perfId, perfName, perfDate, "Главный зал");
            buyForm.ShowDialog();
        }

        private void BtnDetails_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int performanceId = (int)btn.Tag;

            string actorInfo = GetActorsForPerformance(performanceId);
            MessageBox.Show(actorInfo, "Актёры");
        }

        private string GetActorsForPerformance(int performanceId)
        {
            string result = "Актёры:\n";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT full_name, role_description FROM theater.actors WHERE performance_id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", performanceId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fullName = reader.GetString(0);
                            string role = reader.IsDBNull(1) ? "Без роли" : reader.GetString(1);
                            result += $"- {fullName} — {role}\n";
                        }
                    }
                }
            }

            return result;
        }

        private Image GetPosterImageByName(string performanceName)
        {
            switch (performanceName)
            {
                case "Гамлет":
                    return Properties.Resources.гамлет;
                case "Ревизор":
                    return Properties.Resources.ревизор1;
                case "Мастер и Маргарита":
                    return Properties.Resources.мастер;
                default:
                    return Properties.Resources.ревизор;
            }
        }

        private void glava_Load_1(object sender, EventArgs e)
        {

        }
    }
}
