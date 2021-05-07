using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Tools
{
    public static class CmdUtil
    {
        private const string CMDPATH = @"C:\Windows\System32\cmd.exe";

        public static string RunCmd(string[] cmds)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = CMDPATH;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                p.Start();

                foreach (var cmd in cmds)
                {
                    p.StandardInput.WriteLine(cmd);
                }
                
                p.StandardInput.WriteLine("exit");

                var output = p.StandardOutput.ReadToEnd();

                p.WaitForExit();
                p.Close();

                var reg = new Regex("\n");
                var matches = reg.Matches(output);
                if (matches.Count > 1)
                {
                    var start = matches[1].Index;
                    output = output.Substring(start + 1);
                }
                
                return output;
            }
        }
    }
}
