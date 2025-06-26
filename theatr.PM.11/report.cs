using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;


namespace theatr.PM._11
{
    public partial class report: Form
    {
        private string connectionString = "Host=172.20.7.53;Port=5432;Username=st3996;Password=pwd3996;Database=db3996_10";
        public report()
        {
            InitializeComponent();
        }

        private void report_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.DataSource = null;

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            DateTime dateStart = guna2DateTimePicker1.Value.Date;
            DateTime dateEnd = guna2DateTimePicker2.Value.Date.AddDays(1).AddTicks(-1); // конец дня

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT
                        t.id,
                        u.username,
                        p.name AS performance,
                        h.name AS hall,
                        s.seat_number,
                        t.total_price,
                        t.booked_at,
                        st.name AS status
                    FROM theater.tickets t
                    JOIN theater.users u ON t.user_id = u.id
                    JOIN theater.performances p ON t.performance_id = p.id
                    JOIN theater.seats s ON t.seat_id = s.id
                    JOIN theater.halls h ON s.hall_id = h.id
                    JOIN theater.status st ON t.status_id = st.id
                    WHERE t.booked_at BETWEEN @start AND @end
                    ORDER BY t.booked_at";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("start", dateStart);
                    cmd.Parameters.AddWithValue("end", dateEnd);

                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        guna2DataGridView1.DataSource = dt;
                    }
                }
            }
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            admin admin = new admin();
            admin.Show();
            this.Close();
        }
    }
}
