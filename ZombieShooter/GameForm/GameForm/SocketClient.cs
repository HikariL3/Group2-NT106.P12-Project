using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using GameForm;

namespace Client
{
    //static class SocketClient
    //{
    //    [STAThread]
    //    static void Main()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //        Application.Run(new MainGame());
    //    }
    //}

    public class GameClient
    {
        public static Socket clientSocket;
        public static Thread receiveThread;
        private static bool stopThread = false;    
        public static List<Player> players = new List<Player>();
        public static Player localPlayer;

        public static bool isStartGame = false;

        public static bool isCreateRoom = true;
        public static bool isJoinRoom = true;

        public static List<Lobby> lobbies = new List<Lobby>();
        public static string joinedRoom = null;
        public static Lobby joinedLobby = null;

        public static List<string> messages = new List<string>();


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
            while (clientSocket.Connected && !stopThread)
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
                case "ROOMLIST":
                    AddRoomList(payload);
                    break;

                case "JOINED":
                    joinedRoom = payload[1];
                    var lobby = lobbies.SingleOrDefault(r => r.RoomId == payload[1]);
                    if (lobby != null)
                    {
                        joinedLobby = lobby;
                    }
                    break;

                case "LOBBY_INFO":
                    UpdateLobby(payload);
                    break;

                case "READY_INFO":
                    UpdateReadyInfo(payload);
                    break;

                case "SEND_MESSAGE":
                    UpdateMessage(payload[1]);
                    break;

                case "START":
                    isStartGame = true;
                    break;

                case "UPDATE_STATS":
                    UpdateStats(payload);
                    break;

                case "GAMEOVER":
                    if (payload[1] == "True")
                        joinedLobby.IsGameOver = true;
                    break;

                case "PLAYER_DISCONNECTED":
                    HandleDisconnect(payload[1]);  
                    break;

                case "CLEAR_LOBBY":
                    ClearLobby();
                    break;

                case "ERROR_JOIN":
                    isJoinRoom = false;
                    break;

                case "ERROR_CREATE":
                    isCreateRoom = false;
                    break;
            }
        }
        //Cập nhập danh sách phòng
        private static void AddRoomList(string[] payload)
        {
            for (int i = 1; i < payload.Length - 1; i += 2)
            {
                var lobby = lobbies.SingleOrDefault(r => r.RoomId == payload[i]);
                if (lobby == null)
                {
                    Lobby newLobby = new Lobby()
                    {
                        RoomId = payload[i],
                        HostName = payload[i + 1],
                        Host = new Player { Name = payload[i + 1] },
                        PlayersName = new List<string> { payload[i + 1] },
                        Players = new List<Player>
                        {
                            new Player() {Name = payload[i + 1]}
                        }
                    };
                    lobbies.Add(newLobby);
                }
            }
        }

        private static void HandleDisconnect(string playerName)
        {
            if(joinedLobby != null && joinedLobby.PlayersName.Contains(playerName))
            {
                joinedLobby.Players.RemoveAll(p => p.Name == playerName);
                joinedLobby.PlayersName.Remove(playerName);

                if (joinedLobby.HostName == playerName)
                {
                    joinedLobby.HostName = joinedLobby.PlayersName[0];
                    joinedLobby.Host = new Player { Name = joinedLobby.PlayersName[0] };
                }
            }
        }

        private static void UpdateLobby(string[] payload)
        {
            var lobby = lobbies.SingleOrDefault(r => r.RoomId == payload[1]);
            if (lobby != null)
            {
                joinedLobby = lobby;
                int playerCount = Convert.ToInt32(payload[2]);
                string[] playerList = payload[3].Split(',');
                string[] readyPlayerList = payload[4].Split(',');
                for (int i = 0; i < playerCount; i++)
                {
                    if (!lobby.PlayersName.Contains(playerList[i]))
                    {
                        lobby.PlayersName.Add(playerList[i]);
                        lobby.Players.Add(new Player() { 
                            Name = playerList[i],
                            IsReady = bool.Parse(readyPlayerList[i])
                        });
                    }
                    else
                    {
                        lobby.Players[i].IsReady = bool.Parse(readyPlayerList[i]);
                    }
                }
            }
        }
        public static void UpdateReadyInfo(string[] payload)
        {
            var player = joinedLobby.Players.SingleOrDefault(r => r.Name == payload[1]);
            if (player != null)
                player.IsReady = true;
        }

        public static bool CheckIsReady(string name)
        {
            var player = joinedLobby.Players.SingleOrDefault(r => r.Name == name);
            if (player == null) return false;
            return player.IsReady;
        }

        public static bool CheckIsReadyForAll()
        {
            foreach (var player in joinedLobby.Players)
            {
                if (!player.IsReady) return false;
            }
            return true;
        }

        public static bool CheckGameOver()
        {
            if(joinedLobby.IsGameOver)
                return true;
            return false;
        }

        public static void UpdateMessage(string content)
        {
            messages.Add(content);
        }

        public static string GetMessageFromPlayers()
        {
            if (messages.Count == 0) return null;
            string content = messages[0];
            messages.Clear();
            return content;
        }

        public static void UpdateStats(string[] payload)
        {
            foreach (var player in joinedLobby.Players)
            {
                if (player.Name == payload[1])
                {
                    player.Kill += int.Parse(payload[2]);
                    player.Score += int.Parse(payload[3]);
                }
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

        // Ngắt kết nối từ server
        public static void Disconnect()
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                try
                {
                    SendData("DISCONNECT");
                    stopThread = true;

                    if (receiveThread != null && receiveThread.IsAlive)
                    {
                        receiveThread.Join();
                    }

                    if (clientSocket.Connected)
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                    }
                    clientSocket.Close();
                }
                catch (SocketException ex)
                {
                    // Xử lý lỗi nếu cần
                    Console.WriteLine($"Error disconnecting: {ex.Message}");
                }
                finally
                {
                    clientSocket = null; // Đặt lại clientSocket về null
                }
            }
            stopThread = false;
            localPlayer = null;
            isCreateRoom = true;
            isJoinRoom = true;
            isStartGame = false;
        }
        public static void ClearLobby()
        {
            foreach(var lobby in lobbies)
            {
                lobby.Players.Clear();
                lobby.PlayersName.Clear();
                lobby.Host = null;
                lobby.RoomId = null;        
                lobby.HostName = null;       
                lobby.IsGameOver = false;
            }
            lobbies.Clear();
            joinedRoom = null;
            joinedLobby = null;
            isCreateRoom = true;
            isJoinRoom = true;
            isStartGame = false;
        }
    }

    

    public class Player
    {
        public string Id { get; set; }
        public PointF Position { get; set; }
        public bool IsReady { get; set; } = false;
        public string Name { get; set; }
        public int Score { get; set; } = 0;
        public int Kill { get; set; } = 0;

        // Xử lý va chạm với tường
        public void HandleWallCollision()
        {
            // Thay đổi vị trí hoặc trạng thái người chơi khi va chạm
        }
    }

    public class Lobby
    {
        public bool IsGameOver { get; set; } = false;   
        public string RoomId { get; set; }
        public Player Host { get; set; }
        public string HostName { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public List<string> PlayersName { get; set; } = new List<string>();
    }

    public class GameForm : Form
    {
        public GameForm()
        {
            // Khởi tạo giao diện game
            this.DoubleBuffered = true; // Giúp giảm hiện tượng nhấp nháy khi vẽ
            this.FormClosing += GameForm_FormClosing; // Đăng ký sự kiện FormClosing
        }

        // Xử lý sự kiện khi form đóng
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameClient.Disconnect(); // Gọi Disconnect trước khi form đóng
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
