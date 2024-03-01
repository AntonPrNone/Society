using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using QRCoder;
using QRCoder.Xaml;
using Color = System.Drawing.Color;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для AboutProgramControl.xaml
    /// </summary>
    public partial class AboutProgramControl : UserControl
    {
        public AboutProgramControl()
        {
            InitializeComponent();

            CreateQRCode();
        }

        public void CreateQRCode()
        {
            string soucer_xl = "https://github.com/AntonPrNone/Society";
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(soucer_xl, QRCodeGenerator.ECCLevel.L);
            XamlQRCode code = new XamlQRCode(data);
            DrawingImage bitmap = code.GetGraphic(100);

            imageQr.Source = bitmap;
        }
    }
}
