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
using OfficeOpenXml.Style;

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
            _isInitialized = true;

            SelectedFiles = new ObservableCollection<string>();
            FileNames = new ObservableCollection<MappingFile>();
        }

        //设定新文件存放文件夹
        public void SetCustomFilePath()
        {
            customFilePath = TempIndex == "1" ? FileHelper.GetFolderPath() : "自定义";
        }

        //读取文本以,分割成每一个然后解析成字符串列表【屎山代码】
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
        public int GetMaxRowWithContent(string filePath) => ProcessExcelFile(filePath, (worksheet) => worksheet.Dimension?.End.Row ?? -99);

        //检测最高列数
        public int GetMaxColumnWithContent(string filePath) => ProcessExcelFile(filePath, (worksheet) => worksheet.Dimension?.End.Column ?? -99);

        //判断excel文件是否为空文件
        private bool IsEmptyExcel(string filePath) => ProcessExcelFile(filePath, (worksheet) => worksheet.Dimension == null);

        //读取 excel 文件信息
        private T ProcessExcelFile<T>(string filePath, Func<ExcelWorksheet, T> process)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                return process(worksheet);
            }
        }

        //写入单个excel文件
        private void Write_Excel(string filePath, int fileIndex)
        {
            var maxRow = GetMaxRowWithContent(filePath);
            var maxColumn = GetMaxColumnWithContent(filePath);
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
            using var newPackage = new ExcelPackage();
            var newWorksheet = newPackage.Workbook.Worksheets.Add("Sheet1");

            int tempIndex = 1;
            ProcessCells(maxRow, maxColumn, worksheet, newWorksheet, tempIndex, Orientation_1);

            string newFileName = FileNames.Count > 0 ? FileNames[fileIndex].NewFileName : FileNames[fileIndex].CurrentFileName;
            newFileName = Prefix ? AddContent + newFileName : newFileName + AddContent;

            string newFilePath = TempIndex == "1" ? FileHelper.SaveFile(CustomFilePath, newFileName, ".xlsx") : FileHelper.SaveFile(FileHelper.GetFolderPath(filePath), newFileName, ".xlsx");
            tempFilePath = newFilePath;
            newPackage.SaveAs(new FileInfo(newFilePath));
        }

        //遍历写入excel数据
        private void ProcessCells(int maxRow, int maxColumn, ExcelWorksheet sourceWorksheet, ExcelWorksheet targetWorksheet, int tempIndex, bool verticalOrientation)
        {
            int currentRow = 1; // 开始的行号
            int maxExcelColumns = 16384; // Excel的最大列数
            int maxExcelRows = 1048576; // Excel的最大行数
            for (int i = 1; i <= maxColumn; i++)
            {
                for (int j = 1; j <= maxRow; j++)
                {
                    var tempValue = sourceWorksheet.Cells[j, i].Value;
                    if (tempValue != null && tempValue.ToString() != "0" && 
                        double.TryParse(tempValue.ToString(), out _) && tempValue.ToString() != "NaN" &
                                    tempValue != null)
                    {
                        if (verticalOrientation)
                        {
                            if (tempIndex > maxExcelColumns) // 检查是否需要换行
                            {
                                currentRow++; // 移动到下一行
                                tempIndex = 1; // 重置tempIndex为第一列
                                               // 使用Dispatcher在UI线程上显示消息框
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageService.AutoShowDialog("消息📝", "文件数据超出 Excel 单行最大列数，已换行处理", ControlAppearance.Info);
                                });
                            }
                            targetWorksheet.Cells[currentRow, tempIndex].Value = tempValue;
                        }
                        else
                        {
                            if (tempIndex > maxExcelRows) // 检查是否需要换列
                            {
                                currentRow++; // 移动到下一行
                                tempIndex = 1; // 重置tempIndex为第一列
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageService.AutoShowDialog("消息📝", "文件数据超出 Excel 单行最大行数，已换列处理", ControlAppearance.Info);
                                });
                            }
                            targetWorksheet.Cells[tempIndex, currentRow].Value = tempValue;
                        }

                        tempIndex++;
                    }
                }
            }
        }

        //合并为一个文件(相同工作簿)
        private void Merge_Write_Excel(string filePath, int index, string new_file_path)
        {
            int row = GetMaxRowWithContent(filePath);
            int column = GetMaxColumnWithContent(filePath);
            string fileName = FileHelper.GetFileName(filePath);
            string sheetName = "sheet1";
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // 选择要读取的工作表
                int tempindex = 2;
                tempFilePath = new_file_path;   //临时文件位置，打开文件夹位置

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

                    //定排列方式-如果是行排列
                    if (Orientation_1 == true)
                    {
                        new_worksheet.Cells[index, 1].Value = fileName;
                        //遍历最高列数
                        for (int i = 1; i <= column; i++)
                        {
                            //遍历最高行数
                            for (int j = 1; j <= row; j++)
                            {
                                var temp_value = worksheet.Cells[j, i].Value;
                                if(temp_value != null)
                                {
                                    double num;
                                    bool isnum = double.TryParse(temp_value?.ToString(), out num);
                                    //如果格子内容不为null，则将内容写到新建的工作簿第一列
                                    if (temp_value.ToString() != "" & temp_value.ToString() != "0" & isnum & temp_value.ToString() != "NaN")
                                    {
                                        if (tempindex >= 16384)
                                        {
                                            new_worksheet.Cells[index, 1].Value = "警告⚠️：文件 " + fileName + " 数据超出 Excel 最大限制，已截断后续内容";
                                            // 使用Dispatcher在UI线程上显示消息框
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {
                                                MessageService.AutoShowDialog("警告⚠️", fileName + " 数据超出 Excel 单行最大列数，已截断当前文件后续内容", ControlAppearance.Danger);
                                            });
                                            new_package.Save();
                                            return;
                                        }
                                        new_worksheet.Cells[index, tempindex].Value = worksheet.Cells[j, i].Value;
                                        tempindex++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        new_worksheet.Cells[1, index].Value = fileName;
                        //遍历最高列数
                        for (int i = 1; i <= column; i++)
                        {
                            //遍历最高行数
                            for (int j = 1; j <= row; j++)
                            {
                                var temp_value = worksheet.Cells[j, i].Value;
                                double num;
                                bool isnum = double.TryParse(temp_value?.ToString(), out num);
                                //如果格子内容不为null，则将内容写到新建的工作簿第一列
                                if (temp_value.ToString() != "" &
                                    temp_value.ToString() != "0" &
                                    isnum &
                                    temp_value.ToString() != "NaN" &
                                    temp_value != null)
                                {
                                    if (tempindex >= 1048576)
                                    {
                                        new_worksheet.Cells[1, index].Value = "警告⚠️：文件 " + fileName + " 数据超出 Excel 最大限制，已截断后续内容";
                                        // 使用Dispatcher在UI线程上显示消息框
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            MessageService.AutoShowDialog("警告⚠️", fileName + " 数据超出 Excel 单列最大行数，已截断当前文件后续内容", ControlAppearance.Danger);
                                        });
                                        new_package.Save();
                                        return;
                                    }
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
            foreach (var tempName in FileHelper.GetFileNames(SelectedFiles))
            {
                FileNames.Add(new MappingFile
                {
                    CurrentFileIndex = $"({FileHelper.GetFileNames(SelectedFiles).IndexOf(tempName) + 1})",
                    CurrentFileName = tempName,
                    NewFileName = tempName
                });
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
                    string new_file_path = "";
                    if (TempIndex == "1")
                    {
                        new_file_path = FileHelper.SaveFile(CustomFilePath, "MergeFile", ".xlsx");
                    }
                    else
                    {
                        //保存文件到默认文件夹
                        new_file_path = FileHelper.SaveFile(FileHelper.GetFolderPath(SelectedFiles[0]), "MergeFile", ".xlsx");
                    }
                    //遍历文件列表
                    foreach (string filePath in SelectedFiles)
                    {
                        int index = SelectedFiles.IndexOf(filePath);
                        // 检查文件是否为空
                        if (!IsEmptyExcel(filePath))
                        {
                            // 判断是否合并为一个Excel文件
                            if (IsOneFile)
                            {
                                // 合并到一个工作簿
                                Merge_Write_Excel(filePath, temp_index, new_file_path);
                                temp_index++;
                            }
                            else
                            {
                                // 每个文件写入单独的Excel
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
            FileHelper.OpenFolder(tempFilePath);
        }
    }
}
