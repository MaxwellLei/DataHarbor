using DataHarbor.Helpers;
using DataHarbor.Models;
using DataHarbor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace DataHarbor.ViewModels.Windows
{
    public partial class ProjectDataWindowViewModel : ObservableObject
    {
        //是否处于编辑模型
        private bool _isEdit = false;
        //数据库位置
        private string _projectDataLocation = string.Empty;

        //遮罩文本
        [ObservableProperty]
        private string _infoText;

        //是否创建数据项中
        [ObservableProperty]
        private bool _isCreateDataItem;

        //是否拖动
        [ObservableProperty]
        private bool _isDragOver;

        //指示当前数据集项目
        [ObservableProperty]
        private string _projectName;

        //数据集列表
        [ObservableProperty]
        private ObservableCollection<DataItem> _dataItems;

        //文件列表
        [ObservableProperty]
        private string[] _importFiles;      //不含路径，文件名称和后缀
        private string[] _importFilesC;      //完整路径

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

        //状态栏标题
        [ObservableProperty]
        private string _title;

        //状态栏消息
        [ObservableProperty]
        private string _message;

        //状态栏是否开启
        [ObservableProperty]
        private bool _isInfoBarOpen;

        //状态栏类型
        [ObservableProperty]
        private InfoBarSeverity _infoBarSeverity;

        public ProjectDataWindowViewModel(string projectName)
        {
            //获取当前项目名称
            ProjectName = projectName;
            //读取数据库中的数据
            ReadDataItems();
            //获取数据库位置
            _projectDataLocation = GetProjectDataLocation();
            IsDragOver = false;
            IsCreateDataItem = false;
        }

        //状态栏消息通知
        private void SendMessage(string title,string message,InfoBarSeverity? infoBarSeverity)
        {
            Message = message;
            Title = title;
            InfoBarSeverity = infoBarSeverity ?? InfoBarSeverity.Informational;
            IsInfoBarOpen = true;
            Task.Run(() =>
            {
                Thread.Sleep(3500);
                IsInfoBarOpen = false;
            });
            
        }

        //获取数据库的位置
        private string GetProjectDataLocation()
        {
            string tempLocation;
            if (ConfigHelper.ReadConfig("DataStorageLoaction") == "0")
            {
                //获取数据项文件夹
                tempLocation = FileHelper.GetRelativePath() + "\\Data\\" + _projectName + "\\";
            }
            else
            {
                //获取数据项文件夹
                tempLocation = ConfigHelper.ReadConfig("DataStorageLoaction") + "\\Data\\" + _projectName + "\\";
            }
            return tempLocation;
        }

        //清空控件数据
        private void ClearInput()
        {
            DataItemName = "";
            DataItemUkey = "";
            DataItemDescription = "";
            DataItemLink = "";
            ImportFiles = null;
            _importFilesC = null;
        }

        //数据验证
        private bool CheckInput()
        {
            if(DataItemName ==null || DataItemUkey == null)
            {
                SendMessage("错误", "请填写数据项目名称和 Ukey", InfoBarSeverity.Error);
                return false;
            }
            //空值检测
            if(DataItemName == "" || DataItemUkey == "" && ImportFiles == null)
            {
                SendMessage("错误", "请填写数据项目名称和 Ukey", InfoBarSeverity.Error);
                return false;
            }
            //空文件检测
            if (ImportFiles == null)
            {
                SendMessage("错误", "请添加添加数据文件", InfoBarSeverity.Error);
                return false;
            }
            //唯一值检测
            if (DatabaseService.IsUkeyExist(_projectName, DataItemUkey))
            {
                SendMessage("错误", "该数据Ukey已存在", InfoBarSeverity.Error);
                return false;
            }
            return true;
        }

        //读取项目中的数据
        private void ReadDataItems()
        {
            Task.Run(() =>
            {
                IsCreateDataItem = true;
                InfoText = "加载中...";
                DataItems = DatabaseService.GetProjectData(_projectName);
                IsCreateDataItem = false;
            });
        }

        //处理导入的文件——导入后端文件列表
        private void HandleImportFiles(string[] files)
        {
            //后端文件列表
            _importFilesC = new string[files.Length];
            for(int i = 0; i< files.Length; i++)
            {
                _importFilesC[i] = files[i];
            }

            //前端文件列表
            string[] tempFiles = files;
            int tempIndex = 0;
            foreach(var temp in files)
            {
                tempFiles[tempIndex] = FileHelper.GetFileNameWithExtension(temp);
                tempIndex++;
            }
            ImportFiles = tempFiles;
        }

        //存储数据文件
        private void SaveDataFiles()
        {
            //获取数据项文件夹
            var tempLocation = _projectDataLocation + DataItemUkey;
            FileHelper.CreateFolder(tempLocation);
            //存储在默认位置
            if (ConfigHelper.ReadConfig("StorageMode") == "0")
            {
                //复制文件
                FileHelper.CopyFiles(_importFilesC, tempLocation);
            }
            else
            {
                //剪切文件
                FileHelper.MoveFiles(_importFilesC, tempLocation);
            }
        }


        //点击添加数据文件
        [RelayCommand]
        private void AddFile()
        {
            //弹窗选取要添加的文件
            var files = FileHelper.GetFiles();
            if(files != null)
            {
                HandleImportFiles(files.ToArray());
                _isEdit = false;
            }
        }

        //拖入数据文件
        [RelayCommand]
        private void DragFile(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                IsDragOver = false;
                //获取导入的文件路径
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                HandleImportFiles(files);
                _isEdit = false;
            }
        }

        //确认添加数据项目
        [RelayCommand]
        private void AddDataItem()
        {
            IsCreateDataItem = true;
            InfoText = "添加中....";
            if (DataItemUkey.Contains("/"))
            {
                SendMessage("错误", "Ukey不可包含字符:/", InfoBarSeverity.Warning);
                IsCreateDataItem = false;
                return;
            }
            Task.Run(() =>
            {
                if (_isEdit)
                {
                    DatabaseService.UpdateDataItem(_projectName, DataItemName, DataItemUkey,
                                DataItemDescription, DataItemLink, ImportFiles.Length, DateTime.Now);
                    _isEdit = false;
                    //存储数据文件
                    SaveDataFiles();
                    //刷新前端列表
                    ReadDataItems();
                    //清空输入
                    ClearInput();
                    SendMessage("成功", "修改数据项成功", InfoBarSeverity.Success);
                }
                else
                {
                    //检查输入是否完整
                    if (CheckInput())
                    {
                        DatabaseService.InsertDataItem(_projectName, DataItemName, DataItemUkey,
                            DataItemDescription, DataItemLink, ImportFiles.Length, DateTime.Now);
                        _isEdit = false;
                        //存储数据文件
                        SaveDataFiles();
                        //刷新前端列表
                        ReadDataItems();
                        //清空输入
                        ClearInput();
                        SendMessage("成功", "添加数据项成功", InfoBarSeverity.Success);
                    }
                }
                IsCreateDataItem = false;
            });

        }

        //取消添加数据项目
        [RelayCommand]
        private void CancelAddDataItem()
        {
            _isEdit = false;
            ClearInput();
            SendMessage("提示：","取消成功",null);
        }

        //删除数据项目
        [RelayCommand]
        private void DeleteDataItem(object parameter)
        {
            //if(MessageService.ShowContentDialog("警告","是否删除数据项：" + ((DataItem)parameter).Name).Result == ContentDialogResult.Primary)
            //{
                //删除数据库中的数据项
                DatabaseService.DeleteDataItem(_projectName, ((DataItem)parameter).UKey);
                //删除数据文件
                var tempLocation = _projectDataLocation + ((DataItem)parameter).UKey;
                FileHelper.DeleteFolder(tempLocation);
                //刷新前端列表
                ReadDataItems();
                //如果处于编辑模式则清除输入
                if (_isEdit)
                {
                    ClearInput();
                }
                SendMessage("成功","删除数据项成功",InfoBarSeverity.Success);
            //}
        }

        //打开数据项目位置
        [RelayCommand]
        private void OpenDataItem(object parameter)
        {
            //获取数据项文件夹
            var tempLocation = _projectDataLocation + ((DataItem)parameter).UKey;
            FileHelper.OpenFolder(tempLocation);
            SendMessage("成功", "打开文件所在位置成功", InfoBarSeverity.Success);
        }

        //编辑数据项目
        [RelayCommand]
        private void EditDataItem(object parameter)
        {
            //进入编辑模式
            _isEdit = true;
            DataItemName = ((DataItem)parameter).Name;
            DataItemUkey = ((DataItem)parameter).UKey;
            DataItemDescription = ((DataItem)parameter).Describe;
            DataItemLink = ((DataItem)parameter).Link;
            //获取文件夹路径下的文件
            string tempLocation = _projectDataLocation + ((DataItem)parameter).UKey;
            HandleImportFiles(FileHelper.GetFiles(tempLocation).ToArray());
        }

        //删除文件列表文件
        [RelayCommand]
        private void DeleteFileInList(object parameter)
        {
            //删除提示框
            if (_isEdit)
            {
                string tempLocation = _projectDataLocation + DataItemUkey + "\\" + (string)parameter;
                //删除文件
                FileHelper.DeleteFile(tempLocation);
            }
            
            //刷新前后端列表
            string[] tempFiles = new string[ImportFiles.Length-1];
            string[] tempFilesC = new string[ImportFiles.Length-1];
            int tempIndex = 0;
            for (int i = 0;i < ImportFiles.Length; i++)
            {
                if (ImportFiles[i] != (string)parameter)
                {
                    tempFiles[tempIndex] = ImportFiles[i];
                    tempFilesC[tempIndex] = _importFilesC[i];
                    tempIndex++;
                }
            }
            ImportFiles = tempFiles;
            _importFilesC = tempFilesC;
            SendMessage("成功", "删除数据项成功", InfoBarSeverity.Success);
        }

        //添加文件列表文件
        [RelayCommand]
        private void AddFileInList_Click()
        {
            if(_importFilesC != null)
            {
                var tempFiles = FileHelper.GetFiles();
                if(tempFiles != null)
                {
                    string[] combined = _importFilesC.Concat(tempFiles.ToArray()).ToArray();
                    HandleImportFiles(combined);
                }
            }
            else
            {
                SendMessage("消息", "请先选中对应数据项再添加文件", InfoBarSeverity.Informational);
            }
        }

        //拖动添加文件列表
        [RelayCommand]
        private void AddFileInList_Drag(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                IsDragOver = false;
                if(_isEdit)
                {
                    //获取导入的文件路径
                    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files != null)
                    {
                        string[] combined = _importFilesC.Concat(files.ToArray()).ToArray();
                        HandleImportFiles(combined);
                    }
                }
                else
                {
                    DragFile(e);
                }
                
            }
        }

        //文件拖动进入
        [RelayCommand]
        private void DragingFile()
        {
            IsDragOver = true;
        }

        //文件拖动退出
        [RelayCommand]
        private void DragingOutFile()
        {
            IsDragOver = false;
        }
    }
}
