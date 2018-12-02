using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Socket socket;
        static IPAddress IpAddress;
        static int Port;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                IpAddress = GetIp();
                Port = 1997;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IpAddress, Port));
                new Thread(t =>
                {
                    while (true)
                    {
                        byte[] Buffer = new byte[255];
                        int recievedBytes = socket.Receive(Buffer, 0, Buffer.Length, 0);
                        Array.Resize(ref Buffer, recievedBytes);
                        if (!string.IsNullOrEmpty(Encoding.Default.GetString(Buffer)))
                        {
                            lblMessagesHistory.Dispatcher.Invoke(new Action(() => {
                                string Message = Encoding.Default.GetString(Buffer);
                                var item = new TextBlock();
                                item.Text = Message;
                                item.HorizontalAlignment = HorizontalAlignment.Right;
                                item.Foreground = Brushes.Green;
                                stpMessages.Children.Add(item);
                            }));
                        }
                    }
                }).Start();
            }
            catch (Exception ex) { Console.WriteLine(ex); }

        }

        public static IPAddress GetIp()
        {
            string HostName = Dns.GetHostName();
            IPHostEntry HostEntry = Dns.GetHostEntry(HostName);
            return HostEntry.AddressList[HostEntry.AddressList.Length - 1];
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            string message = "Server: " + txtMessageToBeSend.Text + "\n";
            var item = new TextBlock();
            item.Text = message;
            item.HorizontalAlignment = HorizontalAlignment.Left;
            item.Foreground = Brushes.Red;
            stpMessages.Children.Add(item);
            socket.Send(Encoding.Default.GetBytes(message));
        }
    }
}
