using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 二分类贝叶斯感知机
{
    public class Train
    {
        /// <summary>
        /// 读取现有训练集Json
        /// </summary>
        /// <param name="json">Json训练集</param>
        /// <param name="total">词条总数</param>
        /// <returns></returns>
        public static Dictionary<string, AddOneProb> Load(string json)
        {
            Dictionary<string, AddOneProb> d = new Dictionary<string, AddOneProb>();
            if (!string.IsNullOrWhiteSpace(json))
            {
                d = JsonConvert.DeserializeObject<Dictionary<string, AddOneProb>>(json);
            }
            else
            {
                d.Add("neg", new AddOneProb());
                d.Add("pos", new AddOneProb());
            }
            return d;
        }
        /// <summary>
        /// 清洗训练样本,返回d和total
        /// </summary>
        /// <param name="negFilePath">负面词训练集</param>
        /// <param name="negWords">负面词库文本</param>
        /// <param name="posFilePath">正面词训练集</param>
        /// <param name="posWords">正面词库文本</param>
        /// <param name="d">存储正面和负面词集的字典</param>
        /// <param name="stopwords">排除词</param>
        public static void Train_data(string negWords, string negFilters, string posWords, string posFilters,
             ref Dictionary<string, AddOneProb> d, string stopwords)
        {
            List<Tuple<List<string>, string>> data = new List<Tuple<List<string>, string>>();
            var sent_cut = new Jieba();
            sent_cut.NegWords = negFilters;
            sent_cut.PosWords = posFilters;
            foreach (var sent in posWords.Replace("\r", "").Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(sent))
                {
                    continue;
                }
                sent_cut.doc = FilterSymbol(sent);
                sent_cut.stopwords = stopwords;
                var words = sent_cut.handle_sentiment(false);
                foreach(var word in words)
                {
                    d["pos"].Add(word, CoreCalculations.IDCount, 1);
                }
                if (words != null && words.Count > 0)
                {
                    data.Add(new Tuple<List<string>, string>(words, "pos"));
                }
            }
            Console.WriteLine("正面词库导入完毕");
            foreach (var sent in negWords.Replace("\r", "").Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(sent))
                {
                    continue;
                }
                sent_cut.doc = FilterSymbol(sent);
                sent_cut.stopwords = stopwords;
                var words = sent_cut.handle_sentiment();
                foreach (var word in words)
                {
                    d["neg"].Add(word, CoreCalculations.IDCount, 1);
                }
                if (words != null && words.Count > 0)
                {
                    data.Add(new Tuple<List<string>, string>(words, "neg"));
                }
            }
            Console.WriteLine("负面词库导入完毕");

            foreach (var k in d)
            {//计算频率
                k.Value.CalculatRate();
            }
            for (int i = 0; i < 2; i++)
            {
                foreach (var d_ in data)
                {
                    var c = d_.Item2.ToString();
                    CoreCalculations.Sensor(d, d_.Item1, c);//每一条数据做训练
                }
            }
        }

        /// <summary>
        /// 过滤符号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FilterSymbol(string str)
        {
            StringBuilder sb = new StringBuilder();
            string s = "；、";
            foreach (var c in str)
            {
                int ascii = (int)c;
                if (ascii < 48)
                {
                    continue;
                }
                if (ascii > 57 && ascii < 65)
                {
                    continue;
                }
                if (ascii > 90 && ascii < 97)
                {
                    continue;
                }
                if (ascii > 122 && ascii < 128)
                {
                    continue;
                }
                if(s.Contains(c))
                {
                    continue;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
