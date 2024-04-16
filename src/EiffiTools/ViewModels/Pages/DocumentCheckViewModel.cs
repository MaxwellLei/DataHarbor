using DataHarbor.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHarbor.ViewModels.Pages
{
    public partial class DocumentCheckViewModel : ObservableObject
    {
        //单例模式
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _repcontent;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            _isInitialized = true;
        }

        //进行文档互检
        [RelayCommand]
        public void PCheck()
        {
            MessageService.AutoShowDialog("12", "123", Wpf.Ui.Controls.ControlAppearance.Info);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"External\PCheck.exe"; // exe文件的路径
            startInfo.Arguments = "D:\\DATA\\PythonProject\\MLDefault\\文档互检查重\\Test_File"; // 传递给exe的参数
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.StandardOutputEncoding = Encoding.UTF8; // 如果输出包含非ASCII字符

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                // 读取输出
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                Repcontent = output;
                // 这里你可以处理输出
                Console.WriteLine("Output: " + output);
                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Error: " + error);
                }
            }
        }
    }
}
