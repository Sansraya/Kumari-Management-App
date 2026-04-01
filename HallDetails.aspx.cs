using System;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class HallDetails : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadHalls();
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
            }
        }

        

        private void LoadHalls()
        {
            const string sql = "SELECT HALL_ID, HALL_NAME, HALL_CAPACITY FROM HALL ORDER BY HALL_ID";
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvHalls.DataSource = dt;
                gvHalls.DataBind();
                lblCount.Text = $"{dt.Rows.Count} record(s)";
            }
        }

        private void LoadStats()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand("SELECT COUNT(*) FROM HALL", conn))
                    lblTotalHalls.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand("SELECT NVL(SUM(HALL_CAPACITY),0) FROM HALL", conn))
                    lblTotalCapacity.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand("SELECT ROUND(AVG(HALL_CAPACITY),0) FROM HALL", conn))
                    lblAvgCapacity.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                decimal hallId = decimal.Parse(txtHallId.Text.Trim());
                decimal customerId = decimal.Parse(ddlCustomer.SelectedValue);
                decimal movieId = decimal.Parse(ddlMovie.SelectedValue);
                decimal theatreId = decimal.Parse(ddlTheatre.SelectedValue);

                using (var conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    
                    using (var cmd = new OracleCommand(
                        "INSERT INTO HALL (HALL_ID, HALL_NAME, HALL_CAPACITY) VALUES (:HID, :HNAME, :HCAP)", conn))
                    {
                        cmd.Parameters.Add(":HID", OracleDbType.Decimal).Value = hallId;
                        cmd.Parameters.Add(":HNAME", OracleDbType.Varchar2).Value = txtHallName.Text.Trim();
                        cmd.Parameters.Add(":HCAP", OracleDbType.Decimal).Value =
                            string.IsNullOrWhiteSpace(txtHallCapacity.Text)
                                ? (object)DBNull.Value
                                : decimal.Parse(txtHallCapacity.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }

                    
                    using (var cmd = new OracleCommand(
                        "INSERT INTO CUST_MOV_THT_HA (CUSTOMERID, MOVIE_ID, THEATRE_ID, HALL_ID) " +
                        "VALUES (:CID, :MID, :TID, :HID)", conn))
                    {
                        cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = customerId;
                        cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = movieId;
                        cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = theatreId;
                        cmd.Parameters.Add(":HID", OracleDbType.Decimal).Value = hallId;
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMessage("&#10004; Hall added and linked successfully.", true);
                ClearForm();
                LoadHalls();
                LoadStats();
                PopulateDropdowns();
            }
            catch (OracleException ex) when (ex.Number == 1)
            { ShowMessage("&#10008; A hall with that ID already exists.", false); }
            catch (Exception ex)
            { ShowMessage($"&#10008; Error: {ex.Message}", false); }
        }

        

        protected void gvHalls_RowEditing(object sender, GridViewEditEventArgs e)
        {
            string id = gvHalls.DataKeys[e.NewEditIndex].Value.ToString();
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand("SELECT * FROM HALL WHERE HALL_ID=:HID", conn))
            {
                cmd.Parameters.Add(":HID", OracleDbType.Decimal).Value = decimal.Parse(id);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        hfEditId.Value = r["HALL_ID"].ToString();
                        lblEditId.Text = $"(ID: {r["HALL_ID"]})";
                        txtEditName.Text = r["HALL_NAME"].ToString();
                        txtEditCapacity.Text = r["HALL_CAPACITY"].ToString();
                    }
                }
            }
            pnlEdit.Visible = true;
            LoadHalls();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "UPDATE HALL SET HALL_NAME=:HNAME, HALL_CAPACITY=:HCAP WHERE HALL_ID=:HID", conn))
                {
                    cmd.Parameters.Add(":HNAME", OracleDbType.Varchar2).Value = txtEditName.Text.Trim();
                    cmd.Parameters.Add(":HCAP", OracleDbType.Decimal).Value =
                        string.IsNullOrWhiteSpace(txtEditCapacity.Text)
                            ? (object)DBNull.Value
                            : decimal.Parse(txtEditCapacity.Text.Trim());
                    cmd.Parameters.Add(":HID", OracleDbType.Decimal).Value = decimal.Parse(hfEditId.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                pnlEdit.Visible = false;
                ShowMessage("&#10004; Hall updated successfully.", true);
                LoadHalls();
                LoadStats();
            }
            catch (Exception ex)
            { ShowMessage($"&#10008; Update failed: {ex.Message}", false); }
        }

        

        protected void gvHalls_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string id = gvHalls.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    
                    using (var cmd = new OracleCommand(
                        "DELETE FROM CUST_MOV_THT_HA WHERE HALL_ID=:HID", conn))
                    {
                        cmd.Parameters.Add(":HID", OracleDbType.Decimal).Value = decimal.Parse(id);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new OracleCommand("DELETE FROM HALL WHERE HALL_ID=:HID", conn))
                    {
                        cmd.Parameters.Add(":HID", OracleDbType.Decimal).Value = decimal.Parse(id);
                        cmd.ExecuteNonQuery();
                    }
                }
                ShowMessage("&#10004; Hall deleted.", true);
                LoadHalls();
                LoadStats();
            }
            catch (OracleException ex) when (ex.Number == 2292)
            { ShowMessage("&#10008; Cannot delete: hall has other linked records.", false); }
            catch (Exception ex)
            { ShowMessage($"&#10008; Error: {ex.Message}", false); }
        }

        

        protected void gvHalls_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) => LoadHalls();
        protected void btnCancelEdit_Click(object sender, EventArgs e) { pnlEdit.Visible = false; LoadHalls(); }
        protected void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtHallId.Text = txtHallName.Text = txtHallCapacity.Text = "";
            ddlCustomer.SelectedIndex = 0;
            ddlMovie.SelectedIndex = 0;
            ddlTheatre.SelectedIndex = 0;
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