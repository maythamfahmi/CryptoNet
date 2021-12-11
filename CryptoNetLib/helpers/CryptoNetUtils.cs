using System.IO;
using System.Text;

namespace CryptoNetLib.helpers
{
    public static class CryptoNetUtils
    {
        public static byte[] LoadFile(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public static string LoadCertificate(string filename)
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

        public static void SaveKey(string filename, byte[] bytes)
        {
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        public static void SaveKey(string filename, string content)
        {
            var bytes = CryptoNetUtils.StringToBytes(content);
            SaveKey(filename, bytes);
        }
    }
}
