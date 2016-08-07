﻿using System;
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
        List<AccelXYZ> listTopBot = null;
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
            listTopBot = new List<AccelXYZ>();
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
            foreach (AccelXYZ item in listXYZ) // вывод в текстбокс всего
            {
                textBox.Text += item.X;
                //textBox.Text += item.Y;
                //textBox.Text += item.Z;
                textBox.Text += "\n";
            }
            listTopBot.Add(arrayXYZ[0]);//всегда добавляем первый и последний
            for (int i = 0; i < arrayXYZ.Length; i++)
            {
                if(((i+1) < arrayXYZ.Length) && ((i - 1) >= 0)) // проверка что мы в пределах массива. Чтобы работать ТОЛЬКо от 0(включительно) и до -1 от конца.
                    //например если массив из 10 тоесть от 0 до 9 то мы попадем в иф ТОЛЬКо начиная с первого (индекс 1) сдвига и до предпоследнего (длинна в ел. -2)
                {
                    if ( 
                        ((arrayXYZ[i-1].X <= arrayXYZ[i].X) &&(arrayXYZ[i].X >= arrayXYZ[i+1].X)) // если мы нашли самую высокую вершину
                        || ((arrayXYZ[i-1].X >= arrayXYZ[i].X) && (arrayXYZ[i].X <= arrayXYZ[i+1].X))//если мы нашли самую низкую "вершину"
                        )
                    {
                        listTopBot.Add(arrayXYZ[i]);
                    }
                }
            }
            listTopBot.Add(arrayXYZ[arrayXYZ.Length - 1]);//всегда добавляем первый и последний
            foreach (AccelXYZ item in listTopBot) // вывод в текстбокс только вершин - низин
            {
                textBoxTop.Text += item.X;
                //textBox.Text += item.Y;
                //textBox.Text += item.Z;
                textBoxTop.Text += "\n";
            }
            listXYZ = null; //больше ВСЕ значения не нужны
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            var listAllSorted = from i in listTopBot orderby i.X ascending select i.X;
            double min = (listAllSorted.First()*-1); //минимальное минусовое знач.(уже переведенное в плюсовое) для того чтобы знать на сколько поднимать каждое значени
            double max = (listAllSorted.Last() + min);//максимальная вершина после поднятия ТОЛЬКо её значения. Поднятие всех значений происходит ниже.
            textBox.Text = min.ToString();
            textBoxTop.Text = "";
            foreach (AccelXYZ item in listTopBot) // "поднятие" всех значений в плюсовое состояние
            {
                item.X = item.X + min;
                //textBoxTop.Text += item.X;
                ////textBox.Text += item.Y;
                ////textBox.Text += item.Z;
                //textBoxTop.Text += "\n";
            }
            double OneProc = max / 100;
            List<AccelXYZ> listTopBotProc = new List<AccelXYZ>();
            foreach (AccelXYZ item in listTopBot)
            {
                AccelXYZ xyz = new AccelXYZ() { X = item.X/OneProc};//пересохранение в такую же коллекцию только уже в процентах ПОКА ТОЛЬКО ДЛЯ Х
                listTopBotProc.Add(xyz);
            }
            foreach (AccelXYZ item in listTopBotProc)
            {
                textBoxTop.Text += item.X;
                //textBox.Text += item.Y;
                //textBox.Text += item.Z;
                textBoxTop.Text += "\n";
            }

        }
    }
}
