using System;
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
                string message = "Wi-Fi connected to " + gv(output, "SSID") + " " + gv(output, "BSSID") + " " + gv(output, "Signal");
                Console.WriteLine(message);
                ShowToast(message);

                p.WaitForExit();
            }
            else
            {
                Console.WriteLine("Wi-Fi is disconnected");
            }
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

            ToastNotificationManager.CreateToastNotifier("BSSID App").Show(toast);
        }

        private static string gv(string output, string lookup)
        {
            string s = output.Substring(output.IndexOf(lookup));
            s = s.Substring(s.IndexOf(":"));
            return s.Substring(2, s.IndexOf("\n")).Trim();
        }
    }
}
