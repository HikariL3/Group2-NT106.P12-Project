using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZombieGame
{
    public partial class Form1 : Form
    {

        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "right";
        int wallHealth = 300;
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 2;
        Random randNum = new Random();
        int score;
        List<PictureBox> zombiesList = new List<PictureBox>();

        

        public Form1()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            RestartGame();
            this.KeyPreview = true;
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            //Chỉnh máu của tường
            if (wallHealth > 0)
            {
                healthBar.Value = wallHealth;
            }
            else
            {
                gameOver = true;
                GameTimer.Stop();
            }
            //Chỉnh thông tin về đạn và điểm trên màn hình
            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;
            //Di chuyển của player
            if (goLeft == true && player.Left > 0)
            {
                player.Left -= speed;
            }
            if (goRight == true && player.Left + player.Width < 250)
            {
                player.Left += speed;
            }
            if (goUp == true && player.Top > 45)
            {
                player.Top -= speed;
            }
            if (goDown == true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }
            //Duyệt qua các control của form
            foreach (Control x in this.Controls)
            {
                //Nhặt đạn
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    //Khi zombie đến gần tường thì máu của tường giảm
                    if (wall.Bounds.IntersectsWith(x.Bounds))
                    {
                        wallHealth -= 1;
                    }
                    //Chỉnh hướng đi của zombie
                    if(x.Left >= 310 && x.Left <= this.ClientSize.Width)
                    {
                        x.Left -= zombieSpeed;
                    }
                }

                foreach (Control j in this.Controls)
                {
                    //Khi đạn bắn trúng zombie
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            zombiesList.Remove(((PictureBox)x));
                            MakeZombies();
                        }
                    }
                }
            }
        }

        private void RestartGame()
        {
            foreach (PictureBox i in zombiesList)
            {
                this.Controls.Remove(i);
            }

            zombiesList.Clear();

            for (int i = 0; i < 3; i++)
            {
                MakeZombies();
            }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            wallHealth = 300;
            score = 0;
            ammo = 10;

            GameTimer.Start();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver == true)
            {
                return;
            }

            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                //facing = "left";
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                //facing = "right";
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                //facing = "up";
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                //facing = "down";
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(facing);

                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }
        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);
        }

        private void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zleft;
            zombie.Left = this.ClientSize.Width;
            zombie.Top = randNum.Next(60, 600);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            zombiesList.Add(zombie);
            this.Controls.Add(zombie);
            player.BringToFront();
        }

        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = randNum.Next(10, 250 - ammo.Width);
            ammo.Top = randNum.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";
            this.Controls.Add(ammo);

            ammo.BringToFront();
            player.BringToFront();
        }
    }
}
