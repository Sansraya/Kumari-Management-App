using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class TheaterDetails : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { LoadTheaters(); LoadStats(); }
        }

        private void LoadTheaters()
        {
            const string sql = "SELECT THEATRE_ID, THEATRE_NAME, THEATRE_LOCATION FROM THEATRE ORDER BY THEATRE_ID";
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvTheaters.DataSource = dt;
                gvTheaters.DataBind();
                lblCount.Text = $"{dt.Rows.Count} record(s)";
            }
        }

        private void LoadStats()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand("SELECT COUNT(*) FROM THEATRE", conn))
                    lblTotalTheaters.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand("SELECT COUNT(DISTINCT THEATRE_LOCATION) FROM THEATRE", conn))
                    lblCities.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "INSERT INTO THEATRE (THEATRE_ID, THEATRE_NAME, THEATRE_LOCATION) VALUES (:TID, :TNAME, :TLOC)", conn))
                {
                    cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = decimal.Parse(txtTheaterId.Text.Trim());
                    cmd.Parameters.Add(":TNAME", OracleDbType.Varchar2).Value = txtTheaterName.Text.Trim();
                    cmd.Parameters.Add(":TLOC", OracleDbType.Varchar2).Value = txtLocation.Text.Trim();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowMessage("✔ Theater added successfully.", true);
                ClearForm();
                LoadTheaters(); LoadStats();
            }
            catch (OracleException ex) when (ex.Number == 1)
            { ShowMessage("✘ A theater with that ID already exists.", false); }
            catch (Exception ex)
            { ShowMessage($"✘ Error: {ex.Message}", false); }
        }

        protected void gvTheaters_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            string id = gvTheaters.DataKeys[e.NewEditIndex].Value.ToString();
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand("SELECT * FROM THEATRE WHERE THEATRE_ID = :TID", conn))
            {
                cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = decimal.Parse(id);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        hfEditId.Value = r["THEATRE_ID"].ToString();
                        lblEditId.Text = $"(ID: {r["THEATRE_ID"]})";
                        txtEditName.Text = r["THEATRE_NAME"].ToString();
                        txtEditLocation.Text = r["THEATRE_LOCATION"].ToString();
                    }
                }
            }
            pnlEdit.Visible = true;
            LoadTheaters();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "UPDATE THEATRE SET THEATRE_NAME=:TNAME, THEATRE_LOCATION=:TLOC WHERE THEATRE_ID=:TID", conn))
                {
                    cmd.Parameters.Add(":TNAME", OracleDbType.Varchar2).Value = txtEditName.Text.Trim();
                    cmd.Parameters.Add(":TLOC", OracleDbType.Varchar2).Value = txtEditLocation.Text.Trim();
                    cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = decimal.Parse(hfEditId.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                pnlEdit.Visible = false;
                ShowMessage("✔ Theater updated successfully.", true);
                LoadTheaters(); LoadStats();
            }
            catch (Exception ex) { ShowMessage($"✘ Update failed: {ex.Message}", false); }
        }

        protected void gvTheaters_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            string id = gvTheaters.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand("DELETE FROM THEATRE WHERE THEATRE_ID = :TID", conn))
                {
                    cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = decimal.Parse(id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowMessage("✔ Theater deleted.", true);
                LoadTheaters(); LoadStats();
            }
            catch (OracleException ex) when (ex.Number == 2292)
            { ShowMessage("✘ Cannot delete: theater has linked records.", false); }
            catch (Exception ex) { ShowMessage($"✘ Error: {ex.Message}", false); }
        }

        protected void gvTheaters_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e) => LoadTheaters();
        protected void btnCancelEdit_Click(object sender, EventArgs e) { pnlEdit.Visible = false; LoadTheaters(); }
        protected void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtTheaterId.Text = txtTheaterName.Text = txtLocation.Text = "";
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