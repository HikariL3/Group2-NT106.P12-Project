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
using Server;
namespace GameForm
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        NewRoom newRoom;
        GameClient client = new GameClient();
        

        private void loginButton_Click(object sender, EventArgs e)
        {
            bool checkLogin = !string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(ipAddress.Text);

            if (checkLogin)
            {
                try
                {
                    IPAddress ipServer = IPAddress.Parse(ipAddress.Text);
                }
                catch
                {
                    MessageBox.Show("Định dạng địa chỉ IP không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(ipAddress.Text), 9999);
                    GameClient.ConnectToServer(serverEP);

                    string message = string.Format($"CONNECT: {username.Text}");
                    GameClient.SendData(message);

                    this.Hide();
                    newRoom = new NewRoom();
                    newRoom.Show();
                }
                catch
                {
                    MessageBox.Show("Không kết nối được với server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            /*Đóng kết nối -- Tạm thời để đây thôi*/

            //string message = string.Format($"DISCONNECT: {username.Text}");
            //GameClient.SendData(message);
            //this.Show();
        }
    }
}
