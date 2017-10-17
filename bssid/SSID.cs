using System.ComponentModel;

namespace bssid
{
    /// <summary>
    /// TODO: Add properities for Authentication, Channel, Network Type, Radio Type, Cipher, Channel, Signal, State
    /// </summary>
    public class SSID : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private string bssid;

        public SSID()
        {

        }

        public SSID(string name, string bssid)
        {
            this.name = name;
            this.bssid = bssid;
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public string BSSID
        {
            get { return bssid; }
            set
            {
                bssid = value;
                OnPropertyChanged("BSSIDValue");
            }
        }

        protected void OnPropertyChanged(string bssid)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(bssid));
            }
        }
    }
}
