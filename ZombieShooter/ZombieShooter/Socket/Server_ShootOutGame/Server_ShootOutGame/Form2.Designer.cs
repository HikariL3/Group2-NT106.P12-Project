namespace Server_ShootOutGame
{
    partial class Form2
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
            this.ShowingInfo = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // ShowingInfo
            // 
            this.ShowingInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ShowingInfo.Location = new System.Drawing.Point(0, 0);
            this.ShowingInfo.Name = "ShowingInfo";
            this.ShowingInfo.ReadOnly = true;
            this.ShowingInfo.Size = new System.Drawing.Size(800, 450);
            this.ShowingInfo.TabIndex = 0;
            this.ShowingInfo.Text = "";
            this.ShowingInfo.TextChanged += new System.EventHandler(this.ShowingInfo_TextChanged);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ShowingInfo);
            this.Name = "Form2";
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ShowingInfo;
    }
}