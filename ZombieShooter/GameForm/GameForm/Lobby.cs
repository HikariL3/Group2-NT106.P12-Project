using Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameForm
{
    public partial class Lobby : Form
    {
        public Lobby()
        {
            InitializeComponent();
            soLuong.Text = string.Format($"SỐ LƯỢNG: {GameClient.rooms[0].Players.Count}");
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

        private bool checkReady()
        {
            if (readyLabel1.Text == "Chưa sẵn sàng") return false;
            if (readyLabel2.Text == "Chưa sẵn sàng") return false;
            if (readyLabel3.Text == "Chưa sẵn sàng") return false;
            if (readyLabel4.Text == "Chưa sẵn sàng") return false;

            return true;
        }

        private void readyButton_Click(object sender, EventArgs e)
        {
            
        }

        private void checkButton_Click(object sender, EventArgs e)
        {

            string message1 = String.Format($"{GameClient.rooms[0].idRoom} {GameClient.rooms[0].Status} {GameClient.rooms[0].HostName} {GameClient.rooms[0].Players[0]}");
            showMessage.Items.Add(message1);

            string message2 = String.Format($"{GameClient.rooms[0].idRoom} {GameClient.rooms[0].Status} {GameClient.rooms[0].HostName} {GameClient.rooms[0].Players[1]}");
            showMessage.Items.Add(message2);
        }
    }
}
