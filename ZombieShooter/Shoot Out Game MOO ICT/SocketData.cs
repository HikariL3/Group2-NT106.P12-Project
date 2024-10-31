using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoombieShootOut
{
    internal class SocketData
    {
        private int action;  // Hành động của người chơi hoặc server (bắn, di chuyển, cập nhật trạng thái)

        public int Action
        {
            get { return action; }
            set { action = value; }
        }

        private string message;  // Thông tin gửi kèm như vị trí người chơi, trạng thái game, v.v.

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private string playerId;  // ID người chơi liên quan đến hành động này

        public string PlayerId
        {
            get { return playerId; }
            set { playerId = value; }
        }

        private float posX;  // Tọa độ X của người chơi hoặc zombie
        private float posY;  // Tọa độ Y của người chơi hoặc zombie

        public float PosX
        {
            get { return posX; }
            set { posX = value; }
        }

        public float PosY
        {
            get { return posY; }
            set { posY = value; }
        }

        // Constructor để khởi tạo dữ liệu SocketData
        public SocketData(int action, string message, string playerId, float posX, float posY)
        {
            this.Action = action;
            this.Message = message;
            this.PlayerId = playerId;
            this.PosX = posX;
            this.PosY = posY;
        }
    }

    // Các loại lệnh từ client hoặc server
    public enum SocketCommand
    {
        Move,      // Di chuyển người chơi
        Shoot,     // Người chơi bắn
        Collision, // Va chạm với người chơi khác hoặc zombie
        PlayerUpdate, // Cập nhật vị trí và trạng thái người chơi
        ZombieUpdate, // Cập nhật vị trí và trạng thái của zombie
        GameOver   // Trạng thái kết thúc game
    }
}
