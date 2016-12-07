using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace ChallengerBot
{
    class Tools
    {
        public static string ChallengerBotVersion = Application.ProductVersion;
        public static void Log(string text)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\errors.txt";
            try
            {
                if (!File.Exists(path))
                {
                    using (StreamWriter writer = File.CreateText(path))
                    {
                        writer.Write("[" + DateTime.Now + "] " + text + Environment.NewLine);
                        return;
                    }
                }

                if (File.Exists(path))
                {
                    File.AppendAllText(path, "[" + DateTime.Now + "] " + text + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return;
            }
        }

        public static void TitleMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[" + DateTime.Now + "] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message + "\n");
        }

        public static void ConsoleMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[" + DateTime.Now + "] ");
            Console.ForegroundColor = color;
            Console.Write(message + "\n");
        }
    }
}
