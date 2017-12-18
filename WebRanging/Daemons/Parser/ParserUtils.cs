using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebRanging.Daemons.Parser
{
    public static class ParserUtils
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
        }
    }
}