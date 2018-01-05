using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Avalon.Models
{
    public static class Utilities
    {
        public static Random random = new Random();

        public static List<T> Shuffle<T> (List<T> list)
        {
            List<T> shuffledList = new List<T>(list);
            int n = list.Count;
            random = new Random();
            while (n > 1)
            {
                int k = random.Next(n);
                n--;
                T value = shuffledList[k];
                shuffledList[k] = shuffledList[n];
                shuffledList[n] = value;
                System.Threading.Thread.Sleep(5);
            }
            return shuffledList;
        }

        public static string LogList<T>(List<T> list)
        {
            string log = "List:";
            foreach (T elem in list)
            {
                log += " " + elem;
            }
            return log;
        }


        public static T NextOrFirst<T> (T current, T[] array)
        {
            if (array.Last().Equals(current))
            {
                return array.First();
            }
            else
            {
                int index = Array.IndexOf(array, current);
                return array[index + 1];
            }
        }

        public static T PickRandom<T> (HashSet<T> set)
        {
            T[] array = set.ToArray();
            int index = random.Next(array.Length);
            return array[index];
        }

        const string defaultDir = "C:\\temp\\log\\Avalon";
        const string defaultPath = defaultDir + "\\Avalon.log";

        public static void LogToFile(String msg, String path = null, bool append = true)
        {
            if (path == null)
            {
                if (!Directory.Exists(defaultDir))
                {
                    Directory.CreateDirectory(defaultDir);
                }
                path = defaultPath;
            }

            String log = DateTime.Now.ToString() + ":\t" + msg;

            using (StreamWriter file =
                new StreamWriter(path, append))
            {
                file.WriteLine(log);
            }
        }

    }
}
