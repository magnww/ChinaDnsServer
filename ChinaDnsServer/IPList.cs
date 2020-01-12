using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ChinaDnsServer
{
    public class IPList
    {
        long[][] list = new long[0][];

        public static IPList Instance { get; private set; }
        static IPList()
        {
            Instance = new IPList();
            Instance.Load();
        }

        public void Load()
        {
            try
            {
                var file = Path.Combine(Util.GetWorkingDirectory(), "IPList.txt");
                var list = new List<long[]>();
                if (File.Exists(file))
                {
                    foreach (var line in File.ReadAllLines(file))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var ip = line.Substring(0, line.IndexOf('/'));
                            var sub = long.Parse(line.Substring(line.IndexOf('/') + 1));

                            var start = ToInt(ip);
                            var end = start + (long)Math.Pow(2, (32 - sub));
                            list.Add(new long[] { start, end });
                        }
                    }
                }

                this.list = list.ToArray();
            }
            catch (Exception) { }
        }

        public bool IsInList(string IPAddress)
        {
            var ip = ToInt(IPAddress);
            foreach (var startEnd in list)
            {
                if (startEnd[0] <= ip && startEnd[1] > ip)
                {
                    return true;
                }
            }
            return false;
        }

        long ToInt(string addr)
        {
            // careful of sign extension: convert to uint first;
            // unsigned NetworkToHostOrder ought to be provided.
            return (long)(uint)IPAddress.NetworkToHostOrder(
                 (int)IPAddress.Parse(addr).Address);
        }
    }
}
