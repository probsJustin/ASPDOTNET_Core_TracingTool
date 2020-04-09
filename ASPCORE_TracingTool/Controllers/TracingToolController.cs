using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace ASPCORE_TracingTool.Controllers
{
    [ApiController]
    [Route("/")]
    public class TracingTool : ControllerBase
    {
        public string ln(string funcVar)
        {
            return funcVar + '\n';
        }
        public string responseChecker(string funcVar)
        {
            string returnObject = "";
            if(funcVar == "" | funcVar == null)
            {
                returnObject = "Empty";
            }
            else
            {
                returnObject = funcVar;
            }
            return returnObject;
        }
        public Dictionary<string,string> getRequest(string urlFuncVar)
        {
            Dictionary<string,string> myResponse = new Dictionary<string, string> { };
            HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(urlFuncVar);
            try
            {
                HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
                // Insert code that uses the response object.
                myResponse["code"] = (int)HttpWResp.StatusCode + " : " + HttpWResp.StatusDescription.ToString();
                //myResponse["body"] = HttpWResp.
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
        public Dictionary<string,string> requestFactory(Microsoft.AspNetCore.Http.HttpRequest requestFuncVar)
        {
            returnObject["data"] = "";
            Dictionary<string, string> requestInstance = new Dictionary<string, string> { };
            string[] splitPaths = new string[40];
            try
            {
                requestInstance["X-dynaSupLabPath-info"] = responseChecker(requestFuncVar.Headers["X-dynaSupLabPath-info"]);
                requestInstance["X-dynaSupLabPosition-info"] = responseChecker(requestFuncVar.Headers["X-dynaSupLabPosition-info"]);
                requestInstance["X-dynaSupLabRes-info"] = responseChecker(requestFuncVar.Headers["X-dynaSupLabRes-info"]);
                requestInstance["X-dynaSupLabReq-info"] = responseChecker(requestFuncVar.Headers["X-dynaSupLabReq-info"]);

            }
            catch (Exception e)
            {
                returnObject["data"] += e.ToString();
            }
            
            try
            {
                requestInstance["debug"] = Request.Query["debug"];
            }
            catch (Exception e)
            {
                requestInstance["debug"] = "";
            }
            try
            {
                requestInstance["url_passthrough"] = Request.Query["url_passthrough"];
            }
            catch (Exception e)
            {
                requestInstance["url_passthrough"] = "";
            }
            try 
            {
                if (requestInstance["debug"] == "true")
                {
                    returnObject["data"] += ln("Dynatrace SUPLAB Debug Request Information:" + '\n');

                    returnObject["data"] += ln("Dynatrace SUPLAB Debug Request Parameters:");

                    returnObject["data"] += "Debug : " + requestInstance["debug"] + '\n';
                    if (requestInstance["url_passthrough"] != null)
                    {
                        returnObject["data"] += "Url_PassThrough : " + requestInstance["url_passthrough"] + '\n';
                        returnObject["url_passthrough"] = requestInstance["url_passthrough"];
                    }

                    returnObject["debug"] = requestInstance["debug"];

                    returnObject["data"] += '\n' + "Dynatrace SUPLAB Debug Request Headers:" + '\n';

                    foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> I in Request.Headers)
                    {
                        returnObject["data"] = returnObject["data"] + I.Key + " : " + I.Value + '\n';
                    }
                    if (requestInstance["X-dynaSupLabPath-info"] != "")
                    {
                        returnObject["data"] += '\n' + "Dynatrace SUPLAB Debug Path Information:" + '\n';
                        returnObject["data"] += "X-dynaSupLabPath-info : " + requestInstance["X-dynaSupLabPath-info"];
                        splitPaths = requestInstance["X-dynaSupLabPath-info"].Split(",");
                        int counter = 1;
                        returnObject["data"] += '\n';
                        if (requestInstance["X-dynaSupLabPosition-info"] != "")
                        {
                            returnObject["data"] += "URL Selected : " + splitPaths[Int32.Parse(requestInstance["X-dynaSupLabPosition-info"]) - 1] + '\n';

                        }
                            foreach (string x in splitPaths)
                        {
                            returnObject["data"] += "URL : " + counter.ToString() + " : " + x + '\n';
                            counter++;
                        }
                    }
                    if (requestInstance["X-dynaSupLabPosition-info"] != "")
                    {
                        returnObject["data"] += '\n' + "Dynatrace SUPLAB Debug Path Position Information:" + '\n';
                        returnObject["data"] += "X-dynaSupLabPosition-info : " + requestInstance["X-dynaSupLabPosition-info"];
                    }
                    if (requestInstance["X-dynaSupLabReq-info"] != "")
                    {
                        returnObject["data"] += '\n' + "Dynatrace SUPLAB Debug Request Purepath Information:" + '\n';
                        returnObject["data"] += "X-dynaSupLabReq-info : " + requestInstance["X-dynaSupLabReq-info"];
                    }
                    if (requestInstance["X-dynaSupLabRes-info"] != "")
                    {
                        returnObject["data"] += '\n' + "Dynatrace SUPLAB Debug Response Purepath Information:" + '\n';
                        returnObject["data"] += "X-dynaSupLabRes-info : " + requestInstance["X-dynaSupLabRes-info"] + '\n';
                    }
                }
                else
                {
                    returnObject["data"] = "Debug : False" + '\n';
                }
                if(requestInstance["url_passthrough"] != null)
                {
                    Dictionary<string, string> getResponse = new Dictionary<string, string> { };
                    returnObject["data"] += '\n' + "Dynatrace SUPLAB request url_passthrough information: " + '\n';
                    returnObject["data"] += "url_passthrough : " + requestInstance["url_passthrough"] + '\n';
                    returnObject["data"] += "url_passthrough : Enabled" + '\n';
                    returnObject["data"] += "url_passthrough : Attempting" + '\n';
                    getResponse = getRequest(requestInstance["url_passthrough"]);
                    returnObject["data"] += "Response Code: " + getResponse["code"] + '\n';
                }
                else
                {
                    returnObject["url_passthrough_unit"] = "false";
                }

            }
            catch (Exception e)
            {
                returnObject["data"] = e.ToString();
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

    }
}
