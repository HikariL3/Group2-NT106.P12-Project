using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client;
namespace GameForm
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        NewRoom newRoom;

        private void loginButton_Click(object sender, EventArgs e)
        {
            bool checkLogin = !string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(ipAddress.Text);

            if (checkLogin)
            {
                try
                {
                    IPAddress ipServer = IPAddress.Parse(ipAddress.Text);
                    IPEndPoint serverEP = new IPEndPoint(ipServer, 8989); // Sử dụng port 8989 như server đã chỉ định
                    GameClient.ConnectToServer(serverEP); // Gọi phương thức static mà không cần tạo đối tượng

                    string message = $"CONNECT;{username.Text}"; // Sử dụng ký tự phân tách là ';'
                    GameClient.SendData(message); // Gọi phương thức static mà không cần tạo đối tượng

                    this.Hide();
                    newRoom = new NewRoom();
                    newRoom.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không kết nối được với server! Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameClient.Disconnect();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
