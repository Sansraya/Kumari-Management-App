using System;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class TicketDetails : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTickets();
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

                
                const string showSql =
                    @"SELECT DISTINCT
                        s.SHOW_ID,
                        s.SHOWED_DATE,
                        s.SHOW_TIME,
                        chs.MOVIE_ID,
                        m.MOVIE_TITLE,
                        chs.THEATRE_ID,
                        t.THEATRE_NAME,
                        chs.HALL_ID,
                        h.HALL_NAME
                      FROM SHOW s
                      JOIN CUSTOMER_HALL_SHOW chs ON chs.SHOW_ID   = s.SHOW_ID
                      JOIN MOVIE             m   ON m.MOVIE_ID     = chs.MOVIE_ID
                      JOIN THEATRE           t   ON t.THEATRE_ID   = chs.THEATRE_ID
                      JOIN HALL              h   ON h.HALL_ID      = chs.HALL_ID
                      ORDER BY s.SHOW_ID";

                using (var cmd = new OracleCommand(showSql, conn))
                using (var da = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    ddlShow.Items.Clear();
                    ddlShow.Items.Add(new ListItem("-- Select Show --", ""));
                    foreach (DataRow r in dt.Rows)
                    {
                        string displayDate = Convert.ToDateTime(r["SHOWED_DATE"]).ToString("dd/MM/yyyy");
                        string label = $"Show #{r["SHOW_ID"]}  |  {displayDate}  |  {r["MOVIE_TITLE"]}  |  {r["THEATRE_NAME"]}  |  {r["HALL_NAME"]}";
                        // Value encodes all IDs needed for CUSTOMER_SHOW_TICKET
                        string value = $"{r["SHOW_ID"]}|{r["MOVIE_ID"]}|{r["THEATRE_ID"]}|{r["HALL_ID"]}";
                        ddlShow.Items.Add(new ListItem(label, value));
                    }
                }
            }
        }

        

        private void LoadTickets()
        {
            
            const string sql =
                "SELECT t.TICKET_ID, t.TICKET_PRICE, t.SEAT_NUMBER, t.TICKET_STATUS, " +
                "t.TOTALNUMBEROFPEOPLE, " +
                "c.CUSTOMER_NAME, " +
                "cst.SHOW_ID, " +
                "TO_CHAR(s.SHOWED_DATE,'DD/MM/YYYY') AS SHOWED_DATE, " +
                "TO_CHAR(s.SHOW_TIME,'HH24:MI') AS SHOW_TIME_FMT, " +
                "m.MOVIE_TITLE, th.THEATRE_NAME, h.HALL_NAME " +
                "FROM TICKET t " +
                "LEFT JOIN CUSTOMER_SHOW_TICKET cst ON cst.TICKET_ID  = t.TICKET_ID " +
                "LEFT JOIN CUSTOMER             c   ON c.CUSTOMER_ID  = cst.CUSTOMER_ID " +
                "LEFT JOIN SHOW                 s   ON s.SHOW_ID      = cst.SHOW_ID " +
                "LEFT JOIN MOVIE                m   ON m.MOVIE_ID     = cst.MOVIE_ID " +
                "LEFT JOIN THEATRE              th  ON th.THEATRE_ID  = cst.THEATRE_ID " +
                "LEFT JOIN HALL                 h   ON h.HALL_ID      = cst.HALL_ID " +
                "ORDER BY t.TICKET_ID";

            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvTickets.DataSource = dt;
                gvTickets.DataBind();
                lblCount.Text = $"{dt.Rows.Count} ticket(s)";
            }
        }

        private void LoadStats()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand("SELECT COUNT(*) FROM TICKET", conn))
                    lblTotalTickets.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand("SELECT NVL(SUM(TOTALNUMBEROFPEOPLE),0) FROM TICKET", conn))
                    lblTotalPeople.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                using (var cmd = new OracleCommand("SELECT NVL(SUM(TICKET_PRICE),0) FROM TICKET", conn))
                {
                    var val = cmd.ExecuteScalar();
                    lblRevenue.Text = val != null ? $"{Convert.ToDecimal(val):N0}" : "0";
                }
            }
        }

        

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                decimal ticketId = decimal.Parse(txtTicketId.Text.Trim());
                decimal customerId = decimal.Parse(ddlCustomer.SelectedValue);

                
                string[] parts = ddlShow.SelectedValue.Split('|');
                decimal showId = decimal.Parse(parts[0]);
                decimal movieId = decimal.Parse(parts[1]);
                decimal theatreId = decimal.Parse(parts[2]);
                decimal hallId = decimal.Parse(parts[3]);

                using (var conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    
                    using (var cmd = new OracleCommand(
                        "INSERT INTO TICKET (TICKET_ID, TICKET_PRICE, SEAT_NUMBER, TICKET_STATUS, TOTALNUMBEROFPEOPLE) " +
                        "VALUES (:TID, :TPRICE, :SEAT, :TSTATUS, :TPEOPLE)", conn))
                    {
                        cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = ticketId;
                        cmd.Parameters.Add(":TPRICE", OracleDbType.Decimal).Value = decimal.Parse(txtTicketPrice.Text.Trim());
                        cmd.Parameters.Add(":SEAT", OracleDbType.Varchar2).Value = txtSeatNumber.Text.Trim();
                        cmd.Parameters.Add(":TSTATUS", OracleDbType.Varchar2).Value =
                            string.IsNullOrWhiteSpace(ddlTicketStatus.SelectedValue)
                                ? (object)DBNull.Value
                                : ddlTicketStatus.SelectedValue;
                        cmd.Parameters.Add(":TPEOPLE", OracleDbType.Decimal).Value = decimal.Parse(txtTotalPeople.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }

                    
                    using (var cmd = new OracleCommand(
                        "INSERT INTO CUSTOMER_SHOW_TICKET (CUSTOMER_ID, MOVIE_ID, THEATRE_ID, HALL_ID, SHOW_ID, TICKET_ID) " +
                        "VALUES (:CID, :MID, :THTID, :HALID, :SID, :TKID)", conn))
                    {
                        cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = customerId;
                        cmd.Parameters.Add(":MID", OracleDbType.Decimal).Value = movieId;
                        cmd.Parameters.Add(":THTID", OracleDbType.Decimal).Value = theatreId;
                        cmd.Parameters.Add(":HALID", OracleDbType.Decimal).Value = hallId;
                        cmd.Parameters.Add(":SID", OracleDbType.Decimal).Value = showId;
                        cmd.Parameters.Add(":TKID", OracleDbType.Decimal).Value = ticketId;
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMessage("&#10004; Ticket issued and linked successfully.", true);
                ClearForm();
                LoadTickets();
                LoadStats();
                PopulateDropdowns();
            }
            catch (OracleException ex) when (ex.Number == 1)
            { ShowMessage("&#10008; A ticket with that ID already exists.", false); }
            catch (Exception ex)
            { ShowMessage($"&#10008; Error: {ex.Message}", false); }
        }

        

        protected void gvTickets_RowEditing(object sender, GridViewEditEventArgs e)
        {
            string id = gvTickets.DataKeys[e.NewEditIndex].Value.ToString();
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand("SELECT * FROM TICKET WHERE TICKET_ID=:TID", conn))
            {
                cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = decimal.Parse(id);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        hfEditId.Value = r["TICKET_ID"].ToString();
                        lblEditId.Text = $"(ID: {r["TICKET_ID"]})";
                        txtEditPrice.Text = r["TICKET_PRICE"].ToString();
                        txtEditSeat.Text = r["SEAT_NUMBER"].ToString();
                        txtEditTotalPeople.Text = r["TOTALNUMBEROFPEOPLE"].ToString();
                        try { ddlEditStatus.SelectedValue = r["TICKET_STATUS"].ToString(); } catch { }
                    }
                }
            }
            pnlEdit.Visible = true;
            LoadTickets();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "UPDATE TICKET SET TICKET_PRICE=:TPRICE, SEAT_NUMBER=:SEAT, " +
                    "TICKET_STATUS=:TSTATUS, TOTALNUMBEROFPEOPLE=:TPEOPLE WHERE TICKET_ID=:TID", conn))
                {
                    cmd.Parameters.Add(":TPRICE", OracleDbType.Decimal).Value = decimal.Parse(txtEditPrice.Text.Trim());
                    cmd.Parameters.Add(":SEAT", OracleDbType.Varchar2).Value = txtEditSeat.Text.Trim();
                    cmd.Parameters.Add(":TSTATUS", OracleDbType.Varchar2).Value =
                        string.IsNullOrWhiteSpace(ddlEditStatus.SelectedValue)
                            ? (object)DBNull.Value
                            : ddlEditStatus.SelectedValue;
                    cmd.Parameters.Add(":TPEOPLE", OracleDbType.Decimal).Value = decimal.Parse(txtEditTotalPeople.Text.Trim());
                    cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = decimal.Parse(hfEditId.Value);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                pnlEdit.Visible = false;
                ShowMessage("&#10004; Ticket updated successfully.", true);
                LoadTickets();
                LoadStats();
            }
            catch (Exception ex)
            { ShowMessage($"&#10008; Update failed: {ex.Message}", false); }
        }

        

        protected void gvTickets_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string id = gvTickets.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                {
                    conn.Open();

                    // Remove bridging row first
                    using (var cmd = new OracleCommand(
                        "DELETE FROM CUSTOMER_SHOW_TICKET WHERE TICKET_ID=:TID", conn))
                    {
                        cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = decimal.Parse(id);
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new OracleCommand("DELETE FROM TICKET WHERE TICKET_ID=:TID", conn))
                    {
                        cmd.Parameters.Add(":TID", OracleDbType.Decimal).Value = decimal.Parse(id);
                        cmd.ExecuteNonQuery();
                    }
                }
                ShowMessage("&#10004; Ticket deleted.", true);
                LoadTickets();
                LoadStats();
            }
            catch (Exception ex)
            { ShowMessage($"&#10008; Error: {ex.Message}", false); }
        }

        

        protected void gvTickets_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) => LoadTickets();
        protected void btnCancelEdit_Click(object sender, EventArgs e) { pnlEdit.Visible = false; LoadTickets(); }
        protected void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtTicketId.Text = txtTicketPrice.Text = txtSeatNumber.Text = txtTotalPeople.Text = "";
            ddlTicketStatus.SelectedIndex = 0;
            ddlCustomer.SelectedIndex = 0;
            ddlShow.SelectedIndex = 0;
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