// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using DataHarbor.Helpers;
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

        //获取数据库的位置
        private string GetProjectDataLocation()
        {
            string tempLocation;
            if (ConfigHelper.ReadConfig("DataStorageLoaction") == "0")
            {
                //获取数据项文件夹
                tempLocation = FileHelper.GetRelativePath() + "\\Data\\" + _projectName;
            }
            else
            {
                //获取数据项文件夹
                tempLocation = ConfigHelper.ReadConfig("DataStorageLoaction") + "\\Data\\" + _projectName;
            }
            return tempLocation;
        }

        //数据检验
        private bool DataCheck()
        {
            //项目名称不为空
            if (ProjectName == "" && ProjectDescribe =="")
            {
                MessageService.AutoShowDialog("错误", "请填写项目名称和描述", ControlAppearance.Danger);
                return false;
            }
            //名称第一个字符检测，是否为数字
            if (ProjectName[0] >= '0' && ProjectName[0] <= '9')
            {
                MessageService.AutoShowDialog("错误", "数据项目名称首字符不能为数字", ControlAppearance.Danger);
                return false;
            }
            //检测项目是否在数据库中存在
            if (DatabaseService.IsProjectExist("DataSet_Project", ProjectName))
            {
                MessageService.AutoShowDialog("错误", "数据项目名称已存在", ControlAppearance.Danger);
                return false;
            }
            return true;
        }

        //打开数据库所在的位置
        [RelayCommand]
        private void OpenDatabaseLocation()
        {
            FileHelper.OpenFolder(GetProjectDataLocation());
        }

        //打开对应的项目
        [RelayCommand]
        private void OpenDataSetProject(object parameter)
        {
            //打开ProjectDataWindow窗口
            ProjectDataWindow projectDataWindow = new ProjectDataWindow(new ProjectDataWindowViewModel(((DataSetProject)parameter).ProjectName));
            projectDataWindow.Show();
        }

        //删除数据集项目
        [RelayCommand]
        private async void DeleteDataSetProject(object parameter)
        {
            var temp = await MessageService.ShowContentDialog("警告", "是否删除项目：" + ((DataSetProject)parameter).ProjectName + "。删除后将会清空项目及其数据");
            //删除二次确认
            if(temp == ContentDialogResult.Primary)
            {
                //在数据集项目列表删除项目
                DatabaseService.DeleteDataProject("DataSet_Project", ((DataSetProject)parameter).ProjectName);
                //移除前端数据集项目表
                DataSetProjects.Remove((DataSetProject)parameter);
                //删除对应的数据集项目表
                DatabaseService.DeleteTable(((DataSetProject)parameter).ProjectName);
                //删除项目文件夹
                FileHelper.DeleteFolder(GetProjectDataLocation() + "\\" + ((DataSetProject)parameter).ProjectName);
                MessageService.AutoShowDialog("成功", "删除成功", ControlAppearance.Success);
            }
        }

        //创建新数据集项目
        [RelayCommand]
        private void NewDataSetProject()
        {
            if (DataCheck())
            {
                //数据集项目列表添加新项目
                DatabaseService.InsertDataProject("DataSet_Project", ProjectName, ProjectDescribe, 0);
                //创建对于数据集项目的表
                DatabaseService.CreateTable(ProjectName);
                //文件夹路径检测
                FileHelper.CreateFolder(FileHelper.GetRelativePath() + "\\Data\\" + ProjectName);
                //刷新前端列表
                ReadProjectList();
                //清空输入框
                ProjectName = "";
                ProjectDescribe = "";
                MessageService.AutoShowDialog("成功", "创建成功", ControlAppearance.Success);
            }
        }
    }
}
