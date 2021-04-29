using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Paint
{
    class MainViewModel:BaseViewModel
    {
        public enum Sections
        {
            Painting,
            Undoing,
            Redoing,
        }
        //Properties
        public static readonly DependencyProperty MyFontFamilyProperty =
        DependencyProperty.Register("MyFontFamily",
        typeof(FontFamily), typeof(MainWindow), new UIPropertyMetadata(null));

        private LinkedList<UIElement> _Undo;
        public LinkedList<UIElement> Undo {
            get => _Undo;
            set
            {
                _Undo = value;
                OnPropertyChanged("Undo");
            }
        }
        private LinkedList<UIElement> _Redo;
        public LinkedList<UIElement> Redo
        {
            get => _Redo;
            set
            {
                _Redo = value;
                OnPropertyChanged("Redo");
            }
        }

        private LinkedList<UIElement> _InkStrokes;
        public LinkedList<UIElement> InkStrokes
        {
            get => _InkStrokes;
            set
            {
                _InkStrokes = value;
                OnPropertyChanged("StrokesChanged");
            }
        }

        private Sections _section;
        public Sections section
        {
            get => _section;
            set => _section = value;
        }

        //Command
        public ICommand UndoCommand { get; set; }
        public ICommand RedoCommand { get; set; }
        public MainViewModel()
        {
            Undo = new LinkedList<UIElement>();
            Redo = new LinkedList<UIElement>();
            InkStrokes = new LinkedList<UIElement>();
            section = Sections.Painting;
            //Undo command Implementation
            UndoCommand = new RelayCommand<object>((p) =>
            {
                if (Undo.Count() == 0)
                    return false;
                return true;
            }, (p) =>
            {
                Debug.WriteLine(Undo.Count());
                UIElement _undo = Undo.First();
                _section = Sections.Undoing;
                InkStrokes.Remove(_undo);
                Undo.RemoveFirst();
                Redo.AddFirst(_undo);
            });

            //Redo command Implementation
            RedoCommand = new RelayCommand<object>((p) =>
            {
                if (Redo.Count() == 0)
                    return false;
                return true;
            }, (p) =>
            {
                UIElement _redo = Redo.First();
                section = Sections.Redoing;
                //InkStrokes.Add(_redo);
                Redo.RemoveFirst();
                Undo.AddFirst(_redo);
            });
        }
    }
}
