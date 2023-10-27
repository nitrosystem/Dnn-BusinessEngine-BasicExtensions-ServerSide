using Dapper;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using Newtonsoft.Json.Linq;
using NitroSystem.Dnn.BusinessEngine.BasicActions.Models.PublicServices.Webservice;
using NitroSystem.Dnn.BusinessEngine.Framework.Contracts;
using NitroSystem.Dnn.BusinessEngine.Framework.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NitroSystem.Dnn.BusinessEngine.BasicServices.PublicServices
{
    public class WebserviceRestfulService : ServiceBase<WebServiceOptions>, IService
    {
        public override async Task<ServiceResult> ExecuteAsync<T>()
        {
            ServiceResult result = new ServiceResult();

            var webServiceOptions = this.Model;

            try
            {
                var webServiceResult = new JObject();

                string url = this.ParseParam(webServiceOptions.Url);

                if (webServiceOptions.Params != null)
                {
                    List<string> queryParams = new List<string>();
                    foreach (var item in webServiceOptions.Params)
                    {
                        queryParams.Add(item.ParamName + "=" + this.ParseParam((item.ParamValue ?? "").ToString()));
                    }

                    if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
                }

                result.Query = url;

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = webServiceOptions.Method;

                if (webServiceOptions.Headers != null)
                {
                    foreach (var item in webServiceOptions.Headers.Where(h => !h.IsSystem || h.IsSelected))
                    {
                        switch (item.ParamName)
                        {
                            case "Accept":
                                request.Accept = this.ParseParam(item.ParamValue);
                                break;
                            case "Connection":
                                request.Connection = this.ParseParam(item.ParamValue);
                                break;
                            case "Content-Length":
                                request.ContentLength = long.Parse(this.ParseParam(item.ParamValue));
                                break;
                            case "Content-Type":
                                request.ContentType = this.ParseParam(item.ParamValue);
                                break;
                            case "Date":
                                request.Date = DateTime.Parse(this.ParseParam(item.ParamValue));
                                break;
                            case "Expect":
                                request.Expect = this.ParseParam(item.ParamValue);
                                break;
                            case "Host":
                                request.Host = this.ParseParam(item.ParamValue);
                                break;
                            case "If-Modified-Since":
                                request.IfModifiedSince = DateTime.Parse(this.ParseParam(item.ParamValue));
                                break;
                            case "Range":
                                request.AddRange(int.Parse(this.ParseParam(item.ParamValue)));
                                break;
                            case "Referer":
                                request.Referer = this.ParseParam(item.ParamValue);
                                break;
                            case "Transfer-Encoding":
                                request.TransferEncoding = this.ParseParam(item.ParamValue);
                                break;
                            case "User-Agent":
                                request.UserAgent = this.ParseParam(item.ParamValue);
                                break;
                            case "Proxy-Connection":
                                //request.Proxy = this.ParseParam(item.ParamValue);;
                                break;
                            default:
                                request.Headers.Add(item.ParamName, this.ParseParam(item.ParamValue));
                                break;
                        }
                    }
                }

                if (webServiceOptions.Authorization != null)
                {
                    if (webServiceOptions.Authorization.Type == "Bearer")
                    {
                        string bearer = this.ParseParam(webServiceOptions.Authorization.Bearer);

                        request.Headers.Add("Authorization", bearer);
                    }
                    else if (webServiceOptions.Authorization.Type == "Basic")
                    {
                        string username = this.ParseParam(webServiceOptions.Authorization.BasicAuth.Username);
                        string password = this.ParseParam(webServiceOptions.Authorization.BasicAuth.Password);

                        String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                        request.Headers.Add("Authorization", "Basic " + encoded);
                    }
                }

                if (!string.IsNullOrEmpty(webServiceOptions.BodyRaw))
                {
                    string body = this.ParseParam(webServiceOptions.BodyRaw);

                    byte[] dataStream = Encoding.UTF8.GetBytes(body);

                    request.ContentLength = dataStream.Length;
                    Stream newStream = request.GetRequestStream();

                    newStream.Write(dataStream, 0, dataStream.Length);
                    newStream.Close();
                }

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();

                    webServiceResult.Add(new JProperty("StatusCode", response.StatusCode));
                    webServiceResult.Add(new JProperty("StatusDescription", response.StatusDescription));

                    webServiceResult.Add(new JProperty("Headers", JObject.Parse("{}")));

                    foreach (var key in response.Headers.AllKeys)
                    {
                        (webServiceResult["Headers"] as JObject).Add(new JProperty(key, response.Headers[key]));
                    }

                    string text;
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        text = sr.ReadToEnd();
                    }

                    webServiceResult.Add(new JProperty("Body", text));

                }
                catch (Exception ex2)
                {
                    throw new Exception(ex2.Message);
                }

                result.DataRow = webServiceResult;

                return result;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.ErrorException = ex;
            }

            return result;
        }

        public override bool TryParseModel(string serviceSettings)
        {
            throw new NotImplementedException();
        }
    }
}
