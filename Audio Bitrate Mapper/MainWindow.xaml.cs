using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;


namespace Audio_Bitrate_Mapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("Autocorrelation.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr ReadAndProcessSamples(string filePath,
                                                          int samplingFrequency,
                                                          int samplesCount,
                                                          float thresholdValue);

        List<float> inputValues = new List<float>();
        List<int> outputValues = new List<int>();
        IntPtr autocorrResults;
        string inputFilePath;
        string inputFilePathAutocorr;

        string changeTest = "Only for BitBucket pushing purposes";

        public MainWindow()
        {
            InitializeComponent();
        }


        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            textBoxInputFilePath.Text = FileBrowserDialog();
            inputFilePath = textBoxInputFilePath.Text;
        }

        private void buttonMap_Click(object sender, RoutedEventArgs e)
        {
            ReadInputFile();
            MapBitDepth();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text file (*.txt)|*.txt";

            if (sfd.ShowDialog() == true)
                SaveToFile(sfd.FileName);
        }

        private string FileBrowserDialog()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.InitialDirectory = @"C:\Users\Andrei\Desktop\Licenta stuff\exported_wavs";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Boo! I'm a ghost. | *.txt";

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
                return dialog.FileName;

            else return null;
        }

        private void ReadInputFile()
        {
            inputValues = new List<float>();

            using (StreamReader sr = new StreamReader(inputFilePath))
            {
                String line;

                while ((line = sr.ReadLine()) != null)
                    inputValues.Add(float.Parse(line, CultureInfo.InvariantCulture.NumberFormat));
            }
        }

        private void SaveToFile(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {

                for (int i = 0; i < outputValues.Count; i++)
                    sw.WriteLine(outputValues[i]);

                //else if (i % 20 == 0)
                //   sw.Write('\n');

                //else
                //     sw.Write(outputValues[i] + ", ");


                /*
                    sw.Write("unsigned char rawData[" + outputValues.Count + "] = {");

                    for (int i = 0; i < outputValues.Count; i++)
                        if (i == outputValues.Count - 1)
                            sw.Write(outputValues[i] + "};");

                        else if (i % 20 == 0)
                            sw.Write('\n');

                        else
                            sw.Write(outputValues[i] + ", ");
                            */

                /*
                sw.Write("unsigned char rawData[" + outputValues.Count + "] = {");

                for (int i = 0; i < outputValues.Count; i++)
                    if (i == outputValues.Count - 1)
                        sw.Write("0x" + outputValues[i].ToString("X") + "};");

                    else if (i % 10 == 0)
                        sw.Write("0x" + outputValues[i].ToString("X") + ", " + '\n');

                    else
                        sw.Write("0x" + outputValues[i].ToString("X") + ", ");*/
            }
        }

        private void MapBitDepth()
        {
            float inputMin = 0;
            float inputMax = 0;

            int rangeMin = int.Parse(textBoxRangeMin.Text);
            int rangeMax = int.Parse(textBoxRangeMax.Text);

            inputMin = inputValues.Min();
            inputMax = inputValues.Max();

            outputValues = new List<int>();

            for (int i = 0; i < inputValues.Count; i++)
                outputValues.Add((int)Math.Round(rangeMin + (rangeMax - rangeMin) * ((inputValues[i] - inputMin) / (inputMax - inputMin))));
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            int qSteps = 1;

            for (int i = 0; i < int.Parse(textBoxBitDepth.Text); i++)
                qSteps *= 2;

            if (checkBoxSigned.IsChecked == true)
            {
                textBoxRangeMin.Text = ((-qSteps / 2) + 1).ToString();
                textBoxRangeMax.Text = (qSteps / 2).ToString();
            }
            else
            {
                textBoxRangeMin.Text = "0";
                textBoxRangeMax.Text = (qSteps - 1).ToString();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void textBoxBitDepth_Copy_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void buttonOpenAutocorr_Click(object sender, RoutedEventArgs e)
        {
            textBoxInputFilePathAutocorr.Text = FileBrowserDialog();
            inputFilePathAutocorr = textBoxInputFilePathAutocorr.Text;
        }

        private void buttonAutocorrelate_Click(object sender, RoutedEventArgs e)
        {
            string path = textBoxInputFilePathAutocorr.Text;
            int sFreq = int.Parse(textBoxSamplingFreq.Text);
            int sCount = int.Parse(textBoxNumberOfSamples.Text);
            float tVal = float.Parse(textBoxThresholdValue.Text);


           // autocorrResults = new float [2];

            autocorrResults = ReadAndProcessSamples(path, sFreq, sCount, tVal);

            float[] res = new float[2];
            Marshal.Copy(autocorrResults, res, 0, 2);

            textBoxDetectedFreq.Text = res[0].ToString();
            textBoxDetectedPeriod.Text = res[1].ToString();
        }
    }

}
