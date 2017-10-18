using System;
using System.Diagnostics;
using Windows.UI.Notifications;

namespace bssid
{
    class Program
    {
        static void Main(string[] args)
        {
            scrape_netsh();
        }

        /// <summary>
        /// TODO: Handle SSID changes
        /// </summary>
        private static SSID ssid = null;
        private static bool connected = false;

        private static void scrape_netsh()
        {
            Process p = new Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "wlan show interfaces";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            for (;;) // enter cryptic loop
            {
                p.Start();

                string output = p.StandardOutput.ReadToEnd();

                string state = gv(output, "State");

                // State : disconnected
                // State : associating
                // State : authenticating
                // State : connected
                if (state == "connected")
                {
                    if (ssid == null)
                    {
                        ssid = new SSID(gv(output, "SSID"), gv(output, "BSSID"));
                        string message = "Wi-Fi connected to " + ssid.Name + " " + ssid.BSSID + " " + gv(output, "Signal");
                        Console.WriteLine(message);
                        ShowToast(message);
                        p.WaitForExit();
                        connected = true;
                        continue; // goto start of cryptic loop
                    }

                    ssid.Name = gv(output, "SSID");
                    string oldBSSID = ssid.BSSID;
                    string newBSSID = gv(output, "BSSID");
                    string signal = gv(output, "Signal");

                    if (!connected) // if i wasn't connected before, it's a new connection. 
                    {
                        connected = true;
                        string message = "Wi-Fi connected to " + ssid.Name + " " + ssid.BSSID + " " + signal;
                        Console.WriteLine(message);
                        ShowToast(message);
                    }

                    if (ssid.BSSID != newBSSID) // if interface was connected, check to see if there is a new BSSID indicating a BSS ROAM. 
                    {
                        ssid.BSSID = newBSSID;
                        string message = "Roam on " + ssid.Name + " from " + oldBSSID + " to " + newBSSID;
                        Console.WriteLine(message);
                        ShowToast(message);
                    }
                }
                else
                {
                    Console.WriteLine("Wi-Fi is disconnected");
                    connected = false;
                }

                p.WaitForExit();
            }
        }
        
        private static string gv(string output, string lookup)
        { 
            string s = output.Substring(output.IndexOf(lookup));
            s = s.Substring(s.IndexOf(":"));
            return s.Substring(2, s.IndexOf("\n")).Trim();
        }

        /// <summary>
        /// Use COM server with Win32 app to make toast messages persist in the action center https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/10/16/quickstart-handling-toast-activations-from-win32-apps-in-windows-10/
        /// </summary>
        private static void ShowToast(string message)
        {
            // Get a toast XML template
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

            // Fill in the text elements
            Windows.Data.Xml.Dom.XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            for (int i = 0; i < stringElements.Length; i++)
            {
                stringElements[i].AppendChild(toastXml.CreateTextNode(message));
            }

            var toast = new ToastNotification(toastXml);

            ToastNotificationManager.CreateToastNotifier("Wi-Fi Toaster").Show(toast);
        }
    }
}
