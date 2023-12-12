using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

unsafe public class AsmProxy
{
    [DllImport("Asm.dll")]
    private static extern double asmAddTwoDoubles(double a, double b);

    public double executeAsmAddTwoDoubles(double a, double b)
    {
        return asmAddTwoDoubles(a, b);
    }
}


namespace Aplproject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage bitmap;
        public MainWindow()
        {
            InitializeComponent();
            bitmap = new BitmapImage();
        }
        private void loadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    bitmap = convert_to_grayscale(bitmap);
                    int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;
                    byte[] pixelData = new byte[bitmap.PixelHeight * stride];
                    bitmap.CopyPixels(pixelData, stride, 0);
                    displayImage.Source = bitmap;

                    string filePath = "pixelData.txt";

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine("Pixel Data:");
                        writer.WriteLine(bitmap.Format.BitsPerPixel);
                        

                        for (int i = 0; i < pixelData.Length; i++)
                        {

                            writer.Write(pixelData[i] + " ");

                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading the image: {ex.Message}");
                }
            }

        }
        private void cButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void asmButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private BitmapImage convert_to_grayscale(BitmapImage bitmap)
        {
            FormatConvertedBitmap newFormatedBitmapSource = new FormatConvertedBitmap();
            newFormatedBitmapSource.BeginInit();
            newFormatedBitmapSource.Source = bitmap;
            newFormatedBitmapSource.DestinationFormat = PixelFormats.Gray8;
            newFormatedBitmapSource.EndInit();



            BitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(newFormatedBitmapSource));
            encoder.Save(memoryStream);

            BitmapImage grayscaleBitmap = new BitmapImage();
            grayscaleBitmap.BeginInit();
            grayscaleBitmap.StreamSource = memoryStream;
            grayscaleBitmap.EndInit();
            grayscaleBitmap.CacheOption = BitmapCacheOption.OnLoad;

            return grayscaleBitmap;
        }
    }
}
