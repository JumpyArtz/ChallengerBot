using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace ChallengerBot.Utils
{
    public static class Generator
    {
        public static Random r { get; private set; }
        static Generator()
        {
            r = new Random();
        }

        public static int CreateRandom(int min, int max)
        {
            return r.Next(min, max);
        }

        public static void CreateRandomThread(int min, int max)
        {
            Thread.Sleep(r.Next(min, max));
        }
    }
}
