using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class TheatreMovie : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadTheatreDropdown();
        }

        private void LoadTheatreDropdown()
        {
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(
                "SELECT THEATRE_ID, THEATRE_NAME || ' (' || THEATRE_LOCATION || ')' AS DISPLAY " +
                "FROM THEATRE ORDER BY THEATRE_NAME", conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                ddlTheatre.DataSource = dt;
                ddlTheatre.DataTextField = "DISPLAY";
                ddlTheatre.DataValueField = "THEATRE_ID";
                ddlTheatre.DataBind();
                ddlTheatre.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Theatre --", ""));
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlTheatre.SelectedValue)) return;
            decimal theatreId = decimal.Parse(ddlTheatre.SelectedValue);
            LoadTheatreInfo(theatreId);
            LoadMovieShowtimes(theatreId);
        }

        private void LoadTheatreInfo(decimal theatreId)
        {
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(
                "SELECT THEATRE_ID, THEATRE_NAME, THEATRE_LOCATION FROM THEATRE WHERE THEATRE_ID=:TID", conn))
            {
                cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = theatreId;
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        lblTheatreName.Text = r["THEATRE_NAME"].ToString();
                        lblTheatreLocation.Text = r["THEATRE_LOCATION"].ToString();
                        lblTheatreId.Text = r["THEATRE_ID"].ToString();
                        pnlTheatreInfo.Visible = true;
                    }
                }
            }
        }

        // ── Verified correct: all dates formatted in SQL with TO_CHAR so
        //    ASP.NET DataFormatString is not needed (avoids double-format errors).
        //    TOTAL_CUSTOMERS counts distinct customers per show row.
        private void LoadMovieShowtimes(decimal theatreId)
        {
            const string sql = @"
                SELECT
                    M.MOVIE_TITLE,
                    M.GENRE,
                    M.MOVIE_LANGUAGE,
                    M.MOVIE_DURATION,
                    TO_CHAR(M.RELEASE_DATE, 'DD/MM/YYYY')        AS RELEASE_DATE,
                    H.HALL_NAME,
                    H.HALL_CAPACITY,
                    TO_CHAR(S.SHOW_TIME,   'DD/MM/YYYY HH24:MI') AS SHOW_TIME_FMT,
                    TO_CHAR(S.SHOWED_DATE, 'DD/MM/YYYY')         AS SHOWED_DATE_FMT,
                    COUNT(DISTINCT CHS.CUSTOMER_ID)              AS TOTAL_CUSTOMERS
                FROM CUSTOMER_HALL_SHOW CHS
                JOIN MOVIE   M  ON M.MOVIE_ID    = CHS.MOVIE_ID
                JOIN THEATRE TH ON TH.THEATRE_ID = CHS.THEATRE_ID
                JOIN HALL    H  ON H.HALL_ID     = CHS.HALL_ID
                JOIN SHOW    S  ON S.SHOW_ID     = CHS.SHOW_ID
                WHERE CHS.THEATRE_ID = :TID
                GROUP BY
                    M.MOVIE_TITLE, M.GENRE, M.MOVIE_LANGUAGE,
                    M.MOVIE_DURATION, M.RELEASE_DATE,
                    H.HALL_NAME, H.HALL_CAPACITY,
                    S.SHOW_TIME, S.SHOWED_DATE
                ORDER BY S.SHOWED_DATE DESC, M.MOVIE_TITLE";

            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = theatreId;
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvMovies.DataSource = dt;
                gvMovies.DataBind();
                lblCount.Text = $"{dt.Rows.Count} showing(s) found";

                var uniqueMovies = new System.Collections.Generic.HashSet<string>();
                foreach (DataRow row in dt.Rows)
                    uniqueMovies.Add(row["MOVIE_TITLE"].ToString());

                lblUniqueMovies.Text = uniqueMovies.Count.ToString();
                lblTotalShowings.Text = dt.Rows.Count.ToString();
                pnlResults.Visible = true;
            }
        }
    }
}