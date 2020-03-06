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

                string decrypted = new Blowfish(_key.ToCharArray()).DeCryptRawData(crypted);

                CodeOutput.Text = decrypted;
            }
            catch (Exception ex) { throw; }
        }
    }
}
