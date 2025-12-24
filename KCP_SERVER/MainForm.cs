using System;
using System.Windows.Forms;
using KCP_SERVER.Network;

namespace KCP_SERVER
{
    public partial class MainForm : Form
    {
        private KcpServer? _server;

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
