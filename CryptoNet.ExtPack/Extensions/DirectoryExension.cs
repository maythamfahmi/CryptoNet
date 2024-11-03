namespace CryptoNet.ExtPack.Extensions
{
    public static class DirectoryExension
    {
        public static DirectoryInfo? TryGetSolutionDirectoryInfo()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && directory.GetFiles("*.sln").Length == 0)
            {
                directory = directory.Parent;
            }
            return directory;
        }
    }
}
