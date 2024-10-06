using System;
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

namespace Shoot_Out_Game_MOO_ICT
{
    public partial class Form1 : Form
    {

        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up";
        double wallHealth = 20000;
        int speed = 10;
        Random randNum = new Random();
        int score;
        List<Zombie> zombiesList = new List<Zombie>();
        private List<Gun> guns = new List<Gun>();
        private Gun currentGun;
        private bool canFire = true;
        int offset = 20;
        public Form1()
        {
            InitializeComponent();
            InitializeGuns();
            RestartGame();
        }

        private void InitializeGuns()
        {
            // Load images for the guns
            Gun pistol = new Gun("Pistol", 40, 12, 20, 300, 350, 1000,
                                Properties.Resources.pistolup, Properties.Resources.pistoldown,
                                Properties.Resources.pistolleft, Properties.Resources.pistolright);
            Gun shotgun = new Gun("Shotgun", 40, 2, 10, 200, 700, 2000,
                                Properties.Resources.shotgunup, Properties.Resources.shotgundown,
                                Properties.Resources.shotgunleft, Properties.Resources.shotgunright);
            Gun sniper = new Gun("Sniper", 100, 5, 30, 500, 1000, 2000,
                                Properties.Resources.sniperup, Properties.Resources.sniperdown,
                                Properties.Resources.sniperleft, Properties.Resources.sniperright);

            guns.Add(pistol);
            guns.Add(shotgun);
            guns.Add(sniper);
            currentGun = guns[0]; // Start with pistol
            txtGun.Text = "Current Gun: " + currentGun.Name;
            txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (wallHealth > 1)
            {
                healthBar.Value = (int)wallHealth;
            }
            else
            {
                gameOver = true;
                GameTimer.Stop();
            }

            txtKill.Text = "Kills: " + score;

            if (goLeft == true && player.Left > 0)
            {

                player.Left -= speed;

            }
            if (goRight == true && player.Left + player.Width < this.ClientSize.Width)
            {
                if (player.Right < wall.Left)
                {
                    player.Left += speed;
                }
            }
            if (goUp == true && player.Top > 45)
            {
                player.Top -= speed;
            }
            if (goDown == true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
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
                                score++;
                                this.Controls.Remove(zombie.ZombiePictureBox);
                                zombiesList.Remove(zombie);
                                MakeZombies();
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
            wallHealth -= zombie.Damage * 0.005;
            if (wallHealth <= 0)
            {
                // Handle game over or wall destruction
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
                player.Image = currentGun.ImageLeft;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                player.Image = currentGun.ImageRight;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                player.Image = currentGun.ImageUp;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player.Image = currentGun.ImageDown;
            }



        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.C && gameOver == false)
            {
                using (SoundPlayer player = new SoundPlayer(Properties.Resources.gswitch))
                {
                    player.Play();
                }
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
                using (SoundPlayer player = new SoundPlayer(Properties.Resources.empty))
                {
                    player.Play();
                }
            }

                if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }

        }

        private void txtScore_Click(object sender, EventArgs e)
        {

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
                    using (SoundPlayer player = new SoundPlayer(Properties.Resources.pistolshoot))
                    {
                        player.Play();
                    }
                    Bullet shootPistolBullet = new Bullet(bulletSpeed, bulletRange);
                    shootPistolBullet.direction = direction;
                    shootPistolBullet.bulletLeft = player.Left + (player.Width / 2);
                    shootPistolBullet.bulletTop = player.Top + (player.Height / 2);

                    shootPistolBullet.bulletTop = player.Top + (player.Height / 2) + offset;
                    
                    shootPistolBullet.MakeBullet(this);
                    break;
                case "Shotgun":
                    bulletSpeed = 30;
                    bulletRange = 350;
                    Random rand = new Random();
                    using (SoundPlayer player = new SoundPlayer(Properties.Resources.shotgunshoot))
                    {
                        player.Play();
                    }

                    for (int i = 0; i < 7; i++)
                    {
                        Bullet shootShotgunPellet = new Bullet(bulletSpeed, bulletRange);
                        shootShotgunPellet.direction = direction;

                        int spreadAngle = rand.Next(-60, 61);

                        shootShotgunPellet.bulletLeft = player.Left + (player.Width / 2);
                        shootShotgunPellet.bulletTop = player.Top + (player.Height / 2);
                        shootShotgunPellet.bulletTop = player.Top + (player.Height / 2) + offset;

                        ApplySpread(shootShotgunPellet, spreadAngle);

                        shootShotgunPellet.MakeBullet(this);
                    }
                    break;
                case "Sniper":
                    bulletSpeed = 50;
                    bulletRange = 1200;
                    using (SoundPlayer player = new SoundPlayer(Properties.Resources.snipershoot))
                    {
                        player.Play();
                    }
                    Bullet shootSniperBullet = new Bullet(bulletSpeed, bulletRange);
                    shootSniperBullet.direction = direction;
                    shootSniperBullet.bulletLeft = player.Left + (player.Width / 2);
                    shootSniperBullet.bulletTop = player.Top + (player.Height / 2);
                    shootSniperBullet.bulletTop = player.Top + (player.Height / 2) + offset;
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
        //scrap
        private void wall_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //scrap

        private void MakeZombies()
        {
            Random rand = new Random();
            int zombieType = rand.Next(1, 5);
            Zombie zombie = Zombie.CreateZombie(zombieType);


            int minSpawnHeight = 100; //  top margin
            int maxSpawnHeight = this.ClientSize.Height - zombie.ZombiePictureBox.Height - 100; //bottom margin
            zombie.ZombiePictureBox.Left = this.ClientSize.Width + 50;

            zombie.ZombiePictureBox.Top = rand.Next(minSpawnHeight, maxSpawnHeight);
            zombie.ZombiePictureBox.Image = zombie.ImageLeft;

            zombiesList.Add(zombie);
            this.Controls.Add(zombie.ZombiePictureBox);
        }

        private void SwitchGun()
        {
            int currentGunIndex = guns.IndexOf(currentGun);
            currentGunIndex = (currentGunIndex + 1) % guns.Count;
            currentGun = guns[currentGunIndex];
            txtGun.Text = "Current Gun: " + currentGun.Name;

            if (currentGun.CurrentAmmo > 0)
                txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;

            switch (facing)
            {
                case "up":
                    player.Image = currentGun.ImageUp;
                    break;
                case "down":
                    player.Image = currentGun.ImageDown;
                    break;
                case "left":
                    player.Image = currentGun.ImageLeft;
                    break;
                case "right":
                    player.Image = currentGun.ImageRight;
                    break;
            }
        }

        private void ReloadGun()
        {
            using (SoundPlayer player = new SoundPlayer(Properties.Resources.gunload))
            {
                player.Play();
            }
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


        private void RestartGame()
        {
            player.Image = currentGun.ImageUp;


            foreach (Zombie zombie in zombiesList)
            {
                this.Controls.Remove(zombie.ZombiePictureBox);
                zombie.ZombiePictureBox.Dispose();
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

            wallHealth = 100;
            score = 0;
            currentGun.Reload();

            healthBar.Value = (int)wallHealth;
            txtAmmo.Text = "Ammo: " + currentGun.CurrentAmmo;

            player.Left = wall.Left - player.Width - 10;
            player.Top = wall.Top + (wall.Height / 2) - (player.Height / 2);

            canFire = true;

            GameTimer.Start();
        }

    }
}