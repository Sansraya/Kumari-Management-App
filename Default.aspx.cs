using System;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class Default : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadStats();
        }

        private void LoadStats()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();

                
                Exec(conn, "SELECT COUNT(*) FROM CUSTOMER", v => lblCustomers.Text = v);
                Exec(conn, "SELECT COUNT(*) FROM THEATRE", v => lblTheatres.Text = v);
                Exec(conn, "SELECT COUNT(*) FROM MOVIE", v => lblMovies.Text = v);
                Exec(conn, "SELECT COUNT(*) FROM SHOW", v => lblShows.Text = v);
                Exec(conn, "SELECT COUNT(*) FROM TICKET", v => lblTickets.Text = v);
                Exec(conn, "SELECT COUNT(*) FROM HALL", v => lblHalls.Text = v);
                Exec(conn, "SELECT NVL(SUM(HALL_CAPACITY),0) FROM HALL",
                                                             v => lblTotalCapacity.Text = v);
                Exec(conn, "SELECT COUNT(*) FROM TICKET WHERE TICKET_STATUS='ACTIVE'",
                                                             v => lblActiveTickets.Text = v);

                
                Exec(conn, "SELECT COUNT(*) FROM TICKET WHERE TICKET_STATUS='ACTIVE'",
                                                             v => lblActiveTickets2.Text = v);
                Exec(conn, "SELECT COUNT(*) FROM TICKET WHERE TICKET_STATUS='USED'",
                                                             v => lblUsedTickets.Text = v);
                Exec(conn, "SELECT COUNT(*) FROM TICKET WHERE TICKET_STATUS='CANCELLED'",
                                                             v => lblCancelledTickets.Text = v);

                
                try
                {
                    using (var cmd = new OracleCommand(
                        "SELECT HALL_NAME, HALL_CAPACITY FROM HALL ORDER BY HALL_CAPACITY DESC", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var sb = new System.Text.StringBuilder("[");
                        bool first = true;
                        while (reader.Read())
                        {
                            if (!first) sb.Append(",");
                            string name = reader["HALL_NAME"].ToString()
                                              .Replace("\\", "\\\\").Replace("\"", "\\\"");
                            sb.Append($"{{\"name\":\"{name}\",\"cap\":{reader["HALL_CAPACITY"]}}}");
                            first = false;
                        }
                        sb.Append("]");
                        lblHallsJson.Text = sb.ToString();
                    }
                }
                catch { lblHallsJson.Text = "[]"; }
            }
        }

        private void Exec(OracleConnection conn, string sql, Action<string> setter)
        {
            try
            {
                using (var cmd = new OracleCommand(sql, conn))
                    setter(cmd.ExecuteScalar()?.ToString() ?? "0");
            }
            catch { setter("—"); }
        }
    }
}