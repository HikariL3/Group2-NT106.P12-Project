using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Drawing;
using System.Windows.Forms;

namespace GameForm
{
    class Bullet
    {
        public string direction;
        public int bulletLeft;
        public int bulletTop;
        public int speed;
        public int range;
        private PictureBox bullet = new PictureBox();
        private Timer bulletTimer = new Timer();
        private int distanceTraveled = 0;
        public Bullet(int bulletSpeed, int bulletRange)
        {
            speed = bulletSpeed;
            range = bulletRange;
        }


        public void MakeBullet(Form form)
        {
            bullet.BackColor = Color.White;
            bullet.Size = new Size(7, 7);
            bullet.Tag = "bullet";
            bullet.Left = bulletLeft;
            bullet.Top = bulletTop;
            bullet.BringToFront();

            if (form.InvokeRequired)
            {
                form.Invoke((MethodInvoker)delegate
                {
                    form.Controls.Add(bullet);
                });
            }
            else
            {
                form.Controls.Add(bullet);
            }

            bulletTimer.Interval = 35;
            bulletTimer.Tick += new EventHandler(BulletTimerEvent);
            bullet.BringToFront();
            bulletTimer.Start();

        }

        public void MakeBulletSniper(Form form)
        {
            bullet.BackColor = Color.White;
            bullet.Size = new Size(28, 7);
            bullet.Tag = "bullet";
            bullet.Left = bulletLeft;
            bullet.Top = bulletTop;
            bullet.BringToFront();

            if (form.InvokeRequired)
            {
                form.Invoke((MethodInvoker)delegate
                {
                    form.Controls.Add(bullet);
                });
            }
            else
            {
                form.Controls.Add(bullet);
            }

            bulletTimer.Interval = 35;
            bulletTimer.Tick += new EventHandler(BulletTimerEvent);
            bullet.BringToFront();
            bulletTimer.Start();
        }



        private void BulletTimerEvent(object sender, EventArgs e)
        {

            if (direction == "left")
            {
                bullet.Left -= speed;
            }

            if (direction == "right")
            {
                bullet.Left += speed;
            }

            if (direction == "up")
            {
                bullet.Top -= speed;
            }

            if (direction == "down")
            {
                bullet.Top += speed;
            }


            distanceTraveled += speed;

            if (distanceTraveled >= range || bullet.Left < 10 || bullet.Left > 860 || bullet.Top < 10 || bullet.Top > 600)
            {
                bulletTimer.Stop();
                bulletTimer.Dispose();
                bullet.Dispose();
                bulletTimer = null;
                bullet = null;
            }
        }
    }
}