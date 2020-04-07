using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASPCORE_TracingTool.Controllers
{
    [ApiController]
    [Route("/")]
    public class TracingTool : ControllerBase
    {


        Dictionary<string, string> returnObject = new Dictionary<string, string> { };
        Dictionary<string, string> returnInstance = new Dictionary<string, string> { }; 
        public Dictionary<string,string> requestFactory(Microsoft.AspNetCore.Http.HttpRequest requestFuncVar)
        {
            returnObject["data"] = "";
            Dictionary<string, string> requestInstance = new Dictionary<string, string> { };
            try
            {
                requestInstance["debug"] = Request.Query["debug"];
                requestInstance["url_passthrough"] = Request.Query["url_passthrough"];
                if (requestInstance["debug"].ToLower() == "true")
                {

                    returnObject["data"] += "Dynatrace SUPLAB Debug Request Information:" + '\n';

                    returnObject["data"] += "Dynatrace SUPLAB Debug Request Parameters:" + '\n';

                    returnObject["data"] += "Debug : " + requestInstance["debug"] + '\n';
                    returnObject["data"] += "Url_PassThrough : " + requestInstance["url_passthrough"] + '\n';

                    returnObject["url_passthrough"] = requestInstance["url_passthrough"];
                    returnObject["debug"] = requestInstance["debug"];

                    returnObject["data"] += '\n' + "Dynatrace SUPLAB Debug Request Headers:" + '\n';

                    foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> I in Request.Headers)
                    {
                        returnObject["data"] = returnObject["data"] + I.Key + " : " + I.Value + '\n';
                    }
                }
                else
                {
                    returnObject["data"] = "Debug : False";
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
