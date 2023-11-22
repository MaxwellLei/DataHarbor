// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.
using DataHarbor.Services;
using EiffiTools.Helpers;
using System.Globalization;

namespace DataHarbor.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        public DashboardViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private void OnCardClick(string parameter)
        {
            if (String.IsNullOrWhiteSpace(parameter))
            {
                MessageService.AutoShowDialog("提示", "功能开发中...", Wpf.Ui.Controls.ControlAppearance.Info);
                return;
            }

            Type? pageType = NameToPageTypeConverter.Convert(parameter);

            if (pageType == null)
            {
                MessageService.AutoShowDialog("错误", "未找到页面", Wpf.Ui.Controls.ControlAppearance.Danger);
                return;
            }

            _ = _navigationService.Navigate(pageType);
        }
    }
}
