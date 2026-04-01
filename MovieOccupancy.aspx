<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MovieOccupancy.aspx.cs" Inherits="CinemaApp.MovieOccupancy" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Movie Occupancy Report — Kumari Cinemas</title>
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
        .kc-card { background:var(--white); border:1px solid var(--border); border-radius:10px; overflow:hidden; margin-bottom:28px; }
        .kc-card-header { padding:16px 24px; border-bottom:1px solid var(--border); display:flex; align-items:center; justify-content:space-between; }
        .kc-card-title { font-family:'Verdana', 'Times New Roman', serif; font-size:0.9rem; font-weight:700; color:var(--text); }
        .kc-card-body { padding:24px; }
        .kc-info-panel { background:var(--white); border:1px solid var(--border); border-radius:10px; padding:24px; margin-bottom:28px; }
        .kc-info-panel-title { color:var(--purple); font-size:0.7rem; font-weight:700; text-transform:uppercase; letter-spacing:0.08em; margin-bottom:16px; }
        .info-grid-4 { display:grid; grid-template-columns:repeat(4,1fr); gap:14px; }
        .info-card { background:var(--surface-2); border:1px solid var(--border); border-radius:8px; padding:14px 18px; }
        .info-label { color:var(--text-light); font-size:0.68rem; text-transform:uppercase; letter-spacing:0.07em; font-weight:600; margin-bottom:4px; }
        .info-value { color:var(--text); font-size:1rem; font-weight:600; }
        .two-col-search { display:grid; grid-template-columns:2fr 1fr; gap:20px; align-items:end; }
        .form-label { display:block; font-size:0.7rem; font-weight:600; text-transform:uppercase; letter-spacing:0.07em; color:var(--text-muted); margin-bottom:6px; }
        select { width:100%; background:var(--off-white) !important; border:1px solid var(--border-strong) !important; color:var(--text) !important; border-radius:7px !important; padding:9px 13px !important; font-family:'DM Sans',sans-serif !important; font-size:0.875rem !important; outline:none !important; transition:border-color 0.15s,box-shadow 0.15s !important; }
        select:focus { border-color:var(--purple) !important; box-shadow:0 0 0 3px rgba(111,84,153,0.12) !important; background:var(--white) !important; }
        .form-actions { display:flex; gap:10px; margin-top:24px; padding-top:20px; border-top:1px solid var(--border); }
        .btn-primary { background:var(--purple) !important; color:white !important; padding:9px 22px !important; border-radius:7px !important; font-family:'DM Sans',sans-serif !important; font-weight:600 !important; font-size:0.85rem !important; cursor:pointer !important; border:none !important; transition:background 0.15s !important; }
        .btn-primary:hover { background:var(--purple-dark) !important; }
        .asp-grid table { width:100%; border-collapse:collapse; }
        .asp-grid th { background:var(--surface-2); color:var(--text-muted); padding:11px 16px; text-align:left; font-size:0.68rem; font-weight:700; text-transform:uppercase; letter-spacing:0.08em; border-bottom:1px solid var(--border); }
        .asp-grid td { padding:12px 16px; border-bottom:1px solid var(--border); color:var(--text); font-size:0.85rem; vertical-align:middle; }
        .asp-grid tr:last-child td { border-bottom:none; }
        .asp-grid tr:hover td { background:var(--purple-faint); }
        .kc-count-badge { background:var(--purple-faint); color:var(--purple); font-size:0.75rem; font-weight:600; padding:3px 10px; border-radius:20px; }
        .rank-1 { color:#c49a00; font-weight:700; font-size:0.875rem; }
        .rank-2 { color:var(--text-muted); font-weight:700; font-size:0.875rem; }
        .rank-3 { color:#8b5e3c; font-weight:700; font-size:0.875rem; }
        .occ-bar-bg { background:var(--border); border-radius:999px; height:6px; width:100%; min-width:80px; }
        .occ-bar { background:var(--purple); border-radius:999px; height:6px; }
        .occ-pct { color:var(--purple); font-weight:700; min-width:46px; font-size:0.85rem; }
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
                <a href="MovieDetails.aspx" class="active">Movies</a>
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
            <h1 class="kc-page-title">Movie Theatre Occupancy Report</h1>
            <p class="kc-page-subtitle">Top 3 theatres with maximum seat occupancy for a selected movie (active tickets only)</p>
        </div>

        
        <div class="kc-card">
            <div class="kc-card-header">
                <span class="kc-card-title">Select Movie</span>
            </div>
            <div class="kc-card-body">
                <div class="two-col-search">
                    <div>
                        <label class="form-label">Movie *</label>
                        <asp:DropDownList ID="ddlMovie" runat="server"></asp:DropDownList>
                    </div>
                    <div>
                        <asp:Button ID="btnSearch" runat="server" Text="Generate Report" CssClass="btn-primary" OnClick="btnSearch_Click" CausesValidation="false" />
                    </div>
                </div>
            </div>
        </div>

        
        <asp:Panel ID="pnlMovieInfo" runat="server" Visible="false">
            <div class="kc-info-panel">
                <p class="kc-info-panel-title">Movie Details</p>
                <div class="info-grid-4">
                    <div class="info-card">
                        <div class="info-label">Title</div>
                        <div class="info-value"><asp:Label ID="lblMovieTitle" runat="server"></asp:Label></div>
                    </div>
                    <div class="info-card">
                        <div class="info-label">Genre</div>
                        <div class="info-value"><asp:Label ID="lblMovieGenre" runat="server"></asp:Label></div>
                    </div>
                    <div class="info-card">
                        <div class="info-label">Language</div>
                        <div class="info-value"><asp:Label ID="lblMovieLang" runat="server"></asp:Label></div>
                    </div>
                    <div class="info-card">
                        <div class="info-label">Duration</div>
                        <div class="info-value"><asp:Label ID="lblMovieDuration" runat="server"></asp:Label></div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        
        <asp:Panel ID="pnlResults" runat="server" Visible="false">
            <div class="kc-card asp-grid">
                <div class="kc-card-header">
                    <span class="kc-card-title">Top 3 Theatres by Occupancy</span>
                    <asp:Label ID="lblCount" runat="server" CssClass="kc-count-badge"></asp:Label>
                </div>
                <asp:GridView ID="gvOccupancy" runat="server"
                    AutoGenerateColumns="False"
                    EmptyDataText="No occupancy data found for this movie."
                    CssClass="w-full">
                    <Columns>
                        <asp:TemplateField HeaderText="Rank">
                            <ItemTemplate>
                                <span class='<%# Container.DataItemIndex == 0 ? "rank-1" : Container.DataItemIndex == 1 ? "rank-2" : "rank-3" %>'>
                                    <%# Container.DataItemIndex == 0 ? "1st" : Container.DataItemIndex == 1 ? "2nd" : "3rd" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="THEATRE_NAME"     HeaderText="Theatre" />
                        <asp:BoundField DataField="THEATRE_LOCATION" HeaderText="Location" />
                        <asp:BoundField DataField="HALL_NAME"        HeaderText="Hall" />
                        <asp:BoundField DataField="HALL_CAPACITY"    HeaderText="Capacity" />
                        <asp:BoundField DataField="ACTIVE_TICKET_COUNT" HeaderText="Seats Occupied" />
                        <asp:BoundField DataField="TOTAL_SHOWS"      HeaderText="Shows" />
                        <asp:TemplateField HeaderText="Occupancy %">
                            <ItemTemplate>
                                <div style="display:flex; align-items:center; gap:10px;">
                                    <span class="occ-pct"><%# Eval("OCCUPANCY_PCT") %>%</span>
                                    <div class="occ-bar-bg">
                                        <div class="occ-bar" style='width:<%# Math.Min(Convert.ToDouble(Eval("OCCUPANCY_PCT")), 100) %>%'></div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

    </div>
    </form>
</body>
</html>
