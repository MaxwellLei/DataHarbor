using DataHarbor.ViewModels.Pages;
using DataHarbor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using DataHarbor.Services;
using DataHarbor.Helpers;
using Button = Wpf.Ui.Controls.Button;

namespace DataHarbor.Views.Pages
{
    /// <summary>
    /// MappingDataPage.xaml 的交互逻辑
    /// </summary>
    public partial class MappingDataPage : INavigableView<MappingDataViewModel>
    {
        public MappingDataViewModel ViewModel { get; }

        public MappingDataPage(MappingDataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            //拖拽读取
            Drop += File_Drop_Read;
        }

        //拖放读取文件路径
        private void File_Drop_Read(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (FileHelper.IsExcelFile(file))
                    {
                        ViewModel.FileNames.Clear();
                        ViewModel.SelectedFiles.Add(file);
                    }
                    else
                    {
                        MessageService.AutoShowDialog("警告", "请选择Excel文件", ControlAppearance.Danger);
                        ViewModel.IsSeeTip = Visibility.Visible;
                        return;
                    }

                }
                ViewModel.IsSeeTip = Visibility.Hidden;

                //添加姓名
                foreach (var tempName in FileHelper.GetFileNames(ViewModel.SelectedFiles))
                {
                    MappingFile temp = new MappingFile();
                    temp.CurrentFileIndex = "(" + (FileHelper.GetFileNames(ViewModel.SelectedFiles).IndexOf(tempName) + 1) + ")";
                    temp.CurrentFileName = tempName;
                    temp.NewFileName = tempName;
                    ViewModel.FileNames.Add(temp);
                }

            }
        }

        //选择框
        private void ComboBox_Selected(object sender, RoutedEventArgs e)
        {
            if (ViewModel.TempIndex != "0")
            {
                string folderPath = FileHelper.GetFolderPath();
                if (folderPath == "")
                {
                    ViewModel.TempIndex = "0";
                }
                else
                {
                    ViewModel.CustomFilePath = folderPath;
                }
            }
            else
            {
                ViewModel.CustomFilePath = "自定义";
            }
        }

        //选择文件
        private void Select_Button_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<string> templist = FileHelper.GetExcelFiles();
            if (templist != null)
            {
                if (ViewModel.SelectedFiles.Count != 0 || ViewModel.SelectedFiles == null)
                {
                    foreach (var tempName in templist)
                    {
                        ViewModel.SelectedFiles.Add(tempName);
                    }
                    ViewModel.FileNames.Clear();
                }
                else
                {
                    ViewModel.SelectedFiles = templist;
                }
                //添加姓名
                foreach (var tempName in FileHelper.GetFileNames(ViewModel.SelectedFiles))
                {
                    MappingFile temp = new MappingFile();
                    temp.CurrentFileIndex = "(" + (FileHelper.GetFileNames(ViewModel.SelectedFiles).IndexOf(tempName) + 1) + ")";
                    temp.CurrentFileName = tempName;
                    temp.NewFileName = tempName;
                    ViewModel.FileNames.Add(temp);
                }
                ViewModel.IsSeeTip = Visibility.Hidden;
            }
        }

        //向上移动选项
        private void MoveUpTab(object sender, RoutedEventArgs e)
        {
            ViewModel.MoveUpTab(sender);
        }

        //向下移动选项
        private void MoveDownTab(object sender, RoutedEventArgs e)
        {
            ViewModel.MoveDownTab(sender);
        }

        //删除文件
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            // 获取选择的文件名称
            string name = ((MappingFile)((Button)sender).DataContext).CurrentFileName;
            int index = -1;
            foreach (var temp in ViewModel.FileNames)
            {
                if (temp.CurrentFileName == name)
                {
                    //获取删除文件索引
                    index = ViewModel.FileNames.IndexOf(temp);
                    break;
                }
                index++;
            }
            if (index != -1)
            {
                //删除指定列表文件
                ViewModel.SelectedFiles.RemoveAt(index);
                ViewModel.FileNames.Clear();
                //更新文件名称列表
                if (ViewModel.SelectedFiles.Count != 0)
                {
                    //添加姓名
                    foreach (var tempName in FileHelper.GetFileNames(ViewModel.SelectedFiles))
                    {
                        MappingFile temp = new MappingFile();
                        temp.CurrentFileIndex = "(" + (FileHelper.GetFileNames(ViewModel.SelectedFiles).IndexOf(tempName) + 1) + ")";
                        temp.CurrentFileName = tempName;
                        temp.NewFileName = tempName;
                        ViewModel.FileNames.Add(temp);
                    }
                }
                if (ViewModel.SelectedFiles.Count == 0)
                {
                    ViewModel.IsSeeTip = Visibility.Visible;
                }
            }
            else
            {
                //MessageService.AutoShowDialog(LanguageHelper.GetLanguageString("Error"),
                //                            LanguageHelper.GetLanguageString("IndexError")
                //                            , ControlAppearance.Danger);
            }

        }
    }
}
