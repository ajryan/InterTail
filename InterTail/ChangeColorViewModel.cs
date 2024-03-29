using System;
using System.Windows.Media;
using Caliburn.Micro;

namespace InterTail
{
    public class ChangeColorViewModel : Screen
    {
        private readonly TailFile _file;
        private string _color;

        public ChangeColorViewModel(TailFile file)
        {
            _file = file;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            DisplayName = "Change File Color";
        }

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                // TODO: cache this list... or make a real color picker
                foreach (var property in typeof(Colors).GetProperties())
                {
                    if (property.Name.Equals(_color, StringComparison.OrdinalIgnoreCase))
                    {
                        var newFileColor = (Color) property.GetValue(null, null);
                        _file.BgBrush = new SolidColorBrush(newFileColor);
                    }
                }
                NotifyOfPropertyChange(() => Color);
            }
        }
    }
}