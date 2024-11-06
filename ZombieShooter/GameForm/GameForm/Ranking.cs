using Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameForm
{
    public partial class Ranking : Form
    {
        public Ranking()
        {
            InitializeComponent();
            LoadRankingData();
        }

        private void LoadRankingData()
        {
            // Lấy danh sách người chơi từ server
            var players = GameClient.joinedLobby.Players;

            // Sắp xếp người chơi theo điểm số giảm dần
            players.Sort((x, y) => y.Score.CompareTo(x.Score));

            // Hiển thị thông tin người chơi lên bảng xếp hạng
            for (int i = 0; i < players.Count && i < 4; i++)
            {
                var player = players[i];
                switch (i)
                {
                    case 0:
                        statsPlayer1.Visible = true;
                        namePlayer1.Text = player.Name;
                        killPlayer1.Text = player.Kill.ToString();
                        scorePlayer1.Text = player.Score.ToString();
                        break;
                    case 1:
                        statsPlayer2.Visible = true;
                        namePlayer2.Text = player.Name;
                        killPlayer2.Text = player.Kill.ToString();
                        scorePlayer2.Text = player.Score.ToString();
                        break;
                    case 2:
                        statsPlayer3.Visible = true;
                        namePlayer3.Text = player.Name;
                        killPlayer3.Text = player.Kill.ToString();
                        scorePlayer3.Text = player.Score.ToString();
                        break;
                    case 3:
                        statsPlayer4.Visible = true;
                        namePlayer4.Text = player.Name;
                        killPlayer4.Text = player.Kill.ToString();
                        scorePlayer4.Text = player.Score.ToString();
                        break;
                }
            }
        }

        private void thoatButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            GameClient.SendData("CLEAR_LOBBY");
            NewRoom newRoom = new NewRoom();
            this.Hide();
            newRoom.Show();
        }

        private async void Ranking_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameClient.SendData("CLEAR_LOBBY");

            await WaitFunction();

            GameClient.Disconnect();

            Login login = new Login();
            login.Show();
        }
        private async Task WaitFunction()
        {
            await Task.Delay(300);
        }
    }
}
