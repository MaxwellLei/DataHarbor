// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Runtime.CompilerServices;

namespace DataHarbor.Resources
{
    public class ResourceService : INotifyPropertyChanged
    {
        /// <summary>
        /// 资源
        /// </summary>
        private readonly ResourceManager _resourceManager;

        /// <summary>
        /// 懒加载
        /// </summary>
        private static readonly Lazy<ResourceService> _lazy = new Lazy<ResourceService>(() => new ResourceService());
        public static ResourceService Instance => _lazy.Value;
        public event PropertyChangedEventHandler PropertyChanged;

        public ResourceService()
        {
            //获取此命名空间下Resources的Lang的资源
            _resourceManager = new ResourceManager("DataHarbor.Resources.Language", typeof(ResourceService).Assembly);
        }

        /// <summary>
        /// 索引器的写法，传入字符串的下标
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }
                return _resourceManager.GetString(name);
            }
        }

        public void ChangeLanguage(CultureInfo cultureInfo)
        {
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item[]"));  //字符串集合，对应资源的值
        }
    }
}
