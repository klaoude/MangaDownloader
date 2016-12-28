using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Jint;
using System.IO;
using System.Threading;
using System.Web;
using System.Text.RegularExpressions;

namespace MangaDownloaderV2
{
    public class Evader2
    {
        private static string[] DEFAULT_USER_AGENT = {
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:46.0) Gecko/20100101 Firefox/46.0",
            "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:41.0) Gecko/20100101 Firefox/41.0"
        };

        public static WebClient CreateEvadedWebClient(string url)
        {
            var JInt = new Engine();

            var scrapper = (HttpWebRequest)WebRequest.Create(url);
            var userAgent = DEFAULT_USER_AGENT[4];
            var uri = new Uri(url);

            try
            {
                var reponse = scrapper.GetResponse();
                var reader = new StreamReader(reponse.GetResponseStream());
                var html = reader.ReadToEnd();
                return new WebClient();
            }
            catch(WebException ex)
            {
                Thread.Sleep(5);

                var reader = new StreamReader(ex.Response.GetResponseStream());
                var body = reader.ReadToEnd();
                var parsed_url = uri;
                var domain = uri.Host;
                var submit_url = string.Format("{0}://{1}/cdn-cgi/l/chk_jschl", parsed_url.Scheme, domain);

                var initialCookies = ex.Response.Headers;
                var uri_builder = new UriBuilder(submit_url);
                var param = HttpUtility.ParseQueryString(uri_builder.Query);
                param["jschl_vc"] = Regex.Match(body, "name=\"jschl_vc\" value=\"(\\w +)\"").Groups[1].Value;
                param["pass"] = Regex.Match(body, "name=\"pass\" value=\"(.+?)\"").Groups[1].Value;

                var js = Regex.Match(body, @"setTimeout\(function\(\){\s+(var s,t,o,p,b,r,e,a,k,i,n,g,f.+?\r?\n[\s\S]+?a\.value =.+?)\r?\n").Groups[1].Value;
                js = Regex.Replace(js, @"a\.value = (parseInt\(.+?\)).+", "$1");
                js = Regex.Replace(js, @"\s{3,}[a-z](?: = |\.).+", "");

                js = Regex.Replace(js, @"[\n\\']", "");

                long solved = long.Parse(JInt.Execute(js).GetCompletionValue().ToObject().ToString());
                solved += uri.Host.Length;

                param["jschl_answer"] = solved.ToString();

                var method = method;

                Console.Write("test");
            }

            return new WebClient();
        } 
    }

    public class WebClientEx : WebClient
    {
        public WebClientEx(CookieContainer container)
        {
            this.container = container;
        }

        public CookieContainer CookieContainer
        {
            get { return container; }
            set { container = value; }
        }

        private CookieContainer container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = container;
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                container.Add(cookies);
            }
        }
    }
}
