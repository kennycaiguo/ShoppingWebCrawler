﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


using System.Collections.Specialized;
using System.Net.Http;
using ShoppingWebCrawler.Host.Common.Http;
using System.Net;
using ShoppingWebCrawler.Host.Headless;
using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common;
/*
var etaoWeb = new YihaodianWebPageService();

string con = etaoWeb.QuerySearchContent("洗面奶男") ;

System.Diagnostics.Debug.WriteLine(con);
*/
namespace ShoppingWebCrawler.Host.PlatformCrawlers.WebPageService
{
    /// <summary>
    /// 一号店搜索页面抓取
    /// </summary>
    public class YhdWebPageService : BaseWebPageService
    {


        

        public YhdWebPageService()
        {
        }


        /// <summary>
        /// 覆盖抽象属性实现自身的http加载器
        /// </summary>
        public override IBrowserRequestLoader RequestLoader
        {
            get
            {
                return YhdMixReuestLoader.Current;
            }
        }







        ///------------内部类-----------------

        /// <summary>
        /// 一号店的混合请求类
        /// 1 根据传入的搜索url  使用 CEF打开 指定地址
        /// 2 拦截出来请求数据的地址
        /// 3 拦截后 把对应的Cookie拿出来
        /// 4  使用.net httpclient 将请求发送出去 得到相应返回
        /// 
        /// 为了保证性能  保持此类单个实例 
        /// </summary>
        public class YhdMixReuestLoader : BaseBrowserRequestLoader<YhdMixReuestLoader>
        {
            public static readonly string YihaodianSiteUrl = GlobalContext.SupportPlatforms.Find(x => x.Platform == SupportPlatformEnum.Yhd).SiteUrl;

            /// <summary>
            /// 一号店请求 搜索地址页面
            /// </summary>
            private const string templateOfSearchUrl = "http://search.yhd.com/c0-0/k{0}/";

            /// <summary>
            /// 请求客户端
            /// </summary>
            private static CookedHttpClient YihaodianHttpClient;




            /// <summary>
            /// 静态构造函数
            /// </summary>
            static YhdMixReuestLoader()
            {
                //静态创建请求客户端
               // YihaodianHttpClient = new CookiedCefBrowser().BindingHttpClient;

                //初始化头信息
                var requestHeaders = BaseRequest.GetCommonRequestHeaders();
                requestHeaders.Add("Accept-Encoding", "gzip, deflate");//接受gzip流 减少通信body体积
                requestHeaders.Add("Host", "search.yhd.com");
                requestHeaders.Add("Upgrade-Insecure-Requests", "1");
                
                YihaodianHttpClient = new CookedHttpClient();
                HttpServerProxy.FormatRequestHeader(YihaodianHttpClient.Client.DefaultRequestHeaders, requestHeaders);
            }

            public YhdMixReuestLoader()
            {
                ///一号店刷新搜索页cookie的地址
                this.RefreshCookieUrlTemplate =templateOfSearchUrl;

                this.IntiCefWebBrowser();
            }

            public override string LoadUrlGetSearchApiContent(IFetchWebPageArgument queryParas)
            {

                string keyWord = queryParas.KeyWord;
                if (string.IsNullOrEmpty(keyWord))
                {
                    return string.Empty;
                }


                //加载Cookie
                var ckVisitor = new LazyCookieVistor();
                var cks = ckVisitor.LoadCookies(YihaodianSiteUrl);


                //优先使用格式化好的查询地址
                string searchUrl = "";
                if (null != queryParas.ResolvedUrl)
                {
                    searchUrl = queryParas.ResolvedUrl.Url;
                }
                else
                {
                    searchUrl = string.Format(templateOfSearchUrl, keyWord);
                }



                var client = YihaodianHttpClient;
                client.Client.DefaultRequestHeaders.Referrer = new Uri(string.Format("http://search.yhd.com/c0-0/k{0}/", keyWord));
                ////加载cookies
                ////获取当前站点的Cookie
                client.ChangeGlobleCookies(cks, YihaodianSiteUrl);

                // 4 发送请求
                var clientProxy = new HttpServerProxy() { Client = client.Client, KeepAlive = true };

                //注意：对于响应的内容 不要使用内置的文本 工具打开，这个工具有bug.看到的文本不全面
                //使用json格式打开即可看到里面所有的字符串

               string content = clientProxy.GetRequestTransfer(searchUrl, null);



                return content;

            }





        }


    }
}
