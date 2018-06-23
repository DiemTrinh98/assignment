using System;
using YangYesterday.entity;
using YangYesterday.view;

namespace YangYesterday
{
    class Program
    {
        public static YYAccount currentLoggedInYyAccount;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;

            Console.BackgroundColor = System.ConsoleColor.Black;
            Console.ForegroundColor = System.ConsoleColor.Red;
            ApplicationView view = new ApplicationView();
            while (true)
            {
                if (Program.currentLoggedInYyAccount != null)
                {
                    view.GenerateCustomerMenu();
                }
                else
                {
                    view.GenerateDefaultMenu();
                }
            }
        }
    }
}