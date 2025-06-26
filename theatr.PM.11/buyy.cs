using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace theatr.PM._11
{
    public partial class buyy : Form
    {
        private int perfId;
        private string perfName;
        private DateTime perfDate;
        private string hallName;

        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_10";

        private HashSet<int> selectedSeats = new HashSet<int>();
        private Dictionary<int, int> seatPrices = new Dictionary<int, int>(); // seat_number -> price

        public buyy(int performanceId, string performanceName, DateTime performanceDate, string hallName)
        {
            InitializeComponent();

            this.perfId = performanceId;
            this.perfName = performanceName;
            this.perfDate = performanceDate;
            this.hallName = hallName;

            label11.Text = "Спектакль: " + perfName;
            label13.Text = "Дата и время: " + perfDate.ToString("dd.MM.yyyy HH:mm");
            label12.Text = "Зал: " + hallName;

            LoadSeatsFromDatabase();
        }

        private void buyy_Load(object sender, EventArgs e)
        {
            guna2TextBox1.ReadOnly = true;
        }

        private void LoadSeatsFromDatabase()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT s.seat_number, s.price, s.is_booked
                    FROM theater.seats s
                    JOIN theater.performances p ON s.hall_id = p.hall_id
                    WHERE p.id = @perfId
                    ORDER BY s.seat_number;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("perfId", perfId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int seatNumber = reader.GetInt32(0);
                            int price = (int)reader.GetDecimal(1);
                            bool isBooked = reader.GetBoolean(2);

                            seatPrices[seatNumber] = price;

                            Button seatButton = Controls.Find("button" + seatNumber, true).FirstOrDefault() as Button;
                            if (seatButton != null)
                            {
                                seatButton.Text = $"{seatNumber}\n{price}₽";
                                seatButton.BackColor = isBooked ? Color.Red : Color.Black;
                                seatButton.ForeColor = Color.White;
                                seatButton.Enabled = !isBooked;

                                seatButton.Click -= SeatButton_Click;
                                seatButton.Click += SeatButton_Click;
                            }
                        }
                    }
                }
            }
        }

        private void SeatButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int seatNumber = int.Parse(btn.Name.Replace("button", ""));

            if (!btn.Enabled || btn.BackColor == Color.Red)
            {
                MessageBox.Show("Это место уже занято.");
                return;
            }

            if (selectedSeats.Contains(seatNumber))
            {
                selectedSeats.Remove(seatNumber);
                btn.BackColor = Color.Black;
            }
            else
            {
                selectedSeats.Add(seatNumber);
                btn.BackColor = Color.Green;
            }

            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            int total = selectedSeats.Sum(seat => seatPrices.ContainsKey(seat) ? seatPrices[seat] : 0);
            guna2TextBox1.Text = total.ToString() + " руб.";
        }

        private void button38_Click(object sender, EventArgs e)
        {
            if (selectedSeats.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одно место.");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (int seatNumber in selectedSeats)
                        {
                            string seatQuery = @"
                                SELECT id FROM theater.seats 
                                WHERE seat_number = @seat_number AND hall_id = (
                                    SELECT hall_id FROM theater.performances WHERE id = @perfId
                                );";

                            int seatId;
                            using (var cmdSeat = new NpgsqlCommand(seatQuery, conn, tran))
                            {
                                cmdSeat.Parameters.AddWithValue("seat_number", seatNumber);
                                cmdSeat.Parameters.AddWithValue("perfId", perfId);
                                object result = cmdSeat.ExecuteScalar();
                                if (result == null)
                                {
                                    MessageBox.Show($"Ошибка: место {seatNumber} не найдено.");
                                    tran.Rollback();
                                    return;
                                }
                                seatId = Convert.ToInt32(result);
                            }

                            string insertTicket = "INSERT INTO theater.tickets (performance_id, seat_id) VALUES (@perfId, @seatId)";
                            using (var cmdInsert = new NpgsqlCommand(insertTicket, conn, tran))
                            {
                                cmdInsert.Parameters.AddWithValue("perfId", perfId);
                                cmdInsert.Parameters.AddWithValue("seatId", seatId);
                                cmdInsert.ExecuteNonQuery();
                            }

                            string updateSeat = "UPDATE theater.seats SET is_booked = true WHERE id = @seatId";
                            using (var cmdUpdate = new NpgsqlCommand(updateSeat, conn, tran))
                            {
                                cmdUpdate.Parameters.AddWithValue("seatId", seatId);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        MessageBox.Show("Покупка успешна!");
                        LoadSeatsFromDatabase();
                        selectedSeats.Clear();
                        UpdateTotalPrice();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Ошибка покупки: " + ex.Message);
                    }
                }
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            glava glava = new glava();
            glava.Show();
            this.Close();
        }
    }
}