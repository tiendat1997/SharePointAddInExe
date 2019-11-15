using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace SharePointAddInAssignmentWeb.Helpers
{
    public static class WebMVCHelper
    {
        public const string SERVICES_TOKEN = "servicesToken";
        public static bool HasCacheEntry(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpControllerContext");

            string cacheKey = GetCacheKeyValue(httpContext);

            if (!String.IsNullOrEmpty(cacheKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetCacheKeyValue(HttpContextBase httpContext)
        {
            string cookie = httpContext.Request.Cookies[SERVICES_TOKEN].Value;
            if (string.IsNullOrEmpty(cookie) == false)
            {
                return httpContext.Request.Cookies[SERVICES_TOKEN].Value;
            }
            else
            {
                NameValueCollection queryParams = HttpUtility.ParseQueryString(httpContext.Request.QueryString.ToString());
                return queryParams.Get(SERVICES_TOKEN);
            }
        }
    }
}