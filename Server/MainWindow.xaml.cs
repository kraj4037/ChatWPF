using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirstWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Socket socket;
        static string IpAddress;
        static int Port = 1997;

        public MainWindow()
        {
            InitializeComponent();
            IpAddress = GetIPAddress();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Parse(IpAddress), Port));
            socket.Listen(0);
            new Thread(thread=> {
                socket = socket.Accept();
                while (true)
                {
                    Thread.Sleep(500);
                    byte[] Buffer = new byte[255];
                    int recievedbytes = socket.Receive(Buffer, 0, Buffer.Length, 0);
                    Array.Resize(ref Buffer, recievedbytes);
                    string data = Encoding.Default.GetString(Buffer);
                    if (!string.IsNullOrEmpty(data))
                    {
                        lblMessagesHistory.Dispatcher.Invoke(new Action(() =>
                        {
                            var item = new TextBlock();
                            item.Text = data;
                            item.HorizontalAlignment = HorizontalAlignment.Right;
                            item.Foreground = Brushes.Green;
                            stpMessages.Children.Add(item);
                        }));
                    }
                }
            }).Start();
            
        }

        public static string GetIPAddress()
        {
            string HostName = Dns.GetHostName();
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
            IPAddress[] ipAddressList = IpEntry.AddressList;
            return ipAddressList[ipAddressList.Length - 1].ToString();
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            string Message = "Client: " + txtMessageToBeSend.Text.ToString()+"\n";
            var item = new TextBlock();
            item.Text = Message;
            item.HorizontalAlignment = HorizontalAlignment.Left;
            item.Foreground = Brushes.Red;
            stpMessages.Children.Add(item);
            byte[] SendData = Encoding.Default.GetBytes(Message);
            socket.Send(SendData);
        }
    }
}
