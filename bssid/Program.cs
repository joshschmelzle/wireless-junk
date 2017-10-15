using System;

namespace bssid
{
    class Program
    {
        private static string message;

        static void Main(string[] args)
        {
            showInterfaceInformation();
            Console.ReadKey();
        }
        
        private static void showInterfaceInformation()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "wlan show interfaces";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();

            if (!output.Contains("disconnected"))
            {
                message = "Wi-Fi connected to " + gv(output, "SSID") + " " + gv(output, "BSSID") + " " + gv(output, "Signal");
                Console.WriteLine(message);
                p.WaitForExit();
            }
            else
            {
                Console.WriteLine("Wi-Fi is disconnected");
            }
        }
        
        private static string gv(string output, string lookup)
        {
            string s = output.Substring(output.IndexOf(lookup));
            s = s.Substring(s.IndexOf(":"));
            return s.Substring(2, s.IndexOf("\n")).Trim();
        }
    }
}
