using System;
using System.Web.Mvc;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MVCIntegrationKit.Controllers
{
    public class ReturnController : Controller
    {
      
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void Return(FormCollection form)
        {
            try
            {
                const string hashSeq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";

                if (form["status"] == "success")
                {

                    var mercHashVarsSeq = hashSeq.Split('|');
                    Array.Reverse(mercHashVarsSeq);
                    var mercHashString = ConfigurationManager.AppSettings["SALT"] + "|" + form["status"];


                    foreach (var mercHashVar in mercHashVarsSeq)
                    {
                        mercHashString += "|";
                        mercHashString = mercHashString + (form[mercHashVar] != null ? form[mercHashVar] : "");

                    }
                    Response.Write(mercHashString);
                    var mercHash = Generatehash512(mercHashString).ToLower();

                    if (mercHash != form["hash"])
                    {
                        Response.Write("Hash value did not matched");

                    }
                    else
                    {
                        ViewData["Message"] = "Status is successful. Hash value is matched";
                        Response.Write("<br/>Hash value matched");
                    }
                }
                else
                {
                    Response.Write("Hash value did not matched");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
        }

        public string Generatehash512(string text)
        {
            var message = Encoding.UTF8.GetBytes(text);
            var hashString = new SHA512Managed();
            var hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
        }
    }
}
