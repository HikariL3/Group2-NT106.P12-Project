using Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shoot_Out_Game_MOO_ICT;

namespace GameForm
{
    public partial class Lobby : Form
    {
        private CancellationTokenSource _cts;


        public Lobby()
        {
            InitializeComponent();
            this.Load += Lobby_Load;
        }

        private async void Lobby_Load(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            await RunContinuouslyAsync(_cts.Token);

        }

        async Task RunContinuouslyAsync(CancellationToken token)
        {
            GameClient.SendData($"SEND_LOBBY;{GameClient.joinedRoom}");
            await Task.Delay(1000, token);

            while (!token.IsCancellationRequested) // Kiểm tra trạng thái hủy
            {
                InitLobby();
                string s = GameClient.GetMessageFromPlayers();
                if (s != null)
                    showMessage.Items.Add(s);


                if (GameClient.isStartGame)
                {
                    _cts.Cancel();
                    MainGame mainGame = new MainGame();
                    this.Hide();
                    mainGame.Show();
                    //this.Close();
                }

                try
                {
                    // Chờ 1 giây trước khi lặp lại
                    await Task.Delay(500, token);
                }
                catch (TaskCanceledException)
                {
                    // Thoát vòng lặp nếu Task bị hủy
                    break;
                }
            }
        }

        private void InitLobby()
        {
            if (GameClient.localPlayer.Name == GameClient.joinedLobby.Host.Name)
            {
                startButton.Enabled = true;
                startButton.Visible = true;
            }

            string[] playersName = new string[4];
            soLuong.Text = "SỐ LƯỢNG: " + GameClient.joinedLobby.PlayersName.Count.ToString();
            maPhong.Text = "MÃ PHÒNG: " + GameClient.joinedLobby.RoomId;
            int countPlayer = GameClient.joinedLobby.PlayersName.Count;

            for (int i = 0; i < countPlayer; i++)
            {
                switch (i)
                {
                    case 0:
                        namePlayer1.Text = GameClient.joinedLobby.PlayersName[i];
                        avatarPlayer1.Image = Properties.Resources.ares;
                        readyLabel1.Visible = true;
                        if (GameClient.CheckIsReady(namePlayer1.Text))
                        {
                            readyLabel1.Text = "Sẵn sàng";
                            readyLabel1.ForeColor = Color.Lime;
                        }
                        break;

                    case 1:
                        namePlayer2.Text = GameClient.joinedLobby.PlayersName[i];
                        avatarPlayer2.Image = Properties.Resources.knight;
                        readyLabel2.Visible = true;
                        if (GameClient.CheckIsReady(namePlayer2.Text))
                        {
                            readyLabel2.Text = "Sẵn sàng";
                            readyLabel2.ForeColor = Color.Lime;
                        }
                        break;

                    case 2:
                        namePlayer2.Text = GameClient.joinedLobby.PlayersName[i];
                        avatarPlayer3.Image = Properties.Resources.serial_killer;
                        readyLabel3.Visible = true;
                        if (GameClient.CheckIsReady(namePlayer3.Text))
                        {
                            readyLabel3.Text = "Sẵn sàng";
                            readyLabel3.ForeColor = Color.Lime;
                        }
                        break;

                    case 3:
                        namePlayer4.Text = GameClient.joinedLobby.PlayersName[i];
                        avatarPlayer2.Image = Properties.Resources.player1;
                        readyLabel4.Visible = true;
                        if (GameClient.CheckIsReady(namePlayer4.Text))
                        {
                            readyLabel4.Text = "Sẵn sàng";
                            readyLabel4.ForeColor = Color.Lime;
                        }
                        break;
                }
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(messageBox.Text)) return;

            string message = String.Format($"[{GameClient.localPlayer.Name}]: {messageBox.Text}");
            messageBox.Clear();
            showMessage.Items.Add(message);

            GameClient.SendData($"MESSAGE;{message}");
        }

        private void readyButton_Click(object sender, EventArgs e)
        {
            GameClient.SendData("READY");
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (GameClient.CheckIsReadyForAll())
            {
                GameClient.SendData("START");
            }
        }

        private void Lobby_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameClient.Disconnect();
            Login login = new Login();
            login.Show();
            _cts?.Cancel();
            this.Close();
        }
    }
}
