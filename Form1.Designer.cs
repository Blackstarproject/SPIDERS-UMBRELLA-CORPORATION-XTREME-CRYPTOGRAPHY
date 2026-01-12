namespace SPIDERS_UMBRELLA_CORPORATION.UI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.consoleList = new System.Windows.Forms.ListBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();

            // Custom Controls
            this.dnaSpinner = new SPIDERS_UMBRELLA_CORPORATION.UI.Controls.DnaHelixSpinner();
            this.sonarRadar = new SPIDERS_UMBRELLA_CORPORATION.UI.Controls.SonarRadar();
            this.ekgMonitor = new SPIDERS_UMBRELLA_CORPORATION.UI.Controls.EkgMonitor();
            this.btnEncrypt = new SPIDERS_UMBRELLA_CORPORATION.UI.Controls.UmbrellaButton();
            this.btnDecrypt = new SPIDERS_UMBRELLA_CORPORATION.UI.Controls.UmbrellaButton();

            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();

            // Header
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.pnlHeader.Controls.Add(this.btnClose);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height = 40;
            this.pnlHeader.MouseDown += Header_MouseDown;
            this.pnlHeader.MouseMove += Header_MouseMove;
            this.pnlHeader.MouseUp += Header_MouseUp;

            // Title
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Red;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Text = "UMBRELLA CORP :: HIVE TERMINAL";

            // Close Button
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.Gray;
            this.btnClose.Location = new System.Drawing.Point(960, 5);
            this.btnClose.Size = new System.Drawing.Size(30, 30);
            this.btnClose.Text = "X";
            this.btnClose.Click += (s, e) => System.Windows.Forms.Application.Exit();

            // Console List
            this.consoleList.BackColor = System.Drawing.Color.Black;
            this.consoleList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consoleList.Font = new System.Drawing.Font("Consolas", 9F);
            this.consoleList.ForeColor = System.Drawing.Color.LimeGreen;
            this.consoleList.Location = new System.Drawing.Point(20, 60);
            this.consoleList.Size = new System.Drawing.Size(500, 400);

            // DNA Spinner
            this.dnaSpinner.Location = new System.Drawing.Point(850, 60);
            this.dnaSpinner.Size = new System.Drawing.Size(130, 500);
            this.dnaSpinner.HelixColor = System.Drawing.Color.Red;

            // Sonar Radar
            this.sonarRadar.Location = new System.Drawing.Point(540, 60);
            this.sonarRadar.Size = new System.Drawing.Size(200, 200);

            // EKG Monitor
            this.ekgMonitor.Location = new System.Drawing.Point(540, 280);
            this.ekgMonitor.Size = new System.Drawing.Size(290, 100);

            // Buttons
            this.btnEncrypt.BackColor = System.Drawing.Color.DarkRed;
            this.btnEncrypt.Text = "DEPLOY T-VIRUS";
            this.btnEncrypt.Location = new System.Drawing.Point(20, 480);
            this.btnEncrypt.Size = new System.Drawing.Size(350, 50);
            this.btnEncrypt.Click += BtnEncrypt_Click;

            // Decrypt
            this.btnDecrypt.BackColor = System.Drawing.Color.Navy;
            this.btnDecrypt.Text = "INJECT ANTI-VIRUS";
            this.btnDecrypt.Location = new System.Drawing.Point(390, 480);
            this.btnDecrypt.Size = new System.Drawing.Size(350, 50);
            this.btnDecrypt.Click += BtnDecrypt_Click;

            // Form
            this.BackColor = System.Drawing.Color.FromArgb(10, 10, 10);
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.dnaSpinner);
            this.Controls.Add(this.sonarRadar);
            this.Controls.Add(this.ekgMonitor);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.consoleList);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ListBox consoleList;
        private SPIDERS_UMBRELLA_CORPORATION.UI.Controls.DnaHelixSpinner dnaSpinner;
        private SPIDERS_UMBRELLA_CORPORATION.UI.Controls.SonarRadar sonarRadar;
        private SPIDERS_UMBRELLA_CORPORATION.UI.Controls.EkgMonitor ekgMonitor;
        private SPIDERS_UMBRELLA_CORPORATION.UI.Controls.UmbrellaButton btnEncrypt;
        private SPIDERS_UMBRELLA_CORPORATION.UI.Controls.UmbrellaButton btnDecrypt;
    }
}