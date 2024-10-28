using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client;
using Server;

namespace GameForm
{
    public partial class NewRoom : Form
    {
        public NewRoom()
        {
            InitializeComponent();
            listPhong.Items.AddRange(new string[] { "Option 2", "Option 3", "Option 4" });

            ManageRoom(); //Quản lý xem coi có phòng nào có sẵn hoặc mới tạo từ người dùng khác
        }

        GameClient client;
        Lobby lobby;
        

        bool checkMaPhong(string idRoom)
        {
            if (!string.IsNullOrEmpty(maPhong.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Yêu cầu nhập mã phòng!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        void TurnForm()
        {
            lobby = new Lobby();
            this.Hide();
            lobby.Show();
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (!checkMaPhong(maPhong.Text)) return;

            GameClient.SendData($"{maPhong.Text}");

            //Thêm mã phòng vào danh sách phòng --> Đợi server hoàn thành.

            TurnForm();
        }

        private void listPhong_SelectedIndexChanged(object sender, EventArgs e)
        {
            maPhong.Text = listPhong.SelectedItem.ToString();
        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            if (!checkMaPhong(maPhong.Text)) return;

            GameClient.SendData($"{maPhong.Text}");

            TurnForm();
        }

        void ManageRoom() { }
    }
}
