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
            //Tách thông điệp để phân tích
            string[] payload = data.Split(';');
            string messageType = payload[0];
            //Phân loại thông điệp nhận từ server
            switch (messageType)
            {
                //Nhận danh sách phòng mà server gửi cho
                case "ROOMLIST":
                    AddRoomList(payload);
                    break;
                //server thông báo lại là đã vào phòng hoặc không
                case "JOINED":
                    joinedRoom = payload[1];
                    var lobby = lobbies.SingleOrDefault(r => r.RoomId == payload[1]);
                    if (lobby != null)
                    {
                        joinedLobby = lobby;
                    }
                    break;
                 //Nhận danh sách người chơi (tên, trạng thái sẵn sàng) từ server và cập nhập vào class Lobby, Player
                case "LOBBY_INFO":
                    UpdateLobby(payload);
                    break;
                //Nhận từ server xem các người chơi khác sẵn sàng hay chưa
                case "READY_INFO":
                    UpdateReadyInfo(payload);
                    break;
                //Nhận message trong lobby từ các client khác
                case "SEND_MESSAGE":
                    UpdateMessage(payload[1]);
                    break;
                //Nhận thông báo bắt đầu vào game từ server
                case "START":
                    isStartGame = true;
                    break;
                //Nhận thông số (kill, score) từ player khác
                case "UPDATE_STATS":
                    UpdateStats(payload);
                    break;
                //Kiểm tra xem các người chơi khác đã xong trận chưa
                case "GAMEOVER":
                    if (payload[1] == "True")
                        joinedLobby.IsGameOver = true;
                    break;
                //Server thông báo đã xử lý ngắt kết nối của mình
                case "PLAYER_DISCONNECTED":
                    HandleDisconnect(payload[1]);  
                    break;
                //Xóa lobby vừa chơi xong
                case "CLEAR_LOBBY":
                    ClearLobby();
                    break;
                //Lỗi khi tham gia vào phòng (phòng muốn vào ko tồn tại hoặc đủ người hoặc đã vào trận)
                case "ERROR_JOIN":
                    isJoinRoom = false;
                    break;
                //Lỗi khi tạo phòng (phòng đã được tạo)
                case "ERROR_CREATE":
                    isCreateRoom = false;
                    break;
            }
        }
        //Cập nhập danh sách phòng hiện có vào list lobbies
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
        //Xử lý ngắt kết nối
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
        //Cập nhập danh sách các người khác trong cùng 1 lobby
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
        //Cập nhập trạng thái sẵn sàng của người chơi khác
        public static void UpdateReadyInfo(string[] payload)
        {
            var player = joinedLobby.Players.SingleOrDefault(r => r.Name == payload[1]);
            if (player != null)
                player.IsReady = true;
        }
        //Kiểm tra xem người chơi có "name" đã sẵn sàng chưa
        public static bool CheckIsReady(string name)
        {
            var player = joinedLobby.Players.SingleOrDefault(r => r.Name == name);
            if (player == null) return false;
            return player.IsReady;
        }
        //Kiểm tra xem tất cả player đã sẵn sàng chưa
        public static bool CheckIsReadyForAll()
        {
            foreach (var player in joinedLobby.Players)
            {
                if (!player.IsReady) return false;
            }
            return true;
        }
        //Kiểm tra xem còn người chơi khác trong trận không
        public static bool CheckGameOver()
        {
            if(joinedLobby.IsGameOver)
                return true;
            return false;
        }
        //Tin nhắn từ người chơi khác
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
        //Cập nhập thông số (score và kill) vào class lobby và player
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
        //Xóa lobby
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
    }

    public class Lobby
    {
        public bool IsGameOver { get; set; } = false;
        public bool IsStart { get; set; } = false;
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
    }
}
