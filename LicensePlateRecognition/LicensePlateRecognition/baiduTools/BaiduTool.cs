using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LicensePlateRecognition
{
   public class BaiduTool
    {
        /// <summary>
        /// 车牌识别
        /// </summary>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        public static string LicensePlateDemo(string imgPath)
        {
            var url = File.ReadAllBytes(imgPath);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
        {"multi_detect", "true"}
    };
            // 带参数调用网络图片文字识别, 图片参数为远程url图片
            return Common.JsonToResultCarNumber( BaiduConfig.client.LicensePlate(url, options).ToString());
        }
    }
}
