using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 二分类贝叶斯感知机
{
    class CoreCalculations
    {
        /// <summary>
        /// 执行分类
        /// </summary>
        /// <param name="sent"></param>
        /// <param name="d"></param>
        /// <param name="total"></param>
        /// <param name="stopwords"></param>
        /// <returns></returns>
        public static Dictionary<string, double> Classify_(string sent, Dictionary<string, AddOneProb> d, string stopwords)
        {
            Jieba jiebaword = new Jieba();
            jiebaword.doc = Train.FilterSymbol(sent);
            jiebaword.stopwords = stopwords;
            return Sensor(d, jiebaword.JiebaCut());
        }

        /// <summary>
        /// 神经元个数
        /// </summary>
        public static int IDCount = 100;
        
        /// <summary>
        /// 单层感知机
        /// </summary>
        /// <param name="d">记录的输入特征值与权重信息</param>
        /// <param name="x">记录的输入特征</param>
        /// <param name="y">记录的输出分类</param>
        public static Dictionary<string, double> Sensor(Dictionary<string, AddOneProb> d, List<string> x, string y="")
        {
            double total = d.Sum(p => p.Value.Total);
            double neg = 0;
            double pos = 0;
            Dictionary<string, double> temp = new Dictionary<string, double>();
            Dictionary<string, Dictionary<int, double>> ylist = new Dictionary<string, Dictionary<int, double>>();
            ylist.Add("neg", new Dictionary<int, double>());
            ylist.Add("pos", new Dictionary<int, double>());
            for (int id = 1; id <= IDCount; id++)
            {
                if (!string.IsNullOrWhiteSpace(y))
                {
                    if (y == "neg" && id % 2==0)
                    {
                        continue;
                    }
                    if(y=="pos"&&id%2!=0)
                    {
                        continue;
                    }
                }
                ylist["neg"].Add(id, 0);
                ylist["pos"].Add(id, 0);
                neg = 0;
                pos = 0;
                foreach (var word in x)
                {
                    ylist["neg"][id] = 0;
                    ylist["pos"][id] = 0;
                    foreach (var k in d.Keys)
                    {
                        ylist[k][id] = d[k].GetW(word, id);
                        ylist[k][id] = ylist[k][id] * d[k].GetRate(word);
                    }
                    if (string.IsNullOrWhiteSpace(y))
                    {
                        neg += ylist["neg"][id] / (ylist["neg"][id] + ylist["pos"][id]);//逻辑回归算式
                        pos += ylist["pos"][id] / (ylist["pos"][id] + ylist["neg"][id]);//逻辑回归算式
                    }
                    else
                    {
                        neg += ylist["neg"][id];
                        pos += ylist["pos"][id];
                    }
                }
                neg = Math.Abs(neg);
                pos = Math.Abs(pos);
                if (string.IsNullOrWhiteSpace(y))
                {
                    ylist["neg"][id] = neg / x.Count;
                    ylist["pos"][id] = pos / x.Count;
                }
                else
                {
                    ylist["neg"][id] = neg / (neg + pos);
                    ylist["pos"][id] = pos / (pos + neg);
                }
                if (!string.IsNullOrWhiteSpace(y))
                {//有监督训练时候需要反向更新权值
                    Reverse(d, ylist[y], x, y, y == "neg" ? "pos" : "neg", id);
                }
            }
            //neg = ylist["neg"].Where(yy=>yy.Value!=0).Average(yy => yy.Value);
            //pos = ylist["pos"].Where(yy => yy.Value != 0).Average(yy => yy.Value);
            temp.Add("neg", ylist["neg"].Average(yy=>yy.Value));
            temp.Add("pos", ylist["pos"].Average(yy=>yy.Value));
            if (string.IsNullOrWhiteSpace(y))
            {
                //foreach (var word in x)
                //{
                //    Console.WriteLine("词:" + word);
                //    Console.WriteLine("正面频率:" + d["pos"].GetRate(word) + "，权重:");
                //    foreach (var w in d["pos"].W[word])
                //    {
                //        Console.WriteLine(w.Key + ":" + w.Value.W);
                //    }
                //    Console.WriteLine("反面频率:" + d["neg"].GetRate(word) + "，权重:");
                //    foreach (var w in d["neg"].W[word])
                //    {
                //        Console.WriteLine(w.Key + ":" + w.Value.W);
                //    }
                //}
                Console.WriteLine("neg : " + temp["neg"]);
                Console.WriteLine("pos : " + temp["pos"]);
            }

            return temp;
        }
        
        /// <summary>
        /// 反向修正权重
        /// </summary>
        /// <param name="d"></param>
        /// <param name="temp"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="id"></param>
        private static void Reverse(Dictionary<string, AddOneProb> d, Dictionary<int, double> temp, List<string> x, string y,string noy,int id)
        {
            double lr = 0.11;
            //计算新权重
            var loss = 1 - temp[id];
            if (loss < 0.1)
            {
                lr = loss / lr;//动态计算学习率
            }
            foreach (var ad in x)
            {
                var dwc = lr * loss * d[y].GetRate(ad);
                d[y].SetW(ad, id, d[y].GetW(ad, id) + dwc);
                //var dwt = lr * temp[id] * d[noy].GetRate(ad);
                //d[noy].SetW(ad, id, d[noy].GetW(ad, id) + dwt);
            }
        }
    }
}
