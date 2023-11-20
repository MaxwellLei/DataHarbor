// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using DataHarbor.Helpers;
using DataHarbor.Services;
using DataHarbor.Views.Pages;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace DataHarbor.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "EiffiTools";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new ()
        {
            new NavigationViewItem()
            {
                Content = LanguageService.Instance["Home_Page"],
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = LanguageService.Instance["MappingData_Page"],
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.MappingDataPage)
            },
            new NavigationViewItem()
            {
                Content = LanguageService.Instance["Data_Page"],
                Icon = new SymbolIcon { Symbol = SymbolRegular.DatabaseWindow20 },
                TargetPageType = typeof(Views.Pages.DataPage)
            },
            //多栏
            //new NavigationViewItem(LanguageService.Instance["Data_Page"], SymbolRegular.DataHistogram24, typeof(DataPage))
            //{
            //    MenuItems = new object[]
            //    {
            //        new NavigationViewItem(nameof(Snackbar), typeof(DashboardPage)),
            //    }
            //},
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = LanguageService.Instance["Settings"],
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
