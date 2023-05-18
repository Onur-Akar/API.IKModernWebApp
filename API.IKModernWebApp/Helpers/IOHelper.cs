using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using API.IKModernWebApp.ViewModel;
using OfficeOpenXml;
using System.Collections;
using System.Linq;

namespace API.IKModernWebApp.Helpers
{
    public class IOHelper
    {
        public static void CheckAndCreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public static string WhiteListIllegalChars(string str)
        {
            str = str.Replace(" ", "");
            str = str.Replace("/", "-");
            str = str.Replace("\\", "-");
            str = str.Replace("<", "-");
            str = str.Replace(">", "-");
            str = str.Replace("|", "-");
            str = str.Replace(":", "-");
            str = str.Replace("?", "-");
            str = str.Replace("*", "-");
            str = str.Replace("+", "-");
            str = str.Replace("\"", "-");
            return str;
        }

        public static string CreateFilePathWithType(string fileName, string fileType, string dirPath = "")
        {
            string dir = @$"C:\pdf{dirPath}";
            CheckAndCreateDirectory(dir);
            string filename = $@"{fileName}_{DateTime.Now:dd_MM_yyyy_mm_hh_ss}.{fileType}";
            string filePath = Path.Combine(dir, filename);

            return filePath;
        }

        public static bool WriteFileFromByte(byte[] fileByte, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(fileByte, 0, fileByte.Length);
                return true;
            }
        }

        public static void CreateJsonFileFromResults(object obj, string filePath)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
