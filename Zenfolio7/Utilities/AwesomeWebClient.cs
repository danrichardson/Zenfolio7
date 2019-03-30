using System;
using System.Net;

namespace Zenfolio7.Utilities
{
    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 3000; //in milliseconds
            return w;
        }
    }

    public class AwesomeWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
            req.ServicePoint.ConnectionLimit = 20;
            req.Timeout = 10000; //in milliseconds
            return (WebRequest)req;
        }
    }
}
