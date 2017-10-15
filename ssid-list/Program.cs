using NativeWifi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace junk
{
    class Program
    {
        private static Timer bssTimer;

        private static WlanClient wlanClient;

        static void Main(string[] args)
        {
            init();

            wlanClient = new WlanClient();

            Debug.WriteLine("Press the Enter key to exit the application...");
            //Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);

            outputBSSlist();
            
            //bssTimer.Start();
            Console.ReadLine();
            //bssTimer.Stop();
            //bssTimer.Dispose();
            Debug.WriteLine("Terminating the application...");
        }

        private static void SetTimer()
        {
            // create timer with two second interval.
            bssTimer = new System.Timers.Timer(300);

            // create hooks
            bssTimer.Elapsed += OnTimedEvent;
            bssTimer.AutoReset = true;
            bssTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            outputBSSlist();
        }

        private static void outputBSSlist()
        {
            Console.WriteLine("RSSI, BSSID, SSID, Channel, BSS Type Basic Rates, Link Quality");

            foreach (WlanClient.WlanInterface wlanInterface in wlanClient.Interfaces)
            {
                Wlan.WlanBssEntry[] wlanBssEntries = wlanInterface.GetNetworkBssList();

                //Wlan.WlanAvailableNetwork[] wlanAvailableEntries = wlanInterface.GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllManualHiddenProfiles);

                foreach (Wlan.WlanBssEntry wlanBssEntry in wlanBssEntries)
                {
                    // get MAC of BSSID
                    byte[] macAddr = wlanBssEntry.dot11Bssid;
                    var macAddrLen = (uint)macAddr.Length;
                    var str = new string[(int)macAddrLen];
                    for (int i = 0; i < macAddrLen; i++)
                    {
                        str[i] = macAddr[i].ToString("x2");
                    }
                    string mac = string.Join("", str);

                    // get readable version of SSID and let me know if it's blank
                    var ssid2 = System.Text.Encoding.Default.GetString(wlanBssEntry.dot11Ssid.SSID);
                    ssid2 = ssid2.Replace("\0", "").ToLower();
                    if (ssid2 == "")
                        ssid2 = "[NULL]";

                    // get supported rates 
                    string rateMessage = String.Empty;

                    foreach (var r in wlanBssEntry.wlanRateSet.Rates)
                    {
                        // to calculate the basic data transfer rate in Mbps for an arbitrary entry rateSet
                        var rate_in_mbps = (r & 0x7FFF) * 0.5;
                        rateMessage += rate_in_mbps.ToString() + " ";
                        //rateMessage += Convert.ToInt32(r).ToString() + " ";
                    }

                    rateMessage = rateMessage.Trim();
                    
                    // write to console
                    Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}",
                        wlanBssEntry.rssi,
                        mac,
                        ssid2,
                        Get80211Channel(wlanBssEntry.chCenterFrequency),
                        wlanBssEntry.dot11BssType,
                        rateMessage,
                        wlanBssEntry.linkQuality);
                }
            }
        }
        
        private static uint Get80211Channel(uint channelCenterFrequency)
        {
            return ch[channelCenterFrequency / 1000];
        }

        private static Dictionary<uint, uint> ch = new Dictionary<uint, uint>();

        private static void init()
        {
            ch[2412] = 1;
            ch[2417] = 2;
            ch[2422] = 3;
            ch[2427] = 4;
            ch[2432] = 5;
            ch[2437] = 6;
            ch[2442] = 7;
            ch[2447] = 8;
            ch[2452] = 9;
            ch[2457] = 10;
            ch[2462] = 11;
            ch[2467] = 12;
            ch[2472] = 13;
            ch[2484] = 14;
            ch[5180] = 36;
            ch[5190] = 38;
            ch[5200] = 40;
            ch[5210] = 42;
            ch[5220] = 44;
            ch[5230] = 46;
            ch[5240] = 48;
            ch[5250] = 50;
            ch[5260] = 52;
            ch[5270] = 54;
            ch[5280] = 56;
            ch[5290] = 58;
            ch[5300] = 60;
            ch[5310] = 62;
            ch[5320] = 64;
            ch[5510] = 102;
            ch[5520] = 104;
            ch[5530] = 106;
            ch[5540] = 108;
            ch[5550] = 110;
            ch[5560] = 112;
            ch[5570] = 114;
            ch[5580] = 116;
            ch[5590] = 118;
            ch[5600] = 120;
            ch[5610] = 122;
            ch[5620] = 124;
            ch[5630] = 126;
            ch[5640] = 128;
            ch[5660] = 132;
            ch[5670] = 134;
            ch[5680] = 136;
            ch[5690] = 138;
            ch[5700] = 140;
            ch[5710] = 142;
            ch[5720] = 144;
            ch[5745] = 149;
            ch[5755] = 151;
            ch[5765] = 153;
            ch[5775] = 155;
            ch[5785] = 157;
            ch[5795] = 159;
            ch[5805] = 161;
            ch[5825] = 165;
        }
    }
}
