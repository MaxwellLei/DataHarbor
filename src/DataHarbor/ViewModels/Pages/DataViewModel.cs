// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using DataHarbor.Models;
using DataHarbor.Services;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace DataHarbor.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        //单例模式
        private bool _isInitialized = false;

        //数据集项目名称
        [ObservableProperty]
        private string _projectName;

        //数据集项目描述
        [ObservableProperty]
        private string _projectDescribe;

        [ObservableProperty]
        private IEnumerable<DataColor> _colors;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            var random = new Random();
            var colorCollection = new List<DataColor>();

            for (int i = 0; i < 16; i++)
                colorCollection.Add(
                    new DataColor
                    {
                        Color = new SolidColorBrush(
                            Color.FromArgb(
                                (byte)200,
                                (byte)random.Next(0, 250),
                                (byte)random.Next(0, 250),
                                (byte)random.Next(0, 250)
                            )
                        )
                    }
                );

            Colors = colorCollection;

            _isInitialized = true;
        }

        //数据检验
        private bool DataCheck()
        {
            //项目名称不为空
            if (ProjectName != "" && ProjectDescribe !="")
            {
                return true;
            }
            return false;
        }

        //创建新数据集项目
        [RelayCommand]
        private void NewDataSetProject()
        {
            if (DataCheck())
            {
                DatabaseService.InsertData("DataSet_Project",ProjectName,ProjectDescribe,0);
                MessageService.AutoShowDialog("成功","创建成功",ControlAppearance.Success);
            }
            else
            {
                MessageService.AutoShowDialog("错误", "请填写必要数据", ControlAppearance.Danger);
            }
        }
    }
}
