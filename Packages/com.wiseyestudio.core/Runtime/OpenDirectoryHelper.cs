using System.Diagnostics;

namespace WiseyeStudio
{
    public static class OpenDirectoryHelper
    {
        public static void OpenDirectory(string path)
        {
            // Mac doesn't like back slashes at the end
            var trimmedPath = path.TrimEnd(new[]{'\\', '/'});
            Process.Start(trimmedPath);
        }
    }
}