using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Collections.Concurrent;

namespace Server_ShootOutGame
{
    public partial class Form2 : Form
    {
        private static List<Player> connectedPlayers = new List<Player>();
        private static List<Lobby> lobbies = new List<Lobby>();
        private static readonly int port = 8989;
        private TcpListener server;

        private const int MAP_WIDTH = 1024;
        private const int MAP_HEIGHT = 768;
        private const int SPAWN_INTERVAL = 30000; // 30 seconds

        private Dictionary<string, System.Threading.Timer> lobbyTimers = new Dictionary<string, System.Threading.Timer>();
        private Dictionary<string, System.Threading.Timer> zombieSpawnTimers = new Dictionary<string, System.Threading.Timer>();

        const int fixedHeight = 620;
        // Thêm hàng đợi để lưu trữ các thông điệp
        private static BlockingCollection<(Player, string)> messageQueue = new BlockingCollection<(Player, string)>(new ConcurrentQueue<(Player, string)>(), 1000); // Giới hạn kích thước

        public Form2()
        {
            InitializeComponent();
            StartServer();
            Thread processingThread = new Thread(ProcessMessages);
            processingThread.IsBackground = true;
            processingThread.Start();
            this.FormClosing += new FormClosingEventHandler(ServerForm_FormClosing);
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
                    Player newPlayer = new Player { PlayerSocket = client };
                    connectedPlayers.Add(newPlayer);
                    ThreadPool.QueueUserWorkItem(HandleClient, newPlayer);
                }
            });
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void HandleClient(object obj)
        {
            var player = (Player)obj;
            var client = player.PlayerSocket;
            var buffer = new byte[1024];
            StringBuilder dataBuffer = new StringBuilder();

            try
            {
                while (client.Connected)
                {
                    var stream = client.GetStream();
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        dataBuffer.Append(receivedData);

                        // Kiểm tra và tách thông điệp dựa trên ký tự phân tách '\n'
                        while (dataBuffer.ToString().Contains("\n"))
                        {
                            int newlineIndex = dataBuffer.ToString().IndexOf("\n");
                            string message = dataBuffer.ToString(0, newlineIndex);
                            dataBuffer.Remove(0, newlineIndex + 1);

                            //ACTIVATE THIS LINE TO TRACK PLAYERS//
                            //UpdateInfo($"Received message from {client.Client.RemoteEndPoint}: {message}");

                            // Thêm thông điệp vào hàng đợi
                            if (!messageQueue.TryAdd((player, message)))
                            {
                                UpdateInfo("Message queue is full. Dropping message.");
                            }
                        }
                    }
                    Thread.Sleep(10); // Giảm tải CPU
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

        // Luồng xử lý thông điệp từ hàng đợi
        private void ProcessMessages()
        {
            foreach (var (player, message) in messageQueue.GetConsumingEnumerable())
            {
                AnalyzingMessage(message, player);
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
                    if (_lobby != null)
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
                case "UPDATE_POSITION":
                    string playerName = arrPayload[1];
                    string direction = arrPayload[2]; 
                    int x = int.Parse(arrPayload[3]);
                    int y = int.Parse(arrPayload[4]);
                    string gunName1 = arrPayload[5];
                    UpdatePlayerPosition(player, direction, x, y, gunName1);
                    break;
                case "UPDATE_GUN":
                    string playerName1 = arrPayload[1];
                    string gunName = arrPayload[2];
                    string gunUpdateMessage = $"UPDATE_GUN;{playerName1};{gunName}";
                    BroadcastMessage(gunUpdateMessage, player);
                    break;
                case "PLAYER_SHOOT":
                    string shooterName = arrPayload[1];
                    string shootDirection = arrPayload[2];
                    string gunName2 = arrPayload[3];
                    string shootMessage = $"PLAYER_SHOOT;{shooterName};{shootDirection};{gunName2}";
                    BroadcastMessage(shootMessage, player);
                    break;
                case "UPDATE_WALL_HEALTH":
                    double health = double.Parse(arrPayload[1]);
                    UpdateWallHealth(health);
                    break;

                case "MAKE_ZOMBIES1":
                    MakeZombies1(arrPayload[1], player);
                    break;

                case "MAKE_ZOMBIES2":
                    MakeZombies2(arrPayload[1], player);
                    break;

                case "MAKE_ZOMBIES3":
                    MakeZombies3(arrPayload[1], player);
                    break;

                case "MAKE_ZOMBIES4":
                    MakeZombies4(arrPayload[1], player);
                    break;

                case "FINAL_WAVE":
                    FinalWave(arrPayload[1], player);
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

        private void CheckGameOver(Player player)
        {
            bool check = true;
            var lobby = FindLobbyByPlayer(player);
            if (lobby != null)
            {
                foreach (var _player in lobby.Players)
                {
                    if (!_player.IsGameOver)
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
            if (lobby != null)
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

        private void SendRoomList(Player player)
        {
            StringBuilder roomList = new StringBuilder("ROOMLIST;");
            foreach (var lobby in lobbies)
            {
                roomList.Append($"{lobby.RoomId};{lobby.Host.PlayerName};");
            }

            SendMessageToPlayer(player, roomList.ToString());
        }

        private Lobby FindLobbyByPlayer(Player player)
        {
            return lobbies.FirstOrDefault(lobby => lobby.Players.Contains(player));
        }

        private void ShowingInfo_TextChanged(object sender, EventArgs e) { }

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
                    SendMessageToPlayer(_player, lobbyInfo);
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
                ShowingInfo.SelectionStart = ShowingInfo.Text.Length;
                ShowingInfo.ScrollToCaret();
            }
        }

        private void BroadcastRanking(Lobby lobby)
        {
            var rankingData = new StringBuilder("RANKING;");

            var sortedPlayers = lobby.Players.OrderByDescending(p => p.Score).ToList();

            foreach (var player in sortedPlayers)
            {
                rankingData.Append($"{player.PlayerName};{player.KillCount};{player.Score};");
            }

            string rankingMessage = rankingData.ToString().TrimEnd(';');

            foreach (var player in lobby.Players)
            {
                SendMessageToPlayer(player, rankingMessage);
            }
        }

        private void UpdatePlayerPosition(Player player, string direction, int x, int y, string gunName1)
        {
            if (player == null || player.PlayerSocket == null) return; 

            if ((DateTime.Now - player.LastPositionUpdate).TotalMilliseconds < 35) // interval
                return;

            player.X = x;
            player.Y = y;
            player.LastPositionUpdate = DateTime.Now;
            player.CurrentGun = gunName1;

            string positionMessage = $"UPDATE_POSITION;{player.PlayerName};{direction};{x};{y};{gunName1}";
            var lobby = FindLobbyByPlayer(player);

            if (lobby != null)
            {
                //ACTIVATE THIS LINE TO TRACK MESSAGE
                //UpdateInfo($"Broadcasting position of {player.PlayerName} at ({x}, {y},) {direction} using {gunName1}");
                foreach (var otherPlayer in lobby.Players)
                {
                    if (otherPlayer != player && otherPlayer.PlayerSocket != null)
                    {
                        SendMessageToPlayer(otherPlayer, positionMessage);
                    }
                }
            }
        }
        private void BroadcastToLobby(Lobby lobby, string message)
        {
            foreach (var player in lobby.Players)
            {
                SendMessageToPlayer(player, message);
            }
        }

        private void UpdatePlayerGun(Player player, string gunName)
        {
            string[] validGuns = { "Pistol", "Shotgun", "Rifle" };
            if (!validGuns.Contains(gunName))
                return;

            player.CurrentGun = gunName;
            string gunMessage = $"UPDATE_GUN;{player.PlayerName};{gunName}";
            BroadcastMessage(gunMessage, player);
        }

        private void PlayerShoot(Player player, string direction)
        {
            var lobby = FindLobbyByPlayer(player);
            if (lobby == null) return;

            string shootMessage = $"PLAYER_SHOOT;{player.PlayerName};{direction};{player.CurrentGun}";
            BroadcastMessage(shootMessage, player);
        }

        private void UpdateWallHealth(double health)
        {
            string updateWallMessage = $"UPDATE_WALL_HEALTH;{health.ToString()}";
            foreach (var player in connectedPlayers)
            {
                SendMessageToPlayer(player, updateWallMessage);
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

    #region MAKE ZOMBIES
        private async Task SendMakeZombiesAsync(Player player, string type, string positionY, string sound)
        {
            try
            {
                var lobby = FindLobbyByPlayer(player);
                string message = $"MAKE_ZOMBIES;{type};{positionY};{sound}";

                if (lobby != null)
                {
                    foreach (var _player in lobby.Players)
                    {
                        SendMessageToPlayer(_player, message);
                    }
                }
                UpdateInfo(message);
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý lỗi
                UpdateInfo($"Error in SendMakeZombiesToPlayersAsync: {ex.Message}");
            }
        }


        private async void MakeZombies1(string height, Player player)
        {
            Random rand = new Random();
            int spawnChance = rand.Next(1, 101);
            string type = null, positionY = null;

            if (spawnChance <= 65)
            {
                Zombie zombie = Zombie.CreateZombie(4);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "4";
                positionY = Y.ToString();
            }
            else if (spawnChance <= 100)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "3";
                positionY = Y.ToString();
            }

            await SendMakeZombiesAsync(player, type, positionY, "NONE");
        }

        private async void MakeZombies2(string height, Player player)
        {
            Random rand = new Random();
            int spawnChance = rand.Next(1, 101);
            string type = null, positionY = null;
            if (spawnChance <= 40)
            {
                Zombie zombie = Zombie.CreateZombie(4);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "4";
                positionY = Y.ToString();
            }
            else if (spawnChance <= 80)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "3";
                positionY = Y.ToString();

            }
            else if (spawnChance <= 100)
            {
                Zombie zombie = Zombie.CreateZombie(2);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "2";
                positionY = Y.ToString();
            }

            await SendMakeZombiesAsync(player, type, positionY, "NONE");
        }

        private async void MakeZombies3(string height, Player player)
        {
            Random rand = new Random();
            int spawnChance = rand.Next(1, 101);
            string type = null, positionY = null;
            if (spawnChance <= 25)
            {
                Zombie zombie = Zombie.CreateZombie(4);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "4";
                positionY = Y.ToString();
            }
            else if (spawnChance <= 65)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "3";
                positionY = Y.ToString();
            }
            else if (spawnChance <= 100)
            {
                Zombie zombie = Zombie.CreateZombie(2);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "2";
                positionY = Y.ToString();
            }

            await SendMakeZombiesAsync(player, type, positionY, "NONE");
        }

        private async void MakeZombies4(string height, Player player)
        {
            Random rand = new Random();
            int spawnChance = rand.Next(1, 101);
            string type = null, positionY = null;
            if (spawnChance <= 45)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "3";
                positionY = Y.ToString();
            }
            else if (spawnChance <= 85)
            {
                Zombie zombie = Zombie.CreateZombie(2);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "2";
                positionY = Y.ToString();
            }
            else if (spawnChance <= 100)
            {
                Zombie zombie = Zombie.CreateZombie(1);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);
                type = "1";
                positionY = Y.ToString();
            }

            await SendMakeZombiesAsync(player, type, positionY, "NONE");
        }

        private void SendFinalWave(Player player, string type, string positionY, string sound)
        {
            try
            {
                var lobby = FindLobbyByPlayer(player);
                string message = $"MAKE_ZOMBIES;{type};{positionY};{sound}";

                if (lobby != null)
                {
                    foreach (var _player in lobby.Players)
                    {
                        SendMessageToPlayer(_player, message);
                        UpdateInfo($"Message sent to player {_player.PlayerName}: {message}");
                    }
                }
                else
                {
                    UpdateInfo("Lobby not found for player.");
                }
                UpdateInfo(message);
            }
            catch (Exception ex)
            {
                UpdateInfo($"Error in SendMakeZombiesToPlayersAsync: {ex.Message}");
            }
        }

        private void FinalWave(string height, Player player) //when timer reaches 0s
        {
            Random rand = new Random();
            string type = null, positionY = null;
            var lobby = FindLobbyByPlayer(player);
            for (int i = 0; i < 3; i++)
            {
                Zombie zombie = Zombie.CreateZombie(4);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);

                type = "4";
                positionY = Y.ToString();

                SendFinalWave(player, type, positionY, "NONE");

                //await Task.Delay(500);
            }

            //await Task.Delay(1000);

            for (int i = 0; i < 2; i++)
            {
                Zombie zombie = Zombie.CreateZombie(3);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);

                type = "3";
                positionY = Y.ToString();

                SendFinalWave(player, type, positionY, "NONE");
                //await Task.Delay(500);
            }

            //await Task.Delay(2000);

            for (int i = 0; i < 3; i++)
            {
                Zombie zombie = Zombie.CreateZombie(2);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);

                type = "2";
                positionY = Y.ToString();

                SendFinalWave(player, type, positionY, "NONE");
                //await Task.Delay(1000);
            }

            //await Task.Delay(2000);

            for (int i = 0; i < 3; i++)
            {
                Zombie zombie = Zombie.CreateZombie(1);
                int minSpawnHeight = 70;
                int maxSpawnHeight = fixedHeight - zombie.ZombiePictureBox.Height - 100;
                int Y = rand.Next(minSpawnHeight, maxSpawnHeight);

                type = "1";
                positionY = Y.ToString();

                SendFinalWave(player, type, positionY, "bzfinalwave");
                //await Task.Delay(1500);
            }
        }
    }
    #endregion

    #region HELPER CLASSES
    public class Player
    {
        public TcpClient PlayerSocket { get; set; }
        public string PlayerName { get; set; }
        public bool IsHost { get; set; }
        public int KillCount { get; set; } = 0;
        public int Score { get; set; } = 0;
        public bool IsGameOver { get; set; } = false;
        public DateTime LastPositionUpdate { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string CurrentGun { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsReady { get; set; } = false;
    }

    public class Lobby
    {
        public string RoomId { get; set; }
        public Player Host { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public bool IsStart { get; set; } = false;
    }

    class Zombie
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public int Damage { get; set; }
        public int Score { get; set; }
        public string Direction { get; set; }
        public PictureBox ZombiePictureBox { get; set; }


        public Image ImageLeft { get; set; }


        public Zombie(int health, int speed, int damage, int score, string direction, PictureBox zPictureBox, Image imgLeft)
        {
            Health = health;
            Speed = speed;
            Damage = damage;
            Score = score;
            Direction = direction;
            ZombiePictureBox = zPictureBox;
            ImageLeft = imgLeft;

        }
        public static Zombie CreateZombie(int type)
        {
            PictureBox zombiePic = new PictureBox();
            zombiePic.Tag = "zombie";
            zombiePic.SizeMode = PictureBoxSizeMode.AutoSize;

            switch (type)
            {
                case 1:
                    return new Zombie(240, 2, 30, 75, "left", zombiePic,
                        Properties.Resources.bzleft); // variant 1 images
                case 2:
                    return new Zombie(40, 6, 14, 27, "left", zombiePic,
                        Properties.Resources.szleft); // variant 2 images
                case 3:
                    return new Zombie(100, 3, 16, 32, "left", zombiePic,
                        Properties.Resources.tzleft); // variant 3 images
                case 4:
                    return new Zombie(40, 3, 12, 20, "left", zombiePic,
                        Properties.Resources.zleft); // variant 4 images (default)
                default:
                    return null;
            }
        }
    }
    #endregion
}