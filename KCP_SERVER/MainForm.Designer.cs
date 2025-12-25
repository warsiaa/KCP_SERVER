using System.Windows.Forms.DataVisualization.Charting;

namespace KCP_SERVER
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ChartArea chartArea1 = new ChartArea();
            Legend legend1 = new Legend();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblClientCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.chartMetrics = new Chart();
            this.seriesPacketLoss = new Series();
            this.seriesLatency = new Series();
            this.seriesTimeouts = new Series();
            ((System.ComponentModel.ISupportInitialize)(this.chartMetrics)).BeginInit();
            this.SuspendLayout();
            //
            // btnStart
            //
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(94, 29);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Başlat";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(112, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(94, 29);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Durdur";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 110);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(776, 182);
            this.txtLog.TabIndex = 2;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 73);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 20);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Durum: Kapalı";
            // 
            // lblClientCount
            // 
            this.lblClientCount.AutoSize = true;
            this.lblClientCount.Location = new System.Drawing.Point(346, 73);
            this.lblClientCount.Name = "lblClientCount";
            this.lblClientCount.Size = new System.Drawing.Size(17, 20);
            this.lblClientCount.TabIndex = 4;
            this.lblClientCount.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(230, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Bağlı İstemci: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(230, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(278, 15);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(94, 27);
            this.txtPort.TabIndex = 7;
            this.txtPort.Text = "7777";
            //
            // chartMetrics
            //
            this.chartMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right))));
            chartArea1.AxisX.IsMarginVisible = false;
            chartArea1.AxisX.LabelStyle.Format = "HH:mm:ss";
            chartArea1.Name = "ChartArea1";
            this.chartMetrics.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartMetrics.Legends.Add(legend1);
            this.chartMetrics.Location = new System.Drawing.Point(12, 298);
            this.chartMetrics.Name = "chartMetrics";
            this.chartMetrics.Size = new System.Drawing.Size(776, 304);
            this.chartMetrics.TabIndex = 8;
            this.chartMetrics.Text = "Ağ İstatistikleri";
            //
            // seriesPacketLoss
            //
            this.seriesPacketLoss.ChartArea = "ChartArea1";
            this.seriesPacketLoss.ChartType = SeriesChartType.Line;
            this.seriesPacketLoss.Legend = "Legend1";
            this.seriesPacketLoss.Name = "Paket Kaybı";
            //
            // seriesLatency
            //
            this.seriesLatency.ChartArea = "ChartArea1";
            this.seriesLatency.ChartType = SeriesChartType.Line;
            this.seriesLatency.Legend = "Legend1";
            this.seriesLatency.Name = "Gecikme (ms)";
            //
            // seriesTimeouts
            //
            this.seriesTimeouts.ChartArea = "ChartArea1";
            this.seriesTimeouts.ChartType = SeriesChartType.Line;
            this.seriesTimeouts.Legend = "Legend1";
            this.seriesTimeouts.Name = "Timeout";
            this.chartMetrics.Series.Add(this.seriesPacketLoss);
            this.chartMetrics.Series.Add(this.seriesLatency);
            this.chartMetrics.Series.Add(this.seriesTimeouts);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 614);
            this.Controls.Add(this.chartMetrics);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblClientCount);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "MainForm";
            this.Text = "KCP Sunucu";
            ((System.ComponentModel.ISupportInitialize)(this.chartMetrics)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblClientCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private Chart chartMetrics;
        private Series seriesPacketLoss;
        private Series seriesLatency;
        private Series seriesTimeouts;
    }
}
