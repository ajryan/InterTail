using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;

namespace InterTail
{
    public class TailReader
    {
        private readonly AutoResetEvent _exitHandle = new AutoResetEvent(false);
        private readonly List<TailFile>  _tailFiles = new List<TailFile>();

        private readonly ITailModel _tailModel;

        public TailReader(ITailModel tailModel)
        {
            _tailModel = tailModel;
        }

        public void AddFile(TailFile tailFile)
        {
            _tailFiles.Add(tailFile);
        }

        public void RunAsync()
        {
            ThreadPool.QueueUserWorkItem(_ => Run());
        }

        public void Exit()
        {
            _exitHandle.Set();
        }

        private void Run()
        {
            try
            {
                do
                {
                    List<TailLine> newLines = new List<TailLine>();
                    foreach (var file in _tailFiles)
                        newLines.AddRange(file.ReadNewLines());

                    if (newLines.Count > 0)
                        _tailModel.AddLines(newLines);
                }
                while (!_exitHandle.WaitOne(100));
            }
            catch (Exception ex)
            {
                var errorLine = new TailLine(
                    DateTime.Now, String.Empty, "Local", Brushes.Red, "Exception reading log: " + ex);
                _tailModel.AddLine(errorLine);
            }
        }
    }
}