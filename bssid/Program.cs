using System;
using System.Diagnostics;
using System.Threading;
using Windows.UI.Notifications;

namespace bssid
{
    class Program
    {
        static void Main(string[] args)
        {
            showInterfaceInformation();
            Console.ReadKey();
        }
        
        private static SSID ssid;
        private static bool connected = false;
        
        private static void showInterfaceInformation()
        {
            Process p = new Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "wlan show interfaces";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();

            if (!output.Contains("disconnected"))
            {
                ssid = new SSID(gv(output, "SSID"), gv(output, "BSSID"));
                string message = "Wi-Fi connected to " + ssid.Name + " " + ssid.BSSID + " " + gv(output, "Signal");
                Console.WriteLine(message);
                connected = true;
                ShowToast("Wi-Fi is connected");
                p.WaitForExit();
            }
            else
            {
                Console.WriteLine("Wi-Fi is disconnected");
                connected = false;
                p.WaitForExit();
            }

            bool createdNew;
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "11e370ec-2092-45a2-befd-0946ed24769f", out createdNew);
            var signaled = false;
            
            if (!createdNew)
            {
                waitHandle.Set();
                return;
            }

            var timer = new Timer(OnTimerElapsed, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            do
            {
                signaled = waitHandle.WaitOne(TimeSpan.FromMilliseconds(300));
            } while (!signaled);            
        }

        private static void OnTimerElapsed(object state)
        {
            Process p = new Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "wlan show interfaces";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();

            if (!output.Contains("disconnected"))
            {
                ssid.Name = gv(output, "SSID");
                string oldBSSID = ssid.BSSID;
                string newBSSID = gv(output, "BSSID");
                string signal = gv(output, "Signal");

                if (!connected)
                {
                    connected = true;

                    string message = "Wi-Fi connected to " + ssid.Name + " " + ssid.BSSID;
                    Console.WriteLine(message);
                    ShowToast(message);
                }
                
                if (ssid.BSSID != newBSSID)
                {
                    ssid.BSSID = newBSSID;
                    string message = "Roam on " + ssid.Name + " from " + oldBSSID + " to " + newBSSID;
                    Console.WriteLine(message);
                    ShowToast(message);
                }
                
                p.WaitForExit();
            }
            else
            {
                if (connected)
                {
                    connected = false;
                    Console.WriteLine("Wi-Fi is disconnected");
                    ShowToast("Wi-Fi is disconnected");
                    p.WaitForExit();
                }

                p.WaitForExit();
            }
        }

        /// <summary>
        /// TODO: ERROR HANDLING 
        /// </summary>
        private static string gv(string output, string lookup)
        { 
            string s = output.Substring(output.IndexOf(lookup));
            s = s.Substring(s.IndexOf(":"));
            return s.Substring(2, s.IndexOf("\n")).Trim();
        }

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
