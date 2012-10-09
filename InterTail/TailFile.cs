using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;
using Caliburn.Micro;

namespace InterTail
{
    public class TailFile : PropertyChangedBase, IDisposable
    {
        // TODO: app.config
        private static readonly Regex _dateRegex = new Regex(
            @"\d{1,2}/\d{1,2}/\d{2,4} \d{1,2}:\d\d:\d\d (AM|PM)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string _filePath;
        private readonly string _fileFolder;
        private readonly string _fileName;
        private readonly StreamReader _reader;
        private readonly object _readLock = new object();

        private readonly List<TailLine> _tailLines = new List<TailLine>();

        private long _lastMaxOffset = 0;
        private DateTime _lastLineDateTime = DateTime.MinValue;
        private Brush _bgBrush;

        public TailFile(string filePath)
        {
            _filePath = filePath;

            _fileFolder = Path.GetDirectoryName(_filePath);
            _fileFolder = _fileFolder.Substring(_fileFolder.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _fileName = Path.GetFileName(_filePath);

            _reader = new StreamReader(
                new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            _bgBrush = GetRandomBgBrush();
        }

        private static Brush GetRandomBgBrush()
        {
            var rnd = RandomNumberGenerator.Create();
            byte[] rgb = new byte[3];
            rnd.GetBytes(rgb);

            var brush = new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
            brush.Freeze();

            return brush;
        }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                NotifyOfPropertyChange(() => FilePath);
            }
        }

        public Brush BgBrush
        {
            get { return _bgBrush; }
            set
            {
                _bgBrush = value;
                foreach (var line in _tailLines)
                    line.BgBrush = _bgBrush;

                NotifyOfPropertyChange(() => BgBrush);
            }
        }

        public List<TailLine> ReadNewLines()
        {
            List<TailLine> newLines = new List<TailLine>();

            if (!Monitor.TryEnter(_readLock))
                return newLines;

            try
            {
                // use >= in case file was truncated or lines were removed
                if (_lastMaxOffset >= _reader.BaseStream.Length)
                    return newLines;

                _reader.BaseStream.Seek(_lastMaxOffset, SeekOrigin.Begin);

                string line;
                while ((line = _reader.ReadLine()) != null)
                {
                    var dateMatch = _dateRegex.Match(line);
                    if (dateMatch.Success)
                        _lastLineDateTime = DateTime.Parse(dateMatch.Groups[0].Captures[0].ToString());

                    var tailLine = new TailLine(
                        _lastLineDateTime,
                        _fileFolder,
                        _fileName,
                        _bgBrush,
                        line);

                    _tailLines.Add(tailLine);
                    newLines.Add(tailLine);
                }

                _lastMaxOffset = _reader.BaseStream.Position;

                return newLines;
            }
            finally
            {
                Monitor.Exit(_readLock);
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}