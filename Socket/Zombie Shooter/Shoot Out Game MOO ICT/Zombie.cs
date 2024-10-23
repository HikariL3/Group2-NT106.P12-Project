using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing;

namespace Shoot_Out_Game_MOO_ICT
{
    class Zombie
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public int Damage { get; set; }
        public int Score { get; set; }
        public string Direction { get; set; }
        public PictureBox ZombiePictureBox { get; set; }


        public Image ImageLeft { get; set; }


        public Zombie(int health, int speed, int damage, int score, string direction, PictureBox zPictureBox, Image imgLeft)
        {
            Health = health;
            Speed = speed;
            Damage = damage;
            Score = score;
            Direction = direction;
            ZombiePictureBox = zPictureBox;
            ImageLeft = imgLeft;

        }
        public static Zombie CreateZombie(int type)
        {
            PictureBox zombiePic = new PictureBox();
            zombiePic.Tag = "zombie";
            zombiePic.SizeMode = PictureBoxSizeMode.AutoSize;

            switch (type)
            {
                case 1:
                    return new Zombie(250, 2, 32, 60, "left", zombiePic,
                        Properties.Resources.bzleft); // variant 1 images
                case 2:
                    return new Zombie(40, 5, 15, 30, "left", zombiePic,
                        Properties.Resources.szleft); // variant 2 images
                case 3:
                    return new Zombie(100, 3, 15, 30, "left", zombiePic,
                        Properties.Resources.tzleft); // variant 3 images
                case 4:
                    return new Zombie(40, 3, 12, 20, "left", zombiePic,
                        Properties.Resources.zleft); // variant 4 images (default)
                default:
                    return null;
            }
        }
    }
}