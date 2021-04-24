using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Ink;
using System.Diagnostics;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Point iniP;
        Stroke _pre_stroke;
        private MainViewModel MainVM;
        public MainWindow()
        {
            InitializeComponent();
            icv_Paint.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(icv_Paint_MouseDown), true);
            icv_Paint.Strokes.StrokesChanged += Strokes_StrokesChanged;

            icv_Paint.UseCustomCursor = true;
            cbx_Sizes.SelectedIndex = 0;

            //get Data context
            MainVM = PaintWindow.DataContext as MainViewModel;
        }

        private void cbx_Sizes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int iSize = (cbx_Sizes.SelectedIndex * 4) + 2;
            icv_Paint.DefaultDrawingAttributes.Width = icv_Paint.DefaultDrawingAttributes.Height = iSize;
        }
        private void btn_Eraser_Click(object sender, RoutedEventArgs e)
        {
            // icv_Paint.Cursor = new Cursor(new System.IO.MemoryStream(Paint.Properties.Resources.Cursor1));
            icv_Paint.Cursor = Cursors.No;
            icv_Paint.DefaultDrawingAttributes.Color = ((SolidColorBrush)icv_Paint.Background).Color;
        }

        private void btn_Brush_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
        }
        private void Strokes_StrokesChanged(object sender, System.Windows.Ink.StrokeCollectionChangedEventArgs e)
        {
            if (e.Added.Count() != 0)
                MainVM.StrokesChanged(e.Added);
        }
        private void icv_Paint_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Draw square
                if (btn_Shape.IsChecked == true)
                {
                    Point endP = e.GetPosition(icv_Paint);
                    List<Point> pointList = new List<Point>
                    {
                        new Point(iniP.X, iniP.Y),
                        new Point(iniP.X, endP.Y),
                        new Point(endP.X, endP.Y),
                        new Point(endP.X, iniP.Y),
                        new Point(iniP.X, iniP.Y),
                    };
                    StylusPointCollection point = new StylusPointCollection(pointList);
                    Stroke stroke = new Stroke(point)
                    {
                        DrawingAttributes = icv_Paint.DefaultDrawingAttributes.Clone()
                    };
                    if(_pre_stroke != null)
                    {
                        icv_Paint.Strokes.Remove(_pre_stroke);

                    }
                    icv_Paint.Strokes.Add(stroke);
                    _pre_stroke = stroke;
                }                
            }

        }

        private void icv_Paint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((bool)btn_Shape.IsChecked)
            {
                iniP = e.GetPosition(icv_Paint);
            }
        }
    }
}
