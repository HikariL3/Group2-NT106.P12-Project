using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace Server_ShootOutGame
{
    public partial class Form2 : Form
    {
        private static List<Player> connectedPlayers = new List<Player>();
        private static List<Lobby> lobbies = new List<Lobby>(); 
        private static readonly int port = 8989;
        private TcpListener server;

        public Form2()
        {
            InitializeComponent();
            StartServer();
        }
        //Mở server
        private void StartServer()
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            UpdateInfo($"Server is running on port {port}...");

            Thread serverThread = new Thread(() =>
            {
                while (true)
                {
                    var client = server.AcceptTcpClient();
                    UpdateInfo($"Client connected: {client.Client.RemoteEndPoint}");
                    connectedPlayers.Add(new Player { PlayerSocket = client });
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
            });
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        //Xử lý kết nối tới server và nhận thông tin từ client
        private void HandleClient(object obj)
        {
            var client = (TcpClient)obj;
            var buffer = new byte[1024];
            var player = connectedPlayers.Find(p => p.PlayerSocket == client);

            try
            {
                while (client.Connected)
                {
                    var stream = client.GetStream();

                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        UpdateInfo($"Received message from {client.Client.RemoteEndPoint}: {message}");
                        AnalyzingMessage(message, player);
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                UpdateInfo($"Client error: {ex.Message}");
            }
            finally
            {
                client.Close();
                connectedPlayers.Remove(player);
                UpdateInfo("Client disconnected.");
            }
        }
        //Phân tích gói tin nhận được từ client
        private void AnalyzingMessage(string message, Player player)
        {
            //Tách thông điệp (phân tích)
            string[] arrPayload = message.Split(';');
            //Phân loại thông điệp để xử lý đúng theo yêu cầu
            switch (arrPayload[0])
            {
                //client kết nối tới server
                case "CONNECT":
                    player.PlayerName = arrPayload[1];
                    if (connectedPlayers.Count == 1)
                    {
                        player.IsHost = true;
                        UpdateInfo($"{player.PlayerName} is the host.");
                    }
                    break;
                //client ngắt kết nối
                case "DISCONNECT":
                    HandleDisconnect(player);
                    break;
                //client muốn Tạo phòng
                case "CREATE_ROOM":
                    CreateRoom(player, arrPayload[1]);
                    break;
                //Client yêu cầu gửi danh sách phòng hiện có
                case "SEND_ROOM_LIST":
                    SendRoomList(player);
                    break;
                //CLient muốn tham gia vào phòng
                case "JOIN_ROOM":
                    JoinRoom(player, arrPayload[1]);
                    break;
                //client muốn gửi danh sách người chơi (tên, trạng thái sẵn sàng,...) trong cùng 1 phòng
                case "SEND_LOBBY":
                    SendLobbyInfoToAll(player, arrPayload[1]);
                    break;
                //client ra hiệu sẵn sàng và server sẽ cập nhập trạng thái ready của người chơi đó
                case "READY":
                    var lobby = FindLobbyByPlayer(player);
                    lobby.Players.SingleOrDefault(r => r.PlayerName == player.PlayerName).IsReady = true;
                    string readyInfo = $"READY_INFO;{player.PlayerName}";
                    foreach (var _player in lobby.Players)
                    {
                        SendMessageToPlayer(_player, readyInfo);
                    }
                    break;
                //client gửi tin nhắn trong lobby và sẽ gửi tới các người chơi khác
                case "MESSAGE":
                    string content = $"SEND_MESSAGE;{arrPayload[1]}";
                    BroadcastMessage(content, player);
                    break;
                //host phòng gửi tới server việc game bắt đầu và server sẽ thông báo lại cho các player khác để họ cũng bắt đầu
                case "START":
                    StartGameForLobby(player);
                    break;
                //client gửi các thông số về game (score, kill) cho server để cập nhập dữ liệu trong phần class Lobby và Player, 
                //đồng thời gửi tới các người chơi khác thông số của người chơi này
                case "STATS":
                    int killCount = int.Parse(arrPayload[1]);
                    int scoreGained = int.Parse(arrPayload[2]);
                    player.KillCount += killCount;
                    player.Score += scoreGained;
                    UpdatePlayerStats(player);
                    break;
                //client khi nhấn nút xem bảng xếp hạng thì phải kiểm tra xem players khác còn trong trận không
                case "GAMEOVER":
                    CheckGameOver(player);
                    break;
                //yêu cầu gửi bảng xếp hạng cho client
                case "RANKING":
                    var playerLobby = FindLobbyByPlayer(player);
                    if (playerLobby != null)
                    {
                        BroadcastRanking(playerLobby);
                    }
                    break;
                //client sẽ thoát ra và yêu cầu server xóa hết dữ liệu của lobby vừa chơi xong
                case "CLEAR_LOBBY":
                    var _lobby = FindLobbyByPlayer(player);
                    if(_lobby != null)
                    {
                        _lobby.Players.Clear();
                        _lobby.Host = null;
                        lobbies.RemoveAll(l => l.RoomId == _lobby.RoomId);
                    }
                    string clearLobbyMessage = "CLEAR_LOBBY";
                    SendMessageToPlayer(player, clearLobbyMessage);
                    break;
                //Thoát game (tui chưa sài đến....)
                case "DISCONNECTGameRoom":
                    CloseAllGameRooms();
                    break;

                default:
                    //các thông điệp bị sai cấu trúc hoặc không nhận diện được
                    UpdateInfo($"Unknown command received: {arrPayload[0]} from {player.PlayerName}");
                    break;
            }
        }
        //Xử lý client ngắt kết nối (xóa khỏi danh sách người chơi) và gửi lại client đó là đã hoàn thành
        private void HandleDisconnect(Player player)
        {
            connectedPlayers.Remove(player);
            player.PlayerSocket.Close();
            UpdateInfo($"{player.PlayerName} has disconnected.");

            string disconnectMessage = $"PLAYER_DISCONNECTED;{player.PlayerName}";
            BroadcastMessage(disconnectMessage, player);

            var lobby = FindLobbyByPlayer(player);
            if (lobby != null)
            {
                lobby.Players.RemoveAll(p => p.PlayerName == player.PlayerName);
            }
        }
        //Tạo phòng, nếu phòng đã tạo hoặc đủ người thì thông báo lại
        private void CreateRoom(Player player, string id)
        {
            var lobby = lobbies.SingleOrDefault(r => r.RoomId == id);
            if (lobby == null)
            {
                string roomId;
                if (!string.IsNullOrEmpty(id))
                {
                    roomId = id;
                }
                else
                {
                    roomId = GenerateRoomId();
                }
                Lobby newLobby = new Lobby
                {
                    RoomId = roomId,
                    Host = player,
                    Players = new List<Player> { player }
                };
                lobbies.Add(newLobby);
                UpdateInfo($"Lobby {roomId} has been created by {player.PlayerName}.");
                SendRoomList(player);
                string joinMessage = $"JOINED;{roomId}";
                SendMessageToPlayer(player, joinMessage);
            }
            else
            {
                string errorMessage = $"ERROR_CREATE;{id}";
                SendMessageToPlayer(player, errorMessage);
            }
        }
        //Tham gia vào 1 phòng, nếu phòng ko tồn tại hoặc đủ người thì báo lại client đó
        private void JoinRoom(Player player, string roomId)
        {
            var lobby = lobbies.SingleOrDefault(r => r.RoomId == roomId);
            if (lobby != null && lobby.Players.Count < 4 && !lobby.IsStart)
            {
                lobby.Players.Add(player);
                string joinMessage = $"JOINED;{roomId}";
                SendMessageToPlayer(player, joinMessage);
            }
            else
            {
                string errorMessage = $"ERROR_JOIN;{roomId}";
                SendMessageToPlayer(player, errorMessage);
            }
        }
        //Gửi thông báo bắt đầu để client vào game
        private void StartGameForLobby(Player player)
        {
            Lobby lobby = FindLobbyByPlayer(player);
            if (lobby != null)
            {
                lobby.IsStart = true;   
                string startMessage = $"START;{lobby.Host.PlayerName};{string.Join(",", lobby.Players.Select(p => p.PlayerName))}";
                foreach (var p in lobby.Players)
                {
                    SendMessageToPlayer(p, startMessage);
                }
                UpdateInfo("Game started for lobby hosted by " + lobby.Host.PlayerName);
            }
            else
            {
                UpdateInfo("Lobby not found for player: " + player.PlayerName);
            }
        }
        //client kiểm tra xem còn người chơi khác trong trận không
        private void CheckGameOver(Player player)
        {
            bool check = true;
            var lobby = FindLobbyByPlayer(player);
            if(lobby != null)
            {
                foreach(var _player in lobby.Players)
                {
                    if(!_player.IsGameOver)
                    {
                        check = false; 
                        break;
                    }
                }
                string GameOverInfo = $"GAMEOVER;{check.ToString()}";
                SendMessageToPlayer(player, GameOverInfo);
            }
        }
        //Đóng game
        private void CloseAllGameRooms()
        {
            foreach (var player in connectedPlayers)
            {
                string closeMessage = "CLOSE_ALL";
                SendMessageToPlayer(player, closeMessage);
            }
            UpdateInfo("All game rooms have been closed.");
        }
        //Gửi các thông số (kill, score) của client này cho các client khác
        private void UpdatePlayerStats(Player player)
        {
            string updateMessage = $"UPDATE_STATS;{player.PlayerName};{player.KillCount};{player.Score}";
            //BroadcastMessage(updateMessage, player);
            player.IsGameOver = true;
            Lobby lobby = FindLobbyByPlayer(player);
            if (lobby != null)
            {
                foreach (var _player in lobby.Players)
                {
                    SendMessageToPlayer(_player, updateMessage);
                }
            }

            UpdateInfo($"Updated stats for {player.PlayerName} - Kills: {player.KillCount}, Score: {player.Score}");
        }
        //gửi thông báo tới các client khác ngoại trừ client "sender"
        private void BroadcastMessage(string message, Player sender)
        {
            byte[] msgBuffer = Encoding.UTF8.GetBytes(message);
            Lobby lobby = FindLobbyByPlayer(sender);
            if(lobby != null)
            {
                foreach (var player in lobby.Players)
                {
                    if (player.PlayerSocket != sender.PlayerSocket)
                    {
                        SendMessageToPlayer(player, message);
                    }
                }
            }
        }
        //Gửi tin nhắn cho 1 client cụ thể
        private void SendMessageToPlayer(Player player, string message)
        {
            var stream = player.PlayerSocket.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
        //Tạo phòng ngẫu nhiên (nếu ko ghi vào textbox trong form NewGame)
        private static string GenerateRoomId()
        {
            return Guid.NewGuid().ToString().Substring(0, 6);
        }
        //Gửi danh sách phòng cho tất cả client
        private void SendRoomListToAll()
        {
            foreach (var player in connectedPlayers)
            {
                SendRoomList(player);
            }
        }

        //gửi danh sách phòng cho 1 client cụ thể
        private void SendRoomList(Player player)
        {
            StringBuilder roomList = new StringBuilder("ROOMLIST;");
            foreach (var lobby in lobbies)
            {
                roomList.Append($"{lobby.RoomId};{lobby.Host.PlayerName};");//sửa khúc này
            }

            SendMessageToPlayer(player, roomList.ToString());
        }
        //TÌm lobby của người chơi --> hàm này hỗ trợ cho các hàm khác
        private Lobby FindLobbyByPlayer(Player player)
        {
            return lobbies.FirstOrDefault(lobby => lobby.Players.Contains(player));
        }

        private void ShowingInfo_TextChanged(object sender, EventArgs e) { }

        //Gửi danh sách người chơi (tên, trạng thái bắt đầu) cho các client khác khi họ vào lobby
        private void SendLobbyInfoToAll(Player player, string roomId)
        {
            var lobby = lobbies.FirstOrDefault(r => r.RoomId == roomId);
            if (lobby != null)
            {
                string lobbyInfo = $"LOBBY_INFO;{lobby.RoomId};{lobby.Players.Count};" +
                    $"{string.Join(",", lobby.Players.Select(p => p.PlayerName))};" +
                    $"{string.Join(",", lobby.Players.Select(p => p.IsReady.ToString()))}";
                foreach (var _player in connectedPlayers)
                {
                    SendMessageToPlayer(_player, lobbyInfo);//sửa chỗ này
                }
            }
        }
        //hiển thị trên form server
        public void UpdateInfo(string message)
        {
            if (ShowingInfo.InvokeRequired)
            {
                ShowingInfo.Invoke(new Action(() => UpdateInfo(message)));
            }
            else
            {
                ShowingInfo.AppendText(message + Environment.NewLine);
                ShowingInfo.SelectionStart = ShowingInfo.Text.Length; // Cuộn xuống cuối
                ShowingInfo.ScrollToCaret(); // Cuộn đến vị trí con trỏ
            }
        }
        //Gửi bảng xếp hạng cho các client khác
        private void BroadcastRanking(Lobby lobby)
        {
            var rankingData = new StringBuilder("RANKING;");

            // Sắp xếp người chơi trong phòng theo điểm giảm dần
            var sortedPlayers = lobby.Players.OrderByDescending(p => p.Score).ToList();

            foreach (var player in sortedPlayers)
            {
                rankingData.Append($"{player.PlayerName};{player.KillCount};{player.Score};");
            }

            string rankingMessage = rankingData.ToString().TrimEnd(';');

            // Gửi dữ liệu xếp hạng đến tất cả người chơi trong phòng
            foreach (var player in lobby.Players)
            {
                SendMessageToPlayer(player, rankingMessage);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (connectedPlayers)
            {
                foreach (var player in connectedPlayers)
                {
                    try
                    {
                        player.PlayerSocket.GetStream().Close();
                        player.PlayerSocket.Close();
                    }
                    catch (Exception ex)
                    {
                        UpdateInfo($"Error closing client: {ex.Message}");
                    }
                }

                connectedPlayers.Clear();
            }

            if (server != null)
            {
                try
                {
                    server.Stop();
                }
                catch (Exception ex)
                {
                    UpdateInfo($"Error stopping server: {ex.Message}");
                }
            }
        }
        private void Form2_Load(object sender, EventArgs e) { }
    }
    public class Player
    {
        public TcpClient PlayerSocket { get; set; }
        public string PlayerName { get; set; }
        public bool IsHost { get; set; }
        public int Turn { get; set; }
        public bool IsReady { get; set; } = false;
        public bool IsGameOver { get; set; } = false;
        public int Score { get; set; } = 0;
        public int KillCount { get; set; } = 0;
    }

    public class Lobby
    {
        public string RoomId { get; set; }
        public Player Host { get; set; }
        public bool IsStart { get; set; } = false;
        public List<Player> Players { get; set; } = new List<Player>();
    }
}
