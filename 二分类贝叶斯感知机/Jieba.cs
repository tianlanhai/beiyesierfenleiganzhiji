using JiebaNet.Segmenter;
using System.Collections.Generic;

namespace 二分类贝叶斯感知机
{
    /// <summary>
    /// 切词与过滤
    /// </summary>
    class Jieba
    {
        /// <summary>
        /// 当前输入语句
        /// </summary>
        public string doc { get; set; }
        /// <summary>
        /// 已定义正面词
        /// </summary>
        public string PosWords { get; set; }
        /// <summary>
        /// 已定义负面词
        /// </summary>
        public string NegWords { get; set; }
        /// <summary>
        /// 排除词
        /// </summary>
        public string stopwords { get; set; }
        /// <summary>
        /// 切词
        /// </summary>
        /// <returns></returns>
        public List<string> JiebaCut()
        {
            JiebaSegmenter jiebaseg = new JiebaSegmenter();
            var segment = jiebaseg.Cut(doc);
            List<string> cutresult = new List<string>();
            foreach (var i in segment)
            {
                if (stopwords.Contains(i))
                {//不参与计算的词排除
                    continue;
                }
                cutresult.Add(i);
            }
            return cutresult;
        }
        /// <summary>
        /// 切词与过滤
        /// </summary>
        /// <param name="pos">是否过滤，true过滤正面，false过滤负面</param>
        /// <returns></returns>
        public List<string> handle_sentiment(bool pos = true)
        {
            var words = JiebaCut();
            string PosOrNegWords = pos ? PosWords : NegWords;
            List<string> handle_result = new List<string>();
            foreach (var word in words)
            {
                if (PosOrNegWords.Contains(word))
                {//定义过的反向词不参与正向
                    continue;
                }
                handle_result.Add(word);
            }
            return handle_result;
        }
    }
}
