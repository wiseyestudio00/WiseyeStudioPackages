using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

using Sirenix.OdinInspector;

namespace WiseyeStudio.Build.BuildMethod
{
    public abstract class ABuildPipelineBuildMethodExecuteShellScriptScriptableObject : ABuildPipelineBuildMethodScriptableObject
    {
        [Title("Shell Script")]
        [Required]
        [SerializeField] protected string ShellScriptPath;

        protected bool ChmodOnFile(string fileAbsolutePath, string permissions, bool recursive = false)
        {
            fileAbsolutePath = Path.GetFullPath(fileAbsolutePath);
            fileAbsolutePath = fileAbsolutePath.Replace(" ", "\\ ");

            string cmd = recursive ? $"chmod -R {permissions} {fileAbsolutePath}"
                : $"chmod {permissions} {fileAbsolutePath}";

            try
            {
                using Process proc = Process.Start("/bin/bash", $"-c \"{cmd}\"");
                proc.WaitForExit();
                return proc.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public void ExecuteShellScript(string abosulteShellScriptPath, Action<string> onStandardOutput, params string[] arguments)
        {
            // surround arguments with quotes if they contain spaces
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i].Contains(" "))
                {
                    arguments[i] = "\"" + arguments[i] + "\"";
                }
            }

            string ShellArguments = string.Join(" ", arguments);

            ProcessStartInfo startInfo = new()
            {
                FileName = abosulteShellScriptPath,
                Arguments = ShellArguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            Process process = Process.Start(startInfo);
            while (!process.StandardOutput.EndOfStream)
            {
                string newLine = process.StandardOutput.ReadLine();
                onStandardOutput?.Invoke(newLine);
            }
            process.WaitForExit();
        }
    }
}