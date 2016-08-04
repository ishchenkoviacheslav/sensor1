using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Sensors;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace sensor1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Accelerometer _accelerometer;
        List<AccelXYZ> listXYZ = null;
        List<AccelXYZ> listTop = null;
        public MainPage()
        {
            this.InitializeComponent();
            listXYZ = new List<AccelXYZ>();
            _accelerometer = Accelerometer.GetDefault();
            
            if(_accelerometer == null)
            {
                textBox.Text = "kein accelerometer";
            }
        }

        private async void _accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                listXYZ.Add(new AccelXYZ() { X = args.Reading.AccelerationX, Y = args.Reading.AccelerationY, Z = args.Reading.AccelerationZ });
                //textBox.Text = string.Format("X: {0,5:0.00} Y: {1,5:0.00} Z: {2,5:0.00}", args.Reading.AccelerationX, args.Reading.AccelerationY, args.Reading.AccelerationZ);
            });
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_accelerometer != null)
            {
                _accelerometer.ReadingChanged += _accelerometer_ReadingChanged;
            }
            textBox.Text = "";
            textBoxTop.Text = "";
            listXYZ = new List<AccelXYZ>();
            listTop = new List<AccelXYZ>();
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_accelerometer != null)
            {
                _accelerometer.ReadingChanged -= _accelerometer_ReadingChanged;
            }
        }

        private void ShowBtn_Click(object sender, RoutedEventArgs e)
        {
            AccelXYZ[] arrayXYZ = listXYZ.ToArray();
            foreach (AccelXYZ item in listXYZ)
            {
                textBox.Text += item.X;
                //textBox.Text += item.Y;
                //textBox.Text += item.Z;
                textBox.Text += "\n";
            }
            if(arrayXYZ[0].X > arrayXYZ[1].X)//добавление первого если он больше чем второй
            {
                listTop.Add(arrayXYZ[0]);
            }
            for (int i = 0; i < arrayXYZ.Length; i++)
            {
                if(((i+1) < arrayXYZ.Length) && ((i - 1) >= 0))
                {
                    if ((arrayXYZ[i-1].X < arrayXYZ[i].X) &&(arrayXYZ[i].X > arrayXYZ[i+1].X))
                    {
                        listTop.Add(arrayXYZ[i]);
                    }
                }
            }
            if (arrayXYZ[arrayXYZ.Length-1].X > arrayXYZ[arrayXYZ.Length - 2].X) // добавление последнего если он больше чем предпоследний
            {
                listTop.Add(arrayXYZ[arrayXYZ.Length - 1]);
            }
            foreach (AccelXYZ item in listTop)
            {
                textBoxTop.Text += item.X;
                //textBox.Text += item.Y;
                //textBox.Text += item.Z;
                textBoxTop.Text += "\n";
            }
        }
    }
}
