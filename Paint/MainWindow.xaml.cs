using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Media.Animation;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Duration Duration1 = (Duration)Application.Current.Resources["Duration1"];
        private static readonly Duration Duration2 = (Duration)Application.Current.Resources["Duration2"];
        private static readonly Duration Duration3 = (Duration)Application.Current.Resources["Duration3"];
        private static readonly Duration Duration4 = (Duration)Application.Current.Resources["Duration4"];
        private static readonly Duration Duration5 = (Duration)Application.Current.Resources["Duration5"];
        private static readonly Duration Duration7 = (Duration)Application.Current.Resources["Duration7"];
        private static readonly Duration Duration10 = (Duration)Application.Current.Resources["Duration10"];

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
            Image,
            Select,
        }
        //define size
        public const int Small = 2;
        public const int Medium = 5;
        public const int Large = 8;
        int BrushSize = 2;
        bool Fill = false;
        bool Outline = true;

        UIElement shape = null;
        TextBox _textbox = null;

        Tool CurrentTool = Tool.Brush;

        Point startP = new Point(-1, -1);
        Point endP = new Point(-1, -1);
        bool isMouseOver = false;
        ImageBrush img = null;

        //Undo/Redo
        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }
        public LinkedList<UIElement> Undo = new LinkedList<UIElement>();
        public LinkedList<UIElement> Redo = new LinkedList<UIElement>();
        public MainWindow()
        {
            InitializeComponent();
            Set_FontFamilySource();
            Set_FontSizeSource();
            //Undo and Redo Command Implementation
            btn_Undo.Command = new RelayCommand<object>((p) =>
            {
                if (Undo.Count() == 0)
                    return false;
                return true;
            }, (p) =>
            {
                Debug.WriteLine("[UNDO]" + Undo.Count());
                UIElement _undo = Undo.First();
                Undo.RemoveFirst();
                cv_Paint.Children.Remove(_undo);
                Redo.AddFirst(_undo);
            });
            btn_Redo.Command = new RelayCommand<object>((p) =>
            {
                if (Redo.Count == 0)
                    return false;
                return true;
            }, (p) =>
            {
                Debug.WriteLine("[REDO]" + Redo.Count());
                UIElement _redo = Redo.First();
                Redo.RemoveFirst();
                cv_Paint.Children.Add(_redo);
                Undo.AddFirst(_redo);
            });
            ResetControl();

        }

        #region Handler for tools
        private void btn_Eraser_Click(object sender, RoutedEventArgs e)
        {
            // cv_Paint.Cursor = new Cursor(new System.IO.MemoryStream(Paint.Properties.Resources.Cursor1));
            cv_Paint.Cursor = Cursors.No;
            CurrentTool = Tool.Eraser;
            ResetControl();
            btn_Eraser.IsActived = true;
        }
        private void btn_Brush_Click(object sender, RoutedEventArgs e)
        {
            cv_Paint.Cursor = Cursors.Pen;
            CurrentTool = Tool.Brush;
            ResetControl();
            btn_Brush.IsActived = true;
        }
        private void btn_Line_Click(object sender, RoutedEventArgs e)
        {
            cv_Paint.Cursor = Cursors.Pen;
            CurrentTool = Tool.Line;
            ResetControl();
            btn_Line.IsActived = true;
        }
        private void btn_Rectangle_Click(object sender, RoutedEventArgs e)
        {
            cv_Paint.Cursor = Cursors.Pen;
            CurrentTool = Tool.Retangle;
            ResetControl();
            btn_Rectangle.IsActived = true;
            btn_Fill.Visibility = Visibility.Visible;
        }
        private void btn_Ellipse_Click(object sender, RoutedEventArgs e)
        {
            cv_Paint.Cursor = Cursors.Pen;
            CurrentTool = Tool.Ellipse;
            ResetControl();
            btn_Ellipse.IsActived = true;
            btn_Fill.Visibility = Visibility.Visible;
   
        }
        private void btn_Triangle_Click(object sender, RoutedEventArgs e)
        {
            cv_Paint.Cursor = Cursors.Pen;
            CurrentTool = Tool.Triangle;
            ResetControl();
            btn_Triangle.IsActived = true;
            btn_Fill.Visibility = Visibility.Visible;
        }
        private void btn_Pentagon_Click(object sender, RoutedEventArgs e)
        {
            cv_Paint.Cursor = Cursors.Pen;
            CurrentTool = Tool.Pentagon;
            ResetControl();
            btn_Pentagon.IsActived = true;
            btn_Fill.Visibility = Visibility.Visible;
        }
        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            cv_Paint.Cursor = Cursors.Pen;
            CurrentTool = Tool.Select;
            ResetControl();
            btn_Select.IsActived = true;
        }
        private void btn_Text_Click(object sender, RoutedEventArgs e)
        {
            cv_Paint.Cursor = Cursors.IBeam;
            CurrentTool = Tool.Text;
            ResetControl();
            btn_Text.IsActived = true;
            cbx_FontFamily.Visibility = Visibility.Visible;

        }
        private void btn_Image_Click(object sender, RoutedEventArgs e)
        {

            cv_Paint.Cursor = Cursors.Pen;
            CurrentTool = Tool.Image;
            ResetControl();
            btn_Image.IsActived = true;
            OpenFileDialog open = new OpenFileDialog()
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
            };
            if (open.ShowDialog() == true)
            {
                Debug.WriteLine(open.FileName);
                Uri fileUri = new Uri(open.FileName);
                img = new ImageBrush()
                {
                    ImageSource = new BitmapImage(fileUri),
                };
            }
            else
                btn_Brush_Click(sender,e);
        }
        #endregion
        #region handler for size
        private void btn_Outline_Click(object sender, RoutedEventArgs e)
        {
            Outline = !Outline;
            Debug.WriteLine(img_Fill.Source + "");
            string source = @"pack://application:,,,/" + (Outline ? "Resources/outline.png" : @"Resources/fill.png");
            img_Outline.Source = new BitmapImage(new Uri(source));
        }

        private void btn_Fill_Click(object sender, RoutedEventArgs e)
        {
            Fill = !Fill;
            string source = @"pack://application:,,,/" + (Outline ? "Resources/fill.png" : @"Resources/nofill.png");
            img_Fill.Source = new BitmapImage(new Uri(source));
        }

        private void btn_Size_Click(object sender, RoutedEventArgs e)
        {
            switch (BrushSize)
            {
                case Small:
                    BrushSize = Medium;
                    break;
                case Medium:
                    BrushSize = Large;
                    break;
                case Large:
                    BrushSize = Small;
                    break;
            }
            img_Size.Width = img_Size.Height = BrushSize;
        }
        private void cbx_FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_textbox != null)
                _textbox.FontSize = Double.Parse(cbx_FontSize.SelectedValue.ToString());
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
        private void cv_Paint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startP = e.GetPosition(cv_Paint); // get shape start point
            Redo.Clear(); // Can't redo anymore when added new Drawing element
            switch (CurrentTool)
            {
                case Tool.Brush:
                case Tool.Eraser:
                    if (shape == null)
                    {
                        shape = new Ellipse()
                        {
                            Stroke = (CurrentTool == Tool.Brush) ? new SolidColorBrush((Color)clp_Foreground.SelectedColor) : new SolidColorBrush((Color)clp_Background.SelectedColor),
                            Fill = (CurrentTool == Tool.Brush) ? new SolidColorBrush((Color)clp_Foreground.SelectedColor) : new SolidColorBrush((Color)clp_Background.SelectedColor),
                            Width = BrushSize,
                            Height = BrushSize,
                            StrokeThickness = BrushSize,
                            Margin = new Thickness(startP.X, startP.Y, 0, 0)
                        };
                        cv_Paint.Children.Add(shape);
                        Undo.AddFirst(shape);
                    }
                    break;
                case Tool.Select:
                    if (shape == null)
                    {
                        shape = new Ellipse()
                        {
                            Stroke = new SolidColorBrush((Color)clp_Background.SelectedColor),
                            Fill = new SolidColorBrush((Color)clp_Background.SelectedColor),
                            Width = BrushSize,
                            Height = BrushSize,
                            StrokeThickness = BrushSize,
                            Margin = new Thickness(startP.X, startP.Y, 0, 0)
                        };
                        cv_Paint.Children.Add(shape);
                        Undo.AddFirst(shape);
                    }
                    break;
                case Tool.Retangle:
                case Tool.Triangle:
                case Tool.Ellipse:
                case Tool.Pentagon:
                case Tool.Line:
                case Tool.Text:
                case Tool.Image:           
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
                            Foreground = new SolidColorBrush((Color)clp_Foreground.SelectedColor),
                            FontSize = _textbox.FontSize,
                            FontFamily = _textbox.FontFamily,
                            FontStyle = _textbox.FontStyle,
                            FontWeight = _textbox.FontWeight,
                            TextDecorations = _textbox.TextDecorations
                        };
                        cv_Paint.Children.Add(textblock);
                        Undo.AddFirst(textblock);
                        Canvas.SetLeft(textblock, Canvas.GetLeft(_textbox));
                        Canvas.SetTop(textblock, Canvas.GetTop(_textbox));
                    }
                    cv_Paint.Children.Remove(_textbox);
                    _textbox = null;
                }
            }

        }
        private void cv_Paint_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //get position and generate shape 
                endP = e.GetPosition(cv_Paint);
                double sX, sY, eX, eY;
                sX = Math.Min(startP.X, endP.X);
                sY = Math.Min(startP.Y, endP.Y);
                eX = Math.Max(startP.X, endP.X);
                eY = Math.Max(startP.Y, endP.Y);
                if (shape == null)
                {
                    shape = GenerateShape();
                    cv_Paint.Children.Add(shape);
                    if (CurrentTool != Tool.Text)
                        Undo.AddFirst(shape);
                }
                else
                    switch (CurrentTool)
                    {
                        case Tool.Select:
                        case Tool.Brush:
                        case Tool.Eraser:
                            if (shape is Ellipse)
                            {
                                cv_Paint.Children.Remove(shape);
                                Undo.Remove(shape);
                                shape = new Polyline()
                                {
                                    Stroke = (CurrentTool == Tool.Brush) ? new SolidColorBrush((Color)clp_Foreground.SelectedColor) : new SolidColorBrush((Color)clp_Background.SelectedColor),
                                    StrokeThickness = BrushSize,
                                    Points = new PointCollection()
                                    {
                                        startP,
                                        endP,
                                    }
                                };
                                if (CurrentTool == Tool.Select)
                                {
                                    ((Polyline)shape).StrokeDashArray = new DoubleCollection(new double[] { 4, 3 });
                                    ((Polyline)shape).StrokeThickness = 1;
                                    ((Polyline)shape).Stroke = Brushes.Black;
                                }
                                cv_Paint.Children.Add(shape);
                                Undo.AddFirst(shape);
                                break;
                            }
                            Polyline bpolyline = shape as Polyline;
                            bpolyline.Points.Add(endP);
                            shape = bpolyline;
                            break;
                        case Tool.Image:
                            Rectangle Ishape = shape as Rectangle;
                            Ishape.Width = (eX - sX);
                            Ishape.Height = (eY - sY);
                            Ishape.Margin = new Thickness(sX, sY, 0, 0);
                            shape = Ishape;
                            break;
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
        private void cv_Paint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (CurrentTool)
            {
                case Tool.Select:
                    if (cv_Paint.Children.Contains(shape) && !(shape is Ellipse))
                    {
                        cv_Paint.Children.Remove(shape);
                        Undo.Remove(shape);
                        Polygon polygon = new Polygon()
                        {
                            Stroke = Brushes.Transparent,
                            Points = (shape as Polyline).Points,
                            Fill = new SolidColorBrush((Color)clp_Background.SelectedColor)
                        };
                        cv_Paint.Children.Add(polygon);
                        Undo.AddFirst(polygon);
                    }
                    break;
                case Tool.Text:
                    if (cv_Paint.Children.Contains(shape))
                    {
                        cv_Paint.Children.Remove(shape);
                    }
                    if (_textbox == null)
                    {
                        _textbox = new TextBox()
                        {
                            Width = endP.X < 0 ? 100 : Math.Abs(endP.X - startP.X),
                            Height = endP.X < 0 ? 40 : Math.Abs(endP.Y - startP.Y),
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


                        Canvas.SetLeft(_textbox, endP.X < 0 ? startP.X : Math.Min(startP.X, endP.X));
                        Canvas.SetTop(_textbox, endP.X < 0 ? startP.Y : Math.Min(startP.Y, endP.Y));
                        //_textbox.
                        cv_Paint.Children.Add(_textbox);
                        _textbox.Focus();
                        _textbox.MouseEnter += _textbox_MouseEnter;
                        _textbox.MouseLeave += _textbox_MouseLeave;
                    }
                    break;
            }
            img = null;
            shape = null;
            startP.X = startP.Y = -1;
            endP.X = endP.Y = -1;
        }
        #endregion
        //check mouse in
        private void _textbox_MouseLeave(object sender, MouseEventArgs e)
        {
            isMouseOver = false;
        }

        private void _textbox_MouseEnter(object sender, MouseEventArgs e)
        {
            isMouseOver = true;
        }
        //set properties
        private void Set_FontFamilySource()
        {
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
        }
        private void Set_FontSizeSource()
        {
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
        }

        private void ResetControl()
        {
            btn_Undo.IsActived = btn_Redo.IsActived = btn_Eraser.IsActived
                = btn_Rectangle.IsActived = btn_Ellipse.IsActived = btn_Triangle.IsActived = btn_Pentagon.IsActived
                = btn_Line.IsActived = btn_Brush.IsActived = btn_Text.IsActived = btn_Image.IsActived = btn_Select.IsActived = false;
            btn_Fill.Visibility = cbx_FontFamily.Visibility = Visibility.Hidden;
        }
        //function for Genarating shapes
        private UIElement GenerateShape()
        {
            //get color for fill and outline
            SolidColorBrush foreground = Outline ? new SolidColorBrush((Color)clp_Foreground.SelectedColor) : new SolidColorBrush(Colors.Transparent);
            SolidColorBrush background = Fill ? new SolidColorBrush((Color)clp_Background.SelectedColor) : new SolidColorBrush(Colors.Transparent);

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
                        Stroke = new SolidColorBrush((Color)clp_Foreground.SelectedColor), // line's color is always foreground
                        X1 = startP.X,
                        X2 = endP.X,
                        Y1 = startP.Y,
                        Y2 = endP.Y,
                        StrokeThickness = BrushSize,
                    };
                    break;
                case Tool.Retangle:
                    shape = new Rectangle()
                    {
                        Stroke = foreground,
                        Fill = background,
                        Width = (eX - sX),
                        Height = (eY - sY),
                        StrokeThickness = BrushSize,
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
                        StrokeThickness = BrushSize,
                    };
                    break;
                case Tool.Ellipse:
                    shape = new Ellipse()
                    {
                        Stroke = foreground,
                        Fill = background,
                        Width = (eX - sX),
                        Height = (eY - sY),
                        StrokeThickness = BrushSize,
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
                        StrokeThickness = BrushSize,
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
                case Tool.Image:
                    shape = new Rectangle()
                    {
                        Stroke = Brushes.Transparent,
                        Fill = img,
                        Width = (eX - sX),
                        Height = (eY - sY),
                        Margin = new Thickness(sX, sY, 0, 0)
                    };
                    break;
            }
            return shape;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var anim = new DoubleAnimation(0, Duration3);
            anim.Completed += Exit;
            BeginAnimation(OpacityProperty, anim);
        }
        private void Exit(object sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void PaletteGrip_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
