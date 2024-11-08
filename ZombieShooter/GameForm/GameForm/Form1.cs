﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Net.Sockets;
using GameForm;
using System.Net.NetworkInformation;
using Client;
using Server_ShootOutGame;

namespace GameForm
{
    public partial class MainGame : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "right";
        double wallHealth = 250;
        int speed = 10;
        Random randNum = new Random();
        int kill, score;
        List<Zombie> zombiesList = new List<Zombie>();
        private List<Gun> guns = new List<Gun>();
        private Gun currentGun;
        private bool canFire = true;
        int offset = 20;
        Random ranSpawn = new Random();
        int timeLeft = 120;
        private SoundManager soundManager = new SoundManager();
        private bool finalWave = false;
        public MainGame()
        {
            InitializeComponent();
            InitializeGuns();
            RestartGame();
        }
        private void InitializeGuns()
        {
            // Load images for the guns
            Gun pistol = new Gun("Pistol", 40, 12, 25, 500, 350, 1000,
                                Properties.Resources.pistolup, Properties.Resources.pistoldown,
                                Properties.Resources.pistolleft, Properties.Resources.pistolright);
            Gun shotgun = new Gun("Shotgun", 15, 3, 25, 350, 700, 1400,
                                Properties.Resources.shotgunup, Properties.Resources.shotgundown,
                                Properties.Resources.shotgunleft, Properties.Resources.shotgunright);
            Gun sniper = new Gun("Sniper", 100, 5, 50, 1200, 1000, 1600,
                                Properties.Resources.sniperup, Properties.Resources.sniperdown,
                                Properties.Resources.sniperleft, Properties.Resources.sniperright);

            guns.Add(pistol);
            guns.Add(shotgun);
            guns.Add(sniper);
            currentGun = guns[0]; // Start with pistol
            txtGun.Text = "Current Gun: " + currentGun.Name;
            txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;
        }

        private void MainGame_Load(object sender, EventArgs e)
        {
            soundManager.LoadSound("pistol", Properties.Resources.pistolshoot);
            soundManager.LoadSound("shotgun", Properties.Resources.shotgunshoot);
            soundManager.LoadSound("sniper", Properties.Resources.snipershoot);
            soundManager.LoadSound("reload", Properties.Resources.gunload);
            soundManager.LoadSound("switch", Properties.Resources.gswitch);
            soundManager.LoadSound("empty", Properties.Resources.empty);
            soundManager.LoadSound("groan0", Properties.Resources.Groan0);
            soundManager.LoadSound("groan1", Properties.Resources.Groan1);
            soundManager.LoadSound("finalwave", Properties.Resources.finalwave);
            soundManager.LoadSound("bzfinalwave", Properties.Resources.bzfinalwave);
            soundManager.LoadSound("brain", Properties.Resources.brain);
            soundManager.LoadSound("begin", Properties.Resources.begin);
            soundManager.PlaySound("begin");
            name1.Text = GameClient.localPlayer.Name;/////////////////////////


            int Playercount = GameClient.joinedLobby.Players.Count;
            for(int  i = 1; i <= Playercount; i++)
            {
                if(GameClient.localPlayer.Name != GameClient.joinedLobby.Players[i - 1].Name)
                {
                    switch (i)
                    {
                        case 2:
                            PictureBox player2 = new PictureBox();
                            player2.Size = new Size(56, 81);
                            player2.Location = new Point(238, 100);
                            player2.SizeMode = PictureBoxSizeMode.StretchImage;
                            player2.Image = currentGun.ImageRight;

                            Label name2 = new Label();
                            name2.Text = GameClient.joinedLobby.Players[i - 1].Name;
                            name2.BackColor = Color.FromArgb(0, 28, 32);
                            //name2.Size = new Size(35, 15);
                            name2.AutoSize = true;
                            name2.AutoEllipsis = true;
                            name2.Location = new Point(238, 100);
                            name2.ForeColor = SystemColors.ButtonHighlight;
                            name2.Font = new Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                            this.Controls.Add(player2);
                            this.Controls.Add(name2);

                            name2.BringToFront();
                            player2.SendToBack();
                            break;

                        case 3:
                            PictureBox player3 = new PictureBox();
                            player3.Size = new Size(56, 81);
                            player3.Location = new Point(238, 400);
                            player3.SizeMode = PictureBoxSizeMode.StretchImage;
                            player3.Image = currentGun.ImageRight;

                            Label name3 = new Label();
                            name3.Text = GameClient.joinedLobby.Players[i - 1].Name;
                            name3.BackColor = Color.FromArgb(0, 28, 32);
                            //name3.Size = new Size(35, 15);
                            name3.AutoSize = true;
                            name3.AutoEllipsis = true;
                            name3.Location = new Point(238, 400);
                            name3.ForeColor = SystemColors.ButtonHighlight;
                            name3.Font = new Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                            this.Controls.Add(player3);
                            this.Controls.Add(name3);

                            name3.BringToFront();
                            player3.SendToBack();
                            break;

                        case 4:
                            PictureBox player4 = new PictureBox();
                            player4.Size = new Size(56, 81);
                            player4.Location = new Point(238, 550);
                            player4.SizeMode = PictureBoxSizeMode.StretchImage;
                            player4.Image = currentGun.ImageRight;

                            Label name4 = new Label();
                            name4.Text = GameClient.joinedLobby.Players[i - 1].Name;
                            //name4.Size = new Size(35, 15);
                            name4.AutoSize = true;
                            name4.AutoEllipsis = true;
                            name4.BackColor = Color.FromArgb(0, 28, 32);
                            name4.Location = new Point(238, 550);
                            name4.ForeColor = SystemColors.ButtonHighlight;
                            name4.Font = new Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                            this.Controls.Add(player4);
                            this.Controls.Add(name4);

                            name4.BringToFront();
                            player4.SendToBack();
                            break;
                    }
                }
            }
        }


