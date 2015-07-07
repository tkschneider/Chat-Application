using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;
using System.Drawing;

// Author: Tim Schneider
// Project for CSC 355
// North Central College

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isSetUp = false;
        bool DialoghasShown = false;
        bool isServer;
        bool IsRemotePeerOpen = true;
        public int port = 50000;
        public String IP;
        Stream DisplayimageStream = null;
        Stream SendImageStream = null;
        Stream RecievedImageStream = null;
        BitmapImage DisplayBitmapImage = null;
        BitmapImage SendBitmapImage = null;
        BitmapImage RecievedImage = null;
        List<String> chatHistoryList = new List<String>();
        BinaryWriter writer;
        BinaryReader reader;
        TcpClient client;
        NetworkStream netStream;
        TcpListener listener;


        public MainWindow()
        {

            InitializeComponent();

        }

        //Display clientServerWindow and record selection
        private void ShowClientServerDialog(object sender, EventArgs e)
        {
            //if not show create a window
            if (DialoghasShown == false)
            {
                ClientServerDialog clientDialog = new ClientServerDialog();
                clientDialog.Show();
                DialoghasShown = true;
            }
            else //Is shown
            {
                if (isSetUp == false)
                {
                    isServer = (bool)Application.Current.Properties["IsServer"];

                    if (isServer == true)
                    {
                        listener = new TcpListener(IPAddress.Any, port);
                        listener.Start();
                        client = listener.AcceptTcpClient();
                        this.Title = "Server";
                    }
                    else
                    {
                        IP = Application.Current.Properties["IPAddress"].ToString();
                        client = new TcpClient(IP, port);
                        this.Title = "Client";
                    }

                    netStream = client.GetStream();
                    reader = new BinaryReader(netStream);
                    writer = new BinaryWriter(netStream);
                    Thread th = new Thread(StreamInputHandler);
                    th.Start(client);

                    isSetUp = true;
                }
            }
        }

        //This method opens a dialog for user to select an image and displays the image in the sentBox 
        private void ImageSelect(object sender, RoutedEventArgs e)
        {
            // Pop up a file open dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "image files|*.png";
            dlg.ShowDialog();

            // Obtain the selected file
            DisplayimageStream = dlg.OpenFile();

            DisplayBitmapImage = new BitmapImage();
            DisplayBitmapImage.BeginInit();
            DisplayBitmapImage.StreamSource = DisplayimageStream;
            DisplayBitmapImage.EndInit();

            SentBox.Source = DisplayBitmapImage;

            ChatHistory.Text = "Image selected";

            SendImageStream = dlg.OpenFile();
            MemoryStream memoryStream = new MemoryStream();
            SendImageStream.CopyTo(memoryStream);
            long length = memoryStream.Length;
            Byte[] bytes = memoryStream.ToArray();

            writer.Write("Pic");
            writer.Write(length);
            writer.Write(bytes);
            writer.Flush();
        }

        //Display text in chatHistoryBlock and writes message to Stream
        private void DisplayMessage(String message)
        {
            chatHistoryList.Add(message);
            if (chatHistoryList.Count >= 9)
            {
                chatHistoryList.RemoveAt(0);
            }
            ChatHistory.Text = string.Join(Environment.NewLine, chatHistoryList);

        }

        //When user selects send the text in the messageBox is sent
        private void SentButtonClicked(object sender, RoutedEventArgs e)
        {
            String Message = MessageInputBox.Text;
            writer.Write("chat");
            writer.Write(Message);
            writer.Flush();
            DisplayMessage("Me: " + Message);
            MessageInputBox.Clear();
        }

        //If enter is pressed display text in messageBox
        private void EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                String text = MessageInputBox.Text;
                DisplayMessage("Me: " + text);
                writer.Write("chat");
                writer.Write(text);
                writer.Flush();
                MessageInputBox.Clear();
            }
        }

        private void StreamInputHandler(Object ob)
        {

            while (IsRemotePeerOpen)
            {
                String cmd = null;
                try
                {
                    cmd = reader.ReadString().ToLower();
                }
                catch (IOException)
                {
                    // Remote peer has closed  return
                    return;
                }

                switch (cmd)
                {
                    case "done":
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Remote peer has closed.");
                            IsRemotePeerOpen = false;
                            Application.Current.Shutdown(0);
                        });
                        break;

                    case "chat":
                        cmd = reader.ReadString();
                        ChatHistory.Dispatcher.Invoke(() =>
                        {
                            DisplayMessage("Remote: " + cmd);
                        });
                        break;

                    case "pic":

                        long size = reader.ReadInt64();
                        Byte[] bytes = reader.ReadBytes((int)size);

                        RecieveBox.Dispatcher.Invoke(() =>
                            {
                                MemoryStream inputStream = new MemoryStream(bytes);
                                BitmapImage inputBitmap = new BitmapImage();
                                inputBitmap.BeginInit();
                                inputBitmap.StreamSource = inputStream;
                                inputBitmap.EndInit();
                                RecieveBox.Source = inputBitmap;
                            });

                        break;
                }
            }
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsRemotePeerOpen)
            {
                writer.Write("done");
                writer.Flush();
                writer.Close();
            }
        }
    }
}
