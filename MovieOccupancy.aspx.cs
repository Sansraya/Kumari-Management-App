using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class MovieOccupancy : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadMovieDropdown();
        }

        private void LoadMovieDropdown()
        {
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(
                "SELECT MOVIE_ID, MOVIE_TITLE FROM MOVIE ORDER BY MOVIE_TITLE", conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                ddlMovie.DataSource = dt;
                ddlMovie.DataTextField = "MOVIE_TITLE";
                ddlMovie.DataValueField = "MOVIE_ID";
                ddlMovie.DataBind();
                ddlMovie.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Movie --", ""));
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlMovie.SelectedValue)) return;
            decimal movieId = decimal.Parse(ddlMovie.SelectedValue);
            LoadMovieInfo(movieId);
            LoadOccupancy(movieId);
        }

        private void LoadMovieInfo(decimal movieId)
        {
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(
                "SELECT MOVIE_ID, MOVIE_TITLE, GENRE, MOVIE_LANGUAGE, MOVIE_DURATION " +
                "FROM MOVIE WHERE MOVIE_ID=:MID", conn))
            {
                cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = movieId;
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        lblMovieTitle.Text = r["MOVIE_TITLE"].ToString();
                        lblMovieGenre.Text = r["GENRE"].ToString();
                        lblMovieLang.Text = r["MOVIE_LANGUAGE"].ToString();
                        lblMovieDuration.Text = r["MOVIE_DURATION"].ToString() + " min";
                        pnlMovieInfo.Visible = true;
                    }
                }
            }
        }

        private void LoadOccupancy(decimal movieId)
        {
            const string sql = @"
                SELECT * FROM (
                    SELECT
                        TH.THEATRE_NAME,
                        TH.THEATRE_LOCATION,
                        H.HALL_NAME,
                        H.HALL_CAPACITY,
                        SUM(T.TOTALNUMBEROFPEOPLE)        AS ACTIVE_TICKET_COUNT,
                        COUNT(DISTINCT CST.SHOW_ID)       AS TOTAL_SHOWS,
                        ROUND(
                            (SUM(T.TOTALNUMBEROFPEOPLE)
                             / NULLIF(H.HALL_CAPACITY, 0)) * 100
                        , 2)                              AS OCCUPANCY_PCT
                    FROM CUSTOMER_SHOW_TICKET CST
                    JOIN TICKET  T  ON T.TICKET_ID   = CST.TICKET_ID
                    JOIN THEATRE TH ON TH.THEATRE_ID = CST.THEATRE_ID
                    JOIN HALL    H  ON H.HALL_ID     = CST.HALL_ID
                    JOIN SHOW    S  ON S.SHOW_ID     = CST.SHOW_ID
                    WHERE CST.MOVIE_ID       = :MID
                      AND T.TICKET_STATUS    = 'ACTIVE'
                    GROUP BY
                        TH.THEATRE_NAME, TH.THEATRE_LOCATION,
                        H.HALL_NAME, H.HALL_CAPACITY
                    ORDER BY OCCUPANCY_PCT DESC
                )
                WHERE ROWNUM <= 3";

            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = movieId;
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvOccupancy.DataSource = dt;
                gvOccupancy.DataBind();
                lblCount.Text = $"Top {dt.Rows.Count} theatre/hall(s) by active-ticket occupancy";
                pnlResults.Visible = true;
            }
        }
    }
}