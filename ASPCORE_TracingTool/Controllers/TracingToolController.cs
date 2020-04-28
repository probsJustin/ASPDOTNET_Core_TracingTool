using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Http.Features;

namespace ASPCORE_TracingTool.Controllers
{
    [ApiController]
    [Route("/")]
    public class TracingTool : ControllerBase
    {
        Dictionary<string, string> ResponseHeaders = new Dictionary<string, string> { }; 
        Dictionary<string, string> RequestHeaders = new Dictionary<string, string> { };
        const string SLREQ = "X-dynaSupLabReq-info";
        const string SLRES = "X-dynaSupLabRes-info";
        const string SLPATH = "X-dynaSupLabPath-info";
        const string SLPOS = "X-dynaSupLabPosition-info";
        const string SLPASS = "url_passthrough";


        public bool setReqHeader(string funcVar, string index)
        {
            try
            {
                RequestHeaders[index] = funcVar;
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        public bool devdb(string funcVar)
        {
            if(Request.Query["db"] == "True")
            {
                addBody(line("DEV DEBUG:" + funcVar));
                return true;
            }
            else
            {
                return false; 
            }
        }
        public bool AppendToReqHeaders(string funcVar, string index)
        {
            if (RequestHeaders.ContainsKey(index))
            {
                RequestHeaders[index] = funcVar;
                return true; 
            }
            else
            {
                return false;
            }
        }

        public bool AppendToResHeaders(string funcVar, string index)
        {
            if (ResponseHeaders.ContainsKey(index))
            {
                ResponseHeaders[index] = funcVar;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool addBody(string funcVar)
        {
            try
            {
                returnObject["data"] += funcVar;
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }
        public string pp_stamp(string contentLength, string requestURI)
        {
            string returnObject = "";
            DateTimeOffset dto = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            returnObject += "ts=" + dto.ToUnixTimeSeconds().ToString() + ',';
            returnObject += "client-ip=" + Request.HttpContext.Connection.RemoteIpAddress + ',';
            returnObject += "content-length=" + contentLength + ',';
            returnObject += "requestURI=" + requestURI + '$';
            return returnObject;
        }
        public string line(string funcVar = "")
        {
            return funcVar + '\n';
        }

        public string responseChecker(string funcVar)
        //just checks funcVar to see if it is empty - this is alot easier for certain aspects of the request structure
        {
            string returnObject = "";
            if (funcVar == "" | funcVar == null)
            {
                returnObject = "Empty";
            }
            else
            {
                returnObject = funcVar;
            }
            return returnObject;
        }
        public Dictionary<string, string> getRequest(string urlFuncVar, Dictionary<string, string> funcHeader)
        {
            Dictionary<string, string> myResponse = new Dictionary<string, string> { };
            HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(urlFuncVar);
            foreach (KeyValuePair<string, string> instanceHeader in funcHeader)
            {
                HttpWReq.Headers.Add(instanceHeader.Key, instanceHeader.Value);
            }

            try
            {
                HttpWReq.Method = "GET";
                HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
                myResponse["code"] = (int)HttpWResp.StatusCode + " : " + HttpWResp.StatusDescription.ToString();
                myResponse["X-dynaSupLabRes-info"] = HttpWResp.Headers["X-dynaSupLabRes-info"];
                myResponse["X-dynaSupLabReq-info"] = HttpWResp.Headers["X-dynaSupLabReq-info"];
                HttpWResp.Close();
            }
            catch (WebException e)
            {
                HttpWebResponse HttpWResp = (HttpWebResponse)e.Response;
                myResponse["code"] = (int)HttpWResp.StatusCode + " : " + HttpWResp.StatusDescription.ToString();
            }
            return myResponse;
        }

        Dictionary<string, string> returnObject = new Dictionary<string, string> { };
        Dictionary<string, string> returnInstance = new Dictionary<string, string> { };
        public Dictionary<string, string> requestFactory(Microsoft.AspNetCore.Http.HttpRequest requestFuncVar)
        {
            returnObject["data"] = "sure";
            returnObject["data"] = (string)"";
            Dictionary<string, string> requestInstance = new Dictionary<string, string> { };
            string[] splitPaths = new string[40];
            try
            {
                requestInstance["X-dynaSupLabPath-info"] = responseChecker(requestFuncVar.Headers["X-dynaSupLabPath-info"]);
                requestInstance["X-dynaSupLabPosition-info"] = responseChecker(requestFuncVar.Headers["X-dynaSupLabPosition-info"]);
                requestInstance["X-dynaSupLabRes-info"] = responseChecker(requestFuncVar.Headers["X-dynaSupLabRes-info"]);
                requestInstance["X-dynaSupLabReq-info"] = responseChecker(requestFuncVar.Headers["X-dynaSupLabReq-info"]);
                requestInstance["debug"] = responseChecker(Request.Query["debug"]);
                requestInstance["url_passthrough"] = responseChecker(Request.Query["url_passthrough"]);
                requestInstance["html"] = responseChecker(Request.Query["html"]);
                requestInstance["responseCode"] = responseChecker(Request.Query["responseCode"]);
            }
            catch (Exception e)
            {
                returnObject["data"] = line("Checking Incoming Request Variables Failed");
            }

            try
            {
                Dictionary<string, string> getResponse = new Dictionary<string, string> { };

                //Set first request and response headers 
                if (requestInstance["html"] == "True" | requestInstance["html"] == "true")
                {
                    addBody("<!DOCTYPE html><html>");
                    returnObject["type"] = "text/html";
                }
                else
                {
                    returnObject["type"] = "test";
                }
                if (Request.Headers.ContainsKey(SLREQ))
                {
                    devdb(SLREQ + " Header found on initial request.");
                    AppendToReqHeaders(pp_stamp(Request.ContentLength.ToString(), Request.Path), SLREQ);
                }
                else
                {
                    RequestHeaders[SLREQ] = pp_stamp(Request.ContentLength.ToString(), Request.Path);
                    devdb(SLRES + " Header not found on initial request.");
                }
                if (Request.Headers.ContainsKey(SLRES))
                {
                    devdb(SLRES + " Header found on initial request.");
                    AppendToResHeaders(pp_stamp(Request.ContentLength.ToString(), Request.Path), SLRES);
                }
                else 
                {
                    devdb(SLRES + " Header not found on initial request.");
                    ResponseHeaders[SLRES] = pp_stamp(Response.ContentLength.ToString(), Request.Path);
                }
                if (requestInstance["responseCode"] != "Empty")
                {
                    devdb("Setting status code from request as it is not 'Empty'");
                    Response.StatusCode = Int32.Parse(requestInstance["responseCode"]);
                    addBody(line("Support Lab Debug Variable Response Checker"));
                    addBody(line("Status Code: " + Response.StatusCode + line()));
                }



                if (requestInstance["url_passthrough"] != "Empty")
                {
                    devdb("Setting url pass through as it is not 'Empty'");
                    addBody(line(line() + "Dynatrace SUPLAB request url_passthrough information: "));
                    addBody(line("url_passthrough : " + requestInstance["url_passthrough"]));
                    addBody(line("url_passthrough : Enabled"));
                    addBody(line("url_passthrough : Attempting"));
                    getResponse = getRequest(requestInstance["url_passthrough"], RequestHeaders);
                    addBody(line("Response Code: " + getResponse["code"]));
                    if (getResponse.ContainsKey(SLREQ))
                    {
                        devdb(SLPASS + " Request found x dyna sup lab debug headers on the response for: " + SLREQ);
                        AppendToReqHeaders(getResponse[SLREQ], SLREQ);
                        AppendToResHeaders(getResponse[SLREQ], SLREQ);
                    }
                    if (getResponse.ContainsKey(SLRES))
                    {
                        devdb(SLPASS + " Request found x dyna sup lab debug headers on the response for: " + SLRES);
                        AppendToReqHeaders(getResponse[SLRES], SLRES); 
                        AppendToResHeaders(getResponse[SLRES], SLRES);
                    }
                }
                else
                {
                    devdb(SLPASS + " is 'Empty' ----- setting 'url_passthrough_unit' to false");
                    returnObject["url_passthrough_unit"] = "false";
                }
                if (requestInstance["debug"] == "true" | requestInstance["debug"] == "True")
                {
                    addBody(line(line("Dynatrace SUPLAB Debug Request Information:")));
                    addBody(line("Dynatrace SUPLAB Debug Request Parameters:"));
                    addBody(line("Debug : " + requestInstance["debug"]));
                    returnObject["debug"] = requestInstance["debug"];
                    addBody(line(line() + "Dynatrace SUPLAB Debug Request Headers:"));
                    //add headers to the debug body
                    foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> I in Request.Headers)
                    {
                        addBody(line(I.Key + " : " + I.Value));
                    }

                    if (requestInstance["url_passthrough"] != "Empty")
                    {
                        addBody(line("Url_PassThrough : " + requestInstance["url_passthrough"]));
                        addBody(requestInstance["url_passthrough"]);
                    }

                    if (requestInstance["X-dynaSupLabPosition-info"] != "Empty")
                    {
                        addBody(line(line() + "Dynatrace SUPLAB Debug Path Position Information:"));
                        addBody("X-dynaSupLabPosition-info : " + requestInstance["X-dynaSupLabPosition-info"]);
                    }
                    
                }
                else
                {
                    returnObject["data"] = line("Debug : False");
                }
                if (requestInstance["X-dynaSupLabPath-info"] != "Empty")
                {
                    addBody(line(line() + "Dynatrace SUPLAB Debug Path Information:"));
                    addBody("X-dynaSupLabPath-info : " + requestInstance["X-dynaSupLabPath-info"]);
                    splitPaths = requestInstance["X-dynaSupLabPath-info"].Split(",");
                    int counter = 1;
                    returnObject["data"] += line();
                    if (requestInstance["X-dynaSupLabPosition-info"] != "Empty")
                    {
                        Dictionary<string, string> requestResponse = new Dictionary<string, string> { };

                        int pathIndex = Int32.Parse(requestInstance["X-dynaSupLabPosition-info"])- 1;
                        addBody(line("URL Selected : " + splitPaths[pathIndex]));

                        setReqHeader((pathIndex + 1).ToString(), SLPOS);
                        setReqHeader(requestInstance[SLPATH], SLPATH);

                        requestResponse = getRequest(splitPaths[pathIndex], RequestHeaders);
                        //Add to the response headers of the request to this reproducer 
                        if (requestResponse.ContainsKey(SLREQ))
                        {
                            devdb("Found dyna sup lab debug headers on the path request: " + SLREQ);
                            AppendToReqHeaders(requestResponse[SLREQ], SLREQ);
                            AppendToResHeaders(requestResponse[SLREQ], SLREQ);
                        }
                        if (requestResponse.ContainsKey(SLRES))
                        {
                            devdb("Found dyna sup lab debug headers on the path request: " + SLRES);
                            AppendToReqHeaders(requestResponse[SLRES], SLRES);
                            AppendToResHeaders(requestResponse[SLRES], SLRES);
                        }
                        addBody(line("URL Response Code : " + requestResponse["code"]));
                    }
                    foreach (string x in splitPaths)
                    {
                        addBody(line("URL : " + counter.ToString() + " : " + x));
                        counter++;
                    }
                }
                if (requestInstance["html"] == "True" | requestInstance["html"] == "true")
                {
                    addBody("</html>");
                }
            }
            catch (Exception e)
            {
                returnObject["data"] = line(e.ToString());
            }
            return returnObject;
        }


        [HttpGet]
        public ActionResult<string> Get()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_GET")]
        public ContentResult apiTest_GET()
        {
            returnInstance = requestFactory(Request);
            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_POST")]
        public ContentResult apiTest_POST()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_PUT")]
        public ContentResult apiTest_PUT()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_DELETE")]
        public ContentResult apiTest_DELETE()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_SUCCESS")]
        public ContentResult apiTest_SUCCESS()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_FAILURE")]
        public ContentResult apiTest_FAILURE()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_PATH")]
        public ContentResult apiTest_PATH()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_ResponseChecker")]
        public ContentResult apiTest_ResponseChecker()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_CUSTOM")]
        public ContentResult apiTest_CUSTOM()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }
        [HttpGet]
        [Route("/apiTest_DOTNET_REMOTING")]
        public ContentResult apiTest_DOTNET_REMOTING()
        {
            returnInstance = requestFactory(Request);

            return new ContentResult
            {
                ContentType = returnInstance["type"],
                Content = returnInstance["data"]
            };
        }

    }
}

