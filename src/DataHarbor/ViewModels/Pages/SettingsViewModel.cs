// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using DataHarbor.Helpers;
using DataHarbor.Services;
using Microsoft.Win32;
using System.Globalization;
using Wpf.Ui.Controls;

namespace DataHarbor.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        //单例模式
        private bool _isInitialized = false;

        //软件主题【0，浅色；1，深色】
        [ObservableProperty]
        private int? _currentTheme;

        //软件语言【0，中文；1，英语(美国)】
        [ObservableProperty]
        private int? _currentLanguage;

        //检查更新【0，False；1，True】
        [ObservableProperty]
        private int? _currentCheckUpdate;

        //开机自启【0，False；1，True】
        [ObservableProperty]
        private int? _currentBooting;

        //退出模式【0，直接退出；1，最小化到托盘】
        [ObservableProperty]
        private int? _currentCloseMode;

        //弹窗模式【0，静默通知；1，弹窗通知】
        [ObservableProperty]
        private int? _currentPopUp;

        //静默通知时间
        [ObservableProperty]
        private int? _currentPopTime;

        //软件版本号
        [ObservableProperty]
        private string _appVersion = String.Empty;


        //导航进来
        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        //导航出去
        public void OnNavigatedFrom() { }

        //初始化
        private void InitializeViewModel()
        {
            //CurrentTheme = Wpf.Ui.Appearance.Theme.GetAppTheme();
            //AppVersion = $"DataHarbor - {GetAssemblyVersion()}";
            ConfigInit();
            _isInitialized = true;
        }

        //获取软件版本
        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        //初始化设置——读取配置文件
        private void ConfigInit()
        {
            CurrentTheme = Convert.ToInt32(ConfigHelper.ReadConfig("Theme"));
            CurrentLanguage = Convert.ToInt32(ConfigHelper.ReadConfig("Language"));
            CurrentCheckUpdate = Convert.ToInt32(ConfigHelper.ReadConfig("AutoCheckUpdate"));
            CurrentBooting = Convert.ToInt32(ConfigHelper.ReadConfig("AutoBoot"));
            CurrentCloseMode = Convert.ToInt32(ConfigHelper.ReadConfig("CloseMode"));
            CurrentPopUp = Convert.ToInt32(ConfigHelper.ReadConfig("NotificationMode"));
            CurrentPopTime = Convert.ToInt32(ConfigHelper.ReadConfig("NotificationTime"));
        }

        //开机自动启动
        public static void SetStartup(bool onOff)
        {
            string appName = "DataHarbor";
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (onOff)
                rk.SetValue(appName, appPath);
            else
                rk.DeleteValue(appName, false);
        }

        //改变主题
        [RelayCommand]
        private void OnChangeTheme()
        {
            if (CurrentTheme != null)
            {
                if(CurrentTheme == 0)
                {
                    //改变主题
                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light);
                }
                else
                {
                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark);
                }
                //写入配置文件
                ConfigHelper.WriteConfig("Theme", CurrentTheme.ToString());
            }
        }

        //改变语言
        [RelayCommand]
        private void OnChangeLanguage()
        {
            if (CurrentLanguage != null)
            {
                if (CurrentLanguage == 0)
                {
                    //改变语言
                    LanguageService.Instance.ChangeLanguage(new CultureInfo("zh-CN"));
                }
                else
                {
                    LanguageService.Instance.ChangeLanguage(new CultureInfo("en-US"));
                }
                //写入配置文件
                ConfigHelper.WriteConfig("Language", CurrentLanguage.ToString());
            }
        }

        //自动检查更新
        [RelayCommand]
        private void OnChangeAutoCheckUpdate()
        {
            ConfigHelper.WriteConfig("AutoCheckUpdate", CurrentCheckUpdate.ToString());
        }

        //开机自启
        [RelayCommand]
        private void OnChangeAutoBooting()
        {
            //设置开机自启
            SetStartup(Convert.ToBoolean(CurrentBooting));
            ConfigHelper.WriteConfig("AutoBoot", CurrentBooting.ToString());
        }

        //退出模式
        [RelayCommand]
        private void OnChangeCloseMode()
        {
            if (CurrentCloseMode != null)
            {
                //写入配置文件
                ConfigHelper.WriteConfig("CloseMode", CurrentCloseMode.ToString());
            }
        }

        //弹窗模式
        [RelayCommand]
        private void OnChangePopUpMode()
        {
            if (CurrentPopUp != null)
            {
                //写入配置文件
                ConfigHelper.WriteConfig("NotificationMode", CurrentPopUp.ToString());
            }
        }

        //静默通知自动关闭时间
        [RelayCommand]
        private void OnChangePopUpTime()
        {
            if (CurrentPopTime != null)
            {
                //写入配置文件
                ConfigHelper.WriteConfig("NotificationTime", CurrentPopTime.ToString());
            }
        }

        //检查更新
        [RelayCommand]
        private async void CheckUpdate()
        {
            bool temp = await UpdateService.CheckUpdateAsync();
            if (temp)
            {
                MessageService.AutoShowDialog("更新", "检测到新版本", ControlAppearance.Info);
            }
        }
    }
}
