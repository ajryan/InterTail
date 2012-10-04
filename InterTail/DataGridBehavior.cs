using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace InterTail
{
    public class DataGridBehavior
    {
        private static readonly Dictionary<DataGrid, Capture> _associations = new Dictionary<DataGrid, Capture>();

        public static readonly DependencyProperty ScrollOnNewItemProperty = 
            DependencyProperty.RegisterAttached(
                "ScrollOnNewItem", 
                typeof (bool),
                typeof (DataGridBehavior),
                new UIPropertyMetadata(false, OnScrollOnNewItemChanged));

        public static bool GetScrollOnNewItem(DependencyObject obj)
        {
            return (bool) obj.GetValue(ScrollOnNewItemProperty);
        }

        public static void SetScrollOnNewItem(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollOnNewItemProperty, value);
        }

        public static void OnScrollOnNewItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as DataGrid;
            if (dataGrid == null)
                return;

            bool oldValue = (bool) e.OldValue;
            bool newValue = (bool) e.NewValue;
            if (newValue == oldValue)
                return;

            if (newValue)
            {
                dataGrid.Loaded += DataGrid_Loaded;
                dataGrid.Unloaded += DataGrid_Unloaded;
            }
            else
            {
                dataGrid.Loaded -= DataGrid_Loaded;
                dataGrid.Unloaded -= DataGrid_Unloaded;
            }
        }

        private static void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var dataGrid = (DataGrid) sender;
            var itemsNotify = dataGrid.Items as INotifyCollectionChanged;
            if (itemsNotify == null)
                return;

            dataGrid.Loaded -= DataGrid_Loaded;
            _associations[dataGrid] = new Capture(dataGrid);
        }

        private static void DataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            var dataGrid = (DataGrid) sender;
            if (_associations.ContainsKey(dataGrid))
                _associations[dataGrid].Dispose();
            dataGrid.Unloaded -= DataGrid_Unloaded;
        }

        private class Capture : IDisposable
        {
            private readonly DataGrid _dataGrid;
            private readonly INotifyCollectionChanged _itemsNotify;
            private readonly IList _itemsList;
 
            public Capture(DataGrid dataGrid)
            {
                _dataGrid = dataGrid;
                _itemsNotify = dataGrid.ItemsSource as INotifyCollectionChanged;
                _itemsList = dataGrid.ItemsSource as IList;

                if (_itemsNotify != null)
                {
                    _itemsNotify.CollectionChanged += ItemsNotify_CollectionChanged;
                }
            }

            void ItemsNotify_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                object bottomItem = null;

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    bottomItem = e.NewItems[0];
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (_dataGrid.Items.Count < 0)
                    {
                        bottomItem = _dataGrid.Items[_dataGrid.Items.Count - 1];
                    }
                    else
                    {
                        if (_itemsList != null && _itemsList.Count > 0)
                            bottomItem = _itemsList[_itemsList.Count - 1];
                    }
                }

                if (bottomItem != null)
                {
                    _dataGrid.ScrollIntoView(bottomItem);
                    _dataGrid.SelectedItem = bottomItem;
                }
            }

            public void Dispose()
            {
                if (_itemsNotify != null)
                    _itemsNotify.CollectionChanged -= ItemsNotify_CollectionChanged;
            }
        }
    }
}
