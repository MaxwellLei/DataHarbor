using OfficeOpenXml;
using DataHarbor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using System.Reflection.Metadata.Ecma335;
using DataHarbor.Services;
using DataHarbor.Helpers;

namespace DataHarbor.ViewModels.Pages
{
    public partial class MappingDataViewModel : ObservableObject, INavigationAware
    {
        //是否已经实例化
        private bool _isInitialized = false;

        //临时文件路径
        private string tempFilePath = "";

        //读取的文件路径列表
        [ObservableProperty]
        private ObservableCollection<string>? selectedFiles;

        //文件名称列表
        [ObservableProperty]
        private ObservableCollection<MappingFile>? fileNames;

        //操作提示控件是否可见
        [ObservableProperty]
        private Visibility isSeeTip = Visibility.Visible;

        //加载控件可见性
        [ObservableProperty]
        private Visibility isLoadVisible = Visibility.Hidden;

        //禁用用户操作
        [ObservableProperty]
        private bool isEnable = true;

        //新文件名称
        //[ObservableProperty]
        //private string newFileName = "";

        //选择框选择内容
        [ObservableProperty]
        private string tempIndex = "0";

        //新文件存放位置
        [ObservableProperty]
        private string customFilePath = "自定义";

        //文件名称附加内容
        [ObservableProperty]
        private string addContent = "_new";

        //文件名称前后缀
        [ObservableProperty]
        private bool prefix = false;
        [ObservableProperty]
        private bool suffix = true;

        //文件内容排列方式
        [ObservableProperty]
        private bool orientation_0 = true;
        [ObservableProperty]
        private bool orientation_1 = false;

        //文件格式
        [ObservableProperty]
        private bool file_format_xlsx = true;
        //[ObservableProperty]
        //private bool file_format_xls = false;
        //[ObservableProperty]
        //private bool file_format_csv = false;
        //[ObservableProperty]
        //private bool file_format_txt = false;

        //转换完成后打开文件所在位置
        [ObservableProperty]
        private bool isOpenFolder = false;

        //合并为一个文件
        [ObservableProperty]
        private bool isOneFile = false;

        //转换完成清空队列
        [ObservableProperty]
        private bool isCleanList = true;

        //转换完成弹窗通知
        [ObservableProperty]
        private bool isPopup = true;

        //转换完成弹窗通知
        [ObservableProperty]
        private bool isShutDown = false;

        //导航进来
        public void OnNavigatedTo()
        {
            IsSeeTip = Visibility.Visible;
            //单例模式
            if (!_isInitialized)
                InitializeViewModel();

        }
        //导航出去
        public void OnNavigatedFrom() { }


        //初始化
        private void InitializeViewModel()
        {
            // 设置 EPPlus 的 LicenseContext
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            SelectedFiles = new ObservableCollection<string>();
            FileNames = new ObservableCollection<MappingFile>();
        }

        //设定新文件存放文件夹
        public void SetCustomFilePath()
        {
            if (TempIndex == "1")
            {
                string folderPath = FileHelper.GetFolderPath();
                if (folderPath == "")
                {
                    TempIndex = "0";
                }
                else
                {
                    customFilePath = folderPath;
                }
            }
            else
            {
                customFilePath = "自定义";
            }
        }

        //读取文本以,分割成每一个然后解析成字符串列表
        private ObservableCollection<string> ReadText(string filePath)
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            string[] str = filePath.Split(',', '，');
            foreach (string s in str)
            {
                list.Add(s);
            }
            return list;
        }

        //清空所选文件
        [RelayCommand]
        private void ClearSelectedFiles()
        {
            SelectedFiles.Clear();
            FileNames.Clear();
            IsSeeTip = Visibility.Visible;
        }

