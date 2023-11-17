// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using DataHarbor.Models;
using DataHarbor.Services;
using DataHarbor.ViewModels.Windows;
using DataHarbor.Views.Pages;
using DataHarbor.Views.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Wpf.Ui.Common;
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

        //项目实例列表
        [ObservableProperty]
        private ObservableCollection<DataSetProject>? _dataSetProjects;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            ReadProjectList();

            _isInitialized = true;
        }

        //读取数据集项目列表
        private void ReadProjectList()
        {
            Task.Run(() =>
            {
                DataSetProjects = DatabaseService.GetProjectTable("DataSet_Project");
            });
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

        //打开对应的项目
        [RelayCommand]
        private void OpenDataSetProject()
        {
            //打开ProjectDataWindow窗口
            ProjectDataWindow projectDataWindow = new ProjectDataWindow(new ProjectDataWindowViewModel());
            projectDataWindow.Show();
        }

        //删除数据集项目
        [RelayCommand]
        private void DeleteDataSetProject(object parameter)
        {
            DatabaseService.DeleteData("DataSet_Project", ((DataSetProject)parameter).ProjectName);
            DataSetProjects.Remove((DataSetProject)parameter);
            MessageService.AutoShowDialog("成功", "删除成功", ControlAppearance.Success);
        }

        //创建新数据集项目
        [RelayCommand]
        private void NewDataSetProject()
        {
            if (DataCheck())
            {
                if(DatabaseService.IsProjectExist("DataSet_Project", ProjectName))
                {
                    MessageService.AutoShowDialog("错误", "项目已存在", ControlAppearance.Danger);
                }
                else
                {
                    DatabaseService.InsertData("DataSet_Project", ProjectName, ProjectDescribe, 0);
                    DatabaseService.CreateTable(ProjectName);
                    ReadProjectList();
                    ProjectName = "";
                    ProjectDescribe = "";
                    MessageService.AutoShowDialog("成功", "创建成功", ControlAppearance.Success);
                }
            }
            else
            {
                MessageService.AutoShowDialog("错误", "请填写必要数据", ControlAppearance.Danger);
            }
        }
    }
}
