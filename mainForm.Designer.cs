namespace Quests
{
    partial class mainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            panelQuests = new Panel();
            panelProfiles = new Panel();
            profilesPlaceholder = new Label();
            panelProfiles.SuspendLayout();
            SuspendLayout();
            // 
            // panelQuests
            // 
            panelQuests.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelQuests.AutoScroll = true;
            panelQuests.Location = new Point(0, 0);
            panelQuests.Name = "panelQuests";
            panelQuests.Size = new Size(519, 425);
            panelQuests.TabIndex = 0;
            // 
            // panelProfiles
            // 
            panelProfiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelProfiles.AutoScroll = true;
            panelProfiles.Controls.Add(profilesPlaceholder);
            panelProfiles.Location = new Point(0, 0);
            panelProfiles.Name = "panelProfiles";
            panelProfiles.Size = new Size(519, 599);
            panelProfiles.TabIndex = 1;
            // 
            // profilesPlaceholder
            // 
            profilesPlaceholder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            profilesPlaceholder.Font = new Font("Bender", 13F);
            profilesPlaceholder.Location = new Point(0, 1);
            profilesPlaceholder.Name = "profilesPlaceholder";
            profilesPlaceholder.Padding = new Padding(10, 0, 0, 0);
            profilesPlaceholder.Size = new Size(515, 45);
            profilesPlaceholder.TabIndex = 0;
            profilesPlaceholder.Text = "✔️ Profile placeholder";
            profilesPlaceholder.TextAlign = ContentAlignment.MiddleLeft;
            profilesPlaceholder.Visible = false;
            // 
            // mainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 28, 30);
            ClientSize = new Size(519, 599);
            Controls.Add(panelProfiles);
            Controls.Add(panelQuests);
            Font = new Font("Bender", 12F, FontStyle.Bold);
            ForeColor = Color.LightGray;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "mainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quests";
            Load += mainForm_Load;
            LocationChanged += mainForm_LocationChanged;
            KeyDown += mainForm_KeyDown;
            Resize += mainForm_Resize;
            panelProfiles.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelQuests;
        private Panel panelProfiles;
        private Label profilesPlaceholder;
    }
}
