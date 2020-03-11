using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WPF_ProveVarie
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _key = "56yh@oPy";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButEncrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CodeOutput.Text = new Blowfish(_key.ToCharArray()).EnCryptStringByChars(Input.Text);
            }
            catch (Exception ex) { throw; }
        }

        private void ButDecrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CodeInput.Text = new Blowfish(_key.ToCharArray()).DeCryptStringByChars(CodeOutput.Text);
            }
            catch (Exception ex) { throw; }
        }

        private void ButEncryptRaw_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UInt64[] crypted = new Blowfish(_key.ToCharArray()).EnCryptString(Input.Text);

                foreach (UInt64 u64 in crypted)
                {
                    CodeInput.Text += u64.ToString();
                }

                string decrypted = new Blowfish(_key.ToCharArray()).DeCryptRawData(crypted);

                CodeOutput.Text = decrypted;
            }
            catch (Exception ex) { throw; }

        }

        private void ButEncodeUTF8_Click(object sender, RoutedEventArgs e)
        {
            string input = Input.Text;
            string output = "";

            try
            {
                byte[] encodeBytes = Encoding.UTF8.GetBytes(input);

                output += "Numero di bytes di encoding UTF-8: " + encodeBytes.Length + Environment.NewLine;
                foreach (byte b in encodeBytes)
                {
                    //output += ((int)b).ToString() + Environment.NewLine;
                    output += ((char)b).ToString();
                }
                
                Output.Text = output;

                Blowfish blowfish = new Blowfish(_key.ToCharArray());
                CodeOutput.Text = blowfish.DeCryptRawData(blowfish.EnCryptString(encodeBytes));
                
            }
            catch (Exception ex) { }
        }
    }
}
