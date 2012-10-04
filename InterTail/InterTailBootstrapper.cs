using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Caliburn.Micro;

namespace InterTail
{
    public class InterTailBootstrapper : Bootstrapper<TailViewModel>
    {
        private CompositionContainer _container;

        protected override void Configure()
        {
            _container = new CompositionContainer(AssemblySource.Instance.GetComposablePartAggregateCatalog());

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new MahWindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);

            _container.Compose(batch);
        }

        protected override object GetInstance(Type service, string key)
        {
            string contract = string.IsNullOrEmpty(key)
                ? AttributedModelServices.GetContractName(service)
                : key;

            var exports = _container.GetExportedValues<object>(contract).ToList();
            if (exports.Any())
            {
                return exports.First();
            }

            return base.GetInstance(service, key);
        }
    }
}
