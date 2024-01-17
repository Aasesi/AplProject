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
using System.Windows.Media.Media3D;

/******************************************************************************
* Project Title: Image Binarization
* Description  : This algorithm is designed for the process of converting pixels
*                data from an image into binary image with main goal to separate
*                foreground from background. The process involves determining whether
*                each pixel should be classified as white or black based on individual
*                RGB channel values.
*
* 
* Implementation Date: 15.01.2024
* Semester/Academic Year: Semester 5, Winter 2023/2024
* Author: Mikołaj Wilczyński, Konrad Kiełtyka, Weronika Źerańska
* 
* Version: 1.0
* 
* 
******************************************************************************/


/// <summary>
/// Struct with information for assembly function. Contains pointers to arrays and necessary variables.
/// </summary>
public unsafe struct StructToAssembler 
{
    public int* ptrA1;  
    public int* ptrA2;
    public int* ptrA3;
    public int* ptrA4;
    public int* ptrA5;
    public int len;
}

/// <summary>
/// Represents a proxy class for calling assembly code from a DLL.
/// </summary>
unsafe public class AsmProxy
{
    [DllImport("Asm.dll")]
    private static unsafe extern int AsmBinarize(StructToAssembler* structurePtr);

    /// <summary>
    /// Executes the assembly function to binarize data.
    /// </summary>
    /// <param name="structurePtr">A pointer to the structure containing data for the assembly function.</param>
    /// <returns>Returns int value</returns>
    public int executeAsmAddTwoDoubles(StructToAssembler* structurePtr)
    {
        return AsmBinarize(structurePtr);
    }

}

/// <summary>
/// Represents a proxy class for calling C++ code from a DLL.
/// </summary>
unsafe public class CppProxy
{
    [DllImport("Cppdll.dll")]
    private static extern int cppBinarize(int[] redarr, int[] greenarr, int[] bluearr, int* result, int size, int threshold);

    /// <summary>
    /// Executes the C++ function for binarizing data.
    /// </summary>
    /// <param name="redarr">Array of red channel values.</param>
    /// <param name="greenarr">Array of green channel values.</param>
    /// <param name="bluearr">Array of blue channel values.</param>
    /// <param name="result">Pointer to the array where the result will be stored.</param>
    /// <param name="size">Size of the input arrays.</param>
    /// <param name="threshold">Threshold value for binarization.</param>
    /// <returns>The result is integrer representing succesful or failed operation</returns>
    public int execute(int[] redarr, int[] greenarr, int[] bluearr, int* result, int size, int threshold)
    {
        return cppBinarize(redarr, greenarr, bluearr, result, size, threshold);
    }
}


namespace Aplproject
{
    /// <summary>
    /// Represents the main window of the application.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Shared data structure to communicate with the assembler.
        public static StructToAssembler struktura = new StructToAssembler();

        // Displayed images.
        private BitmapImage bitmapImage;
        private BitmapImage binarizationImage;

        // Necessary arrays and data 
        private int[] redData = new int[] {};
        private int[] greenData = new int[] {};
        private int[] blueData = new int[] {};
        private int[] resultData = new int[] {};
        private int runsNumber = 2;
        private int threshold = 150;

        // Size of channel arrays
        private int len = 0;

        // Path to output file
        private string path = "";

