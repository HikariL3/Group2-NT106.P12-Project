using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZoombieShootOut
{
    internal class SocketManager
    {
        #region Client
        private Socket client;
        public bool ConnectToServer(string serverIP, int port)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(iPEndPoint);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to server: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Server
        private Socket server;
        private List<Socket> clientList = new List<Socket>(); // Quản lý danh sách các client kết nối tới server
        public bool isServerRunning = false;

        public void CreateServer(string ip, int port)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(iPEndPoint);
            server.Listen(10);

            Thread acceptClientThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Socket clientSocket = server.Accept();
                        clientList.Add(clientSocket);

                        // Mở một luồng riêng để xử lý dữ liệu cho từng client
                        Thread receiveClientDataThread = new Thread(() => ReceiveDataFromClient(clientSocket));
                        receiveClientDataThread.IsBackground = true;
                        receiveClientDataThread.Start();
                    }
                    catch
                    {
                        MessageBox.Show("Error accepting client.");
                    }
                }
            });

            acceptClientThread.IsBackground = true;
            acceptClientThread.Start();
            isServerRunning = true;
        }

        // Nhận dữ liệu từ client và xử lý
        private void ReceiveDataFromClient(Socket clientSocket)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            while (clientSocket.Connected)
            {
                try
                {
                    int receivedBytes = clientSocket.Receive(buffer);
                    if (receivedBytes > 0)
                    {
                        object receivedData = DeserializeData(buffer);
                        // Xử lý dữ liệu từ client, ví dụ: cập nhật vị trí, va chạm, bắn...
                        HandleReceivedData(receivedData);
                    }
                }
                catch
                {
                    MessageBox.Show("Error receiving data from client.");
                    break;
                }
            }
        }

        private void HandleReceivedData(object data)
        {
            // Tùy theo loại dữ liệu nhận được mà xử lý
            // Ví dụ: cập nhật vị trí người chơi, xử lý va chạm hoặc kết thúc game
            if (data is SocketData socketData)
            {
                switch (socketData.Action)
                {
                    case (int)SocketCommand.Move:
                        // Cập nhật vị trí của người chơi
                        break;
                    case (int)SocketCommand.Shoot:
                        // Xử lý hành động bắn súng
                        break;
                    case (int)SocketCommand.Collision:
                        // Xử lý va chạm
                        break;
                    case (int)SocketCommand.GameOver:
                        // Xử lý kết thúc game
                        break;
                }
            }
        }
        #endregion

        #region Data
        public const int BUFFER_SIZE = 2048;

        // Gửi dữ liệu từ client tới server hoặc ngược lại
        public bool SendData(Socket targetSocket, object data)
        {
            byte[] serializedData = SerializeData(data);
            try
            {
                return targetSocket.Send(serializedData) > 0;
            }
            catch
            {
                MessageBox.Show("Error sending data.");
                return false;
            }
        }

        public object ReceiveData(Socket sourceSocket)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            try
            {
                int receivedBytes = sourceSocket.Receive(buffer);
                if (receivedBytes > 0)
                {
                    return DeserializeData(buffer);
                }
            }
            catch
            {
                MessageBox.Show("Error receiving data.");
            }
            return null;
        }

        public byte[] SerializeData(object o)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, o);
                return ms.ToArray();
            }
        }

        public object DeserializeData(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }

        public string GetIPv4Address(NetworkInterfaceType networkType)
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == networkType && networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ipInfo in networkInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ipInfo.Address.ToString();
                        }
                    }
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
