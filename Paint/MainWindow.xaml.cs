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
            Retangle,
            Triangle,
            Circle,
            Diamond,
        }

        Point beginP;
        Stroke _pre_stroke;
        private MainViewModel MainVM;
        private SolidColorBrush CForeground;
        private SolidColorBrush CBackground;

        private Tool CurrentTool;
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
        private void btn_Rectangle_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Retangle;
        }
        private void btn_Circle_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Circle;
        }
        private void btn_Triangle_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Triangle;
        }
        private void btn_Diamond_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Diamond;
        }
        private List<Point> GenerateEclipseGeometry(Point st, Point ed)
        {
            double a = 0.5 * (ed.X - st.X);
            double b = 0.5 * (ed.Y - st.Y);
            List<Point> pointList = new List<Point>();
            for (double r = 0; r <= 2 * Math.PI; r = r + 0.01)
            {
                pointList.Add(new Point(0.5 * (st.X + ed.X) + a * Math.Cos(r), 0.5 * (st.Y + ed.Y) + b * Math.Sin(r)));
            }
            return pointList;
        }
        private void cbx_Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(cbx_Size.SelectedIndex);
            switch(cbx_Size.SelectedIndex)
            {
                case 0: 
                    icv_Paint.DefaultDrawingAttributes.Width = 2;
                    icv_Paint.DefaultDrawingAttributes.Height = 2;
                    break;
                case 1:
                    icv_Paint.DefaultDrawingAttributes.Width = 5;
                    icv_Paint.DefaultDrawingAttributes.Height = 5;
                    break;
                case 2:
                    icv_Paint.DefaultDrawingAttributes.Width = 8;
                    icv_Paint.DefaultDrawingAttributes.Height = 8;
                    break;
                default:
                    break;
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
                if (CurrentTool == Tool.Retangle)
                {
                    Point endP = e.GetPosition(icv_Paint);
                    List<Point> pointList = new List<Point>
                    {
                        new Point(beginP.X, beginP.Y),
                        new Point(beginP.X, endP.Y),
                        new Point(endP.X, endP.Y),
                        new Point(endP.X, beginP.Y),
                        new Point(beginP.X, beginP.Y),
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
                // Draw Eclipse
                if (CurrentTool == Tool.Circle)
                {
                    Point endP = e.GetPosition(icv_Paint);
                    List<Point> pointList = GenerateEclipseGeometry(beginP, endP);
                    StylusPointCollection point = new StylusPointCollection(pointList);
                    Stroke stroke = new Stroke(point)
                    {
                        DrawingAttributes = icv_Paint.DefaultDrawingAttributes.Clone()
                    };
                    if (_pre_stroke != null)
                    {
                        icv_Paint.Strokes.Remove(_pre_stroke);
                        MainVM.Undo.RemoveFirst();
                    }
                    icv_Paint.Strokes.Add(stroke);
                    _pre_stroke = stroke;
                }
                // Draw triangle
                if (CurrentTool == Tool.Triangle)
                {
                    Point endP = e.GetPosition(icv_Paint);
                    List<Point> pointList = new List<Point>
                    {
                        new Point((beginP.X + endP.X)/2, beginP.Y),
                        new Point(beginP.X, endP.Y),
                        new Point(endP.X, endP.Y),
                        new Point((beginP.X + endP.X)/2, beginP.Y),
                    };
                    StylusPointCollection point = new StylusPointCollection(pointList);
                    Stroke stroke = new Stroke(point)
                    {
                        DrawingAttributes = icv_Paint.DefaultDrawingAttributes.Clone()
                    };
                    if (_pre_stroke != null)
                    {
                        icv_Paint.Strokes.Remove(_pre_stroke);
                        MainVM.Undo.RemoveFirst();
                    }
                    icv_Paint.Strokes.Add(stroke);
                    _pre_stroke = stroke;
                }
                // Draw diamond
                if (CurrentTool == Tool.Diamond)
                {
                    Point endP = e.GetPosition(icv_Paint);
                    List<Point> pointList = new List<Point>
                    {
                        new Point((beginP.X + endP.X)/2, beginP.Y),
                        new Point(endP.X, beginP.Y + (endP.Y - beginP.Y)/3),
                        new Point(beginP.X + (endP.X - beginP.X)*0.75, endP.Y),
                        new Point(beginP.X + (endP.X - beginP.X)*0.25, endP.Y),
                        new Point(beginP.X, beginP.Y + (endP.Y - beginP.Y)/3),
                        new Point((beginP.X + endP.X)/2, beginP.Y),
                    };
                    StylusPointCollection point = new StylusPointCollection(pointList);
                    Stroke stroke = new Stroke(point)
                    {
                        DrawingAttributes = icv_Paint.DefaultDrawingAttributes.Clone()
                    };
                    if (_pre_stroke != null)
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
            if (CurrentTool == Tool.Retangle || CurrentTool == Tool.Triangle || CurrentTool == Tool.Circle || CurrentTool == Tool.Diamond)
            {
                beginP = e.GetPosition(icv_Paint);
                icv_Paint.EditingMode = InkCanvasEditingMode.None;
            }
        }
        private void icv_Paint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CurrentTool == Tool.Retangle || CurrentTool == Tool.Triangle || CurrentTool == Tool.Circle || CurrentTool == Tool.Diamond)
            {
                _pre_stroke = null;
                icv_Paint.EditingMode = InkCanvasEditingMode.Ink;
            }
        }
        private void clp_Foreground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            CForeground = new SolidColorBrush((Color)clp_Foreground.SelectedColor);
            if(icv_Paint != null)
                //if (CurrentTool == Tool.Retangle || CurrentTool == Tool.Triangle || CurrentTool == Tool.Circle || CurrentTool == Tool.Diamond || CurrentTool == Tool.Brush)
                if (CurrentTool != Tool.Eraser)
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
