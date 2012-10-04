using System;
using System.Windows.Media;
using Caliburn.Micro;

namespace InterTail
{
    // TODO: Each TailFile has a collection of its lines
    public class TailFile : PropertyChangedBase
    {
        private string _fileName;
        private Brush _bgColor;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                NotifyOfPropertyChange(() => FileName);
            }
        }

        public Brush BgColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                NotifyOfPropertyChange(() => BgColor);
            }
        }

        public void ChangeColor()
        {
            throw new NotImplementedException();
        }
    }
}