namespace GameForm
{
    partial class Lobby
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
            this.namePlayer1 = new System.Windows.Forms.Label();
            this.readyButton = new System.Windows.Forms.Button();
            this.namePlayer3 = new System.Windows.Forms.Label();
            this.showMessage = new System.Windows.Forms.ListBox();
            this.namePlayer2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.namePlayer4 = new System.Windows.Forms.Label();
            this.readyLabel1 = new System.Windows.Forms.Label();
            this.readyLabel2 = new System.Windows.Forms.Label();
            this.readyLabel3 = new System.Windows.Forms.Label();
            this.readyLabel4 = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.soLuong = new System.Windows.Forms.Label();
            this.maPhong = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.avatarPlayer4 = new System.Windows.Forms.PictureBox();
            this.avatarPlayer1 = new System.Windows.Forms.PictureBox();
            this.avatarPlayer3 = new System.Windows.Forms.PictureBox();
            this.avatarPlayer2 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.avatarPlayer4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.avatarPlayer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.avatarPlayer3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.avatarPlayer2)).BeginInit();
            this.SuspendLayout();
            // 
            // namePlayer1
            // 
            this.namePlayer1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.namePlayer1.Location = new System.Drawing.Point(16, 229);
            this.namePlayer1.Name = "namePlayer1";
            this.namePlayer1.Size = new System.Drawing.Size(100, 31);
            this.namePlayer1.TabIndex = 1;
            this.namePlayer1.Text = "Player1";
            this.namePlayer1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // readyButton
            // 
            this.readyButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readyButton.Location = new System.Drawing.Point(16, 372);
            this.readyButton.Name = "readyButton";
            this.readyButton.Size = new System.Drawing.Size(132, 56);
            this.readyButton.TabIndex = 6;
            this.readyButton.Text = "Sẵn Sàng";
            this.readyButton.UseVisualStyleBackColor = true;
            this.readyButton.Click += new System.EventHandler(this.readyButton_Click);
            // 
            // namePlayer3
            // 
            this.namePlayer3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.namePlayer3.Location = new System.Drawing.Point(280, 229);
            this.namePlayer3.Name = "namePlayer3";
            this.namePlayer3.Size = new System.Drawing.Size(100, 31);
            this.namePlayer3.TabIndex = 3;
            this.namePlayer3.Text = "Player3";
            this.namePlayer3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // showMessage
            // 
            this.showMessage.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showMessage.FormattingEnabled = true;
            this.showMessage.ItemHeight = 16;
            this.showMessage.Location = new System.Drawing.Point(0, 46);
            this.showMessage.Name = "showMessage";
            this.showMessage.Size = new System.Drawing.Size(375, 292);
            this.showMessage.TabIndex = 14;
            // 
            // namePlayer2
            // 
            this.namePlayer2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.namePlayer2.Location = new System.Drawing.Point(149, 229);
            this.namePlayer2.Name = "namePlayer2";
            this.namePlayer2.Size = new System.Drawing.Size(100, 31);
            this.namePlayer2.TabIndex = 2;
            this.namePlayer2.Text = "Player2";
            this.namePlayer2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.showMessage);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.messageBox);
            this.panel1.Controls.Add(this.sendButton);
            this.panel1.Location = new System.Drawing.Point(575, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 416);
            this.panel1.TabIndex = 12;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Image = global::GameForm.Properties.Resources.message;
            this.pictureBox3.Location = new System.Drawing.Point(5, 365);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(40, 40);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 19;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Image = global::GameForm.Properties.Resources.button;
            this.pictureBox2.Location = new System.Drawing.Point(332, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 40);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 18;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::GameForm.Properties.Resources.Assassin;
            this.pictureBox1.Location = new System.Drawing.Point(5, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(51, 42);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 19);
            this.label1.TabIndex = 17;
            this.label1.Text = "Nhóm chat";
            // 
            // messageBox
            // 
            this.messageBox.Font = new System.Drawing.Font("Arial Narrow", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageBox.Location = new System.Drawing.Point(51, 365);
            this.messageBox.Multiline = true;
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(238, 44);
            this.messageBox.TabIndex = 16;
            // 
            // sendButton
            // 
            this.sendButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendButton.Location = new System.Drawing.Point(295, 365);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(77, 44);
            this.sendButton.TabIndex = 15;
            this.sendButton.Text = "Gửi";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // namePlayer4
            // 
            this.namePlayer4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.namePlayer4.Location = new System.Drawing.Point(420, 229);
            this.namePlayer4.Name = "namePlayer4";
            this.namePlayer4.Size = new System.Drawing.Size(100, 31);
            this.namePlayer4.TabIndex = 13;
            this.namePlayer4.Text = "Player4";
            this.namePlayer4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // readyLabel1
            // 
            this.readyLabel1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readyLabel1.ForeColor = System.Drawing.Color.Red;
            this.readyLabel1.Location = new System.Drawing.Point(16, 270);
            this.readyLabel1.Name = "readyLabel1";
            this.readyLabel1.Size = new System.Drawing.Size(100, 58);
            this.readyLabel1.TabIndex = 15;
            this.readyLabel1.Text = "Chưa sẵn sàng";
            this.readyLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.readyLabel1.Visible = false;
            // 
            // readyLabel2
            // 
            this.readyLabel2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readyLabel2.ForeColor = System.Drawing.Color.Red;
            this.readyLabel2.Location = new System.Drawing.Point(149, 270);
            this.readyLabel2.Name = "readyLabel2";
            this.readyLabel2.Size = new System.Drawing.Size(100, 58);
            this.readyLabel2.TabIndex = 16;
            this.readyLabel2.Text = "Chưa sẵn sàng";
            this.readyLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.readyLabel2.Visible = false;
            // 
            // readyLabel3
            // 
            this.readyLabel3.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readyLabel3.ForeColor = System.Drawing.Color.Red;
            this.readyLabel3.Location = new System.Drawing.Point(284, 270);
            this.readyLabel3.Name = "readyLabel3";
            this.readyLabel3.Size = new System.Drawing.Size(100, 58);
            this.readyLabel3.TabIndex = 17;
            this.readyLabel3.Text = "Chưa sẵn sàng";
            this.readyLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.readyLabel3.Visible = false;
            // 
            // readyLabel4
            // 
            this.readyLabel4.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readyLabel4.ForeColor = System.Drawing.Color.Red;
            this.readyLabel4.Location = new System.Drawing.Point(420, 270);
            this.readyLabel4.Name = "readyLabel4";
            this.readyLabel4.Size = new System.Drawing.Size(100, 58);
            this.readyLabel4.TabIndex = 18;
            this.readyLabel4.Text = "Chưa sẵn sàng";
            this.readyLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.readyLabel4.Visible = false;
            // 
            // startButton
            // 
            this.startButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.startButton.Enabled = false;
            this.startButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startButton.ForeColor = System.Drawing.Color.White;
            this.startButton.Location = new System.Drawing.Point(169, 371);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(132, 56);
            this.startButton.TabIndex = 19;
            this.startButton.Text = "Bắt Đầu";
            this.startButton.UseVisualStyleBackColor = false;
            this.startButton.Visible = false;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // soLuong
            // 
            this.soLuong.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.soLuong.Location = new System.Drawing.Point(16, 68);
            this.soLuong.Name = "soLuong";
            this.soLuong.Size = new System.Drawing.Size(139, 31);
            this.soLuong.TabIndex = 25;
            this.soLuong.Text = "SỐ LƯỢNG: 0";
            this.soLuong.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maPhong
            // 
            this.maPhong.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maPhong.Location = new System.Drawing.Point(193, 68);
            this.maPhong.Name = "maPhong";
            this.maPhong.Size = new System.Drawing.Size(327, 31);
            this.maPhong.TabIndex = 23;
            this.maPhong.Text = "MÃ PHÒNG:";
            this.maPhong.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(54, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 31);
            this.label2.TabIndex = 26;
            this.label2.Text = "PHÒNG CHỜ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::GameForm.Properties.Resources.ogre;
            this.pictureBox4.Location = new System.Drawing.Point(16, 15);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(40, 40);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 27;
            this.pictureBox4.TabStop = false;
            // 
            // avatarPlayer4
            // 
            this.avatarPlayer4.Image = global::GameForm.Properties.Resources.avatar_vo_danh_9;
            this.avatarPlayer4.Location = new System.Drawing.Point(420, 117);
            this.avatarPlayer4.Name = "avatarPlayer4";
            this.avatarPlayer4.Size = new System.Drawing.Size(100, 100);
            this.avatarPlayer4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.avatarPlayer4.TabIndex = 14;
            this.avatarPlayer4.TabStop = false;
            // 
            // avatarPlayer1
            // 
            this.avatarPlayer1.Image = global::GameForm.Properties.Resources.avatar_vo_danh_9;
            this.avatarPlayer1.Location = new System.Drawing.Point(16, 117);
            this.avatarPlayer1.Name = "avatarPlayer1";
            this.avatarPlayer1.Size = new System.Drawing.Size(100, 100);
            this.avatarPlayer1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.avatarPlayer1.TabIndex = 0;
            this.avatarPlayer1.TabStop = false;
            // 
            // avatarPlayer3
            // 
            this.avatarPlayer3.Image = global::GameForm.Properties.Resources.avatar_vo_danh_9;
            this.avatarPlayer3.Location = new System.Drawing.Point(284, 117);
            this.avatarPlayer3.Name = "avatarPlayer3";
            this.avatarPlayer3.Size = new System.Drawing.Size(100, 100);
            this.avatarPlayer3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.avatarPlayer3.TabIndex = 3;
            this.avatarPlayer3.TabStop = false;
            // 
            // avatarPlayer2
            // 
            this.avatarPlayer2.Image = global::GameForm.Properties.Resources.avatar_vo_danh_9;
            this.avatarPlayer2.Location = new System.Drawing.Point(149, 117);
            this.avatarPlayer2.Name = "avatarPlayer2";
            this.avatarPlayer2.Size = new System.Drawing.Size(100, 100);
            this.avatarPlayer2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.avatarPlayer2.TabIndex = 2;
            this.avatarPlayer2.TabStop = false;
            // 
            // Lobby
            // 
            this.AcceptButton = this.sendButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(959, 446);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.maPhong);
            this.Controls.Add(this.soLuong);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.readyLabel4);
            this.Controls.Add(this.readyLabel3);
            this.Controls.Add(this.readyLabel2);
            this.Controls.Add(this.readyLabel1);
            this.Controls.Add(this.namePlayer4);
            this.Controls.Add(this.avatarPlayer4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.namePlayer1);
            this.Controls.Add(this.avatarPlayer1);
            this.Controls.Add(this.namePlayer3);
            this.Controls.Add(this.namePlayer2);
            this.Controls.Add(this.avatarPlayer3);
            this.Controls.Add(this.avatarPlayer2);
            this.Controls.Add(this.readyButton);
            this.Name = "Lobby";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lobby";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.avatarPlayer4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.avatarPlayer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.avatarPlayer3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.avatarPlayer2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox avatarPlayer1;
        private System.Windows.Forms.Label namePlayer1;
        private System.Windows.Forms.Button readyButton;
        private System.Windows.Forms.PictureBox avatarPlayer3;
        private System.Windows.Forms.Label namePlayer3;
        private System.Windows.Forms.ListBox showMessage;
        private System.Windows.Forms.PictureBox avatarPlayer2;
        private System.Windows.Forms.Label namePlayer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox messageBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label namePlayer4;
        private System.Windows.Forms.PictureBox avatarPlayer4;
        private System.Windows.Forms.Label readyLabel1;
        private System.Windows.Forms.Label readyLabel2;
        private System.Windows.Forms.Label readyLabel3;
        private System.Windows.Forms.Label readyLabel4;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label soLuong;
        private System.Windows.Forms.Label maPhong;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}