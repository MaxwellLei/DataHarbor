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
        /// ��Դ
        /// </summary>
        private readonly ResourceManager _resourceManager;

        /// <summary>
        /// ������
        /// </summary>
        private static readonly Lazy<ResourceService> _lazy = new Lazy<ResourceService>(() => new ResourceService());
        public static ResourceService Instance => _lazy.Value;
        public event PropertyChangedEventHandler PropertyChanged;

        public ResourceService()
        {
            //��ȡ�������ռ���Resources��Lang����Դ
            _resourceManager = new ResourceManager("DataHarbor.Resources.Language", typeof(ResourceService).Assembly);
        }

        /// <summary>
        /// ��������д���������ַ������±�
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("item[]"));  //�ַ������ϣ���Ӧ��Դ��ֵ
        }
    }
}
