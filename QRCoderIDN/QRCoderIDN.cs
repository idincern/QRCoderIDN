using System;
using System.Drawing;
using System.Windows.Forms;
using QRCoder;

namespace QRCoderIDN
{
    public partial class QRCoderIDN : Form
    {
        public QRCoderIDN()
        {
            InitializeComponent();
            ToolTip pictureBoxToolTip = new ToolTip();
            pictureBoxToolTip.SetToolTip(pictureBox1, "Copy image to clipboard.");
        }

        private void myQrCoder(string mytext)
        {
            // Calculate the desired size for the QR code based on PictureBox size
            int desiredWidth = pictureBox1.Width;
            int desiredHeight = pictureBox1.Height;
            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(mytext, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Get the original QR code image
                    Bitmap qrCodeImage = qrCode.GetGraphic(10);
                    // Resize the QR code image to fit within the PictureBox
                    Bitmap resizedImage = ResizeImage(qrCodeImage, desiredWidth, desiredHeight);
                    // Display the resized image in the PictureBox
                    pictureBox1.Image = resizedImage;
                }
            }
            catch (Exception)
            {
                return;
            }

        }

        private Bitmap ResizeImage(Bitmap originalImage, int newWidth, int newHeight)
        {
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }
            return resizedImage;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            myQrCoder(textBox1.Text);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Copy image to clipboard
                Clipboard.SetImage(pictureBox1.Image);
            }
            else
            {
                MessageBox.Show("No QR code to copy!");
            }
        }
    }
}
