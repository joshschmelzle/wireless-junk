using System.ComponentModel;

namespace bssid
{
    public class SSID : INotifyPropertyChanged
    {
        private string name;
        private string bssid;
        private string signal;
        private decimal rx_rate;
        private decimal tx_rate;
        private int channel;
        private Radio radio; 
        private Cipher cipher;
        private Security security; //authentication
        private InterfaceState interfaceState;

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
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
        
        public SSID()
        {

        }

        public SSID(string name, string bssid, string signal, decimal rx_rate, decimal tx_rate, int channel, Radio radio, Cipher cipher, Security security, InterfaceState interfaceState)
        {
            this.name = name;
            this.bssid = bssid;
            this.signal = signal;
            this.rx_rate = rx_rate;
            this.tx_rate = tx_rate;
            this.radio = radio;
            this.channel = channel;
            this.cipher = cipher;
            this.bssid = bssid;
            this.security = security;
            this.interfaceState = interfaceState;
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (value != this.name)
                {
                    name = value;
                }
            }
        }

        public string BSSID
        {
            get { return this.bssid; }
            set
            {
                if (value != this.bssid)
                {
                    this.bssid = value;
                    OnPropertyChanged("BSSID");
                }
            }
        }

        public string Signal
        {
            get { return this.signal; }
            set
            {
                if (value != this.signal)
                {
                    this.signal = value;
                    OnPropertyChanged("Signal");
                }
            }
        }

        public decimal RX_rate
        {
            get { return this.rx_rate; }
            set
            {
                if (value != this.rx_rate)
                {
                    this.rx_rate = value;
                    OnPropertyChanged("RX_rate");
                }
            }
        }

        public decimal TX_rate
        {
            get { return this.tx_rate; }
            set
            {
                if (value != this.tx_rate)
                {
                    this.tx_rate = value;
                    OnPropertyChanged("TX_rate");
                }
            }
        }

        public int Channel
        {
            get { return this.channel; }
            set
            {
                if (value != this.channel)
                {
                    this.channel = value;
                    OnPropertyChanged("Channel");
                }
            }
        }

        public Radio Radio
        {
            get { return this.radio; }
            set
            {
                if (value != this.radio)
                {
                    radio = value;
                }
            }
        }

        public Cipher Cipher
        {
            get { return this.cipher; }
            set
            {
                if (value != this.cipher)
                {
                    this.cipher = value;
                    OnPropertyChanged("Cipher");
                }
            }
        }

        public Security Security
        {
            get { return this.security; }
            set
            {
                if (value != this.security)
                {
                    this.security = value;
                    OnPropertyChanged("Security");
                }
            }
        }

        public InterfaceState InterfaceState
        {
            get { return this.interfaceState; }
            set
            {
                if (value != this.interfaceState)
                {
                    this.interfaceState = value;
                    OnPropertyChanged("InterfaceState");
                }
            }
        }
    }
}
