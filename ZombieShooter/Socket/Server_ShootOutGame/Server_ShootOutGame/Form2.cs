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

        private void AnalyzingMessage(string message, Player player)
        {
            string[] arrPayload = message.Split(';');

            switch (arrPayload[0])
            {
                case "CONNECT":
                    player.PlayerName = arrPayload[1];
                    if (connectedPlayers.Count == 1)
                    {
                        player.IsHost = true;
                        UpdateInfo($"{player.PlayerName} is the host.");
                    }
                    //SendLobbyInfoToAll(player);
                    break;

                case "DISCONNECT":
                    HandleDisconnect(player);
                    break;

                case "CREATE_ROOM":
                    CreateRoom(player, arrPayload[1]);
                    break;

                case "SEND_ROOM_LIST":
                    SendRoomList(player);
                    break;

                case "JOIN_ROOM":
                    JoinRoom(player, arrPayload[1]);
                    break;

                case "SEND_LOBBY":
                    SendLobbyInfoToAll(player, arrPayload[1]);
                    break;

                case "READY":
                    var lobby = FindLobbyByPlayer(player);
                    lobby.Players.SingleOrDefault(r => r.PlayerName == player.PlayerName).IsReady = true;
                    string readyInfo = $"READY_INFO;{player.PlayerName}";
                    foreach (var _player in lobby.Players)
                    {
                        SendMessageToPlayer(_player, readyInfo);
                    }
                    break;

                case "MESSAGE":
                    string content = $"SEND_MESSAGE;{arrPayload[1]}";
                    BroadcastMessage(content, player);
                    break;

                case "START":
                    StartGameForLobby(player);
                    break;

                case "STATS":
                    int killCount = int.Parse(arrPayload[1]);
                    int scoreGained = int.Parse(arrPayload[2]);
                    player.KillCount += killCount;
                    player.Score += scoreGained;
                    UpdatePlayerStats(player);
                    break;

                case "GAMEOVER":
                    CheckGameOver(player);
                    break;

                case "RANKING":
                    var playerLobby = FindLobbyByPlayer(player);
                    if (playerLobby != null)
                    {
                        BroadcastRanking(playerLobby);
                    }
                    break;

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

                case "DISCONNECTGameRoom":
                    CloseAllGameRooms();
                    break;

                default:
                    UpdateInfo($"Unknown command received: {arrPayload[0]} from {player.PlayerName}");
                    break;
            }
        }

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

        private void JoinRoom(Player player, string roomId)
        {
            var lobby = lobbies.SingleOrDefault(r => r.RoomId == roomId);
            if (lobby != null && lobby.Players.Count < 4)
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

        private void StartGameForLobby(Player player)
        {
            Lobby lobby = FindLobbyByPlayer(player);
            if (lobby != null)
            {
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

        private void CloseAllGameRooms()
        {
            foreach (var player in connectedPlayers)
            {
                string closeMessage = "CLOSE_ALL";
                SendMessageToPlayer(player, closeMessage);
            }
            UpdateInfo("All game rooms have been closed.");
        }

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
        private void SendMessageToPlayer(Player player, string message)
        {
            var stream = player.PlayerSocket.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }

        private static string GenerateRoomId()
        {
            return Guid.NewGuid().ToString().Substring(0, 6);
        }

        private void SendRoomListToAll()
        {
            foreach (var player in connectedPlayers)
            {
                SendRoomList(player);
            }
        }

        //Hàm này đã sửa
        private void SendRoomList(Player player)
        {
            StringBuilder roomList = new StringBuilder("ROOMLIST;");
            foreach (var lobby in lobbies)
            {
                roomList.Append($"{lobby.RoomId};{lobby.Host.PlayerName};");//sửa khúc này
            }

            SendMessageToPlayer(player, roomList.ToString());
        }

        private Lobby FindLobbyByPlayer(Player player)
        {
            return lobbies.FirstOrDefault(lobby => lobby.Players.Contains(player));
        }

        private void ShowingInfo_TextChanged(object sender, EventArgs e) { }

        //hàm này đã sửa
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

    //public class Room
    //{
    //    public string RoomId { get; set; }
    //    public Player Host { get; set; }
    //    public List<Player> Players { get; set; } = new List<Player>();
    //    //public List<string> PlayersName { get; set; } = new List<string>();
    //}

    public class Lobby
    {
        public string RoomId { get; set; }
        public Player Host { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
    }
}
