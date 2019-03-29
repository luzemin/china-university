using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinaUniversity
{
    public class University
    {
        //院校名称
        public string Name { get; set; }

        //院校所在地	院校
        public string Area { get; set; }

        //隶属
        public string Belong { get; set; }

        //院校类型
        public string Type { get; set; }

        //学历层次
        public string Level { get; set; }

        //院校特性：985-211
        public string Tag { get; set; }

        //研究生院
        public bool IsGraduateSchool { get; set; }
    }
}
