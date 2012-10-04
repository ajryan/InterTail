using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace InterTail
{
    public static class CaliburnExtensions
    {
        public static AggregateCatalog GetComposablePartAggregateCatalog(this IObservableCollection<Assembly> assemblySource)
        {
            return new AggregateCatalog(
                assemblySource.Select(a => new AssemblyCatalog(a)).OfType<ComposablePartCatalog>());
        }
    }
}
