using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.IO
{
    public static class FileIOUtility
    {
        public static void SafeWriteAllText(string path, string text)
        {
            // create all the directories if they don't exist in path
            string directoryPath = Path.GetDirectoryName(path);
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(path, text);
        }

        public static void SafeWriteAllBytes(string path, byte[] bytes)
        {
            // create all the directories if they don't exist in path
            string directoryPath = Path.GetDirectoryName(path);
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllBytes(path, bytes);
        }
    }
}
