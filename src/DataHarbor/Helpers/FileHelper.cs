using DataHarbor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;

namespace DataHarbor.Helpers
{
    internal class FileHelper
    {
        //获取文件夹路径
        public static string GetFolderPath()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Description = "请选择文件夹";
            dialog.UseDescriptionForTitle = true;
            dialog.ShowNewFolderButton = true;
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            if ((bool)dialog.ShowDialog())
            {
                return dialog.SelectedPath;
            }
            else
            {
                return "";
            }
        }

        //获取选择的excel文件，可以选择多个文件
        public static ObservableCollection<string> GetExcelFiles()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Excel文件|*.xlsx;*.xls";
            dialog.Title = "请选择Excel文件";
            dialog.RestoreDirectory = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if ((bool)dialog.ShowDialog())
            {
                return new ObservableCollection<string>(dialog.FileNames);
            }
            else
            {
                return null;
            }
        }


        //获取选择的任意文件，可以选择多个文件
        public static ObservableCollection<string> GetFiles()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = "请选择文件";
            dialog.RestoreDirectory = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if ((bool)dialog.ShowDialog())
            {
                return new ObservableCollection<string>(dialog.FileNames);
            }
            else
            {
                return null;
            }
        }

        //传入文件路径，返回文件名称，不带后缀
        public static string GetFileName(string filePath)
        {
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            return fileName;
        }

        //传入文件路径，返回文件名称，带后缀
        public static string GetFileNameWithExtension(string filePath)
        {
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
            return fileName;
        }

        //传入文件路径列表，将每一个文件路径转换为不带后缀的文件名称并返回新的文件列表
        public static ObservableCollection<string> GetFileNames(ObservableCollection<string> filePaths)
        {
            ObservableCollection<string> fileNames = new ObservableCollection<string>();
            foreach (string filePath in filePaths)
            {
                fileNames.Add(GetFileName(filePath));
            }
            return fileNames;
        }

        //传入文件路径，返回文件的文件夹路径
        public static string GetFolderPath(string filePath)
        {
            string folderPath = filePath.Substring(0, filePath.LastIndexOf('\\'));
            return folderPath;
        }

        //传入文件夹路径，保存文件检查是否重名，如果有重名就+1
        public static string SaveFile(string folderPath, string fileName, string extension)
        {
            string filePath = folderPath + "\\" + fileName + extension;
            int i = 1;
            while (System.IO.File.Exists(filePath))
            {
                filePath = folderPath + "\\" + fileName + "(" + i + ")" + extension;
                i++;
            }
            return filePath;
        }

        //检测文件路径是否为excel文件
        public static bool IsExcelFile(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath);
            if (extension == ".xlsx" || extension == ".xls")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //判断传入路径的文件夹是否存在，返回bool值
        public static bool IsFolderExist(string folderPath)
        {
            if (System.IO.Directory.Exists(folderPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //获取当前程序所在的相对路径
        public static string GetRelativePath()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            return path;
        }

        //传入文件夹路径，如果存在则什么也不做，如果不存在则生成对应的文件夹
        public static void CreateFolder(string folderPath)
        {
            if (!IsFolderExist(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
        }

        //传入文件路径，打开文件所在文件夹
        public static void OpenFolder(string filePath)
        {
            if (filePath != "")
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
            }
            else
            {
                MessageService.AutoShowDialog("警告","请选择文件", Wpf.Ui.Controls.ControlAppearance.Danger);
            }
        }
    }
}
