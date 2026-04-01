using System;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class ShowtimeDetails : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadShows();
                LoadStats();
                PopulateDropdowns();
            }
        }

        

        private void PopulateDropdowns()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();

                
                using (var cmd = new OracleCommand(
                    "SELECT CUSTOMER_ID, CUSTOMER_NAME FROM CUSTOMER ORDER BY CUSTOMER_NAME", conn))
                using (var da = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    ddlCustomer.Items.Clear();
                    ddlCustomer.Items.Add(new ListItem("-- Select Customer --", ""));
                    foreach (DataRow r in dt.Rows)
                        ddlCustomer.Items.Add(new ListItem(
                            $"{r["CUSTOMER_NAME"]} (ID: {r["CUSTOMER_ID"]})",
                            r["CUSTOMER_ID"].ToString()));
                }

                
                using (var cmd = new OracleCommand(
                    "SELECT MOVIE_ID, MOVIE_TITLE FROM MOVIE ORDER BY MOVIE_TITLE", conn))
                using (var da = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    ddlMovie.Items.Clear();
                    ddlMovie.Items.Add(new ListItem("-- Select Movie --", ""));
                    foreach (DataRow r in dt.Rows)
                        ddlMovie.Items.Add(new ListItem(
                            $"{r["MOVIE_TITLE"]} (ID: {r["MOVIE_ID"]})",
                            r["MOVIE_ID"].ToString()));
                }

                
                using (var cmd = new OracleCommand(
                    "SELECT THEATRE_ID, THEATRE_NAME FROM THEATRE ORDER BY THEATRE_NAME", conn))
                using (var da = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    ddlTheatre.Items.Clear();
                    ddlTheatre.Items.Add(new ListItem("-- Select Theatre --", ""));
                    foreach (DataRow r in dt.Rows)
                        ddlTheatre.Items.Add(new ListItem(
                            $"{r["THEATRE_NAME"]} (ID: {r["THEATRE_ID"]})",
                            r["THEATRE_ID"].ToString()));
                }

                
                using (var cmd = new OracleCommand(
                    "SELECT HALL_ID, HALL_NAME FROM HALL ORDER BY HALL_NAME", conn))
                using (var da = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    ddlHall.Items.Clear();
                    ddlHall.Items.Add(new ListItem("-- Select Hall --", ""));
                    foreach (DataRow r in dt.Rows)
                        ddlHall.Items.Add(new ListItem(
                            $"{r["HALL_NAME"]} (ID: {r["HALL_ID"]})",
                            r["HALL_ID"].ToString()));
                }
            }
        }

        

        private void LoadShows()
        {
            const string sql = "SELECT SHOW_ID, SHOW_TIME, SHOWED_DATE FROM SHOW ORDER BY SHOW_ID";
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvShows.DataSource = dt;
                gvShows.DataBind();
                lblCount.Text = $"{dt.Rows.Count} record(s)";
            }
        }

        private void LoadStats()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand("SELECT COUNT(*) FROM SHOW", conn))
                    lblTotalShows.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand(
                    "SELECT COUNT(*) FROM SHOW WHERE TRUNC(SHOWED_DATE) = TRUNC(SYSDATE)", conn))
                    lblTodayShows.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand(
                    "SELECT COUNT(*) FROM SHOW WHERE SHOWED_DATE BETWEEN SYSDATE AND SYSDATE+7", conn))
                    lblUpcoming.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                decimal showId = decimal.Parse(txtShowId.Text.Trim());
                decimal customerId = decimal.Parse(ddlCustomer.SelectedValue);
                decimal movieId = decimal.Parse(ddlMovie.SelectedValue);
                decimal theatreId = decimal.Parse(ddlTheatre.SelectedValue);
                decimal hallId = decimal.Parse(ddlHall.SelectedValue);

                using (var conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    
                    using (var cmd = new OracleCommand(
                        "INSERT INTO SHOW (SHOW_ID, SHOW_TIME, SHOWED_DATE) VALUES (:SID, :STIME, :SDATE)", conn))
                    {
                        cmd.Parameters.Add(":SID", OracleDbType.Decimal).Value = showId;
                        cmd.Parameters.Add(":STIME", OracleDbType.Date).Value = DateTime.Parse(txtShowTime.Text);
                        cmd.Parameters.Add(":SDATE", OracleDbType.Date).Value = DateTime.Parse(txtShowedDate.Text);
                        cmd.ExecuteNonQuery();
                    }

                    
                    using (var cmd = new OracleCommand(
                        "INSERT INTO CUSTOMER_HALL_SHOW (CUSTOMER_ID, MOVIE_ID, HALL_ID, THEATRE_ID, SHOW_ID) " +
                        "VALUES (:CID, :MID, :HALID, :TID, :SID)", conn))
                    {
                        cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = customerId;
                        cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = movieId;
                        cmd.Parameters.Add(":HALID", OracleDbType.Decimal).Value = hallId;
                        cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = theatreId;
                        cmd.Parameters.Add(":SID", OracleDbType.Decimal).Value = showId;
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMessage("&#10004; Show added and linked successfully.", true);
                ClearForm();
                LoadShows();
                LoadStats();
                PopulateDropdowns();
            }
            catch (OracleException ex) when (ex.Number == 1)
            { ShowMessage("&#10008; A show with that ID already exists.", false); }
            catch (Exception ex)
            { ShowMessage($"&#10008; Error: {ex.Message}", false); }
        }

        

        protected void gvShows_RowEditing(object sender, GridViewEditEventArgs e)
        {
            string id = gvShows.DataKeys[e.NewEditIndex].Value.ToString();
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand("SELECT * FROM SHOW WHERE SHOW_ID=:SID", conn))
            {
                cmd.Parameters.Add(":SID", OracleDbType.Decimal).Value = decimal.Parse(id);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        hfEditId.Value = r["SHOW_ID"].ToString();
                        lblEditId.Text = $"(ID: {r["SHOW_ID"]})";
                        if (r["SHOW_TIME"] != DBNull.Value)
                            txtEditShowTime.Text = Convert.ToDateTime(r["SHOW_TIME"]).ToString("yyyy-MM-ddTHH:mm");
                        if (r["SHOWED_DATE"] != DBNull.Value)
                            txtEditShowedDate.Text = Convert.ToDateTime(r["SHOWED_DATE"]).ToString("yyyy-MM-dd");
                    }
                }
            }
            pnlEdit.Visible = true;
            LoadShows();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "UPDATE SHOW SET SHOW_TIME=:STIME, SHOWED_DATE=:SDATE WHERE SHOW_ID=:SID", conn))
                {
                    cmd.Parameters.Add(":STIME", OracleDbType.Date).Value = DateTime.Parse(txtEditShowTime.Text);
                    cmd.Parameters.Add(":SDATE", OracleDbType.Date).Value = DateTime.Parse(txtEditShowedDate.Text);
                    cmd.Parameters.Add(":SID", OracleDbType.Decimal).Value = decimal.Parse(hfEditId.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                pnlEdit.Visible = false;
                ShowMessage("&#10004; Show updated successfully.", true);
                LoadShows();
                LoadStats();
            }
            catch (Exception ex)
            { ShowMessage($"&#10008; Update failed: {ex.Message}", false); }
        }

        

        protected void gvShows_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string id = gvShows.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    // Remove bridging rows first
                    using (var cmd = new OracleCommand(
                        "DELETE FROM CUSTOMER_HALL_SHOW WHERE SHOW_ID=:SID", conn))
                    {
                        cmd.Parameters.Add(":SID", OracleDbType.Decimal).Value = decimal.Parse(id);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new OracleCommand("DELETE FROM SHOW WHERE SHOW_ID=:SID", conn))
                    {
                        cmd.Parameters.Add(":SID", OracleDbType.Decimal).Value = decimal.Parse(id);
                        cmd.ExecuteNonQuery();
                    }
                }
                ShowMessage("&#10004; Show deleted.", true);
                LoadShows();
                LoadStats();
            }
            catch (OracleException ex) when (ex.Number == 2292)
            { ShowMessage("&#10008; Cannot delete: show has other linked records.", false); }
            catch (Exception ex)
            { ShowMessage($"&#10008; Error: {ex.Message}", false); }
        }

        

        protected void gvShows_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) => LoadShows();
        protected void btnCancelEdit_Click(object sender, EventArgs e) { pnlEdit.Visible = false; LoadShows(); }
        protected void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtShowId.Text = txtShowTime.Text = txtShowedDate.Text = "";
            ddlCustomer.SelectedIndex = 0;
            ddlMovie.SelectedIndex = 0;
            ddlTheatre.SelectedIndex = 0;
            ddlHall.SelectedIndex = 0;
        }

        private void ShowMessage(string msg, bool success)
        {
            lblMessage.Text = msg;
            lblMessage.Visible = true;
            lblMessage.CssClass = success
                ? "block mb-4 text-green-400 font-medium bg-green-900 bg-opacity-20 px-4 py-2 rounded-lg border border-green-800"
                : "block mb-4 text-red-400 font-medium bg-red-900 bg-opacity-20 px-4 py-2 rounded-lg border border-red-800";
        }
    }
}