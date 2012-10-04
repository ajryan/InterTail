using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;

namespace InterTail
{
    public class TailReader
    {
        // TODO: need one thread across all files
        // TODO: one tailreader that has a collection of TailFile

        // TODO: app.config
        private static readonly Regex _dateRegex = new Regex(
            @"\d{1,2}/\d{1,2}/\d{2,4} \d{1,2}:\d\d:\d\d (AM|PM)",
            RegexOptions.Compiled|RegexOptions.IgnoreCase);

        private readonly AutoResetEvent _exitHandle = new AutoResetEvent(false);

        private readonly string _filePath;
        private readonly string _fileFolder;
        private readonly string _fileName;
        private readonly Brush _backgroundBrush;

        private ITailModel _tailModel;

        public TailReader(string filePath)
        {
            _filePath = filePath;

            _fileFolder = Path.GetDirectoryName(_filePath);
            _fileFolder = _fileFolder.Substring(_fileFolder.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            _fileName = Path.GetFileName(_filePath);

            var rnd = RandomNumberGenerator.Create();
            byte[] rgb = new byte[3];
            rnd.GetBytes(rgb);
            
            _backgroundBrush = new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
            _backgroundBrush.Freeze();
        }

        public Brush BgBrush
        {
            get { return _backgroundBrush; }
        }

        public void Run(ITailModel tailModel)
        {
            _tailModel = tailModel;
            ThreadPool.QueueUserWorkItem(_ => Run());
        }

        public void Exit()
        {
            _exitHandle.Set();
        }

        private void Run()
        {
            using (var streamReader = 
                new StreamReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                try
                {
                    RunUnsafe(streamReader);
                }
                catch (Exception ex)
                {
                    var errorLine = new TailLine(
                        DateTime.Now, String.Empty, "Local", Brushes.Red, "Exception reading log: " + ex);
                    _tailModel.AddLine(errorLine);
                }
            }
        }

        private void RunUnsafe(StreamReader streamReader)
        {
            long lastMaxOffset = 0;
            var lastLineDateTime = DateTime.MinValue;

            // TODO: wait handle for termination
            do
            {
                // use >= in case file was truncated or lines were removed
                if (lastMaxOffset >= streamReader.BaseStream.Length)
                    continue;

                streamReader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                List<TailLine> lines = new List<TailLine>();
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    var dateMatch = _dateRegex.Match(line);
                    if (dateMatch.Success)
                        lastLineDateTime = DateTime.Parse(dateMatch.Groups[0].Captures[0].ToString());

                    var tailLine = new TailLine(
                        lastLineDateTime,
                        _fileFolder,
                        _fileName,
                        _backgroundBrush,
                        line);

                    lines.Add(tailLine);
                }
                _tailModel.AddLines(lines);

                lastMaxOffset = streamReader.BaseStream.Position;
            }
            while (!_exitHandle.WaitOne(100));
        }
    }
}