using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QRCoder;
using QRCoderIDN.Properties;

namespace QRCoderIDN
{
    public partial class QRCoderIDN : Form
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public QRCoderIDN()
        {
            InitializeComponent();
            ToolTip pictureBox1ToolTip = new ToolTip();
            pictureBox1ToolTip.SetToolTip(pictureBox1, "Copy QR Code to clipboard");
            ToolTip pictureBox2ToolTip = new ToolTip();
            pictureBox2ToolTip.SetToolTip(pictureBox2, "Save QR Code");
            ToolTip pictureBox3ToolTip = new ToolTip();
            pictureBox3ToolTip.SetToolTip(pictureBox3, "Clear");
            ToolTip pictureBox4ToolTip = new ToolTip();
            pictureBox4ToolTip.SetToolTip(textBox1, "Text to create QR");
        }

        private async Task GenerateQrCodeAsync(string mytext)
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
                if (textBox1.TextLength > 23)
                {
                    Font newFont = new Font("Calibri", 10, FontStyle.Bold);
                    if (textBox1.TextLength > 92)
                        newFont = new Font("Calibri", 5, FontStyle.Bold);
                    textBox1.Font = newFont;
                }
                else
                {
                    Font newFont = new Font("Calibri", 20, FontStyle.Bold); // You can adjust the size as needed
                    textBox1.Font = newFont;
                }

                if (textBox1.Text.Length != 0)
                {
                    if (textBox1.Text.Length < 12000)
                    {
                        cancellationTokenSource.Cancel();
                        cancellationTokenSource = new CancellationTokenSource();
                        await GenerateQrCodeAsync(textBox1.Text);
                    }
                    else
                        MessageBox.Show("Enter a text with less length.");
                }

                if (textBox1.Text == "")
                {
                    pictureBox1.Image = Resources.empty;
                }
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "png";
                saveFileDialog.FileName = textBox1.Text;
                saveFileDialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;
                    pictureBox1.Image.Save(fileName, ImageFormat.Png);
                }
            }
            else
            {
                MessageBox.Show("No QR code to save!");
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            textBox1.Text = "";
        }
    }
}
