using DataHarbor.Helpers;
using DataHarbor.Models;
using DataHarbor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace DataHarbor.ViewModels.Windows
{
    public partial class ProjectDataWindowViewModel : ObservableObject
    {
        //指示当前数据集项目
        private string _projectName;

        //数据集列表
        [ObservableProperty]
        private ObservableCollection<DataItem> _dataItems;

        //文件列表
        [ObservableProperty]
        private string[] _importFiles;      //不含路径，文件名称和后缀

        //数据项目名称
        [ObservableProperty]
        private string _dataItemName;

        //数据项目Ukey
        [ObservableProperty]
        private string _dataItemUkey;

        //数据项目描述
        [ObservableProperty]
        private string _dataItemDescription;

        //数据项目Link
        [ObservableProperty]
        private string _dataItemLink;

        //数据验证
        private bool CheckInput()
        {
            //空值检测
            if(DataItemName == "" || DataItemUkey == "")
            {
                MessageService.AutoShowDialog("错误", "请填写数据项目名称和Ukey", ControlAppearance.Danger);
                return false;
            }
            //唯一值检测
            if (DataItemName == "" || DataItemUkey == "")
            {
                MessageService.AutoShowDialog("错误", "请填写数据项目名称和Ukey", ControlAppearance.Danger);
                return false;
            }
            return true;
        }

        //处理导入的文件
        private void HandleImportFiles(string[] files)
        {
            string[] tempFiles = files;
            int tempIndex = 0;
            foreach(var temp in files)
            {
                tempFiles[tempIndex] = FileHelper.GetFileNameWithExtension(temp);
                tempIndex++;
            }
            ImportFiles = tempFiles;
        }

        //点击添加数据文件
        [RelayCommand]
        private void AddFile()
        {
            //弹窗选取要添加的文件
            var files = (FileHelper.GetFiles()).ToArray();
            HandleImportFiles(files);
        }

        //拖入数据文件
        [RelayCommand]
        private void DragFile(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //获取导入的文件路径
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                HandleImportFiles(files);
            }
        }

        //确认添加数据项目
        [RelayCommand]
        private void AddDataItem()
        {
            //检查输入是否完整
            if (CheckInput())
            {

            }
        }

        //取消添加数据项目
        [RelayCommand]
        private void CancelAddDataItem()
        {

        }

    }
}
