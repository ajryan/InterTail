using System.Collections.Generic;

namespace InterTail
{
    public interface ITailModel
    {
        void AddLine(TailLine line);
        void AddLines(IEnumerable<TailLine> lines);
    }
}