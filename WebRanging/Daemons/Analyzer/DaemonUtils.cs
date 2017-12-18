using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebRanging.Daemons.Analyzer
{
    public static class DaemonUtils
    {
        public static bool IsUrl(string url)
        {
            try
            {
                using (var client = new WebClient())
                using (var _ = client.OpenRead(url))
                {
                }
            }
            catch (WebException)
            {
                return false;
            }

            return true;
        }

        public static string ReadTextFromUrl(Uri uri)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead(uri))
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    using (var textReader = new StreamReader(stream, Encoding.UTF8, true))
                    {
                        return textReader.ReadToEnd();
                    }
                }
            }
            catch (WebException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static string GetRootDomain(String host)
        {
            var domains = host.Split('.');

            if (domains.Length < 3)
            {
                return host;
            }

            var c = domains.Length;
            if (domains[c - 1].Length < 3 && domains[c - 2].Length <= 3)
            {
                return string.Join(".", domains, c - 3, 3);
            }

            return string.Join(".", domains, c - 2, 2);
        }

        public static bool IsWebPage(string url)
        {
            return !url.EndsWith("zip") || !url.EndsWith("jpg") || !url.EndsWith("png");
        }
    }
}