using System.IO;
using System.Text;

namespace CryptoNetLib.helpers
{
    public static class CryptoNetUtils
    {
        public static byte[] Load(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public static string LoadKey(string filename)
        {
            return BytesToString(File.ReadAllBytes(filename));
        }

        public static string BytesToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static byte[] StringToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }
    }
}
