using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class CustomerTicket : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadCustomerDropdown();
        }

        private void LoadCustomerDropdown()
        {
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(
                "SELECT CUSTOMER_ID, CUSTOMER_NAME, CUSTOMER_NUMBER FROM CUSTOMER ORDER BY CUSTOMER_NAME", conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                var displayTable = dt.Copy();
                displayTable.Columns.Add("DISPLAY_TEXT", typeof(string));
                foreach (DataRow row in displayTable.Rows)
                    row["DISPLAY_TEXT"] = $"{row["CUSTOMER_NAME"]} ({row["CUSTOMER_NUMBER"]})";

                ddlCustomer.DataSource = displayTable;
                ddlCustomer.DataTextField = "DISPLAY_TEXT";
                ddlCustomer.DataValueField = "CUSTOMER_ID";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Customer --", ""));
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTo = string.IsNullOrWhiteSpace(txtDateTo.Text) ? DateTime.Today : DateTime.Parse(txtDateTo.Text);
            DateTime dateFrom = string.IsNullOrWhiteSpace(txtDateFrom.Text) ? dateTo.AddMonths(-6) : DateTime.Parse(txtDateFrom.Text);

            string customerNumber = txtCustomerNumber.Text.Trim();
            if (!string.IsNullOrEmpty(customerNumber))
            {
                decimal? customerId = ResolveCustomerIdByNumber(customerNumber);
                if (customerId == null)
                {
                    lblCount.Text = $"No customer found with number \"{customerNumber}\".";
                    pnlResults.Visible = true;
                    pnlCustomerInfo.Visible = false;
                    gvTickets.DataSource = null;
                    gvTickets.DataBind();
                    return;
                }
                LoadCustomerInfo(customerId.Value);
                LoadTicketHistory(customerId.Value, dateFrom, dateTo);
            }
            else if (!string.IsNullOrEmpty(ddlCustomer.SelectedValue))
            {
                decimal customerId = decimal.Parse(ddlCustomer.SelectedValue);
                LoadCustomerInfo(customerId);
                LoadTicketHistory(customerId, dateFrom, dateTo);
            }
        }

        private decimal? ResolveCustomerIdByNumber(string customerNumber)
        {
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(
                "SELECT CUSTOMER_ID FROM CUSTOMER WHERE TO_CHAR(CUSTOMER_NUMBER) = :CNUM", conn))
            {
                cmd.Parameters.Add(":CNUM", OracleDbType.Varchar2).Value = customerNumber;
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? (decimal?)null : Convert.ToDecimal(result);
            }
        }

        private void LoadCustomerInfo(decimal customerId)
        {
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(
                "SELECT CUSTOMER_ID, CUSTOMER_NAME, CUSTOMER_NUMBER FROM CUSTOMER WHERE CUSTOMER_ID=:CID", conn))
            {
                cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = customerId;
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        lblCustomerName.Text = r["CUSTOMER_NAME"].ToString();
                        lblCustomerNumber.Text = r["CUSTOMER_NUMBER"].ToString();
                        lblCustomerId.Text = r["CUSTOMER_ID"].ToString();
                        pnlCustomerInfo.Visible = true;
                    }
                }
            }
        }

        // ── FIX: now queries CUSTOMER_SHOW_TICKET (actual tickets purchased)
        //         instead of CUSTOMER_HALL_SHOW (show attendance bridging table).
        //         Pulls full ticket details + show/movie/theatre/hall context.
        //         Dates formatted in SQL to avoid ASP.NET DataFormatString conflicts.
        private void LoadTicketHistory(decimal customerId, DateTime dateFrom, DateTime dateTo)
        {
            const string sql = @"
                SELECT
                    T.TICKET_ID,
                    T.TICKET_PRICE,
                    T.SEAT_NUMBER,
                    T.TICKET_STATUS,
                    T.TOTALNUMBEROFPEOPLE,
                    M.MOVIE_TITLE,
                    M.GENRE,
                    M.MOVIE_LANGUAGE,
                    TH.THEATRE_NAME,
                    TH.THEATRE_LOCATION,
                    H.HALL_NAME,
                    H.HALL_CAPACITY,
                    TO_CHAR(S.SHOW_TIME,   'DD/MM/YYYY HH24:MI') AS SHOW_TIME_FMT,
                    TO_CHAR(S.SHOWED_DATE, 'DD/MM/YYYY')         AS SHOWED_DATE_FMT
                FROM CUSTOMER_SHOW_TICKET CST
                JOIN TICKET  T  ON T.TICKET_ID   = CST.TICKET_ID
                JOIN SHOW    S  ON S.SHOW_ID     = CST.SHOW_ID
                JOIN MOVIE   M  ON M.MOVIE_ID    = CST.MOVIE_ID
                JOIN THEATRE TH ON TH.THEATRE_ID = CST.THEATRE_ID
                JOIN HALL    H  ON H.HALL_ID     = CST.HALL_ID
                WHERE CST.CUSTOMER_ID = :CID
                  AND S.SHOWED_DATE BETWEEN :DFROM AND :DTO
                ORDER BY S.SHOWED_DATE DESC, T.TICKET_ID";

            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = customerId;
                cmd.Parameters.Add(":DFROM", OracleDbType.Date).Value = dateFrom;
                cmd.Parameters.Add(":DTO", OracleDbType.Date).Value = dateTo;
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvTickets.DataSource = dt;
                gvTickets.DataBind();
                lblCount.Text = $"{dt.Rows.Count} ticket(s) found in this period";
                lblTotalShows.Text = dt.Rows.Count.ToString();
                pnlResults.Visible = true;
            }
        }
    }
}