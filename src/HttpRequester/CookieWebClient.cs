﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HttpRequester
{
    /// <summary>
    /// https://dennymichael.net/2014/03/16/c-enable-cookie-container-on-system-net-webclient-for-authentication/comment-page-1/
    /// </summary>
    public class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; private set; }

        /// <summary>
        /// This will instanciate an internal CookieContainer.
        /// </summary>
        public CookieWebClient()
        {
            this.CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Use this if you want to control the CookieContainer outside this class.
        /// </summary>
        public CookieWebClient(CookieContainer cookieContainer)
        {
            this.CookieContainer = cookieContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null) return base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }
    }
}
