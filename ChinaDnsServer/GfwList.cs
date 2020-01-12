using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChinaDnsServer
{
    public class GfwList
    {
        Regex[] list = new Regex[0];
        public static GfwList Instance { get; private set; }
        static GfwList()
        {
            Instance = new GfwList();
            Instance.Load();
        }

        public void Load()
        {
            try
            {
                var file = Path.Combine(Util.GetWorkingDirectory(), "GfwList.txt");
                var list = new List<Regex>();
                if (File.Exists(file))
                {
                    var lines = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(file)));
                    foreach (var line in lines.Split('\n'))
                    {
                        if (line == "!##############General List End#################")
                        {
                            break;
                        }

                        if (string.IsNullOrWhiteSpace(line) ||
                            line.StartsWith('!') || line.StartsWith('['))
                        {
                            continue;
                        }

                        string domain;
                        if (line.StartsWith("||"))
                        {
                            domain = line.Substring(2);
                        }
                        else if (line.StartsWith("|"))
                        {
                            domain = line.Substring(1);
                        }
                        else if (line.StartsWith("/"))
                        {
                            continue;
                        }
                        else if (line.StartsWith("@@||"))
                        {
                            domain = line.Substring(4);
                        }
                        else
                        {
                            domain = line;
                        }

                        if (domain.StartsWith("//"))
                        {
                            domain = domain.Substring(2);
                        }
                        else if (domain.StartsWith("http://"))
                        {
                            domain = domain.Substring(7);
                        }
                        else if (domain.StartsWith("https://"))
                        {
                            domain = domain.Substring(8);
                        }

                        domain = domain.TrimStart('.');
                        domain = domain.TrimEnd('/');

                        if (domain.Contains('/'))
                        {
                            domain = domain.Substring(0, domain.IndexOf('/'));
                        }

                        var reg = WildCardToRegular(domain);
                        list.Add(new Regex("^(.*\\.)?" + reg + "$")); // add sub domain
                    }
                }

                this.list = list.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool IsInList(string domain)
        {
            foreach (var reg in list)
            {
                if (reg.IsMatch(domain))
                {
                    return true;
                }
            }

            return false;
        }

        string WildCardToRegular(string value)
        {
            return Regex.Escape(value).Replace("\\*", "[-A-Za-z0-9+&@#/%?=~_|!:,.;]*");
        }
    }
}