        #region CODE FOR HANDLING GAME EVENT
        //BEGIN OF-----------------------------------------------------------------------
        //----------------THESE LINES OF CODE ARE FOR HANDLING GAME EVENT----------------
        //-------------------------------------------------------------------------------

        private void ActualTime_Tick(object sender, EventArgs e)
        {
            if (timeLeft % 7 == 0 && timeLeft != 0)
            {
                int randSound = ranSpawn.Next(1, 4);
                Random rand = new Random();
                if (randSound == 1)
                    soundManager.PlaySound("groan0");
                if (randSound == 2)
                    soundManager.PlaySound("groan1");
                if (randSound == 3)
                    soundManager.PlaySound("brain");
            }
            if (timeLeft > 0)
            {
                timeLeft--;
                txtTimer.Text = "Time: " + timeLeft.ToString();
            }

            if (timeLeft == 0 && !finalWave)
            {
                FinalWave();
                finalWave = true;
            }

            else if (timeLeft <= 30 && timeLeft != 0)
            {
                if (timeLeft % 2 == 0)
                    MakeZombies4();
            }

            else if (timeLeft <= 60 && timeLeft != 0)
            {
                if (timeLeft % 2 == 0)
                    MakeZombies3();
            }

            else if (timeLeft <= 90 && timeLeft != 0)
            {
                if (timeLeft % 3 == 0)
                    MakeZombies2();
            }

            else if (timeLeft <= 120 && timeLeft != 0)
            {
                if (timeLeft % 3 == 0)
                    MakeZombies1();
            }
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (timeLeft <= 0)
            {
                if (zombiesList.Count == 0)
                {
                    gameOver = true;
                    YouWin();
                }
            }

            if (wallHealth > 1)
            {
                healthBar.Value = (int)wallHealth;
            }
            else
            {
                gameOver = true;
                YouLose();
            }

            txtKill.Text = "Kills: " + kill;
            txtScore.Text = "Score: " + score;

            // Initialize movement flags
            bool canMoveLeft = goLeft && player1.Left > 0;
            bool canMoveRight = goRight && player1.Left + player1.Width < this.ClientSize.Width;
            bool canMoveUp = goUp && player1.Top > 45;
            bool canMoveDown = goDown && player1.Top + player1.Height < this.ClientSize.Height;


            // Check for collisions with obstacles
            foreach (Control obstacle in this.Controls)
            {
                if (obstacle is PictureBox &&
                    ((string)obstacle.Name == "wall"))
                {
                    if (player1.Bounds.IntersectsWith(obstacle.Bounds))
                    {
                        if (goLeft && player1.Left < obstacle.Right && player1.Right > obstacle.Right)
                        {
                            canMoveLeft = false;
                        }
                        if (goRight && player1.Right > obstacle.Left && player1.Left < obstacle.Left)
                        {
                            canMoveRight = false;
                        }
                        if (goUp && player1.Top < obstacle.Bottom && player1.Bottom > obstacle.Bottom)
                        {
                            canMoveUp = false;
                        }
                        if (goDown && player1.Bottom > obstacle.Top && player1.Top < obstacle.Top)
                        {
                            canMoveDown = false;
                        }
                    }
                }
            }

            if (canMoveLeft)
            {
                player1.Left -= speed;
                name1.Left -= speed;
            }
            if (canMoveRight)
            {
                player1.Left += speed;
                name1.Left += speed;
            }
            if (canMoveUp)
            {
                player1.Top -= speed;
                name1.Top -= speed;
            }
            if (canMoveDown)
            {
                player1.Top += speed;
                name1.Top += speed;
            }

            foreach (Zombie zombie in zombiesList.ToList())
            {
                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet")
                    {
                        if (zombie.ZombiePictureBox.Bounds.IntersectsWith(j.Bounds))
                        {
                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();

                            zombie.Health -= currentGun.Damage;
                            if (zombie.Health <= 0)
                            {
                                kill++;
                                score += zombie.Score;
                                this.Controls.Remove(zombie.ZombiePictureBox);
                                zombiesList.Remove(zombie);
                                break;
                            }
                        }
                    }
                }

                if (zombie.ZombiePictureBox.Left > wall.Right)
                {
                    zombie.ZombiePictureBox.Left -= zombie.Speed;
                    zombie.ZombiePictureBox.Image = zombie.ImageLeft;
                }
                else
                {
                    DamageWall(zombie);
                }
            }
        }


        private void DamageWall(Zombie zombie)
        {
            wallHealth -= zombie.Damage * 0.01;
            if (wallHealth <= 0)
            {
                gameOver = true;
                GameTimer.Stop();
            }
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
                facing = "left";                           
                player1.Image = currentGun.ImageLeft;       
            }                                              
                                                           
            if (e.KeyCode == Keys.Right)                   
            {                                              
                goRight = true;                            
                facing = "right";                          
                player1.Image = currentGun.ImageRight;      
            }                                              
                                                           
            if (e.KeyCode == Keys.Up)                      
            {                                              
                goUp = true;                               
                facing = "up";                             
                player1.Image = currentGun.ImageUp;         
            }                                              
                                                           
            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player1.Image = currentGun.ImageDown;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.C && gameOver == false)
            {
                soundManager.PlaySound("switch");
                SwitchGun();
            }

            if (e.KeyCode == Keys.R && gameOver == false)
            {
                ReloadGun();
            }
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


            if (e.KeyCode == Keys.Space && currentGun.CurrentAmmo > 0 && gameOver == false && canFire)
            {
                currentGun.CurrentAmmo--;
                txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;
                ShootBullet(facing);


                if (currentGun.CurrentAmmo < 1)
                {
                    txtAmmo.Text = "Ammo: Out of ammo!";
                    txtState.Text = "Press R to reload!";
                }

                //Set canFire to false for the duration of the gun's fire rate

                canFire = false;
                Timer fireRateTimer = new Timer { Interval = currentGun.FireRate };
                fireRateTimer.Tick += (s, evt) => { canFire = true; fireRateTimer.Stop(); };
                fireRateTimer.Start();

            }

            else if (e.KeyCode == Keys.Space && currentGun.CurrentAmmo == 0 && gameOver == false && canFire)
            {
                soundManager.PlaySound("empty");
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }

        }
        //END OF-------------------------------------------------------------------------
        //----------------THESE LINES OF CODE ARE FOR HANDLING GAME EVENT----------------
        //-------------------------------------------------------------------------------
        #endregion

        #region CODE FOR GUN'S MECHANICS
        //BEGIN OF-----------------------------------------------------------------------
        //------------------THESE LINES OF CODE ARE FOR GUN'S MECHANICS------------------
        //-------------------------------------------------------------------------------

        private void SwitchGun()
        {
            int currentGunIndex = guns.IndexOf(currentGun);
            currentGunIndex = (currentGunIndex + 1) % guns.Count;
            currentGun = guns[currentGunIndex];
            txtGun.Text = "Current Gun: " + currentGun.Name;

            if (currentGun.CurrentAmmo > 0)
                txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;
            else if (currentGun.CurrentAmmo < 1)
            {
                txtAmmo.Text = "Ammo: Out of ammo!";
                txtState.Text = "Press R to reload!";
            }

            switch (facing)
            {
                case "up":
                    player1.Image = currentGun.ImageUp;
                    break;
                case "down":
                    player1.Image = currentGun.ImageDown;
                    break;
                case "left":
                    player1.Image = currentGun.ImageLeft;
                    break;
                case "right":
                    player1.Image = currentGun.ImageRight;
                    break;
            }
        }

        private void ReloadGun()
        {
            soundManager.PlaySound("reload");
            txtGun.Text = "Reloading...";
            txtState.Text = "";

            canFire = false;

            Timer reloadTimer = new Timer();
            reloadTimer.Interval = currentGun.ReloadTime;
            reloadTimer.Tick += (s, evt) =>
            {
                currentGun.Reload();
                txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;
                txtGun.Text = "Current Gun: " + currentGun.Name;
                canFire = true;
                reloadTimer.Stop();
            };
            reloadTimer.Start();
        }

        private void ShootBullet(string direction)
        {
            int bulletSpeed;
            int bulletRange;

            switch (currentGun.Name)
            {
                case "Pistol":
                    bulletSpeed = 25;
                    bulletRange = 500;
                    soundManager.PlaySound("pistol");
                    Bullet shootPistolBullet = new Bullet(bulletSpeed, bulletRange);
                    shootPistolBullet.direction = direction;
                    shootPistolBullet.bulletLeft = player1.Left + (player1.Width / 2);
                    shootPistolBullet.bulletTop = player1.Top + (player1.Height / 2);

                    shootPistolBullet.bulletTop = player1.Top + (player1.Height / 2) + offset;

                    shootPistolBullet.MakeBullet(this);
                    break;
                case "Shotgun":
                    bulletSpeed = 25;
                    bulletRange = 350;
                    Random rand = new Random();
                    soundManager.PlaySound("shotgun");

                    for (int i = 0; i < 8; i++)
                    {
                        Bullet shootShotgunPellet = new Bullet(bulletSpeed, bulletRange);
                        shootShotgunPellet.direction = direction;

                        int spreadAngle = rand.Next(-60, 61);

                        shootShotgunPellet.bulletLeft = player1.Left + (player1.Width / 2);
                        shootShotgunPellet.bulletTop = player1.Top + (player1.Height / 2);
                        shootShotgunPellet.bulletTop = player1.Top + (player1.Height / 2) + offset;

                        ApplySpread(shootShotgunPellet, spreadAngle);

                        shootShotgunPellet.MakeBullet(this);
                    }
                    break;
                case "Sniper":
                    bulletSpeed = 50;
                    bulletRange = 1200;
                    soundManager.PlaySound("sniper");
                    Bullet shootSniperBullet = new Bullet(bulletSpeed, bulletRange);
                    shootSniperBullet.direction = direction;
                    shootSniperBullet.bulletLeft = player1.Left + (player1.Width / 2);
                    shootSniperBullet.bulletTop = player1.Top + (player1.Height / 2);
                    shootSniperBullet.bulletTop = player1.Top + (player1.Height / 2) + offset;
                    shootSniperBullet.MakeBulletSniper(this);
                    break;
                default:
                    bulletSpeed = 20;
                    bulletRange = 500;
                    break;
            }

        }

        private void ApplySpread(Bullet bullet, int spreadAngle)
        {
            switch (bullet.direction)
            {
                case "up":
                    bullet.bulletLeft += (int)(Math.Sin(spreadAngle * Math.PI / 180) * bullet.speed);
                    break;
                case "down":
                    bullet.bulletLeft -= (int)(Math.Sin(spreadAngle * Math.PI / 180) * bullet.speed);
                    break;
                case "left":
                    bullet.bulletTop += (int)(Math.Sin(spreadAngle * Math.PI / 180) * bullet.speed);
                    break;
                case "right":
                    bullet.bulletTop -= (int)(Math.Sin(spreadAngle * Math.PI / 180) * bullet.speed);
                    break;
            }
        }

        //END OF-------------------------------------------------------------------------
        //------------------THESE LINES OF CODE ARE FOR GUN'S MECHANICS------------------
        //-------------------------------------------------------------------------------
        #endregion

        #region CODE FOR MAKING ZOMBIES SPAWN
        //BEGIN OF-----------------------------------------------------------------------
        //---------------THESE LINES OF CODE ARE FOR MAKING ZOMBIES SPAWN----------------
        //-------------------------------------------------------------------------------

        private void MakeZombies1()
        {
            int spawnChance = ranSpawn.Next(1, 101);
            Random rand = new Random();
            if (spawnChance <= 65)
            {
                Zombie zombie = Zombie.CreateZombie(4);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
            else if (spawnChance <= 100)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
        }

        private void MakeZombies2()
        {
            int spawnChance = ranSpawn.Next(1, 101);
            Random rand = new Random();
            if (spawnChance <= 40)
            {
                Zombie zombie = Zombie.CreateZombie(4);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
            else if (spawnChance <= 80)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
            else if (spawnChance <= 100)
            {
                Zombie zombie = Zombie.CreateZombie(2);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
        }

        private void MakeZombies3()
        {
            int spawnChance = ranSpawn.Next(1, 101);
            Random rand = new Random();
            if (spawnChance <= 25)
            {
                Zombie zombie = Zombie.CreateZombie(4);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
            else if (spawnChance <= 65)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
            else if (spawnChance <= 100)
            {
                Zombie zombie = Zombie.CreateZombie(2);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
        }

        private void MakeZombies4()
        {
            int spawnChance = ranSpawn.Next(1, 101);
            Random rand = new Random();

            if (spawnChance <= 45)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
            else if (spawnChance <= 85)
            {
                Zombie zombie = Zombie.CreateZombie(2);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
            else if (spawnChance <= 100)
            {
                Zombie zombie = Zombie.CreateZombie(1);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
            }
        }

        private async void FinalWave() //when timer reaches 0s
        {
            soundManager.PlaySound("finalwave");
            Random rand = new Random();

            for (int i = 0; i < 3; i++)
            {
                Zombie zombie = Zombie.CreateZombie(4);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
                await Task.Delay(500);
            }

            await Task.Delay(1000);

            for (int i = 0; i < 2; i++)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
                await Task.Delay(500);
            }

            await Task.Delay(2000);

            for (int i = 0; i < 3; i++)
            {
                Zombie zombie = Zombie.CreateZombie(2);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
                await Task.Delay(1000);
            }

            await Task.Delay(2000);
            soundManager.PlaySound("bzfinalwave");
            for (int i = 0; i < 3; i++)
            {
                Zombie zombie = Zombie.CreateZombie(1);
                int minSpawnHeight = 100;
                int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100;
                zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

                zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
                zombie.ZombiePictureBox.Image = zombie.ImageLeft;

                zombiesList.Add(zombie);
                this.Controls.Add(zombie.ZombiePictureBox);
                await Task.Delay(1500);
            }
        }

        //END OF-------------------------------------------------------------------------
        //---------------THESE LINES OF CODE ARE FOR MAKING ZOMBIES SPAWN----------------
        //-------------------------------------------------------------------------------
        #endregion

        private async void YouWin()
        {
            await Task.Delay(5);
            GameTimer.Stop();
            ActualTime.Stop();
            MessageBox.Show("Game Over! You defeated all the zombies!", "You win!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            GameClient.SendData($"STATS;{kill};{score}");
            Win winForm = new Win();
            winForm.ShowDialog();
            this.Hide();
        }

        private async void YouLose()
        {
            await Task.Delay(5);
            GameTimer.Stop();
            ActualTime.Stop();
            MessageBox.Show("Game Over! You are dead, the zombies destroyed your wall", "You lose!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            GameClient.SendData($"STATS;{kill};{score}");
            Lose loseForm = new Lose();
            loseForm.ShowDialog();
            this.Hide();
        }

        private void MainGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameClient.Disconnect();
            GameClient.ClearLobby();
            Login login = new Login();
            login.Show();
        }

        private void RestartGame()
        {
            player1.Image = currentGun.ImageRight;

            foreach (Zombie zombie in zombiesList)
            {
                this.Controls.Remove(zombie.ZombiePictureBox);
                zombie.ZombiePictureBox.Dispose();
            }
            zombiesList.Clear();

            timeLeft = 120;

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            wallHealth = 250;
            kill = 0;
            score = 0;
            currentGun.Reload();

            healthBar.Value = (int)wallHealth;
            txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;

            player1.Left = wall.Left - player1.Width - 10;
            player1.Top = wall.Top + (wall.Height / 2) - (player1.Height / 2);
            
            //Initialize the name along with the player
            name1.Left = wall.Left - player1.Width - 10;
            name1.Top = wall.Top + (wall.Height / 2) - (player1.Height / 2);

            canFire = true;

            GameTimer.Start();
            ActualTime.Start();
        }
    }
}