        //检测最高行数
        public int GetMaxRowWithContent(string filePath)
        {
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // 选择您要读取的工作表

                int rowCount = worksheet.Dimension.Rows;

                // 从底部开始逐行向上检查，找到第一个非空的行
                for (int row = rowCount; row >= 1; row--)
                {
                    if (worksheet.Cells[row, 1].Value != null)
                    {
                        return row;
                    }
                }

                // 如果没有内容，则返回0或其他适当的值
                return 0;
            }
        }

        //检测最高列数
        public int GetMaxColumnWithContent(string filePath)
        {
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // 选择您要读取的工作表
                int columnCount = worksheet.Dimension.Columns;
                for (int column = columnCount; column >= 1; column--)
                {
                    if (worksheet.Cells[1, column].Value != null)
                    {
                        return column;
                    }
                }
                // 如果没有内容，则返回0或其他适当的值
                return 0;
            }
        }

        //判断excel文件是否为空文件
        private bool IsEmptyExcel(string filePath)
        {
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // 选择您要读取的工作表
                if (worksheet.Dimension != null)
                {
                    int rowCount = worksheet.Dimension.Rows;
                    int columnCount = worksheet.Dimension.Columns;
                    if (rowCount == 0 && columnCount == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        //写入单个excel文件
        private void Write_Excel(string filePath, int fileIndex)
        {
            string new_file_path = "";
            string new_file_name = "";
            string new_sheet_name = "Sheet1";
            string file_format = ".xlsx";
            int row = GetMaxRowWithContent(filePath);
            int column = GetMaxColumnWithContent(filePath);
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // 选择要读取的工作表
                ExcelPackage newPackage = new ExcelPackage();

                ExcelWorksheet newWorksheet = newPackage.Workbook.Worksheets.Add(new_sheet_name);
                int tempindex = 1;
                //定排列方式
                if (Orientation_1 == true)
                {
                    //遍历最高列数
                    for (int i = 1; i <= column; i++)
                    {
                        //遍历最高行数
                        for (int j = 1; j <= row; j++)
                        {
                            //如果格子内容不为null，则将内容写到新建的工作簿第一列
                            if (worksheet.Cells[j, i].Value.ToString() != "")
                            {
                                newWorksheet.Cells[1, tempindex].Value = worksheet.Cells[j, i].Value;
                                tempindex++;
                            }
                        }
                    }
                }
                else
                {
                    //遍历最高列数
                    for (int i = 1; i <= column; i++)
                    {
                        //遍历最高行数
                        for (int j = 1; j <= row; j++)
                        {
                            //如果格子内容不为null，则将内容写到新建的工作簿第一列
                            if (worksheet.Cells[j, i].Value.ToString() != "")
                            {
                                newWorksheet.Cells[tempindex, 1].Value = worksheet.Cells[j, i].Value;
                                tempindex++;
                            }
                        }
                    }
                }

                //如新文件名称被设定为空，就设定为原来的名字
                if (FileNames.Count != 0)
                {
                    new_file_name = FileNames[fileIndex].NewFileName;
                }
                else
                {
                    new_file_name = FileNames[fileIndex].CurrentFileName;
                }

                //前后缀名称
                if (Prefix)
                {
                    new_file_name = AddContent + new_file_name;
                }
                else
                {
                    new_file_name = new_file_name + AddContent;
                }


                //保存文件到指定文件夹
                if (TempIndex == "1")
                {
                    new_file_path = FileHelper.SaveFile(CustomFilePath, new_file_name, file_format);
                }
                else
                {
                    //保存文件到默认文件夹
                    new_file_path = FileHelper.SaveFile(FileHelper.GetFolderPath(filePath), new_file_name, file_format);
                }
                tempFilePath = new_file_path;   //临时文件位置，打开文件夹位置
                newPackage.SaveAs(new System.IO.FileInfo(new_file_path));
            }
        }

        //合并为一个文件（不同工作簿）
        private void Merge_Write_Excel(string filePath, int fileIndex, ExcelPackage newPackage)
        {
            int row = GetMaxRowWithContent(filePath);
            int column = GetMaxColumnWithContent(filePath);
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // 选择要读取的工作表

                //截断文件名称到工作簿名称最大支持字符数量
                string sheetName = FileHelper.GetFileName(filePath);
                if (sheetName.Length > 24)
                {
                    sheetName = sheetName.Substring(0, 24);
                    // 使用Dispatcher在UI线程上显示消息框
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageService.AutoShowDialog("说明", "部分文件名称超出工作簿命名允许长度，已做截断处理", ControlAppearance.Info);
                    });
                    sheetName = (fileIndex + 1).ToString() + "_" + sheetName;
                }
                ExcelWorksheet newWorksheet = newPackage.Workbook.Worksheets.Add(sheetName);
                int tempindex = 1;
                //定排列方式
                if (Orientation_1 == true)
                {
                    //遍历最高列数
                    for (int i = 1; i <= column; i++)
                    {
                        //遍历最高行数
                        for (int j = 1; j <= row; j++)
                        {
                            //如果格子内容不为null，则将内容写到新建的工作簿第一列
                            if (worksheet.Cells[j, i].Value.ToString() != "")
                            {
                                newWorksheet.Cells[1, tempindex].Value = worksheet.Cells[j, i].Value;
                                tempindex++;
                            }
                        }
                    }
                }
                else
                {
                    //遍历最高列数
                    for (int i = 1; i <= column; i++)
                    {
                        //遍历最高行数
                        for (int j = 1; j <= row; j++)
                        {
                            //如果格子内容不为null，则将内容写到新建的工作簿第一列
                            if (worksheet.Cells[j, i].Value.ToString() != "")
                            {
                                newWorksheet.Cells[tempindex, 1].Value = worksheet.Cells[j, i].Value;
                                tempindex++;
                            }
                        }
                    }
                }
                string new_file_path = "";
                //保存文件到指定文件夹
                if (TempIndex == "1")
                {
                    new_file_path = FileHelper.SaveFile(CustomFilePath, "合并文件", ".xlsx");
                }
                else
                {
                    //保存文件到默认文件夹
                    new_file_path = FileHelper.SaveFile(FileHelper.GetFolderPath(filePath), "合并文件", ".xlsx");
                }
                tempFilePath = new_file_path;   //临时文件位置，打开文件夹位置

            }
        }

        //合并为一个文件(相同工作簿)
        private void Merge_Write_Excel(string filePath, int index)
        {
            int row = GetMaxRowWithContent(filePath);
            int column = GetMaxColumnWithContent(filePath);
            string sheetName = "sheet1";
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // 选择要读取的工作表
                int tempindex = 1;
                //保存文件到指定文件夹
                if (index == 1)
                {
                    string new_file_path = "";
                    if (TempIndex == "1")
                    {
                        new_file_path = FileHelper.SaveFile(CustomFilePath, "合并文件", ".xlsx");
                    }
                    else
                    {
                        //保存文件到默认文件夹
                        new_file_path = FileHelper.SaveFile(FileHelper.GetFolderPath(filePath), "合并文件", ".xlsx");
                    }
                    tempFilePath = new_file_path;   //临时文件位置，打开文件夹位置
                }

                // 打开现有的Excel文件
                using (var new_package = new ExcelPackage(new FileInfo(tempFilePath)))
                {
                    // 获取工作簿
                    ExcelWorkbook workbook = new_package.Workbook;

                    // 获取或创建一个工作表（根据sheetName）
                    ExcelWorksheet new_worksheet = workbook.Worksheets.FirstOrDefault(s => s.Name == sheetName);

                    if (new_worksheet == null)
                    {
                        // 如果工作表不存在，创建一个新的工作表
                        new_worksheet = workbook.Worksheets.Add(sheetName);
                    }

                    //定排列方式
                    if (Orientation_1 == true)
                    {
                        //遍历最高列数
                        for (int i = 1; i <= column; i++)
                        {
                            //遍历最高行数
                            for (int j = 1; j <= row; j++)
                            {
                                //如果格子内容不为null，则将内容写到新建的工作簿第一列
                                if (worksheet.Cells[j, i].Value.ToString() != "")
                                {
                                    new_worksheet.Cells[index, tempindex].Value = worksheet.Cells[j, i].Value;
                                    tempindex++;
                                }
                            }
                        }
                    }
                    else
                    {
                        //遍历最高列数
                        for (int i = 1; i <= column; i++)
                        {
                            //遍历最高行数
                            for (int j = 1; j <= row; j++)
                            {
                                //如果格子内容不为null，则将内容写到新建的工作簿第一列
                                if (worksheet.Cells[j, i].Value.ToString() != "")
                                {
                                    new_worksheet.Cells[tempindex, index].Value = worksheet.Cells[j, i].Value;
                                    tempindex++;
                                }
                            }
                        }
                    }
                    // 保存工作簿
                    new_package.Save();
                }
            }
        }

        //获取索引
        public int GetTabIndex(string input)
        {
            int startIndex = input.IndexOf("(");
            int endIndex = input.IndexOf(")");

            if (startIndex >= 0 && endIndex > startIndex)
            {
                return Convert.ToInt32(input.Substring(startIndex + 1, endIndex - startIndex - 1));
            }

            return -1;
        }

        //刷新文件列表
        public void RefreshList()
        {
            FileNames.Clear();
            //添加姓名
            foreach (var tempName in FileHelper.GetFileNames(SelectedFiles))
            {
                MappingFile temp = new MappingFile();
                temp.CurrentFileIndex = "(" + (FileHelper.GetFileNames(SelectedFiles).IndexOf(tempName) + 1) + ")";
                temp.CurrentFileName = tempName;
                temp.NewFileName = tempName;
                FileNames.Add(temp);
            }
        }

        //向上移动列表选项
        public void MoveUpTab(object sender)
        {
            //拿到要移动的选项
            MappingFile fileName = (MappingFile)((Wpf.Ui.Controls.Button)sender).DataContext;
            //获取当前索引
            int select_index = GetTabIndex(fileName.CurrentFileIndex) - 1;
            //如果不在最顶端
            if (select_index > 0)
            {
                string temp = SelectedFiles[select_index - 1];
                SelectedFiles[select_index - 1] = SelectedFiles[select_index];
                SelectedFiles[select_index] = temp;
                RefreshList();
            }
            else
            {
                MessageService.AutoShowDialog("错误", "当前选项已是最顶端", ControlAppearance.Danger);
            }
        }

        //向下移动选项
        public void MoveDownTab(object sender)
        {
            //拿到要移动的选项
            MappingFile fileName = (MappingFile)((Wpf.Ui.Controls.Button)sender).DataContext;
            //获取当前索引
            int select_index = GetTabIndex(fileName.CurrentFileIndex) - 1;
            //如果不在最低端
            if (select_index < SelectedFiles.Count - 1)
            {
                string temp = SelectedFiles[select_index + 1];
                SelectedFiles[select_index + 1] = SelectedFiles[select_index];
                SelectedFiles[select_index] = temp;
                RefreshList();
            }
            else
            {
                MessageService.AutoShowDialog("错误", "当前选项已是最低端", ControlAppearance.Danger);
            }
        }

        //转换 Excel 文件列表
        [RelayCommand]
        private async void ExChanged_Excel_Data()
        {
            if (SelectedFiles.Count == 0)
            {
                MessageService.AutoShowDialog("警告", "请添加需要转换的文件", ControlAppearance.Danger);
            }
            else
            {
                IsLoadVisible = Visibility.Visible;
                IsEnable = false;
                // 异步执行操作 
                await Task.Run(() =>
                {
                    int temp_index = 1;
                    //遍历文件列表
                    foreach (string filePath in SelectedFiles)
                    {
                        int index = SelectedFiles.IndexOf(filePath);
                        // 写入Excel
                        if (!IsEmptyExcel(filePath))
                        {
                            //是否合并为一个excel文件
                            if (IsOneFile)
                            {
                                Merge_Write_Excel(filePath, temp_index);
                                temp_index++;

                            }
                            else
                            {
                                Write_Excel(filePath, index);
                            }
                        }
                        else
                        {
                            // 使用Dispatcher在UI线程上显示消息框
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageService.AutoShowDialog("警告", FileHelper.GetFileName(filePath) + "为异常文件，已经跳过该文件的转换", ControlAppearance.Danger);
                                //Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox();
                                //messageBox.Content = FileHelper.GetFileName(filePath) + "为异常文件，已经跳过该文件的转换";
                                //messageBox.ShowDialogAsync();
                            });
                        }
                    }


                    //转换完成后弹出消息框
                    if (IsPopup)
                    {
                        // 使用Dispatcher在UI线程上显示消息框
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageService.AutoShowDialog("说明", "转换完成", ControlAppearance.Success);
                        });
                    }
                });
                //转换完成后清空队列
                if (IsCleanList)
                {
                    ClearSelectedFiles();
                }

                //转换完成后打开目标文件夹
                if (IsOpenFolder)
                {
                    OpenTargetFolder();
                }
                IsLoadVisible = Visibility.Hidden;
                IsEnable = true;
            }
        }

        //打开目标文件夹
        [RelayCommand]
        private void OpenTargetFolder()
        {

            if (TempIndex == "0" && SelectedFiles.Count != 0)
            {
                FileHelper.OpenFolder(tempFilePath);
            }
            else
            {
                FileHelper.OpenFolder(tempFilePath);
            }

        }
    }
}
