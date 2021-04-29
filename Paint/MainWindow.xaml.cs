using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Ink;
using System.Diagnostics;
using System.Windows.Shapes;

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
            Line,
            Eraser,
            Retangle,
            Triangle,
            Ellipse,
            Pentagon,
            Text,
        }

        UIElement shape = null;
        TextBox _textbox = null;
        private MainViewModel MainVM;

        private SolidColorBrush CForeground;
        private SolidColorBrush CBackground;

        private Tool CurrentTool;
        Point startP, endP;
        bool isMouseOver = false;

        public MainWindow()
        {
            InitializeComponent();
            icv_Paint.AddHandler(InkCanvas.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(icv_Paint_MouseLeftButtonDown), true);
            icv_Paint.AddHandler(InkCanvas.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(icv_Paint_MouseLeftButtonUp), true);
            icv_Paint.Strokes.StrokesChanged += Strokes_StrokesChanged;

            icv_Paint.UseCustomCursor = true;
            cbx_Size.SelectedIndex = 0;

            //set font family source
            List<string> nonReadebleFonts = new List<string>();
            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                ComboBoxItem boxItem = new ComboBoxItem();
                boxItem.Content = font.ToString();
                Uri s = font.BaseUri;
                if (!nonReadebleFonts.Contains(font.ToString()))
                    boxItem.FontFamily = font;

                cbx_FontFamily.Items.Add(boxItem);
            }
            cbx_FontFamily.SelectedIndex = 0;
            //set font size source
            cbx_FontSize.Items.Add(8.0);
            cbx_FontSize.Items.Add(9.0);
            cbx_FontSize.Items.Add(10.0);
            cbx_FontSize.Items.Add(11.0);
            cbx_FontSize.Items.Add(12.0);
            cbx_FontSize.Items.Add(14.0);
            cbx_FontSize.Items.Add(16.0);
            cbx_FontSize.Items.Add(18.0);
            cbx_FontSize.Items.Add(20.0);
            cbx_FontSize.Items.Add(22.0);
            cbx_FontSize.Items.Add(24.0);
            cbx_FontSize.Items.Add(26.0);
            cbx_FontSize.Items.Add(28.0);
            cbx_FontSize.Items.Add(36.0);
            cbx_FontSize.Items.Add(48.0);
            cbx_FontSize.Items.Add(72.0);
            cbx_FontSize.SelectedIndex = 0;
            //get Data context
            MainVM = PaintWindow.DataContext as MainViewModel;
            CurrentTool = Tool.Brush;
            CBackground = new SolidColorBrush(Colors.White);
            CForeground = new SolidColorBrush(Colors.Black);
        }
        private void PaintWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }
        #region Handler for tools
        private void btn_Eraser_Click(object sender, RoutedEventArgs e)
        {
            // icv_Paint.Cursor = new Cursor(new System.IO.MemoryStream(Paint.Properties.Resources.Cursor1));
            icv_Paint.Cursor = Cursors.No;
            icv_Paint.DefaultDrawingAttributes.Color = CBackground.Color;
            CurrentTool = Tool.Eraser;
            icv_Paint.EditingMode = InkCanvasEditingMode.Ink;
        }
        private void btn_Brush_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Brush;
            icv_Paint.EditingMode = InkCanvasEditingMode.Ink;
        }
        private void btn_Line_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Line;
        }
        private void btn_Rectangle_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Retangle;
        }
        private void btn_Ellipse_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Ellipse;
        }
        private void btn_Triangle_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Triangle;
        }
        private void btn_Pentagon_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.Pen;
            icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
            CurrentTool = Tool.Pentagon;
        }
        private void btn_Selection_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.EditingMode = InkCanvasEditingMode.Select;
            //CurrentTool = Tool.Lasso;
        }
        private void btn_Text_Click(object sender, RoutedEventArgs e)
        {
            icv_Paint.Cursor = Cursors.IBeam;
            CurrentTool = Tool.Text;
        }
        #endregion
        #region handler for properties
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
        private void cbx_FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_textbox != null)
                _textbox.FontSize = Double.Parse(cbx_FontSize.SelectedValue.ToString());
        }
        
        private void clp_Foreground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            CForeground = new SolidColorBrush((Color)clp_Foreground.SelectedColor);
            if (icv_Paint != null)
                //if (CurrentTool == Tool.Retangle || CurrentTool == Tool.Triangle || CurrentTool == Tool.Ellipse || CurrentTool == Tool.Pentagon || CurrentTool == Tool.Brush)
                if (CurrentTool != Tool.Eraser)
                    icv_Paint.DefaultDrawingAttributes.Color = CForeground.Color;
        }
        private void clp_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            CBackground = new SolidColorBrush((Color)clp_Background.SelectedColor);
            if (icv_Paint != null)
            {
                if (CurrentTool == Tool.Eraser)
                    icv_Paint.DefaultDrawingAttributes.Color = CBackground.Color;
                //icv_Paint.Background = CBackground;
            }
        }
        #endregion
        #region handler for text
        private void cbx_FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_textbox != null)
                _textbox.FontFamily = new FontFamily(((ComboBoxItem)cbx_FontFamily.SelectedItem).Content.ToString());

        }
        private void btn_B_Checked(object sender, RoutedEventArgs e)
        {
            if (_textbox != null)
                _textbox.FontWeight = FontWeights.Bold;
        }

        private void btn_B_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_textbox != null)
                _textbox.FontWeight = FontWeights.Normal;
        }
        private void btn_I_Checked(object sender, RoutedEventArgs e)
        {
            if (_textbox != null)
                _textbox.FontStyle = FontStyles.Italic;
        }

        private void btn_I_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_textbox != null)
                _textbox.FontStyle = FontStyles.Normal;
        }
        private void btn_U_Checked(object sender, RoutedEventArgs e)
        {
            if (_textbox != null)
                _textbox.TextDecorations = TextDecorations.Underline;
        }

        private void btn_U_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_textbox != null)
                _textbox.TextDecorations = null;
        }
        #endregion
        #region handler for drawing
        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            // if (e.Added.Count() != 0)
            // MainVM.StrokesChanged(e.Added);
        }
        private void icv_Paint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (CurrentTool)
            {
                case Tool.Retangle:
                case Tool.Triangle:
                case Tool.Ellipse:
                case Tool.Pentagon:
                case Tool.Line:
                case Tool.Text:
                    startP = e.GetPosition(icv_Paint);
                    icv_Paint.EditingMode = InkCanvasEditingMode.None;
                    shape = null;
                    break;
            }
            if (_textbox != null)
            {
                if (!isMouseOver)
                {
                    if (_textbox.Text.Length > 0)
                    {
                        TextBlock textblock = new TextBlock()
                        {
                            Width = _textbox.Width,
                            Height = _textbox.Height,
                            TextWrapping = _textbox.TextWrapping,
                            Text = _textbox.Text,
                            Visibility = Visibility.Visible,
                            Foreground = CForeground,
                            FontSize = _textbox.FontSize,
                            FontFamily = _textbox.FontFamily,
                            FontStyle = _textbox.FontStyle,
                            FontWeight = _textbox.FontWeight,
                            TextDecorations = _textbox.TextDecorations
                        };
                        icv_Paint.Children.Add(textblock);
                        InkCanvas.SetLeft(textblock, InkCanvas.GetLeft(_textbox));
                        InkCanvas.SetTop(textblock, InkCanvas.GetTop(_textbox));
                    }
                    icv_Paint.Children.Remove(_textbox);
                    _textbox = null;
                }


            }

        }
        private void icv_Paint_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //get position and generate shape 
                endP = e.GetPosition(icv_Paint);
                if (startP.X == 0 && startP.Y == 0)
                    return;
                double sX, sY, eX, eY;
                sX = Math.Min(startP.X, endP.X);
                sY = Math.Min(startP.Y, endP.Y);
                eX = Math.Max(startP.X, endP.X);
                eY = Math.Max(startP.Y, endP.Y);
                if (shape == null)
                {
                    shape = GenerateShape();
                    icv_Paint.Children.Add(shape);
                }
                else
                    switch (CurrentTool)
                    {
                        case Tool.Text:
                            if (_textbox == null)
                            {
                                Rectangle DRshape = shape as Rectangle;
                                DRshape.Width = (eX - sX);
                                DRshape.Height = (eY - sY);
                                DRshape.Margin = new Thickness(sX, sY, 0, 0);
                                shape = DRshape;
                            }
                            break;
                        case Tool.Retangle:
                            Rectangle Rshape = shape as Rectangle;
                            Rshape.Width = (eX - sX);
                            Rshape.Height = (eY - sY);
                            Rshape.Margin = new Thickness(sX, sY, 0, 0);
                            shape = Rshape;
                            break;
                        case Tool.Triangle:
                            Polygon Tshape = shape as Polygon;
                            Tshape.Points = new PointCollection
                            {
                                new Point((sX + eX)/2, sY),
                                new Point(sX, eY),
                                new Point(eX, eY),
                            };
                            shape = Tshape;
                            break;
                        case Tool.Ellipse:
                            Ellipse Eshape = shape as Ellipse;
                            Eshape.Width = (eX - sX);
                            Eshape.Height = (eY - sY);
                            Eshape.Margin = new Thickness(sX, sY, 0, 0);
                            shape = Eshape;
                            break;
                        case Tool.Pentagon:
                            Polygon Pshape = shape as Polygon;
                            Pshape.Points = new PointCollection
                            {
                                new Point((sX + eX)/2, sY),
                                new Point(eX, sY + (eY - sY)/3),
                                new Point(eX - (eX - sX)/4, eY),
                                new Point(sX + (eX - sX)/4, eY),
                                new Point(sX, sY + (eY - sY)/3),
                            };
                            shape = Pshape;
                            break;
                        case Tool.Line:
                            Line Lshape = shape as Line;
                            Lshape.X1 = startP.X;
                            Lshape.X2 = endP.X;
                            Lshape.Y1 = startP.Y;
                            Lshape.Y2 = endP.Y;
                            shape = Lshape;
                            break;
                    }

            }
        }
        private void icv_Paint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            switch (CurrentTool)
            {
                case Tool.Retangle:
                case Tool.Triangle:
                case Tool.Ellipse:
                case Tool.Pentagon:
                case Tool.Line:
                    shape = null;
                    startP.X = startP.Y = 0;
                    endP.X = endP.Y = 0;
                    break;
                case Tool.Text:
                    if (icv_Paint.Children.Contains(shape))
                        icv_Paint.Children.Remove(shape);
                    if (_textbox == null)
                    {
                        _textbox = new TextBox()
                        {
                            Width = Math.Abs(endP.X - startP.X),
                            Height = Math.Abs(endP.Y - startP.Y),
                            Background = Brushes.Transparent,
                            TextWrapping = TextWrapping.Wrap,
                            FontFamily = new FontFamily(cbx_FontFamily.Text),
                            FontSize = Double.Parse(cbx_FontSize.Text),
                            FontStyle = (bool)btn_I.IsChecked ? FontStyles.Italic : FontStyles.Normal,
                            FontWeight = (bool)btn_B.IsChecked ? FontWeights.Bold : FontWeights.Normal,
                            TextDecorations = (bool)btn_U.IsChecked ? TextDecorations.Underline : null,                        
                        };
                        
                        //resize text box
                        if (_textbox.Width <= 100) _textbox.Width = 100;
                        if (_textbox.Height <= 40) _textbox.Height = 40;


                        InkCanvas.SetLeft(_textbox, Math.Min(startP.X, endP.X));
                        InkCanvas.SetTop(_textbox, Math.Min(startP.Y, endP.Y));
                        //_textbox.
                        icv_Paint.Children.Add(_textbox);
                        _textbox.Focus();
                        _textbox.MouseEnter += _textbox_MouseEnter;
                        _textbox.MouseLeave += _textbox_MouseLeave;
                    }
                    break;
            }

        }
        #endregion



        //function for Genarating shapes
        private UIElement GenerateShape()
        {
            double StrokeThickness = icv_Paint.DefaultDrawingAttributes.Width;
            //get color for fill and outline
            SolidColorBrush foreground = (cbx_Outline.SelectedIndex == 0) ? CForeground : new SolidColorBrush(Colors.Transparent);
            SolidColorBrush background = (cbx_Fill.SelectedIndex == 1) ? CBackground : new SolidColorBrush(Colors.Transparent);

            //get position
            double sX, sY, eX, eY;
            sX = Math.Min(startP.X, endP.X);
            sY = Math.Min(startP.Y, endP.Y);
            eX = Math.Max(startP.X, endP.X);
            eY = Math.Max(startP.Y, endP.Y);

            UIElement shape = null;
            switch(CurrentTool)
            {
                case Tool.Line:
                    shape = new Line()
                    {
                        Stroke = CForeground, // line's color is always foreground
                        X1 = startP.X,
                        X2 = endP.X,
                        Y1 = startP.Y,
                        Y2 = endP.Y,
                        StrokeThickness = StrokeThickness,
                    };
                    break;
                case Tool.Retangle:
                    shape = new Rectangle()
                    {
                        Stroke = foreground,
                        Fill = background,
                        Width = (eX - sX),
                        Height = (eY - sY),
                        StrokeThickness = StrokeThickness,
                        Margin = new Thickness(sX, sY, 0, 0)
                    };
                    break;
                case Tool.Triangle:
                    shape = new Polygon()
                    {
                        Stroke = foreground,
                        Fill = background,
                        Points = new PointCollection
                        {
                            new Point((sX + eX)/2, sY),
                            new Point(sX, eY),
                            new Point(eX, eY),
                        },
                        StrokeThickness = StrokeThickness,
                    };
                    break;
                case Tool.Ellipse:
                    shape = new Ellipse()
                    {
                        Stroke = foreground,
                        Fill = background,
                        Width = (eX - sX),
                        Height = (eY - sY),
                        StrokeThickness = StrokeThickness,
                        Margin = new Thickness(sX, sY, 0, 0)
                    };
                    break;
                case Tool.Pentagon:
                    shape = new Polygon()
                    {
                        Stroke = foreground,
                        Fill = background,
                        Points = new PointCollection
                        {
                            new Point((sX + eX)/2, sY),
                            new Point(eX, sY + (eY - sY)/3),
                            new Point(eX - (eX - sX)/4, eY),
                            new Point(sX + (eX - sX)/4, eY),
                            new Point(sX, sY + (eY - sY)/3),
                        },
                        StrokeThickness = StrokeThickness,
                    };
                    break;
                case Tool.Text:
                    shape = new Rectangle()
                    {
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = new SolidColorBrush(Colors.Transparent),
                        Width = (eX - sX),
                        Height = (eY - sY),
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] { 4, 3 }),
                        Margin = new Thickness(sX, sY, 0, 0)
                    };
                    break;
            }
            return shape;
        }
        private void _textbox_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseOver = false;
        }

        
        private void _textbox_MouseEnter(object sender, MouseEventArgs e)
        {
            isMouseOver = true;
        }
    }
}
