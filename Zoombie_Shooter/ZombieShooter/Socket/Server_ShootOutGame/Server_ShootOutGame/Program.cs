using System;
using System.Windows.Forms;

namespace Server_ShootOutGame
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2()); // Khởi chạy Form2
        }
    }
}
