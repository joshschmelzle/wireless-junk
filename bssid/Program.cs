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
            scrape_netsh();
        }

        private static void scrape_netsh()
        {
            Process p = new Process();
            p.StartInfo.FileName = "netsh.exe";
            p.StartInfo.Arguments = "wlan show interfaces";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            SSID ssid = null;
            bool connected = false;

            for (;;) // enter cryptic loop
            {
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                string interfaceState = gv(output, "State");


                if (interfaceState == "connected")
                {
                    if (ssid == null)
                    {
                        ssid = MakeSSID(output, interfaceState);
                        ConnectedMessage(ssid);
                        p.WaitForExit();
                        connected = true;
                        continue; // goto start of cryptic loop
                    }

                    string checkSSID = gv(output, "SSID");

                    if (checkSSID != ssid.Name)
                    {
                        ssid = MakeSSID(output, interfaceState);
                    }
                    
                    string checkBSSID = gv(output, "BSSID");

                    if (!connected) // if i wasn't connected before, it's a new connection. 
                    {
                        connected = true;
                        ConnectedMessage(ssid);
                    }


                    if (ssid.BSSID != checkBSSID) // if interface was connected, check to see if there is a new BSSID indicating a BSS ROAM. 
                    {
                        ssid.BSSID = checkBSSID;
                        RoamMessage(ssid.Name, ssid.BSSID, checkBSSID);
                    }
                }

                if (interfaceState == "disconnected" && connected)
                {
                    Console.WriteLine("Wi-Fi is disconnected");
                    connected = false;
                    continue;
                }

                p.WaitForExit();

                Thread.Sleep(100);
            }
        }

        private static SSID MakeSSID(string output, string interfaceState)
        {
            SSID ssid;
            string name = gv(output, "SSID");
            string bssid = gv(output, "BSSID");
            string signal = gv(output, "Signal");
            decimal rx_rate = Convert.ToDecimal(gv(output, "Receive"));
            decimal tx_rate = Convert.ToDecimal(gv(output, "Transmit"));
            int channel = Convert.ToInt32(gv(output, "Channel"));
            string radio = gv(output, "Radio").Remove(0, 6);
            string cipher = gv(output, "Cipher");
            string security = gv(output, "Authentication").Replace("-", "_");
            
            ssid = new SSID(name,
                bssid,
                signal,
                rx_rate,
                tx_rate,
                channel,
                (Radio)Enum.Parse(typeof(Radio), radio),
                (Cipher)Enum.Parse(typeof(Cipher), cipher),
                (Security)Enum.Parse(typeof(Security), security),
                (InterfaceState)Enum.Parse(typeof(InterfaceState), interfaceState));
            return ssid;
        }

        private static void RoamMessage(string SSID, string currentBSSID, string newBSSID)
        {
            string message = "Roam on " + SSID + " from " + currentBSSID + " to " + newBSSID;
            Console.WriteLine(message);
            ShowToast(message);
        }

        private static void ConnectedMessage(SSID ssid)
        {
            string message = "Wi-Fi connected to " + ssid.Name + " " + ssid.BSSID + " " + ssid.Signal;
            Console.WriteLine(message);
            ShowToast(message);
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
