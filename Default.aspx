<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CinemaApp.Default" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Kumari Cinemas — Dashboard</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style>
        :root { --purple:#6f5499; --purple-dark:#5a4180; --purple-faint:#f0ecf8; --purple-pale:#e8e1f5; --white:#ffffff; --off-white:#fafaf9; --text:#1a1522; --text-muted:#6b6478; --text-light:#9b93a8; --border:#e2dced; --border-strong:#c8bedd; --surface-2:#f6f3fc; --danger:#c0392b; }
        * { box-sizing:border-box; margin:0; padding:0; }
        body { background:var(--off-white); font-family:'Trebuchet MS','Segoe UI',system-ui,sans-serif; color:var(--text); font-size:0.9rem; line-height:1.6; }        
        .kc-nav { background:var(--white); border-bottom:1px solid var(--border); position:sticky; top:0; z-index:100; }
        .kc-nav-inner { max-width:1300px; margin:0 auto; padding:0 32px; height:60px; display:flex; align-items:center; justify-content:space-between; }
        .kc-logo { font-family:Verdana,'Times New Roman',serif; font-weight:800; font-size:1.1rem; color:var(--purple); text-decoration:none; letter-spacing:-0.02em; }
        .kc-nav-links { display:flex; align-items:center; gap:2px; }
        .kc-nav-links a { color:var(--text-muted); text-decoration:none; padding:6px 12px; border-radius:6px; font-size:0.825rem; font-weight:500; transition:color 0.15s,background 0.15s; }
        .kc-nav-links a:hover { color:var(--purple); background:var(--purple-faint); }
        .kc-nav-links a.active { color:var(--purple); background:var(--purple-faint); font-weight:600; }
        .kc-page { max-width:1300px; margin:0 auto; padding:40px 32px; }
        .kc-page-header { margin-bottom:32px; padding-bottom:24px; border-bottom:1px solid var(--border); }
        .kc-page-eyebrow { font-size:0.68rem; font-weight:700; letter-spacing:0.14em; text-transform:uppercase; color:var(--purple); margin-bottom:6px; }
        .kc-page-title { font-family:'Verdana',serif; font-size:1.75rem; font-weight:700; color:var(--text); letter-spacing:-0.03em; margin-bottom:5px; }
        .kc-page-subtitle { color:var(--text-muted); font-size:0.875rem; }
        .main-grid { display:grid; grid-template-columns:55% 1fr; gap:24px; margin-bottom:32px; }
        .stat-table-card { background:var(--white); border:1px solid var(--border); border-radius:10px; overflow:hidden; align-self:start; }
        .stat-table-card table { width:100%; border-collapse:collapse; }
        .stat-table-card thead th { background:var(--surface-2); padding:10px 20px; text-align:left; font-size:0.65rem; font-weight:700; text-transform:uppercase; letter-spacing:0.09em; color:var(--text-light); border-bottom:1px solid var(--border); }
        .stat-table-card tbody tr { border-bottom:1px solid var(--border); cursor:pointer; }
        .stat-table-card tbody tr:last-child { border-bottom:none; }
        .stat-table-card tbody tr:hover td { background:var(--purple-faint); }
        .stat-table-card tbody td { padding:14px 20px; font-size:0.85rem; vertical-align:middle; }
        .td-name { font-weight:600; color:var(--text); }
        .td-desc { font-size:0.73rem; color:var(--text-light); margin-top:1px; }
        .td-count { font-family:'Verdana',sans-serif; font-size:1.1rem; font-weight:700; color:var(--purple); text-align:right; white-space:nowrap; }
        .td-arrow { text-align:right; color:var(--text-light); font-size:0.8rem; width:24px; }
        .stat-table-card tbody tr:hover .td-arrow { color:var(--purple); }
        .right-col { display:flex; flex-direction:column; gap:16px; }
        .capacity-card { background:var(--purple); border-radius:10px; overflow:hidden; }
        .capacity-top { display:flex; align-items:flex-start; justify-content:space-between; padding:22px 24px 16px; }
        .capacity-lbl { font-size:0.65rem; font-weight:700; text-transform:uppercase; letter-spacing:0.1em; color:rgba(255,255,255,0.5); margin-bottom:4px; }
        .capacity-val { font-family:'Verdana',sans-serif; font-size:2rem; font-weight:700; color:white; line-height:1; margin-bottom:2px; }
        .capacity-sub { font-size:0.7rem; color:rgba(255,255,255,0.45); }
        .capacity-badge { background:rgba(255,255,255,0.12); border-radius:8px; padding:10px 14px; text-align:center; }
        .capacity-badge-lbl { font-size:0.6rem; font-weight:700; text-transform:uppercase; letter-spacing:0.08em; color:rgba(255,255,255,0.5); margin-bottom:4px; }
        .capacity-badge-val { font-family:'Verdana',sans-serif; font-size:1.4rem; font-weight:700; color:white; line-height:1; }
        .capacity-rows { border-top:1px solid rgba(255,255,255,0.12); }
        .cap-row { display:flex; align-items:center; justify-content:space-between; padding:10px 24px; border-bottom:1px solid rgba(255,255,255,0.08); }
        .cap-row:last-child { border-bottom:none; }
        .cap-row-lbl { font-size:0.78rem; color:rgba(255,255,255,0.6); }
        .cap-row-val { font-family:'Verdana',sans-serif; font-size:0.9rem; font-weight:700; color:white; }
        .reports-card { background:var(--white); border:1px solid var(--border); border-radius:10px; overflow:hidden; }
        .reports-card-hdr { padding:12px 20px; border-bottom:1px solid var(--border); font-family:'Verdana',sans-serif; font-size:0.78rem; font-weight:700; color:var(--text); }
        .report-row { display:flex; align-items:center; justify-content:space-between; padding:13px 20px; border-bottom:1px solid var(--border); text-decoration:none; }
        .report-row:last-child { border-bottom:none; }
        .report-row:hover { background:var(--purple-faint); }
        .report-row-left { display:flex; align-items:center; gap:12px; }
        .report-row-dot { width:7px; height:7px; background:var(--purple); border-radius:50%; flex-shrink:0; opacity:0.35; }
        .report-row:hover .report-row-dot { opacity:1; }
        .report-row-name { font-size:0.83rem; font-weight:600; color:var(--text); margin-bottom:1px; }
        .report-row-desc { font-size:0.7rem; color:var(--text-light); }
        .report-row-arrow { font-size:0.75rem; color:var(--text-light); }
        .report-row:hover .report-row-arrow { color:var(--purple); }
        .charts-section { margin-top:32px; }
        .charts-section-title { font-family:'Verdana',serif; font-size:1rem; font-weight:700; color:var(--text); margin-bottom:16px; padding-bottom:12px; border-bottom:1px solid var(--border); }
        .charts-grid { display:grid; grid-template-columns:1fr 1fr 1fr; gap:24px; }
        .chart-card { background:var(--white); border:1px solid var(--border); border-radius:10px; padding:24px; }
        .chart-card-title { font-size:0.68rem; font-weight:700; text-transform:uppercase; letter-spacing:0.1em; color:var(--text-light); margin-bottom:4px; }
        .chart-card-subtitle { font-size:0.78rem; color:var(--text-muted); margin-bottom:20px; }
        .chart-wrap { position:relative; height:220px; }
    </style>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/4.4.1/chart.umd.min.js"></script>
</head>
<body>

    <nav class="kc-nav">
        <div class="kc-nav-inner">
            <a href="Default.aspx" class="kc-logo">Kumari Cinemas</a>
            <div class="kc-nav-links">
                <a href="Default.aspx" class="active">Dashboard</a>
                <a href="CustomerDetails.aspx">Customers</a>
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
            <p class="kc-page-eyebrow">Management System</p>
            <h1 class="kc-page-title">Dashboard</h1>
            <p class="kc-page-subtitle">Overview of your cinema operations</p>
        </div>

        
        <div style="display:none;">
            <asp:Label ID="lblCustomers"    runat="server">0</asp:Label>
            <asp:Label ID="lblTheatres"     runat="server">0</asp:Label>
            <asp:Label ID="lblMovies"       runat="server">0</asp:Label>
            <asp:Label ID="lblShows"        runat="server">0</asp:Label>
            <asp:Label ID="lblTickets"      runat="server">0</asp:Label>
            <asp:Label ID="lblHalls"        runat="server">0</asp:Label>
            <asp:Label ID="lblTotalCapacity" runat="server">0</asp:Label>
            <asp:Label ID="lblActiveTickets" runat="server">0</asp:Label>
        </div>

        <div class="main-grid">

            
            <div class="stat-table-card">
                <table>
                    <thead>
                        <tr>
                            <th>Section</th>
                            <th style="text-align:right;">Count</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr onclick="window.location='CustomerDetails.aspx'">
                            <td><p class="td-name">Customers</p><p class="td-desc">Manage customer records</p></td>
                            <td class="td-count"><span id="m_customers">—</span></td>
                            <td class="td-arrow">&#8594;</td>
                        </tr>
                        <tr onclick="window.location='TheaterDetails.aspx'">
                            <td><p class="td-name">Theatres</p><p class="td-desc">Manage cinema venues</p></td>
                            <td class="td-count"><span id="m_theatres">—</span></td>
                            <td class="td-arrow">&#8594;</td>
                        </tr>
                        <tr onclick="window.location='MovieDetails.aspx'">
                            <td><p class="td-name">Movies</p><p class="td-desc">Manage the film catalogue</p></td>
                            <td class="td-count"><span id="m_movies">—</span></td>
                            <td class="td-arrow">&#8594;</td>
                        </tr>
                        <tr onclick="window.location='ShowDetails.aspx'">
                            <td><p class="td-name">Shows</p><p class="td-desc">Manage show schedules</p></td>
                            <td class="td-count"><span id="m_shows">—</span></td>
                            <td class="td-arrow">&#8594;</td>
                        </tr>
                        <tr onclick="window.location='TicketDetails.aspx'">
                            <td><p class="td-name">Tickets</p><p class="td-desc">Manage issued tickets</p></td>
                            <td class="td-count"><span id="m_tickets">—</span></td>
                            <td class="td-arrow">&#8594;</td>
                        </tr>
                        <tr onclick="window.location='HallDetails.aspx'">
                            <td><p class="td-name">Halls</p><p class="td-desc">Manage cinema halls</p></td>
                            <td class="td-count"><span id="m_halls">—</span></td>
                            <td class="td-arrow">&#8594;</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            
            <div class="right-col">

                <div class="capacity-card">
                    <div class="capacity-top">
                        <div>
                            <p class="capacity-lbl">Total Seat Capacity</p>
                            <p class="capacity-val"><span id="r_capacity">—</span></p>
                            <p class="capacity-sub">seats across all halls</p>
                        </div>
                        <div class="capacity-badge">
                            <p class="capacity-badge-lbl">Active Tickets</p>
                            <p class="capacity-badge-val"><span id="r_active">—</span></p>
                        </div>
                    </div>
                    <div class="capacity-rows">
                        <div class="cap-row">
                            <span class="cap-row-lbl">Total tickets issued</span>
                            <span class="cap-row-val"><span id="r_tickets">—</span></span>
                        </div>
                        <div class="cap-row">
                            <span class="cap-row-lbl">Halls registered</span>
                            <span class="cap-row-val"><span id="r_halls">—</span></span>
                        </div>
                        <div class="cap-row">
                            <span class="cap-row-lbl">Movies in catalogue</span>
                            <span class="cap-row-val"><span id="r_movies">—</span></span>
                        </div>
                        <div class="cap-row">
                            <span class="cap-row-lbl">Shows scheduled</span>
                            <span class="cap-row-val"><span id="r_shows">—</span></span>
                        </div>
                        <div class="cap-row">
                            <span class="cap-row-lbl">Customers registered</span>
                            <span class="cap-row-val"><span id="r_customers">—</span></span>
                        </div>
                        <div class="cap-row">
                            <span class="cap-row-lbl">Theatres</span>
                            <span class="cap-row-val"><span id="r_theatres">—</span></span>
                        </div>
                    </div>
                </div>

                <div class="reports-card">
                    <div class="reports-card-hdr">Complex Reports</div>
                    <a href="CustomerTicket.aspx" class="report-row">
                        <div class="report-row-left">
                            <div class="report-row-dot"></div>
                            <div>
                                <p class="report-row-name">Customer Ticket Report</p>
                                <p class="report-row-desc">Show history per customer over 6 months</p>
                            </div>
                        </div>
                        <span class="report-row-arrow">&#8594;</span>
                    </a>
                    <a href="TheatreMovie.aspx" class="report-row">
                        <div class="report-row-left">
                            <div class="report-row-dot"></div>
                            <div>
                                <p class="report-row-name">Theatre Movie Report</p>
                                <p class="report-row-desc">Movies and showtimes per theatre</p>
                            </div>
                        </div>
                        <span class="report-row-arrow">&#8594;</span>
                    </a>
                    <a href="MovieOccupancy.aspx" class="report-row">
                        <div class="report-row-left">
                            <div class="report-row-dot"></div>
                            <div>
                                <p class="report-row-name">Movie Occupancy Report</p>
                                <p class="report-row-desc">Top 3 theatres by seat occupancy per movie</p>
                            </div>
                        </div>
                        <span class="report-row-arrow">&#8594;</span>
                    </a>
                </div>

            </div>
        </div>

        <script>
            window.addEventListener('DOMContentLoaded', function () {
                function mirror(srcId, destId) {
                    var src = document.getElementById(srcId);
                    var dest = document.getElementById(destId);
                    if (src && dest) dest.textContent = src.textContent.trim();
                }
                mirror('<%= lblCustomers.ClientID %>', 'm_customers');
                mirror('<%= lblTheatres.ClientID %>', 'm_theatres');
                mirror('<%= lblMovies.ClientID %>', 'm_movies');
                mirror('<%= lblShows.ClientID %>', 'm_shows');
                mirror('<%= lblTickets.ClientID %>', 'm_tickets');
                mirror('<%= lblHalls.ClientID %>', 'm_halls');
                mirror('<%= lblTotalCapacity.ClientID %>', 'r_capacity');
                mirror('<%= lblActiveTickets.ClientID %>', 'r_active');
                mirror('<%= lblTickets.ClientID %>', 'r_tickets');
                mirror('<%= lblHalls.ClientID %>', 'r_halls');
                mirror('<%= lblMovies.ClientID %>', 'r_movies');
                mirror('<%= lblShows.ClientID %>', 'r_shows');
                mirror('<%= lblCustomers.ClientID %>',    'r_customers');
                mirror('<%= lblTheatres.ClientID %>', 'r_theatres');
            });
        </script>


        
        <div style="display:none;">
            <asp:Label ID="lblActiveTickets2"    runat="server">0</asp:Label>
            <asp:Label ID="lblUsedTickets"       runat="server">0</asp:Label>
            <asp:Label ID="lblCancelledTickets"  runat="server">0</asp:Label>
            <asp:Label ID="lblHallsJson"         runat="server">[]</asp:Label>
        </div>

        
        <div class="charts-section">
            <p class="charts-section-title">Statistics &amp; Charts</p>
            <div class="charts-grid">

                
                <div class="chart-card">
                    <p class="chart-card-title">Ticket Status</p>
                    <p class="chart-card-subtitle">Breakdown of all issued tickets</p>
                    <div class="chart-wrap">
                        <canvas id="chartTicketStatus"></canvas>
                    </div>
                </div>

                
                <div class="chart-card">
                    <p class="chart-card-title">Records Overview</p>
                    <p class="chart-card-subtitle">Count per management area</p>
                    <div class="chart-wrap">
                        <canvas id="chartOverview"></canvas>
                    </div>
                </div>

                
                <div class="chart-card">
                    <p class="chart-card-title">Hall Capacities</p>
                    <p class="chart-card-subtitle">Seats available per hall</p>
                    <div class="chart-wrap">
                        <canvas id="chartHallCap"></canvas>
                    </div>
                </div>

            </div>
        </div>

        <script>
        window.addEventListener('DOMContentLoaded', function () {

            var purple      = '#6f5499';
            var purpleMid   = '#9b7fd4';
            var purpleLight = '#c5b3e8';
            var purpleFaint = '#e8e1f5';
            var danger      = '#c0392b';
            var muted       = '#9b93a8';

            function val(id) {
                var el = document.getElementById(id);
                return el ? parseInt(el.textContent.trim()) || 0 : 0;
            }
            function strVal(id) {
                var el = document.getElementById(id);
                return el ? el.textContent.trim() : '[]';
            }

            
            var active    = val('<%= lblActiveTickets2.ClientID %>');
            var used      = val('<%= lblUsedTickets.ClientID %>');
            var cancelled = val('<%= lblCancelledTickets.ClientID %>');

            new Chart(document.getElementById('chartTicketStatus'), {
                type: 'doughnut',
                data: {
                    labels: ['Active', 'Used', 'Cancelled'],
                    datasets: [{
                        data: [active, used, cancelled],
                        backgroundColor: [purple, purpleMid, purpleLight],
                        borderWidth: 2,
                        borderColor: '#fff'
                    }]
                },
                options: {
                    responsive: true, maintainAspectRatio: false,
                    plugins: {
                        legend: { position: 'bottom', labels: { font: { size: 11 }, padding: 14 } }
                    },
                    cutout: '62%'
                }
            });

            
            new Chart(document.getElementById('chartOverview'), {
                type: 'bar',
                data: {
                    labels: ['Customers','Theatres','Movies','Shows','Tickets','Halls'],
                    datasets: [{
                        label: 'Records',
                        data: [
                            val('<%= lblCustomers.ClientID %>'),
                            val('<%= lblTheatres.ClientID %>'),
                            val('<%= lblMovies.ClientID %>'),
                            val('<%= lblShows.ClientID %>'),
                            val('<%= lblTickets.ClientID %>'),
                            val('<%= lblHalls.ClientID %>')
                        ],
                        backgroundColor: [purple,purpleMid,purpleLight,purple,purpleMid,purpleLight],
                        borderRadius: 5,
                        borderSkipped: false
                    }]
                },
                options: {
                    responsive: true, maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { beginAtZero: true, ticks: { precision: 0, font: { size: 10 } }, grid: { color: '#f0ecf8' } },
                        x: { ticks: { font: { size: 10 } }, grid: { display: false } }
                    }
                }
            });

            
            var hallsRaw = strVal('<%= lblHallsJson.ClientID %>');
            var halls = [];
            try { halls = JSON.parse(hallsRaw); } catch (e) { halls = []; }

            var hallLabels = halls.map(function (h) { return h.name; });
            var hallData = halls.map(function (h) { return h.cap; });

            new Chart(document.getElementById('chartHallCap'), {
                type: 'bar',
                data: {
                    labels: hallLabels.length ? hallLabels : ['No halls yet'],
                    datasets: [{
                        label: 'Capacity',
                        data: hallData.length ? hallData : [0],
                        backgroundColor: purple,
                        borderRadius: 5,
                        borderSkipped: false
                    }]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true, maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        x: { beginAtZero: true, ticks: { precision: 0, font: { size: 10 } }, grid: { color: '#f0ecf8' } },
                        y: { ticks: { font: { size: 10 } }, grid: { display: false } }
                    }
                }
            });
        });
        </script>

    </div>
    </form>
</body>
</html>