        /// <summary>
        /// Initializes a new instance of the "MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            bitmapImage = new BitmapImage();
            binarizationImage = new BitmapImage();
        }
        /// <summary>
        /// Event handler for the "Load Image" button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// 
        /// Function executes code for running asm or cpp functions in user defained number of runs 
        /// and collecting data from completion of the functions.
        /// 
        /// </summary>
        /// <param name="checkcpp"> Boolean value of cpp checkbox</param>
        /// <param name="checkasm">Boolean value of asm checkbox</param>
        private void executeFunctions(bool checkcpp, bool checkasm)
        {
            
            if(checkcpp == true || checkasm== true)
            {
                int progress_value = 0;
                string statisticsCpp = "";
                string statisticsAsm = "";
                if (checkasm == true)
                {
                    double[] time_of_execution_entire_func = new double[runsNumber];
                    Stopwatch sw = new Stopwatch();
                    int[] temp_result = new int[len];
                    unsafe
                    {
                        fixed (StructToAssembler* aAddress = &struktura)
                        {
                            int[] threshtable = {threshold, threshold, threshold, threshold};
                            fixed (int* ptr1 = redData, ptr2 = greenData, ptr3 = blueData, ptr4 = temp_result, ptr5 = threshtable)
                            {
                                struktura.ptrA1 = ptr1;
                                struktura.ptrA2 = ptr2;
                                struktura.ptrA3 = ptr3;
                                struktura.ptrA4 = ptr4;
                                struktura.ptrA5 = ptr5;
                                struktura.len = len;
                                AsmProxy asmm = new AsmProxy();
                                for(int i = 0; i< runsNumber; i++)
                                {
                                    sw.Start();
                                    int c = asmm.executeAsmAddTwoDoubles(aAddress);
                                    sw.Stop();
                                    TimeSpan elapsed = sw.Elapsed;
                                    time_of_execution_entire_func[i] = elapsed.TotalMilliseconds;
                                    sw.Reset();
                                    temp_result = new int[len];
                                    progress_value++;
                                    UpdateProgressBar(progress_value);
                                }
                                double sum = 0;
                                double average = 0;
                                double best_time = 0;
                                double worst_time = 0;
                                best_time = time_of_execution_entire_func[1];
                                worst_time = time_of_execution_entire_func[1];
                                
                                for (int i = 1; i < runsNumber; i++)
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

                                statisticsAsm = $"Asm Function:\nAverage = {average}\nBest time = {best_time}\nWorst time = {worst_time}";
                            };
                        }
                    }
                }
                if(checkcpp == true)
                {
                    double[] time_of_execution_entire_func = new double[runsNumber];
                    Stopwatch sw2 = new Stopwatch();
                    CppProxy proxy = new CppProxy();
                    int[] temp_result = new int[len];
                    unsafe
                    {
                        fixed (int* ptr = temp_result)
                        {
                            for (int i = 0; i < runsNumber; i++)
                            {
                                sw2.Start();
                                int c = proxy.execute(redData, greenData, blueData, ptr, len, threshold);
                                sw2.Stop();
                                TimeSpan elapsed = sw2.Elapsed;
                                time_of_execution_entire_func[i] = elapsed.TotalMilliseconds;
                                progress_value++;
                                UpdateProgressBar(progress_value);
                                sw2.Reset();
                            }
                            double sum = 0;
                            double average = 0;
                            double best_time = time_of_execution_entire_func[1];
                            double worst_time = time_of_execution_entire_func[1];
                            for (int i = 1; i < runsNumber; i++)
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

                            statisticsCpp = $"C++Function:\nAverage = {average}\nBest time = {best_time}\nWorst time = {worst_time}";
                        }
                    }
                }
                UpdateStatisticsText(statisticsAsm, statisticsCpp);
            }
        }

        /// <summary>
        /// 
        /// Loads binarized image.
        /// 
        /// </summary>
        /// <param name="im_width"> width of the image</param>
        /// <param name="im_height"> height of the image</param>
        unsafe private void show_binarized_image(int im_width, int im_height)
        {
            int[] temp_result = new int[len];
            CppProxy proxy = new CppProxy();
            Bitmap bmp = new Bitmap(im_width, im_height);
            fixed (int* ptr = temp_result)
            {
                int c = proxy.execute(redData, greenData, blueData, ptr, len, threshold);
            }
            int ind = 0;
            for (int y = 0; y < im_height; y++)
            {
                for (int x = 0; x < im_width; x++)
                {
                    System.Drawing.Color pixelColor = System.Drawing.Color.FromArgb(temp_result[ind], temp_result[ind], temp_result[ind]);
                    bmp.SetPixel(x, y, pixelColor);
                    ind += 1;
                }
            }
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

        /// <summary>
        /// Updates the value of the progress bar on the UI thread.
        /// </summary>
        /// <param name="value">The new value for the progress bar.</param>
        private void UpdateProgressBar(int value)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = value;
            });
        }

        /// <summary>
        /// Updates the statistics text blocks on the UI thread.
        /// </summary>
        /// <param name="asm_stats">Statistics string for the assembly code.</param>
        /// <param name="cpp_stats">Statistics string for the C++ code.</param>
        private void UpdateStatisticsText(string asm_stats, string cpp_stats)
        {
            Dispatcher.Invoke(() =>
            {
                CppStatisticsTextblock.Text = cpp_stats;
                AsmStatisticsTextblock.Text = asm_stats;
            });
        }

        /// <summary>
        /// Event handler for the "Run" button click.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void runButton_Click(object sender, RoutedEventArgs e)
        {

            if(bitmapImage.UriSource == null && bitmapImage.StreamSource == null)
            {
                return;
            }
            Run.IsEnabled = false;
            progressBar.Value = 0;
            Bitmap pixelbitmap = new Bitmap(path);

            int width = pixelbitmap.Width;
            int height = pixelbitmap.Height;

            redData = new int[width * height];
            greenData = new int[width * height];
            blueData = new int[width * height];
            len = width * height;

            int index2 = 0;
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

                    redData[index2] = red;
                    greenData[index2] = green;
                    blueData[index2] = blue;

                    index2++;

                }
            }
            int progress_max = 0;
            bool cpp_checked = cppFunctionCheckbox.IsChecked ?? false;
            bool asm_checked = asmFunctionCheckbox.IsChecked ?? false;
            if (cpp_checked == true)
            {
                progress_max += len;
            }
            if(asm_checked == true)
            {
                progress_max += len;
            }
            progressBar.Maximum = runsNumber;
            await Task.Run(() => executeFunctions(cpp_checked, asm_checked));
            show_binarized_image(width, height);
            Run.IsEnabled = true;

        }

        /// <summary>
        /// Event handler for handling text input in the "Runs" TextBox.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RunsTextBoxinput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Use a regular expression to allow only numeric input.
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        /// <summary>
        /// Event handler for handling the lost focus event of the "Runs" TextBox.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Event handler for handling text input in the "Threshold" TextBox.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ThresholdTextbox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Use a regular expression to allow only numeric input.
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Event handler for handling the lost focus event of the "Threshold" TextBox.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
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
    }
}
