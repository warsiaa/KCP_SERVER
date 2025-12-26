using ScottPlot.WinForms;

namespace KCP_SERVER
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblClientCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.formsPlotMetrics = new FormsPlot();

            this.SuspendLayout();

            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Size = new System.Drawing.Size(94, 29);
            this.btnStart.Text = "Başlat";
            this.btnStart.Click += new System.EventHandler(this.StartButton_Click);

            this.btnStop.Location = new System.Drawing.Point(112, 12);
            this.btnStop.Size = new System.Drawing.Size(94, 29);
            this.btnStop.Text = "Durdur";
            this.btnStop.Click += new System.EventHandler(this.StopButton_Click);

            this.txtLog.Location = new System.Drawing.Point(12, 110);
            this.txtLog.Multiline = true;
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(776, 182);

            this.lblStatus.Location = new System.Drawing.Point(12, 73);
            this.lblStatus.Text = "Durum: Kapalı";

            this.label1.Location = new System.Drawing.Point(230, 73);
            this.label1.Text = "Bağlı İstemci:";

            this.lblClientCount.Location = new System.Drawing.Point(346, 73);
            this.lblClientCount.Text = "0";

            this.label2.Location = new System.Drawing.Point(230, 18);
            this.label2.Text = "Port:";

            this.txtPort.Location = new System.Drawing.Point(278, 15);
            this.txtPort.Size = new System.Drawing.Size(94, 27);
            this.txtPort.Text = "7777";

            this.formsPlotMetrics.Location = new System.Drawing.Point(12, 298);
            this.formsPlotMetrics.Size = new System.Drawing.Size(776, 304);

            this.ClientSize = new System.Drawing.Size(800, 614);
            this.Controls.Add(this.formsPlotMetrics);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblClientCount);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);

            this.Text = "KCP Sunucu";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblClientCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private FormsPlot formsPlotMetrics;
    }
}
