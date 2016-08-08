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
        List<AccelXYZ> listTopBot = null;
        List<AccelXYZ> listTopBotProc = null;
        List<int> listReport = null;
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
            double minFirst = listAllSorted.First();
            double min = 0;
            if (minFirst<0)
            {
                min = (minFirst*-1); //минимальное минусовое знач.(уже переведенное в плюсовое) для того чтобы знать на сколько поднимать каждое значени
            }
            else
            {
                min = minFirst;
            }
            textBoxTop.Text = "";
            textBox.Text = min.ToString();
            double max = (listAllSorted.Last() + min);//максимальная вершина после поднятия ТОЛЬКо её значения. Поднятие всех значений происходит ниже.
            foreach (AccelXYZ item in listTopBot) // "поднятие" всех значений в плюсовое состояние
            {
                item.X = item.X + min;
                //textBoxTop.Text += item.X;
                ////textBox.Text += item.Y;
                ////textBox.Text += item.Z;
                //textBoxTop.Text += "\n";
            }
            double OneProc = max / 100;
            listTopBotProc = new List<AccelXYZ>();
            foreach (AccelXYZ item in listTopBot)
            {
                AccelXYZ xyz = new AccelXYZ() { X = item.X/OneProc};//пересохранение в такую же коллекцию только уже в процентах ПОКА ТОЛЬКО ДЛЯ Х
                listTopBotProc.Add(xyz);
            }
            int pyat = 0;
            int dvadzat = 0;
            int sorok = 0;
            int sestdesyat = 0;
            int vosemdesyat = 0;
            int devanostoPyat = 0;
            int sto = 0;
            foreach (AccelXYZ item in listTopBotProc)//перезапись текстбокса но уже процентами
            {
                textBoxTop.Text += item.X;
                //textBox.Text += item.Y;
                //textBox.Text += item.Z;
                textBoxTop.Text += "\n";
                if(item.X <= 5)
                {
                    pyat++;
                }
                if ((item.X > 5) && (item.X <= 20))
                {
                    dvadzat++;
                }
                if ((item.X > 20) && (item.X <= 40))
                {
                    sorok++;
                }
                if ((item.X > 40) && (item.X <= 60))
                {
                    sestdesyat++;
                }
                if ((item.X > 60) && (item.X <= 80))
                {
                    vosemdesyat++;
                }
                if ((item.X > 80) && (item.X <= 95))
                {
                    devanostoPyat++;
                }
                if (item.X > 95)
                {
                    sto++;
                }
            }
            listReport = new List<int>() { listTopBotProc.Count, pyat, dvadzat, sorok, sestdesyat, vosemdesyat, devanostoPyat, sto };
            string report = string.Format("All {0}| <=5 {1}|5-20 {2}|20-40 {3}|40-60 {4}|60-80 {5}|80-95 {6}|>95 {7}", listReport[0], listReport[1], listReport[2], listReport[3], listReport[4], listReport[5], listReport[6], listReport[7]);
            if (textBoxFirst.Text == "0")
            {
                textBoxFirst.Text = report;
            }
            textBoxFirstNew.Text = report;
        }

        private void buttonToList_Click(object sender, RoutedEventArgs e)
        {
            textBoxFirst.Text = "0";
        }
    }
}
