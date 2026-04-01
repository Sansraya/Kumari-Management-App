<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerTicket.aspx.cs" Inherits="CinemaApp.CustomerTicket" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Customer Ticket Report — Kumari Cinemas</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        :root { --purple:#6f5499; --purple-dark:#5a4180; --purple-faint:#f0ecf8; --purple-pale:#e8e1f5; --white:#ffffff; --off-white:#fafaf9; --text:#1a1522; --text-muted:#6b6478; --text-light:#9b93a8; --border:#e2dced; --border-strong:#c8bedd; --surface-2:#f6f3fc; --danger:#c0392b; }
        * { box-sizing:border-box; margin:0; padding:0; }
        body { background:var(--off-white); font-family:'Trebuchet MS', 'Segoe UI', system-ui, sans-serif; color:var(--text); font-size:0.9rem; line-height:1.6; }
        .kc-nav { background:var(--white); border-bottom:1px solid var(--border); position:sticky; top:0; z-index:100; }
        .kc-nav-inner { max-width:1300px; margin:0 auto; padding:0 32px; height:60px; display:flex; align-items:center; justify-content:space-between; }
        .kc-logo { font-family:'Verdana', 'Times New Roman', serif;; font-weight:800; font-size:1.1rem; color:var(--purple); text-decoration:none; letter-spacing:-0.02em; }
        .kc-nav-links { display:flex; align-items:center; gap:2px; }
        .kc-nav-links a { color:var(--text-muted); text-decoration:none; padding:6px 12px; border-radius:6px; font-size:0.825rem; font-weight:500; }
        .kc-nav-links a:hover { color:var(--purple); background:var(--purple-faint); }
        .kc-nav-links a.active { color:var(--purple); background:var(--purple-faint); font-weight:600; }
        .kc-page { max-width:1300px; margin:0 auto; padding:40px 32px; }
        .kc-page-header { margin-bottom:32px; padding-bottom:24px; border-bottom:1px solid var(--border); }
        .kc-page-eyebrow { font-size:0.68rem; font-weight:700; letter-spacing:0.14em; text-transform:uppercase; color:var(--purple); margin-bottom:6px; }
        .kc-page-title { font-family:'Verdana', 'Times New Roman', serif;; font-size:1.75rem; font-weight:700; color:var(--text); letter-spacing:-0.03em; margin-bottom:5px; }
        .kc-page-subtitle { color:var(--text-muted); font-size:0.875rem; }
        .kc-card { background:var(--white); border:1px solid var(--border); border-radius:10px; overflow:hidden; margin-bottom:28px; }
        .kc-card-header { padding:16px 24px; border-bottom:1px solid var(--border); display:flex; align-items:center; justify-content:space-between; }
        .kc-card-title { font-family:'Verdana', 'Times New Roman', serif;; font-size:0.9rem; font-weight:700; color:var(--text); }
        .kc-card-body { padding:24px; }
        .kc-info-panel { background:var(--white); border:1px solid var(--border); border-radius:10px; padding:24px; margin-bottom:28px; }
        .kc-info-panel-title { font-family:'Verdana', 'Times New Roman', serif;; font-size:0.85rem; font-weight:700; color:var(--purple); margin-bottom:16px; text-transform:uppercase; letter-spacing:0.08em; font-size:0.7rem; }
        .info-grid { display:grid; grid-template-columns:repeat(3,1fr); gap:14px; }
        .info-card { background:var(--surface-2); border:1px solid var(--border); border-radius:8px; padding:14px 18px; }
        .info-label { color:var(--text-light); font-size:0.68rem; text-transform:uppercase; letter-spacing:0.07em; font-weight:600; margin-bottom:4px; }
        .info-value { color:var(--text); font-size:1rem; font-weight:600; }
        .form-grid { display:grid; gap:20px; }
        .form-col-3 { grid-template-columns:repeat(3,1fr); }
        .form-col-4 { grid-template-columns:repeat(4,1fr); }
        .form-label { display:block; font-size:0.7rem; font-weight:600; text-transform:uppercase; letter-spacing:0.07em; color:var(--text-muted); margin-bottom:6px; }
        input[type="text"],input[type="date"],select { width:100%; background:var(--off-white) !important; border:1px solid var(--border-strong) !important; color:var(--text) !important; border-radius:7px !important; padding:9px 13px !important; font-family:'DM Sans',sans-serif !important; font-size:0.875rem !important; outline:none !important; transition:border-color 0.15s,box-shadow 0.15s !important; }
        input:focus,select:focus { border-color:var(--purple) !important; box-shadow:0 0 0 3px rgba(111,84,153,0.12) !important; background:var(--white) !important; }
        .form-actions { display:flex; gap:10px; margin-top:24px; padding-top:20px; border-top:1px solid var(--border); }
        .btn-primary { background:var(--purple) !important; color:white !important; padding:9px 22px !important; border-radius:7px !important; font-family:'DM Sans',sans-serif !important; font-weight:600 !important; font-size:0.85rem !important; cursor:pointer !important; border:none !important; transition:background 0.15s !important; }
        .btn-primary:hover { background:var(--purple-dark) !important; }
        .kc-hint { color:var(--text-light); font-size:0.75rem; margin-top:8px; }
        .kc-stats { display:grid; gap:14px; margin-bottom:28px; }
        .kc-stat-card { background:var(--white); border:1px solid var(--border); border-radius:10px; padding:20px; position:relative; overflow:hidden; }
        .kc-stat-card::before { content:''; position:absolute; top:0; left:0; right:0; height:3px; background:var(--purple); }
        .kc-stat-label { font-size:0.68rem; font-weight:700; text-transform:uppercase; letter-spacing:0.09em; color:var(--text-light); margin-bottom:8px; }
        .kc-stat-value { font-family:'Verdana', 'Times New Roman', serif;; font-size:1.9rem; font-weight:700; color:var(--purple); line-height:1; }
        .asp-grid table { width:100%; border-collapse:collapse; }
        .asp-grid th { background:var(--surface-2); color:var(--text-muted); padding:11px 16px; text-align:left; font-size:0.68rem; font-weight:700; text-transform:uppercase; letter-spacing:0.08em; border-bottom:1px solid var(--border); }
        .asp-grid td { padding:12px 16px; border-bottom:1px solid var(--border); color:var(--text); font-size:0.85rem; vertical-align:middle; }
        .asp-grid tr:last-child td { border-bottom:none; }
        .asp-grid tr:hover td { background:var(--purple-faint); }
        .kc-count-badge { background:var(--purple-faint); color:var(--purple); font-size:0.75rem; font-weight:600; padding:3px 10px; border-radius:20px; }
    </style>
