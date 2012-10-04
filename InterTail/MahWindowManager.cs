using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace InterTail
{
    public class MahWindowManager : WindowManager
    {
        private static readonly ResourceDictionary[] _resources;

        static MahWindowManager()
        {
            _resources = new ResourceDictionary[]
            {
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.AnimatedTabControl.xaml", UriKind.RelativeOrAbsolute) }
            };
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var metroWindow = view as MetroWindow;
            if (metroWindow == null)
            {
                var userControl = view as UserControl;
                double minHeight = 150D;
                double minWidth = 500D;
                double? height = null;
                double? width = null;
                if (userControl != null)
                {
                    minHeight = userControl.MinHeight;
                    minWidth = userControl.MinWidth;
                    height = userControl.Height;
                    width = userControl.Width;
                }

                metroWindow = new MetroWindow
                {
                    Content = view,
                    SizeToContent = SizeToContent.Manual,
                    MinHeight = minHeight,
                    MinWidth = minWidth,
                    SaveWindowPosition = true
                };
                if (height.HasValue && width.HasValue)
                {
                    metroWindow.Height = height.Value;
                    metroWindow.Width = width.Value;
                }

                foreach (var resourceDict in _resources)
                {
                    metroWindow.Resources.MergedDictionaries.Add(resourceDict);
                }

                metroWindow.SetValue(View.IsGeneratedProperty, true);
                var owner = this.InferOwnerOf(metroWindow);
                if (owner != null)
                {
                    metroWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    metroWindow.Owner = owner;
                }
                else
                {
                    metroWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            else
            {
                var owner = this.InferOwnerOf(metroWindow);
                if (owner != null && isDialog)
                    metroWindow.Owner = owner;
            }

            return metroWindow;
        }
    }
}
