﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Shoot_Out_Game_MOO_ICT;

namespace Client
{
    static class SocketClient

    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainGame());
        }
    }
    public class GameClient
    {
        public static Socket clientSocket;
        public static Thread receiveThread;
        public static List<Player> players = new List<Player>();
        public static Player localPlayer;

        // Kết nối đến server
        public static void ConnectToServer(IPEndPoint serverEP)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(serverEP);
            receiveThread = new Thread(ReceiveData);
            receiveThread.Start();
        }

        // Gửi dữ liệu đến server
        public static void SendData(string data)
        {
            byte[] sendData = Encoding.UTF8.GetBytes(data);
            clientSocket.Send(sendData);
        }

        // Nhận dữ liệu từ server
        private static void ReceiveData()
        {
            byte[] buffer = new byte[1024];
            while (clientSocket.Connected)
            {
                int receivedBytes = clientSocket.Receive(buffer);
                string receivedData = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                ProcessReceivedData(receivedData);
            }
        }

        // Xử lý dữ liệu nhận được từ server
        private static void ProcessReceivedData(string data)
        {
            string[] payload = data.Split(';');
            string messageType = payload[0];

            switch (messageType)
            {
                case "PlayerUpdate":
                    UpdatePlayerPositions(payload);
                    break;
                case "Collision":
                    HandleCollision(payload);
                    break;
                    // Thêm các loại thông điệp khác nếu cần
            }
        }

        // Cập nhật vị trí của người chơi
        private static void UpdatePlayerPositions(string[] payload)
        {
            for (int i = 1; i < payload.Length; i++)
            {
                string[] playerData = payload[i].Split(',');
                string playerId = playerData[0];
                float x = float.Parse(playerData[1]);
                float y = float.Parse(playerData[2]);

                var player = players.FirstOrDefault(p => p.Id == playerId);
                if (player != null)
                {
                    player.Position = new PointF(x, y);
                }
                else
                {
                    // Nếu là người chơi mới, thêm vào danh sách
                    players.Add(new Player { Id = playerId, Position = new PointF(x, y) });
                }
            }
        }

        // Xử lý va chạm với tường hoặc người chơi
        private static void HandleCollision(string[] payload)
        {
            string collisionType = payload[1]; // Tường hoặc người chơi
            if (collisionType == "Wall")
            {
                // Xử lý va chạm với tường
                localPlayer.HandleWallCollision();
            }
            else if (collisionType == "Player")
            {
                // Cập nhật hình ảnh người chơi khác
                string playerId = payload[2];
                // Xử lý cập nhật hình ảnh cho playerId
            }
        }

        // Chức năng bắn đạn
        public static void Shoot()
        {
            // Xử lý việc bắn súng
            // Không gửi thông tin lên server
        }

        // Di chuyển người chơi
        public static void MovePlayer(float deltaX, float deltaY)
        {
            // Cập nhật vị trí người chơi
            localPlayer.Position = new PointF(localPlayer.Position.X + deltaX, localPlayer.Position.Y + deltaY);
            SendData($"PlayerUpdate;{localPlayer.Id},{localPlayer.Position.X},{localPlayer.Position.Y}");
        }
    }

    public class Player
    {
        public string Id { get; set; }
        public PointF Position { get; set; }

        // Xử lý va chạm với tường
        public void HandleWallCollision()
        {
            // Thay đổi vị trí hoặc trạng thái người chơi khi va chạm
        }
    }

    public class GameForm : Form
    {
        public GameForm()
        {
            // Khởi tạo giao diện game
            this.DoubleBuffered = true; // Giúp giảm hiện tượng nhấp nháy khi vẽ
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Vẽ tất cả người chơi trên màn hình
            foreach (var player in GameClient.players)
            {
                e.Graphics.FillEllipse(Brushes.Blue, player.Position.X, player.Position.Y, 20, 20);
            }
        }

        // Các phương thức xử lý sự kiện game như KeyDown, MouseClick...
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    GameClient.MovePlayer(0, -5); // Di chuyển lên
                    break;
                case Keys.S:
                    GameClient.MovePlayer(0, 5); // Di chuyển xuống
                    break;
                case Keys.A:
                    GameClient.MovePlayer(-5, 0); // Di chuyển trái
                    break;
                case Keys.D:
                    GameClient.MovePlayer(5, 0); // Di chuyển phải
                    break;
                case Keys.Space:
                    GameClient.Shoot(); // Bắn đạn
                    break;
            }
            Invalidate(); // Yêu cầu vẽ lại màn hình
        }
    }
}