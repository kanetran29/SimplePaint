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
        public enum Tool 
        {
            Brush,
            Eraser,
            Shape
        }

        public enum Shape
        {
            Retangle,
            Triangle,
            Circle,
            Diamond,
        }

        public enum Size
        {
            Large,
            Medium,
            Small,
        }

        Point iniP;
        Stroke _pre_stroke;
        private MainViewModel MainVM;
        private SolidColorBrush CForeground;
        private SolidColorBrush CBackground;

        private Tool CurrentTool;
        private Size CurrentSize;
        public MainWindow()
        {
            InitializeComponent();
            icv_Paint.AddHandler(InkCanvas.MouseLeftButtonDownEvent, new MouseButtonEventHandler(icv_Paint_MouseLeftButtonDown), true);
            icv_Paint.AddHandler(InkCanvas.MouseLeftButtonUpEvent, new MouseButtonEventHandler(icv_Paint_MouseLeftButtonUp), true);
            icv_Paint.Strokes.StrokesChanged += Strokes_StrokesChanged;

            icv_Paint.UseCustomCursor = true;
            cbx_Size.SelectedIndex = 0;

            //get Data context
            MainVM = PaintWindow.DataContext as MainViewModel;
            CurrentTool = Tool.Brush;
            CBackground = new SolidColorBrush(Colors.White);
            CForeground = new SolidColorBrush(Colors.Black);
        }
        private void btn_Eraser_Click(object sender, RoutedEventArgs e)
        {
            // icv_Paint.Cursor = new Cursor(new System.IO.MemoryStream(Paint.Properties.Resources.Cursor1));
            icv_Paint.Cursor = Cursors.No;
            icv_Paint.DefaultDrawingAttributes.Color = CBackground.Color;
            CurrentTool = Tool.Eraser;
        }

        private void btn_Brush_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Brush;
        }
        private void cbx_Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(cbx_Size.SelectedIndex);
            switch(cbx_Size.SelectedIndex)
            {
                case 0: 
                    CurrentSize = Size.Small;
                    icv_Paint.DefaultDrawingAttributes.Width = 2;
                    icv_Paint.DefaultDrawingAttributes.Height = 2;
                    break;
                case 1:
                    CurrentSize = Size.Medium;
                    icv_Paint.DefaultDrawingAttributes.Width = 5;
                    icv_Paint.DefaultDrawingAttributes.Height = 5;
                    break;
                case 2:
                    CurrentSize = Size.Large;
                    icv_Paint.DefaultDrawingAttributes.Width = 8;
                    icv_Paint.DefaultDrawingAttributes.Height = 8;
                    break;
                default: break;
            }
                
        }
        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (e.Added.Count() != 0)
                MainVM.StrokesChanged(e.Added);
        }
        private void icv_Paint_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Draw square
                if (CurrentTool == Tool.Shape)
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
                        MainVM.Undo.RemoveFirst();
                    }
                    icv_Paint.Strokes.Add(stroke);
                    _pre_stroke = stroke;
                }                
            }

        }

        private void icv_Paint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentTool == Tool.Shape)
            {
                iniP = e.GetPosition(icv_Paint);
                icv_Paint.EditingMode = InkCanvasEditingMode.None;
            }
        }
        private void icv_Paint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CurrentTool == Tool.Shape)
            {
                _pre_stroke = null;
                icv_Paint.EditingMode = InkCanvasEditingMode.Ink;
            }
        }
        private void clp_Foreground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            CForeground = new SolidColorBrush((Color)clp_Foreground.SelectedColor);
            if(icv_Paint != null)
                if(CurrentTool == Tool.Shape || CurrentTool == Tool.Brush)
                    icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
        }

        private void clp_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            CBackground = new SolidColorBrush((Color)clp_Background.SelectedColor);
            if (icv_Paint != null)
                if (CurrentTool == Tool.Eraser)
                    icv_Paint.DefaultDrawingAttributes.Color = CBackground.Color;
        }

    }
}
