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

        private Brush _bgBrush;
        public Brush BgBrush
        {
            get { return _bgBrush; }
            set
            {
                _bgBrush = value;
                NotifyOfPropertyChange(() => BgBrush);
            }
        }

        public TailLine(DateTime timeStamp, string folder, string file, Brush backgroundBrush, string text)
        {
            Timestamp = timeStamp;
            Folder = folder;
            File = file;
            BgBrush = backgroundBrush;
            Text = text;
        }
    }
}