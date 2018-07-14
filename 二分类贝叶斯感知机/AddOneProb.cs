using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 二分类贝叶斯感知机
{
    public class AddOneProb
    {
        /// <summary>
        /// 储存词频
        /// </summary>
        public Dictionary<string, double> Freq = new Dictionary<string, double>();
        /// <summary>
        /// 储存权重（先验概率）
        /// </summary>
        public Dictionary<string, Dictionary<int,WMatrix>> W = new Dictionary<string, Dictionary<int, WMatrix>>();
        /// <summary>
        /// 存在率
        /// </summary>
        private Dictionary<string, double> Rate = new Dictionary<string, double>();
        /// <summary>
        /// 存在总数
        /// </summary>
        private double total = 0.0;
        /// <summary>
        /// 获取总数
        /// </summary>
        public double Total
        {
            get
            {
                return total;
            }
        }
        /// <summary>
        /// 不存在时候，默认为有一个
        /// </summary>
        private int none = 1;
        
        /// <summary>
        /// 获取词的频率
        /// </summary>
        /// <param name="key">词</param>
        /// <param name="para1">频率</param>
        public void Get(string key, out double para1)
        {
            if (!Freq.ContainsKey(key))
            {
                para1 = none;
            }
            else
            {
                para1 = Freq[key];
            }
        }
        /// <summary>
        /// 计算存在率
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void CalculatRate()
        {
            Rate = new Dictionary<string, double>();
            foreach (var k in Freq)
            {
                Rate.Add(k.Key, k.Value / total);
            }
        }
        /// <summary>
        /// 获取存在率
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetRate(string key)
        {
            if(Rate.ContainsKey(key))
            {
                return Rate[key];
            }
            double para1;
            Get(key,  out para1);
            Rate.Add(key, para1 / total);
            return Rate[key];
        }
        /// <summary>
        /// 获取权重（先验概率）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ID">神经元编号</param>
        /// <returns></returns>
        public double GetW(string key,int ID)
        {
            if(!W.ContainsKey(key))
            {
                W.Add(key, new Dictionary<int, WMatrix>());
            }
            if (!W[key].ContainsKey(ID))
            {
                WMatrix w = new WMatrix();
                w.ID = ID;
                w.W =Convert.ToDouble( ID)/10;
                W[key].Add(ID, w);
            }
            return W[key][ID].W;
        }
        /// <summary>
        /// 设置权重
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ID"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetW(string key,int ID,double value)
        {
            if (!W.ContainsKey(key))
            {
                W.Add(key, new Dictionary<int, WMatrix>());
            }
            if(!W[key].ContainsKey(ID))
            {
                WMatrix w = new WMatrix();
                w.ID = ID;
                w.W = value;
                W[key].Add(ID, w);
            }
            W[key][ID].W = value;
        }
        /// <summary>
        /// 添加词频
        /// </summary>
        /// <param name="key"></param>
        /// <param name="idCount">神经元个数</param>
        /// <param name="value"></param>
        public void Add(string key,int idCount, int value)
        {
            if(!Freq.ContainsKey(key))
            {
                Freq.Add(key, 1);
            }
            if(!W.ContainsKey(key))
            {
                W.Add(key, new Dictionary<int, WMatrix>());
                for (int id = 1; id <= idCount; id++)
                {
                    WMatrix w = new WMatrix();
                    w.ID = id;
                    w.W =Convert.ToDouble( id)/10;
                    W[key].Add(id, w);
                }
            }
            Freq[key] += value;
            total += value;
        }
        /// <summary>
        /// 打印词频
        /// </summary>
        public void DPrint()
        {
            Console.WriteLine(Freq.Count);
            Console.ReadKey();
            foreach (var key in Freq.Keys)
            {
                Console.WriteLine(key + " : " + Freq[key].ToString());
            }
        }
    }
}
