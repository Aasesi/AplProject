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
using System.Drawing;

unsafe public class AsmProxy
{
    [DllImport("Asm.dll")]
    private static extern double asmAddTwoDoubles(double a, double b);

    public double executeAsmAddTwoDoubles(double a, double b)
    {
        return asmAddTwoDoubles(a, b);
    }
}

public class CppProxy
{
    [DllImport("Cppdll.dll")]
    private static extern int Add(int a, int b);

    public int execute(int a, int b)
    {
        return Add(a, b);
    }
}


namespace Aplproject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage bitmapImage;
        private uint[] pixelData = new uint[] {};
        public MainWindow()
        {
            InitializeComponent();
            bitmapImage = new BitmapImage();
        }
        private void loadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string path = openFileDialog.FileName;
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(path);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    displayImage.Source = bitmapImage;

                    Bitmap pixelbitmap = new Bitmap(path);

                    int width = pixelbitmap.Width;
                    int height = pixelbitmap.Height;

                    pixelData = new uint[width * height * 3];


                    int index = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {

                            System.Drawing.Color pixelColor = pixelbitmap.GetPixel(x, y);



                            uint red = pixelColor.R;
                            uint green = pixelColor.G;
                            uint blue = pixelColor.B;

                            red = Math.Max(0, Math.Min(255, red));
                            green = Math.Max(0, Math.Min(255, green));
                            blue = Math.Max(0, Math.Min(255, blue));

                            // Alternative
                            //uint rgbValue = red * 1000000 + green * 1000 + blue;
                            //pixelData[index++] = rgbValue;

                            pixelData[index++] = red;
                            pixelData[index++] = green;
                            pixelData[index++] = blue;
                        }
                    }
                    string outputFilePath = "output_rgb_values.txt"; 

                    try
                    {
                        using (StreamWriter writer = new StreamWriter(outputFilePath))
                        {
                            foreach (int rgbValue in pixelData)
                            {
                                writer.WriteLine(rgbValue);
                            }
                            CppProxy p = new CppProxy();
                            int value = p.execute(1, 1111);
                            writer.WriteLine(value);
                        }
                        Console.WriteLine("RGB values written to file: " + outputFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error writing to file: " + ex.Message);
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
