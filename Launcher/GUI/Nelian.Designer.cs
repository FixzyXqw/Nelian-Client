namespace Nelian
{
    partial class Nelian
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Nelian));
            Client = new Panel();
            SuspendLayout();
            // 
            // Client
            // 
            Client.Dock = DockStyle.Fill;
            Client.Location = new Point(0, 0);
            Client.Name = "Client";
            Client.Size = new Size(884, 521);
            Client.TabIndex = 0;
            Client.Paint += Client_Paint;
            // 
            // Nelian
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 521);
            Controls.Add(Client);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Nelian";
            Text = "Nelian";
            Load += Nelian_Load;
            ResumeLayout(false);
        }

        #endregion

        private Panel Client;
    }
}
