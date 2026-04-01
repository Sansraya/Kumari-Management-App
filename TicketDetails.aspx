<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TicketDetails.aspx.cs" Inherits="CinemaApp.TicketDetails" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ticket Details — Kumari Cinemas</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        :root { --purple:#6f5499; --purple-dark:#5a4180; --purple-faint:#f0ecf8; --purple-pale:#e8e1f5; --white:#ffffff; --off-white:#fafaf9; --text:#1a1522; --text-muted:#6b6478; --text-light:#9b93a8; --border:#e2dced; --border-strong:#c8bedd; --surface-2:#f6f3fc; --danger:#c0392b; }
        * { box-sizing:border-box; margin:0; padding:0; }
        body { background:var(--off-white); font-family:'Trebuchet MS', 'Segoe UI', system-ui, sans-serif; color:var(--text); font-size:0.9rem; line-height:1.6; }
        .kc-nav { background:var(--white); border-bottom:1px solid var(--border); position:sticky; top:0; z-index:100; }
        .kc-nav-inner { max-width:1300px; margin:0 auto; padding:0 32px; height:60px; display:flex; align-items:center; justify-content:space-between; }
        .kc-logo { font-family:'Verdana', 'Times New Roman', serif; font-weight:800; font-size:1.1rem; color:var(--purple); text-decoration:none; letter-spacing:-0.02em; }
        .kc-nav-links { display:flex; align-items:center; gap:2px; }
        .kc-nav-links a { color:var(--text-muted); text-decoration:none; padding:6px 12px; border-radius:6px; font-size:0.825rem; font-weight:500; }
        .kc-nav-links a:hover { color:var(--purple); background:var(--purple-faint); }
        .kc-nav-links a.active { color:var(--purple); background:var(--purple-faint); font-weight:600; }
        .kc-page { max-width:1300px; margin:0 auto; padding:40px 32px; }
        .kc-page-header { margin-bottom:32px; padding-bottom:24px; border-bottom:1px solid var(--border); }
        .kc-page-eyebrow { font-size:0.68rem; font-weight:700; letter-spacing:0.14em; text-transform:uppercase; color:var(--purple); margin-bottom:6px; }
        .kc-page-title { font-family:'Verdana', 'Times New Roman', serif; font-size:1.75rem; font-weight:700; color:var(--text); letter-spacing:-0.03em; margin-bottom:5px; }
        .kc-page-subtitle { color:var(--text-muted); font-size:0.875rem; }
        .kc-stats { display:grid; gap:14px; margin-bottom:28px; }
        .kc-stat-card { background:var(--white); border:1px solid var(--border); border-radius:10px; padding:20px; position:relative; overflow:hidden; }
        .kc-stat-card::before { content:''; position:absolute; top:0; left:0; right:0; height:3px; background:var(--purple); }
        .kc-stat-label { font-size:0.68rem; font-weight:700; text-transform:uppercase; letter-spacing:0.09em; color:var(--text-light); margin-bottom:8px; }
        .kc-stat-value { font-family:'Verdana', 'Times New Roman', serif; font-size:1.9rem; font-weight:700; color:var(--purple); line-height:1; margin-bottom:4px; }
        .kc-card { background:var(--white); border:1px solid var(--border); border-radius:10px; overflow:hidden; margin-bottom:28px; }
        .kc-card-header { padding:16px 24px; border-bottom:1px solid var(--border); display:flex; align-items:center; justify-content:space-between; }
        .kc-card-title { font-family:'Verdana', 'Times New Roman', serif; font-size:0.9rem; font-weight:700; color:var(--text); }
        .kc-card-body { padding:24px; }
        .kc-edit-panel { background:var(--white); border:2px solid var(--purple); border-radius:10px; overflow:hidden; margin-bottom:28px; }
        .kc-edit-panel .kc-card-header { background:var(--purple-faint); border-bottom:1px solid var(--purple-pale); }
        .kc-edit-panel .kc-card-title { color:var(--purple); }
        .kc-edit-label { color:var(--purple); font-size:0.8rem; font-weight:600; margin-left:8px; }
        .form-grid { display:grid; gap:20px; }
        .form-col-3 { grid-template-columns:repeat(3,1fr); }
        .form-col-2 { grid-template-columns:repeat(2,1fr); }
        .section-divider { grid-column:1/-1; border-top:1px dashed var(--border-strong); padding-top:16px; margin-top:4px; }
        .section-divider-label { font-size:0.68rem; font-weight:700; letter-spacing:0.1em; text-transform:uppercase; color:var(--purple); margin-bottom:4px; }
        .show-hint { font-size:0.7rem; color:var(--text-light); margin-top:4px; }
        .form-label { display:block; font-size:0.7rem; font-weight:600; text-transform:uppercase; letter-spacing:0.07em; color:var(--text-muted); margin-bottom:6px; }
        input[type="text"],input[type="date"],input[type="datetime-local"],select { width:100%; background:var(--off-white) !important; border:1px solid var(--border-strong) !important; color:var(--text) !important; border-radius:7px !important; padding:9px 13px !important; font-family:'Trebuchet MS','Segoe UI',system-ui,sans-serif !important; font-size:0.875rem !important; outline:none !important; transition:border-color 0.15s,box-shadow 0.15s !important; }
        input:focus,select:focus { border-color:var(--purple) !important; box-shadow:0 0 0 3px rgba(111,84,153,0.12) !important; background:var(--white) !important; }
        .form-actions { display:flex; gap:10px; margin-top:24px; padding-top:20px; border-top:1px solid var(--border); }
        .btn-primary { background:var(--purple) !important; color:white !important; padding:9px 22px !important; border-radius:7px !important; font-family:'Trebuchet MS','Segoe UI',system-ui,sans-serif !important; font-weight:600 !important; font-size:0.85rem !important; cursor:pointer !important; border:none !important; transition:background 0.15s !important; }
        .btn-primary:hover { background:var(--purple-dark) !important; }
        .btn-secondary { background:var(--white) !important; color:var(--text-muted) !important; padding:9px 22px !important; border-radius:7px !important; font-family:'Trebuchet MS','Segoe UI',system-ui,sans-serif !important; font-weight:500 !important; font-size:0.85rem !important; cursor:pointer !important; border:1px solid var(--border-strong) !important; transition:background 0.15s !important; }
        .btn-secondary:hover { background:var(--purple-faint) !important; color:var(--purple) !important; border-color:var(--purple) !important; }
        .validation-error { color:var(--danger); font-size:0.72rem; margin-top:4px; display:block; }
        .asp-grid table { width:100%; border-collapse:collapse; }
        .asp-grid th { background:var(--surface-2); color:var(--text-muted); padding:11px 16px; text-align:left; font-size:0.68rem; font-weight:700; text-transform:uppercase; letter-spacing:0.08em; border-bottom:1px solid var(--border); }
        .asp-grid td { padding:12px 16px; border-bottom:1px solid var(--border); color:var(--text); font-size:0.85rem; vertical-align:middle; }
        .asp-grid tr:last-child td { border-bottom:none; }
        .asp-grid tr:hover td { background:var(--purple-faint); }
        .action-edit { color:var(--purple); font-weight:600; text-decoration:none; font-size:0.8rem; margin-right:12px; padding:3px 8px; border-radius:4px; }
        .action-delete { color:var(--danger); font-weight:600; text-decoration:none; font-size:0.8rem; padding:3px 8px; border-radius:4px; }
        .kc-count-badge { background:var(--purple-faint); color:var(--purple); font-size:0.75rem; font-weight:600; padding:3px 10px; border-radius:20px; }
        .status-badge { display:inline-block; font-size:0.7rem; font-weight:700; letter-spacing:0.06em; text-transform:uppercase; padding:2px 8px; border-radius:4px; }
        .status-active { background:#e8f5ee; color:#2d7a5f; }
        .status-used { background:var(--surface-2); color:var(--text-muted); }
        .status-cancelled { background:#fde8e7; color:var(--danger); }
    </style>
</head>
<body>
    <nav class="kc-nav">
        <div class="kc-nav-inner">
            <a href="Default.aspx" class="kc-logo">Kumari Cinemas</a>
            <div class="kc-nav-links">
                <a href="Default.aspx">Dashboard</a>
                <a href="CustomerDetails.aspx">Customers</a>
                <a href="TheaterDetails.aspx">Theatres</a>
                <a href="MovieDetails.aspx">Movies</a>
                <a href="ShowDetails.aspx">Shows</a>
                <a href="TicketDetails.aspx" class="active">Tickets</a>
                <a href="HallDetails.aspx">Halls</a>
            </div>
        </div>
    </nav>

    <form id="form1" runat="server">
    <div class="kc-page">

        <div class="kc-page-header">
            <p class="kc-page-eyebrow">Basic Management</p>
            <h1 class="kc-page-title">Ticket Management</h1>
            <p class="kc-page-subtitle">Issue, update and manage cinema tickets with full associations</p>
        </div>

        <asp:Label ID="lblMessage" runat="server" Visible="false" style="display:block; padding:12px 16px; border-radius:8px; font-size:0.85rem; margin-bottom:20px;"></asp:Label>

        <div class="kc-stats" style="grid-template-columns:repeat(3,1fr); max-width:700px;">
            <div class="kc-stat-card">
                <p class="kc-stat-label">Total Tickets</p>
                <asp:Label ID="lblTotalTickets" runat="server" CssClass="kc-stat-value">0</asp:Label>
            </div>
            <div class="kc-stat-card">
                <p class="kc-stat-label">Total People</p>
                <asp:Label ID="lblTotalPeople" runat="server" CssClass="kc-stat-value">0</asp:Label>
            </div>
            <div class="kc-stat-card">
                <p class="kc-stat-label">Total Revenue</p>
                <asp:Label ID="lblRevenue" runat="server" CssClass="kc-stat-value">0</asp:Label>
            </div>
        </div>

        <div class="kc-card">
            <div class="kc-card-header">
                <span class="kc-card-title">Issue New Ticket</span>
            </div>
            <div class="kc-card-body">
                <div class="form-grid form-col-3">

                    
                    <div>
                        <label class="form-label">Ticket ID *</label>
                        <asp:TextBox ID="txtTicketId" runat="server" placeholder="e.g. 1"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTicketId"
                            ErrorMessage="Ticket ID required" CssClass="validation-error"
                            Display="Dynamic" ValidationGroup="AddTicket" />
                    </div>
                    <div>
                        <label class="form-label">Ticket Price *</label>
                        <asp:TextBox ID="txtTicketPrice" runat="server" placeholder="e.g. 500"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTicketPrice"
                            ErrorMessage="Price required" CssClass="validation-error"
                            Display="Dynamic" ValidationGroup="AddTicket" />
                    </div>
                    <div>
                        <label class="form-label">Seat Number *</label>
                        <asp:TextBox ID="txtSeatNumber" runat="server" placeholder="e.g. A12"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSeatNumber"
                            ErrorMessage="Seat number required" CssClass="validation-error"
                            Display="Dynamic" ValidationGroup="AddTicket" />
                    </div>
                    <div>
                        <label class="form-label">Ticket Status</label>
                        <asp:DropDownList ID="ddlTicketStatus" runat="server">
                            <asp:ListItem Value="">-- Select Status --</asp:ListItem>
                            <asp:ListItem Value="ACTIVE">Active</asp:ListItem>
                            <asp:ListItem Value="USED">Used</asp:ListItem>
                            <asp:ListItem Value="CANCELLED">Cancelled</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div>
                        <label class="form-label">Total Number of People *</label>
                        <asp:TextBox ID="txtTotalPeople" runat="server" placeholder="e.g. 2"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTotalPeople"
                            ErrorMessage="Total people required" CssClass="validation-error"
                            Display="Dynamic" ValidationGroup="AddTicket" />
                    </div>

                    
                    <div class="section-divider">
                        <p class="section-divider-label">&#128279; Link Ticket — Customer / Show (Movie + Theatre + Hall)</p>
                    </div>
                    <div>
                        <label class="form-label">Customer *</label>
                        <asp:DropDownList ID="ddlCustomer" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCustomer"
                            InitialValue="" ErrorMessage="Customer required"
                            CssClass="validation-error" Display="Dynamic" ValidationGroup="AddTicket" />
                    </div>
                    
                    <div style="grid-column: span 2;">
                        <label class="form-label">Show * &nbsp;<span style="font-weight:400;text-transform:none;letter-spacing:0;">(selecting a show auto-fills Movie, Theatre &amp; Hall)</span></label>
                        <asp:DropDownList ID="ddlShow" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlShow"
                            InitialValue="" ErrorMessage="Show required"
                            CssClass="validation-error" Display="Dynamic" ValidationGroup="AddTicket" />
                        <p class="show-hint">The show list is joined with CUSTOMER_HALL_SHOW so each entry already carries its Movie, Theatre and Hall IDs.</p>
                    </div>

                </div>
                <div class="form-actions">
                    <asp:Button ID="btnInsert" runat="server" Text="Issue Ticket" CssClass="btn-primary"
                        OnClick="btnInsert_Click" ValidationGroup="AddTicket" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn-secondary"
                        OnClick="btnClear_Click" CausesValidation="false" />
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlEdit" runat="server" Visible="false" CssClass="kc-edit-panel">
            <div class="kc-card-header">
                <div style="display:flex; align-items:center;">
                    <span class="kc-card-title">Edit Ticket</span>
                    <asp:Label ID="lblEditId" runat="server" CssClass="kc-edit-label"></asp:Label>
                </div>
            </div>
            <div class="kc-card-body">
                <asp:HiddenField ID="hfEditId" runat="server" />
                <div class="form-grid form-col-2">
                    <div>
                        <label class="form-label">Ticket Price *</label>
                        <asp:TextBox ID="txtEditPrice" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEditPrice"
                            ErrorMessage="Price required" CssClass="validation-error"
                            Display="Dynamic" ValidationGroup="EditTicket" />
                    </div>
                    <div>
                        <label class="form-label">Seat Number *</label>
                        <asp:TextBox ID="txtEditSeat" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEditSeat"
                            ErrorMessage="Seat required" CssClass="validation-error"
                            Display="Dynamic" ValidationGroup="EditTicket" />
                    </div>
                    <div>
                        <label class="form-label">Ticket Status</label>
                        <asp:DropDownList ID="ddlEditStatus" runat="server">
                            <asp:ListItem Value="">-- Select Status --</asp:ListItem>
                            <asp:ListItem Value="ACTIVE">Active</asp:ListItem>
                            <asp:ListItem Value="USED">Used</asp:ListItem>
                            <asp:ListItem Value="CANCELLED">Cancelled</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div>
                        <label class="form-label">Total Number of People *</label>
                        <asp:TextBox ID="txtEditTotalPeople" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEditTotalPeople"
                            ErrorMessage="Total people required" CssClass="validation-error"
                            Display="Dynamic" ValidationGroup="EditTicket" />
                    </div>
                </div>
                <div class="form-actions">
                    <asp:Button ID="btnUpdate" runat="server" Text="Save Changes" CssClass="btn-primary"
                        OnClick="btnUpdate_Click" ValidationGroup="EditTicket" />
                    <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn-secondary"
                        OnClick="btnCancelEdit_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>

        <div class="kc-card asp-grid">
            <div class="kc-card-header">
                <span class="kc-card-title">All Tickets</span>
                <asp:Label ID="lblCount" runat="server" CssClass="kc-count-badge"></asp:Label>
            </div>
            <asp:GridView ID="gvTickets" runat="server"
                AutoGenerateColumns="False" DataKeyNames="TICKET_ID"
                OnRowEditing="gvTickets_RowEditing"
                OnRowDeleting="gvTickets_RowDeleting"
                OnRowCancelingEdit="gvTickets_RowCancelingEdit"
                EmptyDataText="No tickets found." CssClass="w-full">
                <Columns>
                    <asp:BoundField DataField="TICKET_ID"           HeaderText="ID"       ReadOnly="True" />
                    <asp:BoundField DataField="TICKET_PRICE"        HeaderText="Price"    DataFormatString="{0:N0}" />
                    <asp:BoundField DataField="SEAT_NUMBER"         HeaderText="Seat" />
                    <asp:BoundField DataField="TICKET_STATUS"       HeaderText="Status" />
                    <asp:BoundField DataField="TOTALNUMBEROFPEOPLE" HeaderText="People" />
                    <asp:BoundField DataField="CUSTOMER_NAME"       HeaderText="Customer" />
                    <asp:TemplateField HeaderText="Show Details">
                        <ItemTemplate>
                            <span style="font-size:0.78rem; line-height:1.6;">
                                <strong>Show #<%# Eval("SHOW_ID") %></strong><br />
                                <%# Eval("MOVIE_TITLE") %><br />
                                <span style="color:var(--text-muted);">
                                    <%# Eval("SHOWED_DATE") %> &nbsp;&#8226;&nbsp;
                                    <%# Eval("SHOW_TIME_FMT") %><br />
                                    <%# Eval("THEATRE_NAME") %> &nbsp;&#8226;&nbsp; <%# Eval("HALL_NAME") %>
                                </span>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CommandName="Edit"   Text="Edit"   CssClass="action-edit" />
                            <asp:LinkButton runat="server" CommandName="Delete" Text="Delete" CssClass="action-delete"
                                OnClientClick="return confirm('Delete this ticket?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>

    </div>
    </form>
</body>
</html>
