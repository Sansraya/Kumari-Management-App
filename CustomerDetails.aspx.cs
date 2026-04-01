using System;
using System.Data;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace CinemaApp
{
    public partial class CustomerDetails : System.Web.UI.Page
    {
        private string ConnStr => System.Configuration.ConfigurationManager
                                      .ConnectionStrings["OracleConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { LoadCustomers(); LoadStats(); }
        }

        private void LoadCustomers()
        {
            const string sql =
                "SELECT CUSTOMER_ID, CUSTOMER_NAME, CUSTOMER_NUMBER FROM CUSTOMER ORDER BY CUSTOMER_ID";

            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand(sql, conn))
            {
                conn.Open();
                var da = new OracleDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                gvCustomers.DataSource = dt;
                gvCustomers.DataBind();
                lblRecordCount.Text = $"{dt.Rows.Count} record(s)";
            }
        }

        private void LoadStats()
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                conn.Open();
                using (var cmd = new OracleCommand("SELECT COUNT(*) FROM CUSTOMER", conn))
                    lblTotalCustomers.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "INSERT INTO CUSTOMER (CUSTOMER_ID, CUSTOMER_NAME, CUSTOMER_NUMBER) VALUES (:CID, :CNAME, :CNUM)", conn))
                {
                    cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = decimal.Parse(txtCustomerId.Text.Trim());
                    cmd.Parameters.Add(":CNAME", OracleDbType.Varchar2).Value = txtCustomerName.Text.Trim();
                    cmd.Parameters.Add(":CNUM", OracleDbType.Decimal).Value = decimal.Parse(txtCustomerNumber.Text.Trim());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                ShowMessage("✔ Customer added successfully.", true);
                ClearForm(); LoadCustomers(); LoadStats();
            }
            catch (OracleException ex) when (ex.Number == 1)
            { ShowMessage("✘ A customer with that ID already exists.", false); }
            catch (Exception ex) { ShowMessage($"✘ Error: {ex.Message}", false); }
        }

        protected void gvCustomers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            string id = gvCustomers.DataKeys[e.NewEditIndex].Value.ToString();
            using (var conn = new OracleConnection(ConnStr))
            using (var cmd = new OracleCommand("SELECT * FROM CUSTOMER WHERE CUSTOMER_ID=:CID", conn))
            {
                cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = decimal.Parse(id);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        hfEditId.Value = r["CUSTOMER_ID"].ToString();
                        lblEditId.Text = $"(ID: {r["CUSTOMER_ID"]})";
                        txtEditName.Text = r["CUSTOMER_NAME"].ToString();
                        txtEditNumber.Text = r["CUSTOMER_NUMBER"].ToString();
                    }
                }
            }
            pnlEdit.Visible = true; LoadCustomers();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand(
                    "UPDATE CUSTOMER SET CUSTOMER_NAME=:CNAME, CUSTOMER_NUMBER=:CNUM WHERE CUSTOMER_ID=:CID", conn))
                {
                    cmd.Parameters.Add(":CNAME", OracleDbType.Varchar2).Value = txtEditName.Text.Trim();
                    cmd.Parameters.Add(":CNUM", OracleDbType.Decimal).Value = decimal.Parse(txtEditNumber.Text.Trim());
                    cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = decimal.Parse(hfEditId.Value);
                    conn.Open(); cmd.ExecuteNonQuery();
                }
                pnlEdit.Visible = false;
                ShowMessage("✔ Customer updated successfully.", true);
                LoadCustomers(); LoadStats();
            }
            catch (Exception ex) { ShowMessage($"✘ Update failed: {ex.Message}", false); }
        }

        protected void gvCustomers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string id = gvCustomers.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                using (var conn = new OracleConnection(ConnStr))
                using (var cmd = new OracleCommand("DELETE FROM CUSTOMER WHERE CUSTOMER_ID=:CID", conn))
                {
                    cmd.Parameters.Add(":CID", OracleDbType.Decimal).Value = decimal.Parse(id);
                    conn.Open(); cmd.ExecuteNonQuery();
                }
                ShowMessage("✔ Customer deleted.", true); LoadCustomers(); LoadStats();
            }
            catch (OracleException ex) when (ex.Number == 2292)
            { ShowMessage("✘ Cannot delete: customer has linked records.", false); }
            catch (Exception ex) { ShowMessage($"✘ Error: {ex.Message}", false); }
        }

        protected void gvCustomers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) => LoadCustomers();
        protected void btnCancelEdit_Click(object sender, EventArgs e) { pnlEdit.Visible = false; LoadCustomers(); }
        protected void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtCustomerId.Text = txtCustomerName.Text = txtCustomerNumber.Text = "";
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