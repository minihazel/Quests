namespace Quests
{
    partial class messageWindow
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
            messageText = new Label();
            btnCancel = new Button();
            btnOK = new Button();
            SuspendLayout();
            // 
            // messageText
            // 
            messageText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messageText.Font = new Font("Bender", 14F);
            messageText.Location = new Point(22, 21);
            messageText.Name = "messageText";
            messageText.Padding = new Padding(10);
            messageText.Size = new Size(451, 121);
            messageText.TabIndex = 0;
            messageText.Text = "Placeholder text";
            messageText.MouseDown += messageText_MouseDown;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Bender", 14F);
            btnCancel.Location = new Point(333, 171);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(150, 45);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Cursor = Cursors.Hand;
            btnOK.FlatAppearance.BorderColor = SystemColors.WindowFrame;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Font = new Font("Bender", 14F);
            btnOK.Location = new Point(168, 171);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(150, 45);
            btnOK.TabIndex = 2;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // messageWindow
            // 
            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 28, 30);
            ClientSize = new Size(500, 233);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(messageText);
            Cursor = Cursors.Hand;
            Font = new Font("Bender", 12F, FontStyle.Bold);
            ForeColor = Color.LightGray;
            FormBorderStyle = FormBorderStyle.None;
            Name = "messageWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Load += messageWindow_Load;
            Paint += messageWindow_Paint;
            MouseDown += messageWindow_MouseDown;
            ResumeLayout(false);
        }

        #endregion

        public Label messageText;
        private Button btnCancel;
        private Button btnOK;
    }
}