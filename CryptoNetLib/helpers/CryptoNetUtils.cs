using System.IO;
using System.Text;

namespace CryptoNetLib.helpers
{
    public static class CryptoNetUtils
    {
        public static byte[] LoadFileToBytes(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public static string LoadFileToString(string filename)
        {
            return BytesToString(LoadFileToBytes(filename));
        }

        public static string BytesToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static byte[] StringToBytes(string content)
        {
            return Encoding.ASCII.GetBytes(content);
        }

        public static void SaveKey(string filename, byte[] bytes)
        {
            using var fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
        }

        public static void SaveKey(string filename, string content)
        {
            var bytes = StringToBytes(content);
            SaveKey(filename, bytes);
        }
    }
}
