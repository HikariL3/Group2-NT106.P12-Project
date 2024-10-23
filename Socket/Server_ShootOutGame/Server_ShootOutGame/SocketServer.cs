using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Server
{
    public class Program
    {
        private static readonly ILogger _logger;
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static readonly GameState _gameState;

        static Program()
        {
            _logger = new FileLogger("log.txt");
            _gameState = new GameState();
        }

        static async Task Main(string[] args)
        {
            try
            {
                TcpListener server = new TcpListener(IPAddress.Any, 9999);
                server.Start();
                _logger.Log("Server is running...");

                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var client = await server.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Server error: {ex.Message}");
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                var player = new Player(client);
                var lobby = _gameState.CreateLobby(player);
                await _gameState.AddPlayer(player);
                await BroadcastMessageAsync($"Player {player.Name} has joined lobby {lobby.Id}");

                var buffer = new byte[1024];
                var stream = client.GetStream();

                while (client.Connected && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (stream.DataAvailable)
                    {
                        var message = await ReadMessageAsync(stream, buffer);
                        await ProcessMessageAsync(message, player, lobby);
                    }
                    await Task.Delay(10); // Prevent CPU spinning
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Client error: {ex.Message}");
            }
        }

        private static async Task<string> ReadMessageAsync(NetworkStream stream, byte[] buffer)
        {
            var message = new StringBuilder();
            while (stream.DataAvailable)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                message.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
            }
            return message.ToString();
        }

        private static async Task ProcessMessageAsync(string message, Player player, Lobby lobby)
        {
            try
            {
                var parts = message.Split('|');
                switch (parts[0].ToUpper())
                {
                    case "MOVE":
                        await HandleMovementAsync(parts[1], player, lobby);
                        break;
                    case "SPAWN_ZOMBIE":
                        await SpawnZombieAsync(lobby);
                        break;
                    case "DAMAGE_WALL":
                        await UpdateWallHealthAsync(parts[1], lobby);
                        break;
                    case "SCORE":
                        await UpdateScoreAsync(player, parts[1]);
                        break;
                    default:
                        _logger.LogWarning($"Unknown message type: {parts[0]}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message processing error: {ex.Message}");
            }
        }

        private static bool IsValidMovement(string direction)
        {
            string[] validDirections = { "UP", "DOWN", "LEFT", "RIGHT" };
            return Array.IndexOf(validDirections, direction.ToUpper()) != -1;
        }

        private static bool ValidateDamageInfo(string damageInfo)
        {
            try
            {
                string[] parts = damageInfo.Split(',');
                if (parts.Length != 2) return false;

                int damage = int.Parse(parts[0]);
                int wallId = int.Parse(parts[1]);

                return damage > 0 && wallId >= 0;
            }
            catch
            {
                return false;
            }
        }

        private static async Task HandleMovementAsync(string direction, Player player, Lobby lobby)
        {
            if (!IsValidMovement(direction))
            {
                _logger.LogWarning($"Invalid movement direction: {direction}");
                return;
            }

            await BroadcastMessageAsync($"MOVE|{player.Name}|{direction}", lobby);
        }

        private static async Task SpawnZombieAsync(Lobby lobby)
        {
            var zombie = new Zombie();
            await _gameState.AddZombie(lobby.Id, zombie);
            await BroadcastMessageAsync($"ZOMBIE_SPAWNED|{zombie.Position}|{lobby.Id}");
        }

        private static async Task UpdateWallHealthAsync(string damageInfo, Lobby lobby)
        {
            if (!ValidateDamageInfo(damageInfo))
            {
                _logger.LogWarning($"Invalid damage info: {damageInfo}");
                return;
            }

            await BroadcastMessageAsync($"WALL_DAMAGE|{damageInfo}|{lobby.Id}", lobby);
        }

        private static async Task UpdateScoreAsync(Player player, string scoreChange)
        {
            if (int.TryParse(scoreChange, out int change))
            {
                player.UpdateScore(change);
                await BroadcastMessageAsync($"SCORE_UPDATE|{player.Name}|{player.Score}");
            }
            else
            {
                _logger.LogWarning($"Invalid score change value: {scoreChange}");
            }
        }

        private static async Task BroadcastMessageAsync(string message, Lobby lobby = null)
        {
            var data = Encoding.UTF8.GetBytes(message);
            var tasks = new List<Task>();

            if (lobby == null)
            {
                foreach (var player in _gameState.GetAllPlayers())
                {
                    tasks.Add(SendToPlayerAsync(player, data));
                }
            }
            else
            {
                foreach (var player in lobby.Players)
                {
                    tasks.Add(SendToPlayerAsync(player, data));
                }
            }

            await Task.WhenAll(tasks);
        }

        private static async Task SendToPlayerAsync(Player player, byte[] data)
        {
            try
            {
                if (player.IsConnected)
                {
                    await player.Client.GetStream().WriteAsync(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send message to player {player.Name}: {ex.Message}");
            }
        }
    }

    public class GameState
    {
        private readonly Dictionary<int, Lobby> _lobbies;
        private readonly Dictionary<int, List<Zombie>> _zombiesInLobbies;
        private readonly List<Player> _players;
        private int _lobbyCounter;

        public GameState()
        {
            _lobbies = new Dictionary<int, Lobby>();
            _zombiesInLobbies = new Dictionary<int, List<Zombie>>();
            _players = new List<Player>();
            _lobbyCounter = 0;
        }

        public Lobby CreateLobby(Player host)
        {
            var lobby = new Lobby(_lobbyCounter++, host);
            _lobbies[lobby.Id] = lobby;
            return lobby;
        }

        public Task AddPlayer(Player player)
        {
            _players.Add(player);
            return Task.CompletedTask;
        }

        public Task AddZombie(int lobbyId, Zombie zombie)
        {
            if (!_zombiesInLobbies.ContainsKey(lobbyId))
            {
                _zombiesInLobbies[lobbyId] = new List<Zombie>();
            }
            _zombiesInLobbies[lobbyId].Add(zombie);
            return Task.CompletedTask;
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _players;
        }
    }

    public class Player
    {
        public TcpClient Client { get; }
        public string Name { get; }
        public int Score { get; private set; }
        public bool IsConnected => Client?.Connected ?? false;

        public Player(TcpClient client)
        {
            Client = client;
            Name = "Player" + new Random().Next(1000).ToString();
            Score = 0;
        }

        public void UpdateScore(int change)
        {
            Score += change;
        }
    }

    public class Lobby
    {
        public int Id { get; }
        public List<Player> Players { get; }

        public Lobby(int id, Player host)
        {
            Id = id;
            Players = new List<Player>();
            Players.Add(host);
        }
    }

    public class Zombie
    {
        public string Position { get; }

        public Zombie()
        {
            Position = GenerateRandomPosition();
        }

        private string GenerateRandomPosition()
        {
            Random random = new Random();
            return $"{random.Next(100)},{random.Next(100)}";
        }
    }

    public interface ILogger
    {
        void Log(string message);
        void LogError(string message);
        void LogWarning(string message);
    }

    public class FileLogger : ILogger
    {
        private readonly string _logPath;
        private readonly object _lock;

        public FileLogger(string logPath)
        {
            _logPath = logPath;
            _lock = new object();
        }

        public void Log(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogError(string message)
        {
            WriteLog("ERROR", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        private void WriteLog(string level, string message)
        {
            lock (_lock)
            {
                File.AppendAllText(_logPath,
                    string.Format("[{0}] [{1}] {2}\n",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    level,
                    message));
            }
        }
    }
}