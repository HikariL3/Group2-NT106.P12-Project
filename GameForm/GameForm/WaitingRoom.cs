using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameForm
{
    public partial class WaitingRoom : Form
    {
        public WaitingRoom()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(messageBox.Text)) return;

            string message = String.Format($"[Name]: {messageBox.Text}");
            messageBox.Clear();
            showMessage.Items.Add(message);
        }



        private void namePlayer2_Click(object sender, EventArgs e)
        {

        }

        private void readyButton_Click(object sender, EventArgs e)
        {

        }

        private void soLuong_Click(object sender, EventArgs e)
        {

        }

        private void maPhong_Click(object sender, EventArgs e)
        {

        }
    }
}
