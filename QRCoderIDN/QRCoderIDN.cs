using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;

namespace QRCoderIDN
{
    public partial class QRCoderIDN : Form
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public QRCoderIDN()
        {
            InitializeComponent();
            ToolTip pictureBoxToolTip = new ToolTip();
            pictureBoxToolTip.SetToolTip(pictureBox1, "Copy image to clipboard.");
        }

        private async Task GenerateQrCodeAsync(string mytext, CancellationToken cancellationToken)
        {
            // Calculate the desired size for the QR code based on PictureBox size
            int desiredWidth = pictureBox1.Width;
            int desiredHeight = pictureBox1.Height;

            // Check if cancellation is requested at appropriate points
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(mytext, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                await Task.Run(() => 
                {  
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    Bitmap resizedImage = ResizeImage(qrCodeImage, desiredWidth, desiredHeight);
                    pictureBox1.Invoke((MethodInvoker)delegate { pictureBox1.Image = resizedImage; });
                });
            }
        }
       
        private Bitmap ResizeImage(Bitmap originalImage, int newWidth, int newHeight)
        {
            Bitmap resizedImage = new Bitmap(newWidth, newHeight, originalImage.PixelFormat);

            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

        private async void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length < 12000)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource = new CancellationTokenSource();
                    await GenerateQrCodeAsync(textBox1.Text, cancellationTokenSource.Token);
                }
                else
                    MessageBox.Show("Enter a text with less length.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Input is invalid.");
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
                Clipboard.SetImage(pictureBox1.Image);
            else
                MessageBox.Show("No QR code to copy!");
        }
    }
}
