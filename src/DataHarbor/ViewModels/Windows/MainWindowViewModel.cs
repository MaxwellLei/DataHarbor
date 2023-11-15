// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using DataHarbor.Views.Pages;
using System.Collections.ObjectModel;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace DataHarbor.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "WPF UI - DataHarbor";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            //多栏
            new NavigationViewItem("Data", SymbolRegular.DataHistogram24, typeof(DashboardPage))
            {
                MenuItems = new object[]
                {
                    new NavigationViewItem(nameof(Snackbar), typeof(DataPage)),
                    new NavigationViewItem(nameof(ContentDialog), typeof(DashboardPage)),
                }   
            },
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
