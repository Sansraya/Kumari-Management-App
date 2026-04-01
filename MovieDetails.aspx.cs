using System;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class MovieDetails : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { LoadMovies(); LoadStats(); }
        }

        private void LoadMovies()
        {
            const string sql = "SELECT MOVIE_ID, MOVIE_TITLE, GENRE, MOVIE_LANGUAGE, MOVIE_DURATION, RELEASE_DATE FROM MOVIE ORDER BY MOVIE_ID";
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvMovies.DataSource = dt;
                gvMovies.DataBind();
                lblCount.Text = $"{dt.Rows.Count} record(s)";
            }
        }

        private void LoadStats()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand("SELECT COUNT(*) FROM MOVIE", conn))
                    lblTotalMovies.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand("SELECT COUNT(DISTINCT GENRE) FROM MOVIE", conn))
                    lblGenres.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand("SELECT ROUND(AVG(MOVIE_DURATION),0) FROM MOVIE", conn))
                    lblAvgDuration.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "INSERT INTO MOVIE (MOVIE_ID, MOVIE_TITLE, GENRE, MOVIE_LANGUAGE, MOVIE_DURATION, RELEASE_DATE) VALUES (:MID, :TITLE, :GENRE, :LANG, :DUR, :RDATE)", conn))
                {
                    cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = decimal.Parse(txtMovieId.Text.Trim());
                    cmd.Parameters.Add(":TITLE", OracleDbType.Varchar2).Value = txtMovieTitle.Text.Trim();
                    cmd.Parameters.Add(":GENRE", OracleDbType.Varchar2).Value = ddlGenre.SelectedValue;
                    cmd.Parameters.Add(":LANG", OracleDbType.Varchar2).Value = txtLanguage.Text.Trim();
                    cmd.Parameters.Add(":DUR", OracleDbType.Decimal).Value = decimal.Parse(txtDuration.Text.Trim());
                    cmd.Parameters.Add(":RDATE", OracleDbType.Date).Value =
                        string.IsNullOrWhiteSpace(txtReleaseDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtReleaseDate.Text);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowMessage("✔ Movie added successfully.", true);
                ClearForm(); LoadMovies(); LoadStats();
            }
            catch (OracleException ex) when (ex.Number == 1)
            { ShowMessage("✘ A movie with that ID already exists.", false); }
            catch (Exception ex) { ShowMessage($"✘ Error: {ex.Message}", false); }
        }

        protected void gvMovies_RowEditing(object sender, GridViewEditEventArgs e)
        {
            string id = gvMovies.DataKeys[e.NewEditIndex].Value.ToString();
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand("SELECT * FROM MOVIE WHERE MOVIE_ID=:MID", conn))
            {
                cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = decimal.Parse(id);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        hfEditId.Value = r["MOVIE_ID"].ToString();
                        lblEditId.Text = $"(ID: {r["MOVIE_ID"]})";
                        txtEditTitle.Text = r["MOVIE_TITLE"].ToString();
                        txtEditLanguage.Text = r["MOVIE_LANGUAGE"].ToString();
                        txtEditDuration.Text = r["MOVIE_DURATION"].ToString();
                        if (r["RELEASE_DATE"] != DBNull.Value)
                            txtEditReleaseDate.Text = Convert.ToDateTime(r["RELEASE_DATE"]).ToString("yyyy-MM-dd");
                        try { ddlEditGenre.SelectedValue = r["GENRE"].ToString(); } catch { }
                    }
                }
            }
            pnlEdit.Visible = true; LoadMovies();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "UPDATE MOVIE SET MOVIE_TITLE=:TITLE, GENRE=:GENRE, MOVIE_LANGUAGE=:LANG, MOVIE_DURATION=:DUR, RELEASE_DATE=:RDATE WHERE MOVIE_ID=:MID", conn))
                {
                    cmd.Parameters.Add(":TITLE", OracleDbType.Varchar2).Value = txtEditTitle.Text.Trim();
                    cmd.Parameters.Add(":GENRE", OracleDbType.Varchar2).Value = ddlEditGenre.SelectedValue;
                    cmd.Parameters.Add(":LANG", OracleDbType.Varchar2).Value = txtEditLanguage.Text.Trim();
                    cmd.Parameters.Add(":DUR", OracleDbType.Decimal).Value = decimal.Parse(txtEditDuration.Text.Trim());
                    cmd.Parameters.Add(":RDATE", OracleDbType.Date).Value =
                        string.IsNullOrWhiteSpace(txtEditReleaseDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtEditReleaseDate.Text);
                    cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = decimal.Parse(hfEditId.Value);
                    conn.Open(); cmd.ExecuteNonQuery();
                }
                pnlEdit.Visible = false;
                ShowMessage("✔ Movie updated successfully.", true);
                LoadMovies(); LoadStats();
            }
            catch (Exception ex) { ShowMessage($"✘ Update failed: {ex.Message}", false); }
        }

        protected void gvMovies_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string id = gvMovies.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand("DELETE FROM MOVIE WHERE MOVIE_ID=:MID", conn))
                {
                    cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = decimal.Parse(id);
                    conn.Open(); cmd.ExecuteNonQuery();
                }
                ShowMessage("✔ Movie deleted.", true); LoadMovies(); LoadStats();
            }
            catch (OracleException ex) when (ex.Number == 2292)
            { ShowMessage("✘ Cannot delete: movie has linked showtimes.", false); }
            catch (Exception ex) { ShowMessage($"✘ Error: {ex.Message}", false); }
        }

        protected void gvMovies_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) => LoadMovies();
        protected void btnCancelEdit_Click(object sender, EventArgs e) { pnlEdit.Visible = false; LoadMovies(); }
        protected void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtMovieId.Text = txtMovieTitle.Text = txtLanguage.Text = txtDuration.Text = txtReleaseDate.Text = "";
            ddlGenre.SelectedIndex = 0;
        }

        private void ShowMessage(string msg, bool success)
        {
            lblMessage.Text = msg; lblMessage.Visible = true;
            lblMessage.CssClass = success
                ? "block mb-4 text-green-400 font-medium bg-green-900 bg-opacity-20 px-4 py-2 rounded-lg border border-green-800"
                : "block mb-4 text-red-400 font-medium bg-red-900 bg-opacity-20 px-4 py-2 rounded-lg border border-red-800";
        }
    }
}