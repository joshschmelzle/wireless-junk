﻿using System;

namespace bssid
{
    class Program
    {
        private static string message;

        static void Main(string[] args)
        {
            showConnectedId();
            Console.ReadKey();
        }

        // Show SSID and Signal Strength
        private static void showConnectedId()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "wlan show interfaces";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string s = p.StandardOutput.ReadToEnd();
            string s1 = s.Substring(s.IndexOf("SSID"));
            s1 = s1.Substring(s1.IndexOf(":"));
            s1 = s1.Substring(2, s1.IndexOf("\n")).Trim();

            string s2 = s.Substring(s.IndexOf("BSSID"));
            s2 = s2.Substring(s2.IndexOf(":"));
            s2 = s2.Substring(2, s2.IndexOf("\n")).Trim();

            string s3 = s.Substring(s.IndexOf("Signal"));
            s3 = s3.Substring(s3.IndexOf(":"));
            s3 = s3.Substring(2, s3.IndexOf("\n")).Trim();

            message = "Wi-Fi connected to " + s1 + " " + s2 + " " + s3;
            Console.WriteLine(message);
            p.WaitForExit();
        }
    }
}
