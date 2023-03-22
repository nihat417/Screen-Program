using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
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

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    private Socket client;
    private EndPoint remoteEP;
    private IPAddress ip;
    private int port;
    private SocketReceiveFromResult recieve;

    public MainWindow()
    {
        InitializeComponent();
        ClientThings();
    }

    public void ClientThings()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        ip = IPAddress.Parse("127.0.0.1");
        port = 12345;
        remoteEP = new IPEndPoint(ip, port);
    }


    private async void Screen_Click(object sender, RoutedEventArgs e)
    {
        var size = ushort.MaxValue - 29;
        var buffer = new byte[size];
        await client.SendToAsync(buffer, SocketFlags.None, remoteEP);
        List<byte> list = new List<byte>();

        var len = 0;
        do
        {
            try
            {
                recieve = await client.ReceiveFromAsync(buffer, SocketFlags.None, remoteEP);
                len = recieve.ReceivedBytes;
                list.AddRange(buffer.Take(len));
                var image = GetImage(list.ToArray());
                Imagephto.Source = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        } while (len == buffer.Length);
    }

    //
    private static BitmapImage GetImage(byte[] imageInfo)
    {
        var image = new BitmapImage();

        using (var memoryStream = new MemoryStream(imageInfo))
        {
            memoryStream.Position = 0;
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = memoryStream;
            image.EndInit();
        }

        image.Freeze();

        return image;
    }
}
