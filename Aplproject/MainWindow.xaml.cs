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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Security.Policy;

unsafe public class AsmProxy
{
    [DllImport("Asm.dll")]
    private static unsafe extern int ProcAsm2(int* a, int pos);

    public int executeAsmAddTwoDoubles(int* a, int pos)
    {
        return ProcAsm2(a, pos);
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
        private int[] redData = new int[] {};
        private int[] greenData = new int[] {};
        private int[] blueData = new int[] {};
        private int runsNumber = 1;
        private int threshold = 150;
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
                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(path);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    displayImage.Source = bitmapImage;
                    binarizedImage.Source = null;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading the image: {ex.Message}");
                }
            }

        }

        private double[] getCppResults(int size)
        {
            CppProxy proxy = new CppProxy();
            Stopwatch sw2 = new Stopwatch();
            double[] time_of_execution_entire_func = new double[runsNumber];
            for (int i = 0; i < runsNumber; i++)
            {
                IntPtr binarized_array;

                sw2.Start();
                binarized_array = proxy.execute(pixelData, size, threshold);
                sw2.Stop();
                TimeSpan elapsed = sw2.Elapsed;
                time_of_execution_entire_func[i] = elapsed.TotalMilliseconds;
                sw2.Reset();
                Marshal.FreeHGlobal(binarized_array);
            }
            return time_of_execution_entire_func; 
        }

        //private async void executeFunctions()
        //{
            
        //    if(asmFunctionCheckbox.IsChecked == true || cppFunctionCheckbox.IsChecked== true)
        //    {

        //    }
        //}

        // Trzeba sprawdzic najpierw czy image jest empty.
        unsafe private void runButton_Click(object sender, RoutedEventArgs e)
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


                    // Dodatkowy kod dla osobnych tablic

                }
            }

            CppProxy proxy = new CppProxy();
            int size = (int)(width * height * 3);
            IntPtr binarized_array = proxy.execute(pixelData, size, threshold);



            if(asmFunctionCheckbox.IsChecked == true)
            {
                int[] n1Array = { 1, 2, 3, 4, 5, 6 };
                unsafe
                {
                    fixed (int* aAddress = &n1Array[0])
                    {
                        AsmProxy asmm = new AsmProxy();
                        int c = asmm.executeAsmAddTwoDoubles(aAddress, 4);
                    }
                }
            }


            // Można dorobic jakis graph
            double[] time_of_execution_entire_func = new double[runsNumber];

            if(cppFunctionCheckbox.IsChecked == true)
            {
                Stopwatch sw2 = new Stopwatch();

                for (int i = 0; i < runsNumber; i++)
                {
                    sw2.Start();
                    binarized_array = proxy.execute(pixelData, size, threshold);
                    sw2.Stop();
                    TimeSpan elapsed = sw2.Elapsed;
                    time_of_execution_entire_func[i] = elapsed.TotalMilliseconds;
                    sw2.Reset();
                }

                double sum = 0;
                double average = 0;
                double best_time = time_of_execution_entire_func[1];
                double worst_time = time_of_execution_entire_func[1];
                for(int i = 1; i<runsNumber;i++)
                {
                    sum += time_of_execution_entire_func[i];
                    if (time_of_execution_entire_func[i] < best_time)
                    {
                        best_time = time_of_execution_entire_func[i];
                    }
                    if (time_of_execution_entire_func[i] > worst_time)
                    {
                        worst_time = time_of_execution_entire_func[i];
                    }
                }
                average = sum / (runsNumber - 1);

                string statistics = $"C++ Function:\nAverage = {average}\nBest time = {best_time}\nWorst time = {worst_time}";
                StatisticsTextblock.Text = statistics; 
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

            binarizationImage = new BitmapImage();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;
                binarizationImage.BeginInit();
                binarizationImage.StreamSource = memoryStream;
                binarizationImage.CacheOption = BitmapCacheOption.OnLoad;
                binarizationImage.EndInit();
            }
                binarizedImage.Source = binarizationImage;
        }

        private void RunsTextBoxinput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }
        private void RunBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(RunTextBox.Text))
            {
                RunTextBox.Text = "1";
            }
            int val = int.Parse(RunTextBox.Text);
            if(val < 1)
            {
                MessageBox.Show("Value must be above 1");
                RunTextBox.Text = "1";
            }
            else
            {
                runsNumber = val;
            }
        }
        private void ThresholdTextbox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void ThresholdBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ThresholdTextBox.Text))
            {
                ThresholdTextBox.Text = "0";
            }
            int val = int.Parse(ThresholdTextBox.Text);
            if (val > 255)
            {
                MessageBox.Show("Value must be between 0 and 255");
                ThresholdTextBox.Text = "255";
            }
            else
            {
                threshold = val;
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
