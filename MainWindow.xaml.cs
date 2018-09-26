using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace GameOfLifeWpf
{



    //nachbarn - sasiad, felder - pola, anzahlZelenBreit - szerokosc , Hoch - wysokosc
    //periodische randbedingungen!!!!!
    //felder -pola


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        public class MyPoint
        {
            public Point point;
            public Brush brush;
            public MyPoint(Point p, Brush b)
            {
                point = p;
                brush = b;
            }
        }

        public class MyRectangle 
        {
            int id;
            public Rectangle rectangle;

            public MyRectangle(int id, Rectangle rect)
            {
                this.id = id;
                rectangle = rect;

            }
          
        }
            
       
        


        public Random rand = new Random();
        List<Brush> usedColors = new List<Brush>();
        List<Brush> idUsedColors = new List<Brush>();
        int cellWidth;
        int cellHeight;
        double gap = 0.0;
        double timeSpan = 0.1;

        List<Point> tempPoints = new List<Point>();
        int color = 0;
        DispatcherTimer timer = new DispatcherTimer();


        Rectangle[,] pola;
        Brush[,] tempBrushes;
        MyRectangle[,] polaWithId;

        List<MyPoint> polaToFill = new List<MyPoint>();

        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(timeSpan);
            timer.Tick += Timer_Tick;


        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            //if (PeriodicRadio.IsChecked == true)
            //    PlayPeriodic();
            //else if (NotPeriodicRadio.IsChecked == true)
            //    PlayNonPeriodic();
            if (PeriodicRadio.IsChecked == true)
                GrowPeriodic();
            else if (NotPeriodicRadio.IsChecked == true)
                GrowNonPeriodic();





        }


        private void InitializeBoard()
        {
            usedColors.Add(Brushes.Cyan);

            board.Children.Clear();

            
            if (int.TryParse(IdCount.Text, out int idCount))
            {
                for (int i = 0; i < idCount; i++)
                    idUsedColors.Add(GetRandomBrush());
            }


            if (int.TryParse(WidthTextBox.Text, out int inputWidth))
            {
                cellWidth = inputWidth;
                cellHeight = inputWidth;
                pola = new Rectangle[cellWidth, cellHeight];
                tempBrushes = new Brush[cellWidth, cellHeight];


                Rectangle r;
                int tempId = 0;

                Brush result;
                Type brushesType = typeof(Brushes);
                PropertyInfo[] properties = brushesType.GetProperties();




                double rectangleWidth = board.ActualWidth / cellWidth - gap;
                for (int i = 0; i < cellHeight; i++)
                    for (int j = 0; j < cellWidth; j++)
                    {
                        r= new Rectangle();
                        r.Width = rectangleWidth;
                        r.Height = rectangleWidth;
                        //r.Fill = Brushes.Cyan;

                        tempId = rand.Next(idCount);
                        r.Fill = (Brush)properties[tempId].GetValue(null, null);
                        

                        board.Children.Add(r);
                        Canvas.SetLeft(r, j * board.ActualWidth / cellWidth);
                        Canvas.SetTop(r, i * board.ActualHeight / cellHeight);
                        r.MouseDown += R_MouseDown;
                        pola[i, j] = r;
                       
                    }
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = true;

            }
            else
                MessageBox.Show("Wrong data!");




        }


        private void R_MouseDown(object sender, MouseButtonEventArgs e)
        {

            //((Rectangle)sender).Fill = 
            //    (((Rectangle)sender).Fill == Brushes.Cyan) ? Brushes.Red : Brushes.Cyan;

           
                    ((Rectangle)sender).Fill =
                     (((Rectangle)sender).Fill == Brushes.Cyan) ? GetRandomBrush() : Brushes.Cyan;
 

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitializeBoard();
            InitializeButton.IsEnabled = false;
            ClearButton.IsEnabled = true;
            GrowButton.IsEnabled = true;

        }

        public void PlayNonPeriodic()
        {


            int[,] liczbaSasiadow = new int[cellHeight, cellWidth];

            for (int i = 1; i < cellHeight - 1; i++)
                for (int j = 1; j < cellWidth - 1; j++)
                {
                    int sasiedzi = 0;

                    int iGora = i - 1;
                    if (iGora < 0)
                    {
                        iGora = cellHeight - 1;
                    }
                    int iDol = i + 1;
                    if (iDol >= cellHeight)
                    {
                        iDol = 0;
                    }

                    int jLewo = j - 1;
                    if (jLewo < 0)
                    {
                        jLewo = cellWidth - 1;
                    }
                    int jPrawo = j + 1;
                    if (jPrawo >= cellWidth)
                    {
                        jPrawo = 0;
                    }


                    if (pola[iGora, jLewo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iGora, j].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iGora, jPrawo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[i, jLewo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[i, jPrawo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iDol, jLewo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iDol, j].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iDol, jPrawo].Fill == Brushes.Red)
                        sasiedzi++;

                    liczbaSasiadow[i, j] = sasiedzi;


                }

            for (int i = 0; i < cellHeight; i++)
                for (int j = 0; j < cellWidth; j++)
                {
                    if (liczbaSasiadow[i, j] < 2 || liczbaSasiadow[i, j] > 3)
                        pola[i, j].Fill = Brushes.Cyan;
                    else if (liczbaSasiadow[i, j] == 3)
                        pola[i, j].Fill = Brushes.Red;
                }
        }

        private void PlayPeriodic()
        {
            int[,] liczbaSasiadow = new int[cellHeight, cellWidth];

            for (int i = 0; i < cellHeight; i++)
                for (int j = 0; j < cellWidth; j++)
                {
                    int sasiedzi = 0;

                    int iGora = i - 1;
                    if (iGora < 0)
                    {
                        iGora = cellHeight - 1;
                    }
                    int iDol = i + 1;
                    if (iDol >= cellHeight)
                    {
                        iDol = 0;
                    }

                    int jLewo = j - 1;
                    if (jLewo < 0)
                    {
                        jLewo = cellWidth - 1;
                    }
                    int jPrawo = j + 1;
                    if (jPrawo >= cellWidth)
                    {
                        jPrawo = 0;
                    }


                    if (pola[iGora, jLewo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iGora, j].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iGora, jPrawo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[i, jLewo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[i, jPrawo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iDol, jLewo].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iDol, j].Fill == Brushes.Red)
                        sasiedzi++;
                    if (pola[iDol, jPrawo].Fill == Brushes.Red)
                        sasiedzi++;

                    liczbaSasiadow[i, j] = sasiedzi;


                }

            for (int i = 0; i < cellHeight; i++)
                for (int j = 0; j < cellWidth; j++)
                {
                    if (liczbaSasiadow[i, j] < 2 || liczbaSasiadow[i, j] > 3)
                        pola[i, j].Fill = Brushes.Cyan;
                    else if (liczbaSasiadow[i, j] == 3)
                        pola[i, j].Fill = Brushes.Red;
                }
        }

        private void Play()
        {

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            PeriodicRadio.IsEnabled = true;
            NotPeriodicRadio.IsEnabled = true;
            ClearButton.IsEnabled = true;
        }

        private void StartButton_Click_1(object sender, RoutedEventArgs e)
        {
            timer.Start();
            PeriodicRadio.IsEnabled = false;
            NotPeriodicRadio.IsEnabled = false;
            ClearButton.IsEnabled = false;

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < cellHeight; i++)
                for (int j = 0; j < cellWidth; j++)
                {
                    pola[i, j].Fill = Brushes.Cyan;
                }
            tempPoints.Clear();
        }


        private void GrowButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            // GrowPeriodic();
            // GrowNonPeriodic();
        }

        public void GrowPeriodic()
        {

            if(TemporaryRandomGrainRadio.IsChecked==true)
            {
                int iterator = 0;
                do
                {
                   int x = rand.Next(cellHeight);
                   int y = rand.Next(cellHeight);

                    if(pola[x,y].Fill==Brushes.Cyan)
                    {
                        pola[x, y].Fill = GetRandomBrush();
                        break;
                    }
                    if (iterator++ >= cellHeight * cellHeight)
                    {
                        break;
                    }
                } while (true);
            }

            Brush tempBrush = null;

            for (int i = 0; i < cellHeight; i++)
                for (int j = 0; j < cellWidth; j++)
                {




                    if (pola[i, j].Fill != Brushes.Cyan)
                    {

                        tempBrush = pola[i, j].Fill;

                        int iGora = i - 1;
                        if (iGora < 0)
                        {
                            iGora = cellHeight - 1;
                        }
                        int iDol = i + 1;
                        if (iDol >= cellHeight)
                        {
                            iDol = 0;
                        }

                        int jLewo = j - 1;
                        if (jLewo < 0)
                        {
                            jLewo = cellWidth - 1;
                        }
                        int jPrawo = j + 1;
                        if (jPrawo >= cellWidth)
                        {
                            jPrawo = 0;
                        }


                        if (VonNeumanRadio.IsChecked == true)
                        {

                            if (pola[iGora, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                        }
                        else if (MooreRadio.IsChecked == true)
                        {
                            if (pola[iGora, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            if (pola[i, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, j), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }



                        else if (RightHexRadio.IsChecked == true)
                        {
                            if (pola[iGora, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }

                        else if (LeftHexRadio.IsChecked == true)
                        {
                            if (pola[iGora, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                        }
                        else if (HexRandRadio.IsChecked == true)
                        {

                            int gap1 = rand.Next(8);
                            int gap2;
                            do
                            {
                                gap2 = rand.Next(8);
                            } while (gap2 == gap1);



                            if (pola[iGora, j].Fill == Brushes.Cyan && gap1 != 0 && gap2 != 0)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan && gap1 != 1 && gap2 != 1)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan && gap1 != 2 && gap2 != 2)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan && gap1 != 3 && gap2 != 3)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            //if (pola[i, j].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                            //    polaToFill.Add(new MyPoint(new Point(i, j), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan && gap1 != 5 && gap2 != 5)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jLewo].Fill == Brushes.Cyan && gap1 != 6 && gap2 != 6)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan && gap1 != 7 && gap2 != 7)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }

                        else if (HexRandRadio.IsChecked == true)
                        {

                            int gap1 = rand.Next(8);
                            int gap2;
                            do
                            {
                                gap2 = rand.Next(8);
                            } while (gap2 == gap1);



                            if (pola[iGora, j].Fill == Brushes.Cyan && gap1 != 0 && gap2 != 0)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan && gap1 != 1 && gap2 != 1)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan && gap1 != 2 && gap2 != 2)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan && gap1 != 3 && gap2 != 3)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            //if (pola[i, j].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                            //    polaToFill.Add(new MyPoint(new Point(i, j), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan && gap1 != 5 && gap2 != 5)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jLewo].Fill == Brushes.Cyan && gap1 != 6 && gap2 != 6)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan && gap1 != 7 && gap2 != 7)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }

                        else if (PentRandRadio.IsChecked == true)
                        {

                            int gap1 = rand.Next(8);
                            int gap2, gap3;

                            do
                            {
                                gap2 = rand.Next(8);
                            } while (gap2 == gap1);

                            do
                            {
                                gap3 = rand.Next(8);
                            } while (gap3 == gap1 || gap3 == gap2);



                            if (pola[iGora, j].Fill == Brushes.Cyan && gap1 != 0 && gap2 != 0 && gap3 != 0)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan && gap1 != 1 && gap2 != 1 && gap3 != 1)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan && gap1 != 2 && gap2 != 2 && gap3 != 2)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan && gap1 != 3 && gap2 != 3 && gap3 != 3)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            //if (pola[i, j].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                            //    polaToFill.Add(new MyPoint(new Point(i, j), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4 && gap3 != 4)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan && gap1 != 5 && gap2 != 5 && gap3 != 5)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jLewo].Fill == Brushes.Cyan && gap1 != 6 && gap2 != 6 && gap3 != 6)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan && gap1 != 7 && gap2 != 7 && gap3 != 7)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }


                    }
                }

            foreach (MyPoint p in polaToFill)
                pola[(int)p.point.X, (int)p.point.Y].Fill = p.brush;

            polaToFill.Clear();

        }
        public void GrowNonPeriodic()
        {
            if (TemporaryRandomGrainRadio.IsChecked == true)
            {
                int iterator = 0;
                do
                {
                    int x = rand.Next(cellHeight);
                    int y = rand.Next(cellHeight);

                    if (pola[x, y].Fill == Brushes.Cyan)
                    {
                        pola[x, y].Fill = GetRandomBrush();
                        break;
                    }
                    if (iterator++ >= cellHeight * cellHeight *cellHeight * cellHeight)
                    {
                        break;
                    }
                } while (true);
            }

            Brush tempBrush = null;

            for (int i = 1; i < cellHeight - 1; i++)
                for (int j = 1; j < cellWidth - 1; j++)
                {

                    if (pola[i, j].Fill != Brushes.Cyan)
                    {

                        tempBrush = pola[i, j].Fill;


                        if (i == 1 && j == 1 && pola[i, j - 1].Fill == tempBrush && pola[i - 1, j].Fill == tempBrush)
                            polaToFill.Add(new MyPoint(new Point(i - 1, j - 1), tempBrush));
                        if (i == 1 && j == cellWidth - 2 && pola[i, j + 1].Fill == tempBrush && pola[i - 1, j].Fill == tempBrush)
                            polaToFill.Add(new MyPoint(new Point(i - 1, j + 1), tempBrush));

                        if (i == cellHeight - 2 && j == 1 && pola[i, j - 1].Fill == tempBrush && pola[i + 1, j].Fill == tempBrush)
                            polaToFill.Add(new MyPoint(new Point(i + 1, j - 1), tempBrush));
                        if (i == cellHeight - 2 && j == cellWidth - 2 && pola[i, j + 1].Fill == tempBrush && pola[i + 1, j].Fill == tempBrush)
                            polaToFill.Add(new MyPoint(new Point(i + 1, j + 1), tempBrush));




                        int iGora = i - 1;
                        if (iGora < 0)
                        {
                            iGora = cellHeight - 1;
                        }
                        int iDol = i + 1;
                        if (iDol >= cellHeight)
                        {
                            iDol = 0;
                        }

                        int jLewo = j - 1;
                        if (jLewo < 0)
                        {
                            jLewo = cellWidth - 1;
                        }
                        int jPrawo = j + 1;
                        if (jPrawo >= cellWidth)
                        {
                            jPrawo = 0;
                        }



                        if (VonNeumanRadio.IsChecked == true)
                        {

                            if (pola[iGora, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                        }
                        else if (MooreRadio.IsChecked == true)
                        {
                            if (pola[iGora, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            if (pola[i, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, j), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }

                        else if (RightHexRadio.IsChecked == true)
                        {
                            if (pola[iGora, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }

                        else if (LeftHexRadio.IsChecked == true)
                        {
                            if (pola[iGora, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                        }
                        else if (HexRandRadio.IsChecked == true)
                        {

                            int gap1 = rand.Next(8);
                            int gap2;
                            do
                            {
                                gap2 = rand.Next(8);
                            } while (gap2 == gap1);



                            if (pola[iGora, j].Fill == Brushes.Cyan && gap1 != 0 && gap2 != 0)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan && gap1 != 1 && gap2 != 1)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan && gap1 != 2 && gap2 != 2)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan && gap1 != 3 && gap2 != 3)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            //if (pola[i, j].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                            //    polaToFill.Add(new MyPoint(new Point(i, j), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan && gap1 != 5 && gap2 != 5)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jLewo].Fill == Brushes.Cyan && gap1 != 6 && gap2 != 6)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan && gap1 != 7 && gap2 != 7)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }

                        else if (HexRandRadio.IsChecked == true)
                        {

                            int gap1 = rand.Next(8);
                            int gap2;
                            do
                            {
                                gap2 = rand.Next(8);
                            } while (gap2 == gap1);



                            if (pola[iGora, j].Fill == Brushes.Cyan && gap1 != 0 && gap2 != 0)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan && gap1 != 1 && gap2 != 1)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan && gap1 != 2 && gap2 != 2)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan && gap1 != 3 && gap2 != 3)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            //if (pola[i, j].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                            //    polaToFill.Add(new MyPoint(new Point(i, j), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan && gap1 != 5 && gap2 != 5)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jLewo].Fill == Brushes.Cyan && gap1 != 6 && gap2 != 6)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan && gap1 != 7 && gap2 != 7)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }

                        else if (PentRandRadio.IsChecked == true)
                        {

                            int gap1 = rand.Next(8);
                            int gap2, gap3;

                            do
                            {
                                gap2 = rand.Next(8);
                            } while (gap2 == gap1);

                            do
                            {
                                gap3 = rand.Next(8);
                            } while (gap3 == gap1 || gap3 == gap2);



                            if (pola[iGora, j].Fill == Brushes.Cyan && gap1 != 0 && gap2 != 0 && gap3 != 0)
                                polaToFill.Add(new MyPoint(new Point(iGora, j), tempBrush));
                            if (pola[iGora, jLewo].Fill == Brushes.Cyan && gap1 != 1 && gap2 != 1 && gap3 != 1)
                                polaToFill.Add(new MyPoint(new Point(iGora, jLewo), tempBrush));
                            if (pola[iGora, jPrawo].Fill == Brushes.Cyan && gap1 != 2 && gap2 != 2 && gap3 != 2)
                                polaToFill.Add(new MyPoint(new Point(iGora, jPrawo), tempBrush));
                            if (pola[i, jLewo].Fill == Brushes.Cyan && gap1 != 3 && gap2 != 3 && gap3 != 3)
                                polaToFill.Add(new MyPoint(new Point(i, jLewo), tempBrush));
                            //if (pola[i, j].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4)
                            //    polaToFill.Add(new MyPoint(new Point(i, j), tempBrush));
                            if (pola[i, jPrawo].Fill == Brushes.Cyan && gap1 != 4 && gap2 != 4 && gap3 != 4)
                                polaToFill.Add(new MyPoint(new Point(i, jPrawo), tempBrush));
                            if (pola[iDol, j].Fill == Brushes.Cyan && gap1 != 5 && gap2 != 5 && gap3 != 5)
                                polaToFill.Add(new MyPoint(new Point(iDol, j), tempBrush));
                            if (pola[iDol, jLewo].Fill == Brushes.Cyan && gap1 != 6 && gap2 != 6 && gap3 != 6)
                                polaToFill.Add(new MyPoint(new Point(iDol, jLewo), tempBrush));
                            if (pola[iDol, jPrawo].Fill == Brushes.Cyan && gap1 != 7 && gap2 != 7 && gap3 != 7)
                                polaToFill.Add(new MyPoint(new Point(iDol, jPrawo), tempBrush));
                        }

                    }
                }

            foreach (MyPoint p in polaToFill)
                pola[(int)p.point.X, (int)p.point.Y].Fill = p.brush;

            polaToFill.Clear();

        }



        private void SubmitGrainCountButton_Click(object sender, RoutedEventArgs e)
        {

            int x, y;

            if (int.TryParse(GrainCount.Text, out int grainCount))
            {

                if (RandomGrainButton.IsChecked == true)
                {
                    if (grainCount < cellHeight * cellHeight)
                    {

                        for (int i = 0; i < grainCount; i++)
                        {
                            x = rand.Next(cellHeight);
                            y = rand.Next(cellHeight);

                            pola[x, y].Fill = GetRandomBrush();
                        }



                    }
                    else
                        MessageBox.Show("To many grains.");
                }

                else if(TemporaryGraintButton.IsChecked==true)
                {
                    int gap = cellWidth / grainCount;

                    for (int i = 0; i < cellWidth; i += gap)
                        for (int j = 0; j < cellHeight; j += gap)
                            pola[i, j].Fill = GetRandomBrush();
                }


                else if(RandomWithRGrainButton.IsChecked==true)
                {
                    

                    Boolean isOk;
                    bool enoughPlace = true;

                    int iterator = 0;
                    if (int.TryParse(RadiusTextBox.Text, out int radius))
                    {
                        bool isRadiusOk;

                        bool noSpace = false;


                        for (int i = 0; i < grainCount; i++)
                        {
                            isRadiusOk = true;

                            
                                if (tempPoints.Count == 0)
                                {
                                    x = rand.Next(cellHeight);
                                    y = rand.Next(cellHeight);
                                    tempPoints.Add(new Point(x, y));
                                    //pola[x, y].Fill = GetRandomBrush();
                                }
                                else
                                {





                                iterator = 0; 
                                    do
                                    {
                                        isRadiusOk = true;
                                        x = rand.Next(cellHeight);
                                        y = rand.Next(cellHeight);
                                        Point[] pointsArray = tempPoints.ToArray();

                                        for (int j =0; j< tempPoints.Count; j++)
                                        {
                                            if (Math.Sqrt(Math.Pow(pointsArray[j].X - x, 2) + Math.Pow(pointsArray[j].Y - y, 2)) < radius)
                                            {
                                                isRadiusOk = false;
                                                break;
                                            }
                                        }
                                        

                                   
                                       
                                        if (isRadiusOk)
                                            break;

                                    iterator++;
                                    if (iterator == 10000)
                                    {
                                        MessageBox.Show("Brak miejsca na kolejne ziarno");
                                        isRadiusOk = false;
                                        noSpace = true;
                                        break;
                                    }
                                    } while (!isRadiusOk);


                                    if(isRadiusOk)
                                         tempPoints.Add(new Point(x, y));
                                if (noSpace)
                                    break;

                                   
                                }

                            }
                            foreach (Point p in tempPoints)
                                if (pola[(int)p.X, (int)p.Y].Fill == Brushes.Cyan)
                                    pola[(int)p.X, (int)p.Y].Fill = GetRandomBrush();
                         
                        
                    }
                }
            }
            else
                MessageBox.Show("Wrong data");
        }

        public Brush GetRandomBrush()
        {
            Brush result = Brushes.Transparent;

            

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();


            while (true)
            {
                int random = rand.Next(properties.Length);
                result = (Brush)properties[random].GetValue(null, null);

                if (!usedColors.Contains(result))
                {
                    return result;


                }
            }
        }


       public void Click()
        {
            for (int i = 0; i < cellHeight; i++)
                for (int j = 0; j < cellWidth; j++)
                {



                    int iGora = i - 1;
                    if (iGora < 0)
                    {
                        iGora = cellHeight - 1;
                    }
                    int iDol = i + 1;
                    if (iDol >= cellHeight)
                    {
                        iDol = 0;
                    }

                    int jLewo = j - 1;
                    if (jLewo < 0)
                    {
                        jLewo = cellWidth - 1;
                    }
                    int jPrawo = j + 1;
                    if (jPrawo >= cellWidth)
                    {
                        jPrawo = 0;
                    }


                    Brush tempBrush = null;
                    Brush minBrush = null;
                    int tempSuma = 0;
                    int minSuma = 0;
                    Brush currentBrush = pola[i, j].Fill;


                    if (VonNeumanRadio.IsChecked == true)
                    {


                        if (pola[iGora, j].Fill != currentBrush)
                            minSuma++;
                        if (pola[i, jLewo].Fill != currentBrush)
                            minSuma++;
                        if (pola[i, jPrawo].Fill != currentBrush)
                            minSuma++;
                        if (pola[iDol, j].Fill != currentBrush)
                            minSuma++;
                        minBrush = currentBrush;



                        foreach (Brush b in idUsedColors)
                        {
                            tempSuma = 0;

                            if (pola[iGora, j].Fill != b)
                                tempSuma++;
                            if (pola[i, jLewo].Fill != b)
                                tempSuma++;
                            if (pola[i, jPrawo].Fill != b)
                                tempSuma++;
                            if (pola[iDol, j].Fill != b)
                                tempSuma++;

                            if (tempSuma <= minSuma)
                            {
                                minBrush = b;
                                minSuma = tempSuma;
                            }

                        }
                        tempBrushes[i, j] = minBrush;
                    }

                    else if (MooreRadio.IsChecked == true)
                    {

                        if (pola[iGora, j].Fill != currentBrush)
                            minSuma++;
                        if (pola[iGora, jLewo].Fill != currentBrush)
                            minSuma++;
                        if (pola[iGora, jPrawo].Fill != currentBrush)
                            minSuma++;
                        if (pola[i, jLewo].Fill != currentBrush)
                            minSuma++;
                        if (pola[i, jPrawo].Fill != currentBrush)
                            minSuma++;
                        if (pola[iDol, j].Fill != currentBrush)
                            minSuma++;
                        if (pola[iDol, jLewo].Fill != currentBrush)
                            minSuma++;
                        if (pola[iDol, jPrawo].Fill != currentBrush)
                            minSuma++;
                        minBrush = currentBrush;



                        foreach (Brush b in idUsedColors)
                        {
                            tempSuma = 0;

                            if (pola[iGora, j].Fill != b)
                                tempSuma++;
                            if (pola[iGora, jLewo].Fill != b)
                                tempSuma++;
                            if (pola[iGora, jPrawo].Fill != b)
                                tempSuma++;
                            if (pola[i, jLewo].Fill != b)
                                tempSuma++;
                            if (pola[i, jPrawo].Fill != b)
                                tempSuma++;
                            if (pola[iDol, j].Fill != b)
                                tempSuma++;
                            if (pola[iDol, jPrawo].Fill != b)
                                tempSuma++;
                            if (pola[iDol, jLewo].Fill != b)
                                tempSuma++;

                            if (tempSuma <= minSuma)
                            {
                                minBrush = b;
                                minSuma = tempSuma;
                            }

                        }
                        tempBrushes[i, j] = minBrush;

                    }








                }

            for (int i = 0; i < cellHeight; i++)
                for (int j = 0; j < cellWidth; j++)
                    pola[i, j].Fill = tempBrushes[i, j];

           // System.Threading.Thread.Sleep(10);

    
        }



        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();



        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Click();
        }


        private void MooreButton_Click(object sender, RoutedEventArgs e)
        {


            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }

        private void InitializeMC_Click(object sender, RoutedEventArgs e)
        {
            board.Children.Clear();


            if (int.TryParse(IdCount.Text, out int idCount))
            {
                for (int i = 0; i < idCount; i++)
                    idUsedColors.Add(GetRandomBrush());
            }

            if (int.TryParse(WidthTextBox.Text, out int inputWidth))
            {
                cellWidth = inputWidth;
                cellHeight = inputWidth;
                pola = new Rectangle[cellWidth, cellHeight];
                tempBrushes = new Brush[cellWidth, cellHeight];


                Rectangle r;
                int tempId = 0;

            




                double rectangleWidth = board.ActualWidth / cellWidth - gap;
                for (int i = 0; i < cellHeight; i++)
                    for (int j = 0; j < cellWidth; j++)
                    {
                        r = new Rectangle();
                        r.Width = rectangleWidth;
                        r.Height = rectangleWidth;
                        //r.Fill = Brushes.Cyan;

                        tempId = rand.Next(idCount);
                        r.Fill = idUsedColors[rand.Next(idUsedColors.Count)];


                        board.Children.Add(r);
                        Canvas.SetLeft(r, j * board.ActualWidth / cellWidth);
                        Canvas.SetTop(r, i * board.ActualHeight / cellHeight);
                        r.MouseDown += R_MouseDown;
                        pola[i, j] = r;

                    }


            }
            else
                MessageBox.Show("Wrong data!");



        }

        private void stopButon_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }
    }
}
