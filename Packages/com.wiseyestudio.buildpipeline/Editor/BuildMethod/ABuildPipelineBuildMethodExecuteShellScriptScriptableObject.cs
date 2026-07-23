using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    public abstract class ABuildPipelineBuildMethodExecuteShellScriptScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Title("Shell Script")]
        [Required]
        [FormerlySerializedAs("ShellScriptPath")]
        [SerializeField] protected string shellScriptPath;

        protected bool ChmodOnFile(string fileAbsolutePath, string permissions, bool recursive = false)
        {
            fileAbsolutePath = Path.GetFullPath(fileAbsolutePath);
            fileAbsolutePath = fileAbsolutePath.Replace(" ", "\\ ");

            var cmd = recursive ? $"chmod -R {permissions} {fileAbsolutePath}"
                : $"chmod {permissions} {fileAbsolutePath}";

            try
            {
                using var proc = Process.Start("/bin/bash", $"-c \"{cmd}\"");
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        protected static void ExecuteShellScript(string absoluteShellScriptPath, Action<string> onStandardOutput, params string[] arguments)
        {
            // surround arguments with quotes if they contain spaces
            for (var i = 0; i < arguments.Length; i++)
            {
                if (arguments[i].Contains(" "))
                {
                    arguments[i] = "\"" + arguments[i] + "\"";
                }
            }

            var shellArguments = string.Join(" ", arguments);

            ProcessStartInfo startInfo = new()
            {
                FileName = absoluteShellScriptPath,
                Arguments = shellArguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var process = Process.Start(startInfo);
            while (process.StandardOutput.EndOfStream == false)
            {
                var newLine = process.StandardOutput.ReadLine();
                onStandardOutput?.Invoke(newLine);
            }
            process.WaitForExit();
        }
    }
}