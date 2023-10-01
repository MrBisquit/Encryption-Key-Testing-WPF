using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncryptionKeyTestingWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        long dtn = 0;
        int key = 0;
        Encryption encryption = new Encryption();

        List<Task> tasks = new List<Task>();
        int activeTasks = 0;

        int totalBytes = 0;
        int bytesProcessed = 0;

        int status = 0; // 0 = Idle, 1 = Starting, 2 = Aborting, 3 = Processing, 4 = Finished
        bool processing = false;

        Stopwatch sw = new Stopwatch();

        bool isEncrypt = true;
        bool isByteEncryption = true;
        bool isASCII = true;

        byte[] encrypted;
        byte[] decrypted;

        bool wasLastEncrypted = false;
        bool wasLastASCII = false;

        private async void StartKeyCycle()
        {
            while(true)
            {
                if (!string.IsNullOrEmpty(CustomKey.Text))
                {
                    key = int.Parse(CustomKey.Text);
                }
                else
                {
                    encryption.GetKey();

                    dtn = encryption.dtn;
                    key = encryption.key;
                }

                await Task.Delay(5000);
            }
        }

        private async void StartUpdateUICycle()
        {
            while(true)
            {
                EncryptionKey.Content = $"Encryption key: {key}";
                ActiveTasks.Content = $"Active tasks: {activeTasks}";
                BytesProcessed.Content = $"Bytes processed: {bytesProcessed} Bytes/{totalBytes} Bytes";
                if(status == 0)
                {
                    Status.Content = "Status: Idle";
                } else if(status == 1)
                {
                    Status.Content = "Status: Starting";
                } else if (status == 1)
                {
                    Status.Content = "Status: Aborting";
                } else if (status == 1)
                {
                    Status.Content = "Status: Processing";
                } else if (status == 1)
                {
                    Status.Content = "Status: Finished";
                } else
                {
                    Status.Content = "Status: Unknown";
                }
                if(processing)
                {
                    StartButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                } else
                {
                    StartButton.IsEnabled = true;
                    StopButton.IsEnabled = false;
                }
                TimeElapsed.Content = "Time elapsed: " + sw.Elapsed.ToString(@"hh\:mm\:ss\:fff");

                if (!string.IsNullOrEmpty(CustomKey.Text))
                {
                    encryption.key = int.Parse(CustomKey.Text);
                    key = int.Parse(CustomKey.Text);
                }

                isEncrypt = (bool)IsEncrypt.IsChecked;
                isByteEncryption = (bool)IsByteEncryption.IsChecked;
                isASCII = (bool)IsASCII.IsChecked;

                await Task.Delay(100);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = 1150;
            MinWidth = 1150;

            StartKeyCycle();
            StartUpdateUICycle();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            encrypted = new byte[0];
            decrypted = new byte[0];

            status = 1;
            processing = true;

            sw.Reset();
            sw.Start();

            byte[] start;

            if(isASCII)
            {
                start = Encoding.ASCII.GetBytes(Input.Text);
            } else
            {
                start = Encoding.UTF8.GetBytes(Input.Text);
            }

            totalBytes = start.Length;
            Progress.Maximum = totalBytes;

            if(isByteEncryption)
            {
                if (isEncrypt)
                {
                    wasLastEncrypted = true;
                    encrypted = encryption.EncryptBytes(start);
                    if (isASCII)
                    {
                        wasLastASCII = true;
                        Output.Text = Encoding.ASCII.GetString(encrypted);
                    }
                    else
                    {
                        wasLastASCII = false;
                        Output.Text = Encoding.UTF8.GetString(encrypted);
                    }
                } else
                {
                    wasLastEncrypted = false;
                    decrypted = encryption.DecryptBytes(start);
                    if (isASCII)
                    {
                        wasLastASCII = true;
                        Output.Text = Encoding.ASCII.GetString(decrypted);
                    }
                    else
                    {
                        wasLastASCII = false;
                        Output.Text = Encoding.UTF8.GetString(decrypted);
                    }
                }
            } else
            {
                if(isEncrypt)
                {
                    wasLastEncrypted = true;
                    string enc = encryption.Encrypt(Input.Text);
                    Output.Text = enc;

                    if(isASCII)
                    {
                        wasLastASCII = true;
                        encrypted = Encoding.ASCII.GetBytes(enc);
                    } else
                    {
                        wasLastASCII = false;
                        encrypted = Encoding.UTF8.GetBytes(enc);
                    }
                } else
                {
                    string dec = encryption.Decrypt(Input.Text);
                    Output.Text = dec;
                    wasLastEncrypted = false;
                    if (isASCII)
                    {
                        wasLastASCII = true;
                        decrypted = Encoding.ASCII.GetBytes(dec);
                    }
                    else
                    {
                        wasLastASCII = false;
                        decrypted = Encoding.UTF8.GetBytes(dec);
                    }
                }
            }

            wasLastASCII = isASCII;

            bytesProcessed = totalBytes;
            Progress.Value = bytesProcessed;

            sw.Stop();

            status = 0;
            processing = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            status = 0;
            processing = false;

            sw.Stop();
        }

        private void BottomToTop_Click(object sender, RoutedEventArgs e)
        {
            if(wasLastEncrypted)
            {
                if(wasLastASCII)
                {
                    Input.Text = Encoding.ASCII.GetString(encrypted);
                } else
                {
                    Input.Text = Encoding.UTF8.GetString(encrypted);
                }
            } else
            {
                if(wasLastASCII)
                {
                    Input.Text = Encoding.ASCII.GetString(decrypted);
                } else
                {
                    Input.Text = Encoding.UTF8.GetString(decrypted);
                }
            }
            Output.Text = "";
        }

        private void OFASCII_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if((bool)openFileDialog.ShowDialog())
            {
                byte[] data = File.ReadAllBytes(openFileDialog.FileName);
                Input.Text= Encoding.ASCII.GetString(data);
            }
        }

        private void OFUTF8_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                byte[] data = File.ReadAllBytes(openFileDialog.FileName);
                Input.Text = Encoding.UTF8.GetString(data);
            }
        }

        private void OFUTF7_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                byte[] data = File.ReadAllBytes(openFileDialog.FileName);
                Input.Text = Encoding.UTF7.GetString(data);
            }
        }

        private void OFUTF32_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if ((bool)openFileDialog.ShowDialog())
            {
                byte[] data = File.ReadAllBytes(openFileDialog.FileName);
                Input.Text = Encoding.UTF32.GetString(data);
            }
        }
    }
}