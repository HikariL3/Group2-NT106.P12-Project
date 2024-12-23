﻿using Client;
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
using System.Diagnostics;

namespace GameForm
{
    public partial class Lobby : Form
    {
        private CancellationTokenSource _cts;

        public Lobby()
        {
            InitializeComponent();
            namePlayer1.AutoEllipsis = true;
            namePlayer2.AutoEllipsis = true;
            namePlayer3.AutoEllipsis = true;
            namePlayer4.AutoEllipsis = true;
            this.Load += Lobby_Load;

            GameClient.OnReceiveMessage += UpdateMessage;
        }

        private async void Lobby_Load(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            await RunContinuouslyAsync(_cts.Token);

        }

        private async Task RunContinuouslyAsync(CancellationToken token)
        {
            GameClient.SendData($"SEND_LOBBY;{GameClient.joinedRoom}");
            await Task.Delay(100, token);

            while (!token.IsCancellationRequested)
            {
                InitLobby();

                if (GameClient.isStartGame)
                {
                    // Synchronize player list from `joinedLobby` to `GameClient.players` once game starts
                    if (GameClient.players.Count == 0)
                    {
                        foreach (var lobbyPlayer in GameClient.joinedLobby.Players)
                        {
                            GameClient.players.Add(new Player
                            {
                                Name = lobbyPlayer.Name,
                                Position = new PointF(0, 0) // Set initial spawn point or position
                            });
                        }
                        Debug.WriteLine("All players added to GameClient.players for MainGame start.");
                    }

                    // Cancel lobby task and start MainGame
                    _cts.Cancel();
                    MainGame mainGame = new MainGame();
                    this.Hide();
                    mainGame.Show();
                }

                try
                {
                    await Task.Delay(500, token);
                }
                catch (TaskCanceledException)
                {
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
            else
            {
                startButton.Enabled = false;
                startButton.Visible = false;
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
                        namePlayer3.Text = GameClient.joinedLobby.PlayersName[i];
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
                        avatarPlayer4.Image = Properties.Resources.player1;
                        readyLabel4.Visible = true;
                        if (GameClient.CheckIsReady(namePlayer4.Text))
                        {
                            readyLabel4.Text = "Sẵn sàng";
                            readyLabel4.ForeColor = Color.Lime;
                        }
                        break;
                }
            }
            for(int i = countPlayer; i < 4; i++)
            {
                switch(i)
                {
                    case 0:
                        namePlayer1.Text = "Player1";
                        avatarPlayer1.Image = Properties.Resources.avatar_vo_danh_9;
                        readyLabel1.Visible = false;
                        readyLabel1.Text = "Chưa sẵn sàng";
                        readyLabel1.ForeColor = Color.Red;
                        break;

                    case 1:
                        namePlayer2.Text = "Player2";
                        avatarPlayer2.Image = Properties.Resources.avatar_vo_danh_9;
                        readyLabel2.Visible = false;
                        readyLabel2.Text = "Chưa sẵn sàng";
                        readyLabel2.ForeColor = Color.Red;
                        break;

                    case 2:
                        namePlayer3.Text = "Player3";
                        avatarPlayer3.Image = Properties.Resources.avatar_vo_danh_9;
                        readyLabel3.Visible = false;
                        readyLabel3.Text = "Chưa sẵn sàng";
                        readyLabel3.ForeColor = Color.Red;
                        break;

                    case 3:
                        namePlayer4.Text = "Player4";
                        avatarPlayer4.Image = Properties.Resources.avatar_vo_danh_9;
                        readyLabel4.Visible = false;
                        readyLabel4.Text = "Chưa sẵn sàng";
                        readyLabel4.ForeColor = Color.Red;
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
            else
            {
                MessageBox.Show("Các người chơi khác vẫn chưa sẵn sàng!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateMessage(string message)
        {
            if(showMessage.InvokeRequired)
            {
                showMessage.Invoke(new Action(() => UpdateMessage(message)));
            }
            else
            {
                showMessage.Items.Add(message.ToString());
            }
        }

        private void Lobby_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            _cts?.Cancel();          
            GameClient.Disconnect();  
            GameClient.ClearLobby();
            Login login = new Login(); 
            login.Show();
        }

    }
}
