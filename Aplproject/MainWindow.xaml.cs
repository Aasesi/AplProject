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
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

unsafe public class AsmProxy
{
    [DllImport("Asm.dll")]
    private static extern double asmAddTwoDoubles(double a, double b);

    public double executeAsmAddTwoDoubles(double a, double b)
    {
        return asmAddTwoDoubles(a, b);
    }
}

unsafe public class CppProxy
{
    [DllImport("Cppdll.dll")]
    private static extern IntPtr Add(int[] arr, int size, int threshold);

    public IntPtr execute(int[] arr, int size, int threshold)
    {
        return Add(arr, size, threshold);
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
        private BitmapImage binarizationImage;
        private int[] pixelData = new int[] {};
        private int runsNumber = 1;
        private int threshold = 0;
        private string path = "";
        public MainWindow()
        {
            InitializeComponent();
            bitmapImage = new BitmapImage();
            binarizationImage = new BitmapImage();
        }
        unsafe private void loadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif; *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    path = openFileDialog.FileName;
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(path);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    displayImage.Source = bitmapImage;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading the image: {ex.Message}");
                }
            }

        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {
            Bitmap pixelbitmap = new Bitmap(path);

            int width = pixelbitmap.Width;
            int height = pixelbitmap.Height;

            pixelData = new int[width * height * 3];

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color pixelColor = pixelbitmap.GetPixel(x, y);

                    int red = pixelColor.R;
                    int green = pixelColor.G;
                    int blue = pixelColor.B;

                    red = Math.Max(0, Math.Min(255, red));
                    green = Math.Max(0, Math.Min(255, green));
                    blue = Math.Max(0, Math.Min(255, blue));

                    pixelData[index++] = red;
                    pixelData[index++] = green;
                    pixelData[index++] = blue;
                }
            }

            CppProxy proxy = new CppProxy();
            int size = (int)(width * height * 3);
            IntPtr binarized_array = proxy.execute(pixelData, size, 80);


            if(asmFunctionCheckbox.IsChecked == true)
            {

            }

            if(cppFunctionCheckbox.IsChecked == true)
            {
                for (int i = 0; i < runsNumber; i++)
                {
                    binarized_array = proxy.execute(pixelData, size, 80);
                }
            }
            Marshal.Copy(binarized_array, pixelData, 0, size);
            Bitmap bmp = new Bitmap(width, height);

            int ind = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    System.Drawing.Color pixelColor = System.Drawing.Color.FromArgb(pixelData[ind], pixelData[ind + 1], pixelData[ind + 2]);
                    bmp.SetPixel(x, y, pixelColor);
                    ind += 3;
                }
            }
            bmp.Save("output_image.png", System.Drawing.Imaging.ImageFormat.Png);
            string imagePath = System.IO.Directory.GetCurrentDirectory();
            Marshal.FreeHGlobal(binarized_array);


            binarizationImage.BeginInit();
            binarizationImage.UriSource = new Uri(imagePath + "/output_image.png");
            binarizationImage.CacheOption = BitmapCacheOption.OnLoad;
            binarizationImage.EndInit();

            binarizedImage.Source = binarizationImage;
        }

        private void RunsTextBoxinput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void RunTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RunTextBox.Text, out int value))
            {
                if (value < 0 || value > 255)
                {
                    MessageBox.Show("Value must be between 0 and 255");
                    RunTextBox.Text = "0";
                }
                else
                {
                    runsNumber = value;
                }
            }
            else if (RunTextBox.Text != "-")
            {
                MessageBox.Show("Please enter a valid numeric value");
                RunTextBox.Text = "0";
            }
        }

        private void ThresholdTextbox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void ThresholdChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RunTextBox.Text, out int value))
            {
                if (value < 1)
                {
                    MessageBox.Show("Value must be above 1");
                    RunTextBox.Text = "1";
                }
                else
                {
                    threshold = value;
                }
            }
            else if (RunTextBox.Text != "-")
            {
                MessageBox.Show("Please enter a valid numeric value");
                RunTextBox.Text = "1";
            }
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
