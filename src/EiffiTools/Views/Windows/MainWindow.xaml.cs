// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using DataHarbor.Helpers;
using DataHarbor.Services;
using DataHarbor.ViewModels.Windows;
using System.Globalization;
using Wpf.Ui.Controls;

namespace DataHarbor.Views.Windows
{
    public partial class MainWindow
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow(
            MainWindowViewModel viewModel,
            INavigationService navigationService,
            IServiceProvider serviceProvider,
            ISnackbarService snackbarService,
            IContentDialogService contentDialogService
        )
        {
            Wpf.Ui.Appearance.Watcher.Watch(this);

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            navigationService.SetNavigationControl(NavigationView);
            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
            contentDialogService.SetContentPresenter(RootContentDialog);

            NavigationView.SetServiceProvider(serviceProvider);

            MessageService.SnackbarPresenter = SnackbarPresenter;
            MessageService.ContentPresenter = RootContentDialog;
        }

        //关闭窗体
        private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ConfigHelper.ReadConfig("CloseMode") == "1")
            {
                // 将窗口隐藏并最小化到托盘
                e.Cancel = true;
                this.Visibility = Visibility.Hidden;
                this.WindowState = WindowState.Minimized;
            }
        }

        //显示窗体
        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Topmost = true;
            this.Activate();

            // 将置顶属性重置为 false，在窗口获得焦点时再次激活
            Dispatcher.BeginInvoke(new Action(() => { this.Topmost = false; }));
        }

        //退出程序
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
