using System;
using System.Windows.Forms;
using KCP_SERVER.Network;

namespace KCP_SERVER
{
    public partial class MainForm : Form
    {
        private KcpServer? _server;
        private const int MaxChartPoints = 120;

        public MainForm()
        {
            InitializeComponent();
            UpdateUiState();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtPort.Text, out var port) || port <= 0 || port > 65535)
            {
                MessageBox.Show("Geçerli bir port numarası girin (1-65535).", "Hatalı Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_server != null)
                return;

            _server = new KcpServer(port);
            _server.Log += OnServerLog;
            _server.ClientCountChanged += OnClientCountChanged;
            _server.MetricsUpdated += OnMetricsUpdated;
            _server.Start();

            AppendLog($"Sunucu {port} portunda başlatıldı.");
            UpdateUiState();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (_server == null)
                return;

            _server.Stop();
            _server.Dispose();
            _server = null;

            AppendLog("Sunucu durduruldu.");
            OnClientCountChanged(0);
            ClearCharts();
            UpdateUiState();
        }

        private void OnServerLog(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(OnServerLog), message);
                return;
            }

            AppendLog(message);
        }

        private void OnClientCountChanged(int count)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(OnClientCountChanged), count);
                return;
            }

            lblClientCount.Text = count.ToString();
        }

        private void OnMetricsUpdated(MetricSample sample)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<MetricSample>(OnMetricsUpdated), sample);
                return;
            }

            AddChartPoint(seriesPacketLoss, sample.Timestamp, sample.PacketLoss);
            AddChartPoint(seriesLatency, sample.Timestamp, sample.AverageLatencyMs);
            AddChartPoint(seriesTimeouts, sample.Timestamp, sample.TimeoutCount);
        }

        private void AddChartPoint(System.Windows.Forms.DataVisualization.Charting.Series series, DateTime timestamp, double value)
        {
            series.Points.AddXY(timestamp, value);

            while (series.Points.Count > MaxChartPoints)
            {
                series.Points.RemoveAt(0);
            }

            chartMetrics.ChartAreas[0].RecalculateAxesScale();
        }

        private void AppendLog(string message)
        {
            if (txtLog.TextLength > 0)
            {
                txtLog.AppendText(Environment.NewLine);
            }

            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        private void UpdateUiState()
        {
            bool running = _server != null;
            btnStart.Enabled = !running;
            btnStop.Enabled = running;
            txtPort.Enabled = !running;
            lblStatus.Text = running ? "Durum: Çalışıyor" : "Durum: Kapalı";
        }

        private void ClearCharts()
        {
            seriesPacketLoss.Points.Clear();
            seriesLatency.Points.Clear();
            seriesTimeouts.Points.Clear();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (_server != null)
            {
                _server.Stop();
                _server.Dispose();
                _server = null;
            }
        }
    }
}
