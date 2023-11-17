using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHarbor.Models
{
    public class MappingFile
    {
        //当前文件序号
        public string CurrentFileIndex { get; set; }

        //当前文件名称
        public string CurrentFileName { get; set; }

        //新文件名称
        public string NewFileName { get; set; }
    }
}
