using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
            // Сначало необходимо уставновить NuGet: QRCoder
            // Ссылка на XL таблицу
            string soucer_xl = "https://github.com/AntonPrNone/Society"; //внутри кавычек надо вставить ссылку
            // Создание переменной библиотеки QRCoder
            QRCoder.QRCodeGenerator qr = new QRCoder.QRCodeGenerator();
            // Присваеваем значиения
            QRCoder.QRCodeData data = qr.CreateQrCode(soucer_xl, QRCoder.QRCodeGenerator.ECCLevel.L);
            // переводим в Qr
            QRCoder.QRCode code = new QRCoder.QRCode(data);
            Bitmap bitmap = code.GetGraphic(100);
            /// Создание картинки
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                imageQr.Source = bitmapimage;
            }
        }
    }
}