</head>
<body>
    <nav class="kc-nav">
        <div class="kc-nav-inner">
            <a href="Default.aspx" class="kc-logo">Kumari Cinemas</a>
            <div class="kc-nav-links">
                <a href="Default.aspx">Dashboard</a>
                <a href="CustomerDetails.aspx" class="active">Customers</a>
                <a href="TheaterDetails.aspx">Theatres</a>
                <a href="MovieDetails.aspx">Movies</a>
                <a href="ShowDetails.aspx">Shows</a>
                <a href="TicketDetails.aspx">Tickets</a>
                <a href="HallDetails.aspx">Halls</a>
            </div>
        </div>
    </nav>

    <form id="form1" runat="server">
    <div class="kc-page">

        <div class="kc-page-header">
            <p class="kc-page-eyebrow">Complex Reports</p>
            <h1 class="kc-page-title">Customer Show History</h1>
            <p class="kc-page-subtitle">View all shows attended by a customer within a 6-month period</p>
        </div>

        
        <div class="kc-card">
            <div class="kc-card-header">
                <span class="kc-card-title">Search Customer</span>
            </div>
            <div class="kc-card-body">
                <div class="form-grid form-col-4">
                    <div>
                        <label class="form-label">Select Customer</label>
                        <asp:DropDownList ID="ddlCustomer" runat="server"></asp:DropDownList>
                    </div>
                    <div>
                        <label class="form-label">— or — Customer Number</label>
                        <asp:TextBox ID="txtCustomerNumber" runat="server" placeholder="e.g. C00123"></asp:TextBox>
                    </div>
                    <div>
                        <label class="form-label">From Date</label>
                        <asp:TextBox ID="txtDateFrom" runat="server" TextMode="Date"></asp:TextBox>
                    </div>
                    <div>
                        <label class="form-label">To Date</label>
                        <asp:TextBox ID="txtDateTo" runat="server" TextMode="Date"></asp:TextBox>
                    </div>
                </div>
                <div class="form-actions">
                    <asp:Button ID="btnSearch" runat="server" Text="Generate Report" CssClass="btn-primary" OnClick="btnSearch_Click" CausesValidation="false" />
                </div>
                <p class="kc-hint">Customer Number takes priority over the dropdown if both are filled. Leave dates blank to default to the last 6 months.</p>
            </div>
        </div>

        
        <asp:Panel ID="pnlCustomerInfo" runat="server" Visible="false">
            <div class="kc-info-panel">
                <p class="kc-info-panel-title">Customer Details</p>
                <div class="info-grid">
                    <div class="info-card">
                        <div class="info-label">Customer ID</div>
                        <div class="info-value"><asp:Label ID="lblCustomerId" runat="server"></asp:Label></div>
                    </div>
                    <div class="info-card">
                        <div class="info-label">Customer Name</div>
                        <div class="info-value"><asp:Label ID="lblCustomerName" runat="server"></asp:Label></div>
                    </div>
                    <div class="info-card">
                        <div class="info-label">Customer Number</div>
                        <div class="info-value"><asp:Label ID="lblCustomerNumber" runat="server"></asp:Label></div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        
        <asp:Panel ID="pnlResults" runat="server" Visible="false">
            <div class="kc-stats" style="grid-template-columns:repeat(1,1fr); max-width:200px;">
                <div class="kc-stat-card">
                    <p class="kc-stat-label">Total Tickets</p>
                    <asp:Label ID="lblTotalShows" runat="server" CssClass="kc-stat-value">0</asp:Label>
                </div>
            </div>

            <div class="kc-card asp-grid">
                <div class="kc-card-header">
                    <span class="kc-card-title">Ticket Purchase History</span>
                    <asp:Label ID="lblCount" runat="server" CssClass="kc-count-badge"></asp:Label>
                </div>
                <asp:GridView ID="gvTickets" runat="server"
                    AutoGenerateColumns="False"
                    EmptyDataText="No tickets found for this customer in the selected period."
                    CssClass="w-full">
                    <Columns>
                        <asp:BoundField DataField="TICKET_ID"          HeaderText="Ticket ID" />
                        <asp:BoundField DataField="TICKET_PRICE"       HeaderText="Price (NPR)" DataFormatString="{0:N0}" />
                        <asp:BoundField DataField="SEAT_NUMBER"        HeaderText="Seat" />
                        <asp:BoundField DataField="TICKET_STATUS"      HeaderText="Status" />
                        <asp:BoundField DataField="TOTALNUMBEROFPEOPLE" HeaderText="People" />
                        <asp:BoundField DataField="MOVIE_TITLE"        HeaderText="Movie" />
                        <asp:BoundField DataField="GENRE"              HeaderText="Genre" />
                        <asp:BoundField DataField="MOVIE_LANGUAGE"     HeaderText="Language" />
                        <asp:BoundField DataField="THEATRE_NAME"       HeaderText="Theatre" />
                        <asp:BoundField DataField="THEATRE_LOCATION"   HeaderText="Location" />
                        <asp:BoundField DataField="HALL_NAME"          HeaderText="Hall" />
                        <asp:BoundField DataField="HALL_CAPACITY"      HeaderText="Capacity" />
                        <asp:BoundField DataField="SHOW_TIME_FMT"      HeaderText="Show Time" />
                        <asp:BoundField DataField="SHOWED_DATE_FMT"    HeaderText="Date" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

    </div>
    </form>
</body>
</html>
