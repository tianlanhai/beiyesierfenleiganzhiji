using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace 二分类贝叶斯感知机
{
    public class FileHandle
    {
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <returns>返回字符串，stopword不拆分</returns>
        public static string ReadTxtToEnd(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                var files = Directory.GetFiles(filePath);
                string text = string.Empty;
                foreach (var file in files)
                {
                    text += ReadTxtToEnd(file);
                }
                return text;
            }
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            using (var sr = new StreamReader(filePath, Encoding.Default))
            {
                return sr.ReadToEnd();
            }
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileHead"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void WriteTxtToEnd(string filePath, string fileHead, string str)
        {
            if (Directory.Exists(filePath))
            {
                var files = Directory.GetFiles(filePath).Select(f => new FileInfo(f).Name.TrimStart(fileHead.ToCharArray()).TrimEnd(".txt".ToCharArray())).Select(f => Convert.ToInt32(f)).ToList();
                int fileCount = 0;
                if (files.Count > 0)
                {
                    files = files.OrderByDescending(f => f).ToList();
                    fileCount = files.FirstOrDefault() + 1;
                }
                StreamWriter sw = File.CreateText(filePath + "\\" + fileHead + fileCount + ".txt");
                sw.WriteLine(str);
                sw.Close();
                return;
            }
            File.WriteAllLines(filePath, new string[] { str });
        }
    }
}
