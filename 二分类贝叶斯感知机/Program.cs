using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 二分类贝叶斯感知机
{
    class Program
    {
        readonly static string SentimentFilepath = System.Environment.CurrentDirectory + "\\sentiment\\";
        static void Main(string[] args)
        {
            var sentimentJson = FileHandle.ReadTxtToEnd(SentimentFilepath + "sentiment_json.txt");
            var d = Train.Load(sentimentJson);
            var posFilters = FileHandle.ReadTxtToEnd(SentimentFilepath + "pos");//正面词
            var posWords = FileHandle.ReadTxtToEnd(SentimentFilepath + "pos_train");//正面训练集
            var negFilters = FileHandle.ReadTxtToEnd(SentimentFilepath + "neg");//负面词
            var negWords = FileHandle.ReadTxtToEnd(SentimentFilepath + "neg_train");//负面训练集
            var stopWords = FileHandle.ReadTxtToEnd(SentimentFilepath + "stopwords");//不参与计算词
            Train.Train_data(negWords, negFilters, posWords, posFilters, ref d, stopWords);//导入训练集（可以自己根据需要替换训练集）

            var test = FileHandle.ReadTxtToEnd(SentimentFilepath + "testpos");//测试集
            double countResult = 0;
            double posCount = 0;
            double negCount = 0;
            Dictionary<string, double> limitCount = new Dictionary<string, double>();
            limitCount.Add("neg", 0);
            limitCount.Add("pos", 0);
            foreach (var t in test.Replace("\r", "").Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(t))
                {
                    continue;
                }
                countResult++;
                var sent = CoreCalculations.Classify_(t, d, stopWords);
                double limit = Math.Abs(sent["pos"] - sent["neg"]);
                if (sent["neg"] > sent["pos"])
                {
                    if (limit <= 0.2)
                    {
                        limitCount["neg"]++;
                    }
                    negCount++;
                }
                else
                {
                    if (limit <= 0.2)
                    {
                        limitCount["pos"]++;
                    }
                    posCount++;
                }
            }
            Console.WriteLine("模糊率：" + limitCount.Sum(l => l.Value) / countResult);
            Console.WriteLine("负面率：" + negCount / countResult);
            Console.WriteLine("正面率：" + posCount / countResult);
            countResult -= limitCount.Sum(l => l.Value);
            negCount -= limitCount["neg"];
            posCount -= limitCount["pos"];
            Console.WriteLine("去模糊后负面率：" + negCount / countResult);
            Console.WriteLine("去模糊后正面率：" + posCount / countResult);
            Console.ReadLine();
        }
    }
}
