using System.Diagnostics;

namespace UnityAppiumTests
{
    public class ShellHelper
    {
        string rootDir = "/";

        public ShellHelper(string rootDir) => this.rootDir = rootDir;
        public void StartAltTester() => StartProcess(@"/StartAltTester.app/Contents/MacOS/Automator Application Stub");
        public void StopAltTester() => StartProcess(@"/StopAltTester.app/Contents/MacOS/Automator Application Stub");
        public void StartAppium() => StartProcess(@"/StartAppium.app/Contents/MacOS/Automator Application Stub");
        public void StopAppium() => StartProcess(@"/StopAppium.app/Contents/MacOS/Automator Application Stub");

        private void StartProcess(string cmd)
        {
            string path = Path.Join(rootDir, cmd);
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = path;
            proc.StartInfo.Arguments = "";
            proc.StartInfo.UseShellExecute = false; 
            proc.StartInfo.RedirectStandardOutput = false;
            proc.Start();
        }
    }
}
