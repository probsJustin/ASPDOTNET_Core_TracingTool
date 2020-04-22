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
        public string appendHeaders(string funcVarOut, string funcVarIn)
        {
            string returnObject = funcVarIn;
            returnObject += funcVarOut;
            return returnObject;
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

        public Dictionary<string, string> getRequest(string urlFuncVar)
        {
            Dictionary<string, string> myResponse = new Dictionary<string, string> { };
            HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(urlFuncVar);
            try
            {
                HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
                // Insert code that uses the response object.
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

            }
            catch (Exception e)
            {
                returnObject["data"] = line("Checking Incoming Request Variables Failed");
            }

            try
            {
                Dictionary<string, string> getResponse = new Dictionary<string, string> { };

                if (requestInstance["url_passthrough"] != "Empty")
                {
                    returnObject["data"] += line(line() + "Dynatrace SUPLAB request url_passthrough information: ");
                    returnObject["data"] += line("url_passthrough : " + requestInstance["url_passthrough"]);
                    returnObject["data"] += line("url_passthrough : Enabled");
                    returnObject["data"] += line("url_passthrough : Attempting");
                    getResponse = getRequest(requestInstance["url_passthrough"]);
                    returnObject["data"] += line("Response Code: " + getResponse["code"]);
                }
                else
                {
                    returnObject["url_passthrough_unit"] = "false";
                }
                if (requestInstance["debug"] == "true" | requestInstance["debug"] == "True")
                {
                    returnObject["data"] += line(line("Dynatrace SUPLAB Debug Request Information:"));
                    returnObject["data"] += line("Dynatrace SUPLAB Debug Request Parameters:");
                    returnObject["data"] += line("Debug : " + requestInstance["debug"]);

                    if (requestInstance["url_passthrough"] != "Empty")
                    {
                        returnObject["data"] += line("Url_PassThrough : " + requestInstance["url_passthrough"]);
                        returnObject["url_passthrough"] = requestInstance["url_passthrough"];
                    }

                    returnObject["debug"] = requestInstance["debug"];

                    returnObject["data"] += line(line() + "Dynatrace SUPLAB Debug Request Headers:");

                    foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> I in Request.Headers)
                    {
                        returnObject["data"] = line(returnObject["data"] + I.Key + " : " + I.Value);
                    }
                    if (requestInstance["X-dynaSupLabPath-info"] != "Empty")
                    {
                        returnObject["data"] += line(line() + "Dynatrace SUPLAB Debug Path Information:");
                        returnObject["data"] += "X-dynaSupLabPath-info : " + requestInstance["X-dynaSupLabPath-info"];
                        splitPaths = requestInstance["X-dynaSupLabPath-info"].Split(",");
                        int counter = 1;
                        returnObject["data"] += line();
                        if (requestInstance["X-dynaSupLabPosition-info"] != "Empty")
                        {
                            Dictionary<string, string> newHeaders = new Dictionary<string, string> { };
                            int pathIndex = Int32.Parse(requestInstance["X-dynaSupLabPosition-info"]) - 1;
                            returnObject["data"] += line("URL Selected : " + splitPaths[pathIndex]);
                            newHeaders["X-dynaSupLabPosition-info"] = (pathIndex + 2).ToString();
                            newHeaders["X-dynaSupLabPath-info"] = requestInstance["X-dynaSupLabPath-info"];
                            if (requestInstance["X-dynaSupLabRes-info"] != "")
                            {
                                newHeaders["X-dynaSupLabRes-info"] = requestInstance["X-dynaSupLabRes-info"];
                            }
                            if (requestInstance["X-dynaSupLabReq-info"] != "")
                            {
                                newHeaders["X-dynaSupLabReq-info"] = requestInstance["X-dynaSupLabReq-info"];
                            }
                            returnObject["data"] += line("URL Response Code : " + getRequest(splitPaths[pathIndex], newHeaders)["code"]);
                        }
                        foreach (string x in splitPaths)
                        {
                            returnObject["data"] += line("URL : " + counter.ToString() + " : " + x);
                            counter++;
                        }
                    }
                    if (requestInstance["X-dynaSupLabPosition-info"] != "Empty")
                    {
                        returnObject["data"] += line(line() + "Dynatrace SUPLAB Debug Path Position Information:");
                        returnObject["data"] += "X-dynaSupLabPosition-info : " + requestInstance["X-dynaSupLabPosition-info"];
                    }
                    if (requestInstance["X-dynaSupLabReq-info"] != "Empty")
                    {
                        returnObject["data"] += line(line() + "Dynatrace SUPLAB Debug Request Purepath Information:");
                        returnObject["data"] += "X-dynaSupLabReq-info : " + requestInstance["X-dynaSupLabReq-info"];
                        Response.Headers.Add("X-dynaSupLabReq-info", appendHeaders(pp_stamp(Request.ContentLength.ToString(), Request.Path), getResponse["X-dynaSupLabReq-info"].ToString()));
                    }
                    else
                    {
                        returnObject["data"] += line(line() + "Dynatrace SUPLAB Debug Request Purepath Information:");
                        returnObject["data"] += "X-dynaSupLabReq-info : " + requestInstance["X-dynaSupLabReq-info"];
                        Response.Headers.Add("X-dynaSupLabReq-info", appendHeaders(pp_stamp(Request.ContentLength.ToString(), Request.Path), getResponse["X-dynaSupLabReq-info"].ToString()));
                    }
                    if (requestInstance["X-dynaSupLabRes-info"] != "Empty")
                    {
                        returnObject["data"] += line(line() + "Dynatrace SUPLAB Debug Response Purepath Information:");
                        returnObject["data"] += line("X-dynaSupLabRes-info : " + requestInstance["X-dynaSupLabRes-info"]);
                        Response.Headers.Add("X-dynaSupLabRes-info", appendHeaders("<my test append>", getResponse["X-dynaSupLabRes-info"].ToString()));
                    }
                    else
                    {
                        returnObject["data"] += line(line() + "Dynatrace SUPLAB Debug Response Purepath Information:");
                        returnObject["data"] += line("X-dynaSupLabRes-info : " + requestInstance["X-dynaSupLabRes-info"]);
                        Response.Headers.Add("X-dynaSupLabRes-info", appendHeaders("<my test append>", getResponse["X-dynaSupLabRes-info"].ToString()));
                    }


                }
                else
                {
                    returnObject["data"] = line("Debug : False");
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
        public ActionResult<string> apiTest_GET()
        {
            returnInstance = requestFactory(Request);
            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_POST")]
        public ActionResult<string> apiTest_POST()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_PUT")]
        public ActionResult<string> apiTest_PUT()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_DELETE")]
        public ActionResult<string> apiTest_DELETE()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_SUCCESS")]
        public ActionResult<string> apiTest_SUCCESS()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_FAILURE")]
        public ActionResult<string> apiTest_FAILURE()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_PATH")]
        public ActionResult<string> apiTest_PATH()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_ResponseChecker")]
        public ActionResult<string> apiTest_ResponseChecker()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_CUSTOM")]
        public ActionResult<string> apiTest_CUSTOM()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }
        [HttpGet]
        [Route("/apiTest_DOTNET_REMOTING")]
        public ActionResult<string> apiTest_DOTNET_REMOTING()
        {
            returnInstance = requestFactory(Request);

            return returnInstance["data"];
        }

    }
}

