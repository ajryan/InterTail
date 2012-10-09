using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Microsoft.Win32;

namespace InterTail
{
    [Export(typeof (TailViewModel))]
    public class TailViewModel : Screen, ITailModel
    {
        private readonly IWindowManager _windowManager;
        private readonly object _linesLock = new object();
        private readonly TailReader _tailReader;
        
        [ImportingConstructor]
        public TailViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            _tailReader = new TailReader(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = "InterTail";
            _tailReader.RunAsync();
        }

        protected override void OnDeactivate(bool close)
        {
            _tailReader.Exit();
            base.OnDeactivate(close);
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        private BindableCollection<TailLine> _lines = new BindableCollection<TailLine>();
        public BindableCollection<TailLine> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                NotifyOfPropertyChange(() => Lines);
            }
        }

        private BindableCollection<TailFile> _files = new BindableCollection<TailFile>();
        public BindableCollection<TailFile> Files
        {
            get { return _files; }
            set
            {
                _files = value;
                NotifyOfPropertyChange(() => Files);
            }
        }

        private TailFile _selectedFile;
        public TailFile SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                NotifyOfPropertyChange(() => SelectedFile);
                NotifyOfPropertyChange(() => CanChangeColor);
            }
        }

        public bool CanChangeColor
        {
            get { return SelectedFile != null; }
        }

        public void ChangeColor()
        {
            if (SelectedFile == null)
                return;

            _windowManager.ShowDialog(new ChangeColorViewModel(SelectedFile));
        }

        public void LoadFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".log",
                Filter = "Log files (.log)|*.log|Text files (.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = true
            };

            bool? selected = openFileDialog.ShowDialog();
            if (selected != true)
                return;

            foreach (var filePath in openFileDialog.FileNames)
            {
                if (Files.Any(f => f.FilePath == filePath))
                    continue;

                var tailFile = new TailFile(filePath);
                Files.Add(tailFile);
                _tailReader.AddFile(tailFile);
            }
        }

        public void Clear()
        {
            lock (_linesLock)
            {
                Lines.Clear();
            }
        }

        public void AddLine(TailLine line)
        {
            AddLines(new []{ line });
        }

        public void AddLines(IEnumerable<TailLine> lines)
        {
            lock (_linesLock)
            {
                Status = "Loading...";

                var sortedList = new List<TailLine>(Lines);
                sortedList.AddRange(lines);
                sortedList.Sort((line1, line2) => line1.Timestamp.CompareTo(line2.Timestamp));

                Lines.IsNotifying = false;
                Lines.Clear();
                Lines.IsNotifying = true;

                Lines.AddRange(sortedList);

                Status = "Idle";
            }
        }
    }
}
