using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GameForm
{
    public partial class NewRoom : Form
    {
        public NewRoom()
        {
            InitializeComponent();
        }

        Lobby lobby;

        bool checkMaPhong(string idRoom)
        {
            if (!string.IsNullOrEmpty(maPhong.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Yêu cầu nhập mã phòng!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        void TurnForm()
        {
            lobby = new Lobby();
            this.Hide();
            lobby.Show();
        }

        private async void createButton_Click(object sender, EventArgs e)
        {
            GameClient.SendData($"CREATE_ROOM;{maPhong.Text}");

            await WaitFunction();

            if (GameClient.isCreateRoom)
            {
                TurnForm();
            }
            else
            {
                MessageBox.Show($"Phòng {maPhong.Text} đã được tạo!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listPhong_SelectedIndexChanged(object sender, EventArgs e)
        {
            maPhong.Text = listPhong.SelectedItem.ToString();
        }

        private async void joinButton_Click(object sender, EventArgs e)
        {
            if (!checkMaPhong(maPhong.Text)) return;

            GameClient.SendData($"JOIN_ROOM;{maPhong.Text}");

            await WaitFunction();

            if(GameClient.isJoinRoom)
            {
                TurnForm();
            }
            else
            {
                MessageBox.Show($"Phòng {maPhong.Text} chưa được tạo hoặc đã đủ người hoặc đã bắt đầu chơi!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void showRoomList_Click(object sender, EventArgs e)
        {
            GameClient.SendData("SEND_ROOM_LIST");

            await WaitFunction();

            listPhong.Items.Clear();
            int count = GameClient.lobbies.Count;
            for (int i = 0; i < count; i++)
            {
                listPhong.Items.Add(GameClient.lobbies[i].RoomId);
            }
        }

        private async Task WaitFunction()
        {
            await Task.Delay(700);
        }

        private void NewRoom_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameClient.Disconnect();
            GameClient.ClearLobby();
            Login login = new Login();
            login.Show();
        }
    }
}
