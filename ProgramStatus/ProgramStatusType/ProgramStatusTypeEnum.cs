using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramStatus
{
    public enum ProgramStatusTypeEnum
    {
        /// <summary>
        /// 出题态
        /// </summary>
        TYPE_MakeOutQuestions,
        /// <summary>
        /// 制作标准答案态
        /// </summary>
        TYPE_MakeStandardAnswers,
        /// <summary>
        /// 学生答题态
        /// </summary>
        TYPE_StudentsAnswer,
        /// <summary>
        /// 查阅态
        /// </summary>
        TYPE_Query
    }
}
