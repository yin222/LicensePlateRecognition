using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LicensePlateRecognition
{
   public class Common
    {
        /// <summary>
        /// 提取json中的车牌号
        /// </summary>
        public static string JsonToResultCarNumber(string result)
        {
            result = result.Replace("\"", "'");
            result = Regex.Match(result, "(?<='number': ').*?(?=',)").Value;
            return result.Length > 0 ? result : "未识别出结果";
        }
    }
}
