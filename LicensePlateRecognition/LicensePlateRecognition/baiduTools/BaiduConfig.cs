using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace LicensePlateRecognition
{
   public class BaiduConfig
    {
        // 设置APPID/AK/SK
        public static string APP_ID = "15880802";
        public static string API_KEY = "bzB8Lj9gOG1baBPNTqIjHGD4";
        public static string SECRET_KEY = "kAjGPUFXc8hWMGugEGCwPT0dGqVPxhbD";

        public static Baidu.Aip.Ocr.Ocr client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
        
    }
}
