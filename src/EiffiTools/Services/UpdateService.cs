using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataHarbor.Services
{
    internal class UpdateService
    {
        private static string server_url = "https://eiffitools-1303234197.cos.ap-beijing.myqcloud.com/Resource/eiffitools_update.json";
        //读取服务器的json文件，解析软件版本并对比当前版本，返回布尔值
        public static async Task<bool> CheckUpdateAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(server_url);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(result);
                    var version = json.RootElement.GetProperty("version").GetString();
                    var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    return version != currentVersion;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //检测本地网络是否链接以及服务器json文件是否存在可以访问
        public static async Task<bool> CheckNetworkAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(server_url);
                    response.EnsureSuccessStatusCode();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //读取服务器json文件，返回更新日志
        public static async Task<string> GetUpdateLogAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(server_url);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(result);
                    var log = json.RootElement.GetProperty("log").GetString();
                    return log;
                }
            }
            catch (Exception)
            {
                return "获取更新日志失败";
            }
        }

        //读取服务器json文件，返回安装包下载地址
        public static async Task<string> GetDownloadUrlAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(server_url);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(result);
                    var url = json.RootElement.GetProperty("download_url").GetString();
                    return url;
                }
            }
            catch (Exception)
            {
                return "获取下载地址失败";
            }
        }

        //读取服务器json文件，返回公告
        public static async Task<string> GetAnnouncementAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(server_url);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(result);
                    var url = json.RootElement.GetProperty("announcement").GetString();
                    return url;
                }
            }
            catch (Exception)
            {
                return "获取下载地址失败";
            }
        }

    }
}
