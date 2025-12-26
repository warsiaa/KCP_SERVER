using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KCP_SERVER.Network;
using ScottPlot;
using ScottPlot.Plottables;

namespace KCP_SERVER
{
    public partial class MainForm : Form
    {
        private KcpServer? _server;
        private const int MaxChartPoints = 120;
        private readonly List<double> _timePoints = new();
        private readonly List<double> _packetLossPoints = new();
        private readonly List<double> _latencyPoints = new();
        private readonly List<double> _timeoutPoints = new();
        private Scatter? _packetLossScatter;
        private Scatter? _latencyScatter;
        private Scatter? _timeoutScatter;

        public MainForm()
        {
            InitializeComponent();
            ConfigureChart();
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

        private void ConfigureChart()
        {
            formsPlotMetrics.Plot.Title("Ağ İstatistikleri");
            formsPlotMetrics.Plot.XLabel("Zaman");
            formsPlotMetrics.Plot.YLabel("Değer");
            formsPlotMetrics.Plot.Legend.IsVisible = true;
            formsPlotMetrics.Plot.Axes.DateTimeTicksBottom();

            RenderChart();
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

            AddChartPoint(sample.Timestamp, sample.PacketLoss, sample.AverageLatencyMs, sample.TimeoutCount);
        }

        private void AddChartPoint(DateTime timestamp, double packetLoss, double latency, double timeouts)
        {
            double timeValue = timestamp.ToOADate();

            _timePoints.Add(timeValue);
            _packetLossPoints.Add(packetLoss);
            _latencyPoints.Add(latency);
            _timeoutPoints.Add(timeouts);

            TrimChartPoints();

            RenderChart();
        }

        private void TrimChartPoints()
        {
            while (_timePoints.Count > MaxChartPoints)
            {
                _timePoints.RemoveAt(0);
                _packetLossPoints.RemoveAt(0);
                _latencyPoints.RemoveAt(0);
                _timeoutPoints.RemoveAt(0);
            }
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
            _timePoints.Clear();
            _packetLossPoints.Clear();
            _latencyPoints.Clear();
            _timeoutPoints.Clear();

            RenderChart();
        }

        private void RenderChart()
        {
            var times = _timePoints.ToArray();
            var packetLossValues = _packetLossPoints.ToArray();
            var latencyValues = _latencyPoints.ToArray();
            var timeoutValues = _timeoutPoints.ToArray();

            formsPlotMetrics.Plot.Clear();

            _packetLossScatter = formsPlotMetrics.Plot.Add.Scatter(times, packetLossValues, label: "Paket Kaybı", color: Colors.OrangeRed);
            _latencyScatter = formsPlotMetrics.Plot.Add.Scatter(times, latencyValues, label: "Gecikme (ms)", color: Colors.DeepSkyBlue);
            _timeoutScatter = formsPlotMetrics.Plot.Add.Scatter(times, timeoutValues, label: "Timeout", color: Colors.ForestGreen);

            formsPlotMetrics.Plot.Axes.AutoScale();
            formsPlotMetrics.Refresh();
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
