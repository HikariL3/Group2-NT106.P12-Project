namespace GameForm
{
    partial class Form1
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
            this.password = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.TextBox();
            this.gameName = new System.Windows.Forms.Label();
            this.ZombieIcon = new System.Windows.Forms.PictureBox();
            this.backgroundImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ZombieIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundImg)).BeginInit();
            this.SuspendLayout();
            // 
            // password
            // 
            this.password.BackColor = System.Drawing.Color.White;
            this.password.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.password.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.password.Location = new System.Drawing.Point(351, 179);
            this.password.Multiline = true;
            this.password.Name = "password";
            this.password.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.password.Size = new System.Drawing.Size(190, 33);
            this.password.TabIndex = 17;
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.Color.Tomato;
            this.loginButton.FlatAppearance.BorderColor = System.Drawing.Color.Tomato;
            this.loginButton.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Location = new System.Drawing.Point(351, 234);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(190, 40);
            this.loginButton.TabIndex = 15;
            this.loginButton.Text = "BẮT ĐẦU";
            this.loginButton.UseVisualStyleBackColor = false;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordLabel.Location = new System.Drawing.Point(347, 156);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(71, 20);
            this.passwordLabel.TabIndex = 14;
            this.passwordLabel.Text = "Mã phòng";
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameLabel.Location = new System.Drawing.Point(347, 85);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(109, 20);
            this.usernameLabel.TabIndex = 13;
            this.usernameLabel.Text = "Tên người chơi";
            // 
            // username
            // 
            this.username.BackColor = System.Drawing.Color.White;
            this.username.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.username.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.username.Location = new System.Drawing.Point(351, 108);
            this.username.Multiline = true;
            this.username.Name = "username";
            this.username.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.username.Size = new System.Drawing.Size(190, 33);
            this.username.TabIndex = 12;
            // 
            // gameName
            // 
            this.gameName.AutoSize = true;
            this.gameName.Font = new System.Drawing.Font("Arial Narrow", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gameName.Location = new System.Drawing.Point(380, 38);
            this.gameName.Name = "gameName";
            this.gameName.Size = new System.Drawing.Size(161, 31);
            this.gameName.TabIndex = 11;
            this.gameName.Text = "Zombie Game";
            // 
            // ZombieIcon
            // 
            this.ZombieIcon.Image = global::GameForm.Properties.Resources.pngwing_com;
            this.ZombieIcon.Location = new System.Drawing.Point(300, 16);
            this.ZombieIcon.Name = "ZombieIcon";
            this.ZombieIcon.Size = new System.Drawing.Size(83, 69);
            this.ZombieIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ZombieIcon.TabIndex = 16;
            this.ZombieIcon.TabStop = false;
            // 
            // backgroundImg
            // 
            this.backgroundImg.Image = global::GameForm.Properties.Resources._1804058af6cd71e04a87ca365e9553ce454318c872f329d93229b8329f501d31;
            this.backgroundImg.Location = new System.Drawing.Point(0, 1);
            this.backgroundImg.Name = "backgroundImg";
            this.backgroundImg.Size = new System.Drawing.Size(294, 349);
            this.backgroundImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.backgroundImg.TabIndex = 10;
            this.backgroundImg.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 350);
            this.Controls.Add(this.password);
            this.Controls.Add(this.ZombieIcon);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.username);
            this.Controls.Add(this.gameName);
            this.Controls.Add(this.backgroundImg);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ZombieIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.PictureBox ZombieIcon;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Label gameName;
        private System.Windows.Forms.PictureBox backgroundImg;
    }
}

