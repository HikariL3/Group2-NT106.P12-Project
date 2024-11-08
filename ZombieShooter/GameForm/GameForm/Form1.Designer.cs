namespace GameForm
{
    partial class MainGame
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainGame));
            this.txtAmmo = new System.Windows.Forms.Label();
            this.txtKill = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.healthBar = new System.Windows.Forms.ProgressBar();
            this.GameTimer = new System.Windows.Forms.Timer(this.components);
            this.txtGun = new System.Windows.Forms.Label();
            this.txtState = new System.Windows.Forms.Label();
            this.txtScore = new System.Windows.Forms.Label();
            this.txtTimer = new System.Windows.Forms.Label();
            this.ActualTime = new System.Windows.Forms.Timer(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.player1 = new System.Windows.Forms.PictureBox();
            this.wall = new System.Windows.Forms.PictureBox();
            this.name1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wall)).BeginInit();
            this.SuspendLayout();
            // 
            // txtAmmo
            // 
            this.txtAmmo.AutoSize = true;
            this.txtAmmo.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAmmo.ForeColor = System.Drawing.Color.White;
            this.txtAmmo.Location = new System.Drawing.Point(10, 41);
            this.txtAmmo.Name = "txtAmmo";
            this.txtAmmo.Size = new System.Drawing.Size(87, 22);
            this.txtAmmo.TabIndex = 0;
            this.txtAmmo.Text = "Ammo: 0";
            // 
            // txtKill
            // 
            this.txtKill.AutoSize = true;
            this.txtKill.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKill.ForeColor = System.Drawing.Color.White;
            this.txtKill.Location = new System.Drawing.Point(470, 7);
            this.txtKill.Name = "txtKill";
            this.txtKill.Size = new System.Drawing.Size(98, 22);
            this.txtKill.TabIndex = 0;
            this.txtKill.Text = "Kills: 0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(643, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "Health: ";
            // 
            // healthBar
            // 
            this.healthBar.Location = new System.Drawing.Point(725, 7);
            this.healthBar.Maximum = 250;
            this.healthBar.Name = "healthBar";
            this.healthBar.Size = new System.Drawing.Size(187, 23);
            this.healthBar.TabIndex = 1;
            this.healthBar.Value = 250;
            // 
            // GameTimer
            // 
            this.GameTimer.Enabled = true;
            this.GameTimer.Interval = 35;
            this.GameTimer.Tick += new System.EventHandler(this.MainTimerEvent);
            // 
            // txtGun
            // 
            this.txtGun.AutoSize = true;
            this.txtGun.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGun.ForeColor = System.Drawing.Color.White;
            this.txtGun.Location = new System.Drawing.Point(10, 8);
            this.txtGun.Name = "txtGun";
            this.txtGun.Size = new System.Drawing.Size(219, 22);
            this.txtGun.TabIndex = 3;
            this.txtGun.Text = "Current Gun: Pistol";
            // 
            // txtState
            // 
            this.txtState.AutoSize = true;
            this.txtState.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtState.ForeColor = System.Drawing.Color.White;
            this.txtState.Location = new System.Drawing.Point(10, 550);
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(0, 22);
            this.txtState.TabIndex = 5;
            // 
            // txtScore
            // 
            this.txtScore.AutoSize = true;
            this.txtScore.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScore.ForeColor = System.Drawing.Color.White;
            this.txtScore.Location = new System.Drawing.Point(274, 7);
            this.txtScore.Name = "txtScore";
            this.txtScore.Size = new System.Drawing.Size(98, 22);
            this.txtScore.TabIndex = 8;
            this.txtScore.Text = "Score: 0";
            // 
            // txtTimer
            // 
            this.txtTimer.AutoSize = true;
            this.txtTimer.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimer.ForeColor = System.Drawing.Color.White;
            this.txtTimer.Location = new System.Drawing.Point(820, 550);
            this.txtTimer.Name = "txtTimer";
            this.txtTimer.Size = new System.Drawing.Size(87, 22);
            this.txtTimer.TabIndex = 11;
            this.txtTimer.Text = "Time: 0";
            // 
            // ActualTime
            // 
            this.ActualTime.Enabled = true;
            this.ActualTime.Interval = 1000;
            this.ActualTime.Tick += new System.EventHandler(this.ActualTime_Tick);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(451, 32);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(473, 8);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 32);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(473, 8);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // player1
            // 
            this.player1.Image = ((System.Drawing.Image)(resources.GetObject("player1.Image")));
            this.player1.Location = new System.Drawing.Point(238, 250);
            this.player1.Name = "player1";
            this.player1.Size = new System.Drawing.Size(56, 81);
            this.player1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.player1.TabIndex = 2;
            this.player1.TabStop = false;
            // 
            // wall
            // 
            this.wall.BackColor = System.Drawing.Color.Black;
            this.wall.Image = ((System.Drawing.Image)(resources.GetObject("wall.Image")));
            this.wall.Location = new System.Drawing.Point(310, 31);
            this.wall.Margin = new System.Windows.Forms.Padding(2);
            this.wall.Name = "wall";
            this.wall.Size = new System.Drawing.Size(57, 548);
            this.wall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.wall.TabIndex = 4;
            this.wall.TabStop = false;
            // 
            // name1
            // 
            this.name1.AutoSize = true;
            this.name1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.name1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.name1.Location = new System.Drawing.Point(236, 250);
            this.name1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.name1.Name = "name1";
            this.name1.Size = new System.Drawing.Size(35, 15);
            this.name1.TabIndex = 13;
            this.name1.Text = "name";
            // 
            // MainGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(28)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(924, 579);
            this.Controls.Add(this.name1);
            this.Controls.Add(this.txtTimer);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtScore);
            this.Controls.Add(this.txtState);
            this.Controls.Add(this.wall);
            this.Controls.Add(this.player1);
            this.Controls.Add(this.txtGun);
            this.Controls.Add(this.healthBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtKill);
            this.Controls.Add(this.txtAmmo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainGame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zombie Shooter";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainGame_FormClosed);
            this.Load += new System.EventHandler(this.MainGame_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyIsDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyIsUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.player1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wall)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtAmmo;
        private System.Windows.Forms.Label txtKill;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar healthBar;
        private System.Windows.Forms.Timer GameTimer;
        private System.Windows.Forms.Label txtGun;
        private System.Windows.Forms.Label txtState;
        private System.Windows.Forms.Label txtScore;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label txtTimer;
        private System.Windows.Forms.Timer ActualTime;
        private System.Windows.Forms.PictureBox player1;
        private System.Windows.Forms.PictureBox wall;
        private System.Windows.Forms.Label name1;
    }
}

