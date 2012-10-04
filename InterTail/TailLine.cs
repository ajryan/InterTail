using System;
using System.Windows.Media;
using Caliburn.Micro;

namespace InterTail
{
    public class TailLine : PropertyChangedBase
    {
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                NotifyOfPropertyChange(() => Timestamp);
            }
        }

        private string _folder;
        public string Folder
        {
            get { return _folder; }
            set
            {
                _folder = value;
                NotifyOfPropertyChange(() => Folder);
            }
        }

        private string _file;
        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                NotifyOfPropertyChange(() => File);
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        private Brush _bgColor;
        public Brush BgColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                NotifyOfPropertyChange(() => BgColor);
            }
        }

        public TailLine(DateTime timeStamp, string folder, string file, Brush backgroundBrush, string text)
        {
            Timestamp = timeStamp;
            Folder = folder;
            File = file;
            BgColor = backgroundBrush;
            BgColor.Freeze();
            Text = text;
        }
    }
}