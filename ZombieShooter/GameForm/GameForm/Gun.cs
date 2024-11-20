using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace GameForm
{
    public class Gun
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public int MaxAmmo { get; set; }
        public int CurrentAmmo { get; set; }
        public int BulletSpeed { get; set; }
        public int Range { get; set; }
        public int FireRate { get; set; } // Measured in milliseconds
        public int ReloadTime { get; set; }
        public Image ImageUp { get; set; }
        public Image ImageDown { get; set; }
        public Image ImageLeft { get; set; }
        public Image ImageRight { get; set; }

        public Gun(string name, int damage, int maxAmmo, int bulletSpeed, int range, int fireRate,
                   int reload, Image imgUp, Image imgDown, Image imgLeft, Image imgRight)
        {
            Name = name;
            Damage = damage;
            MaxAmmo = maxAmmo;
            CurrentAmmo = maxAmmo;
            BulletSpeed = bulletSpeed;
            Range = range;
            FireRate = fireRate;
            ReloadTime = reload;
            ImageUp = imgUp;
            ImageDown = imgDown;
            ImageLeft = imgLeft;
            ImageRight = imgRight;
        }

        public void Reload()
        {
            CurrentAmmo = MaxAmmo;
        }

        public static int GetDamage(string name)
        {
            switch (name)
            {
                case "Pistol":
                    return 40;
                    break;

                case "Shotgun":
                    return 20;
                    break;

                case "Sniper":
                    return 100;
                    break;

                default:
                    return 0;
                    break;
            }
        }
    }
}
