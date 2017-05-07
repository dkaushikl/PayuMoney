using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;


namespace MVCIntegrationKit.Controllers
{
    public class DemoController : Controller
    {

        public ActionResult Demo()
        {
            return View();
        }

        [HttpPost]
        public void Demo(FormCollection form)
        {
            var firstName = form["txtfirstname"];
            var amount = form["txtamount"];
            var productInfo = form["txtprodinfo"];
            var email = form["txtemail"];
            var phone = form["txtphone"];
            var key = ConfigurationManager.AppSettings["MERCHANT_KEY"];
            var salt = ConfigurationManager.AppSettings["SALT"];

            var myremotepost = new RemotePost { Url = ConfigurationManager.AppSettings["PAYU_BASE_URL"] + "/_payment" };
            myremotepost.Add("key", key);
            myremotepost.Add("txnid", Generatetxnid());
            myremotepost.Add("amount", amount);
            myremotepost.Add("productinfo", productInfo);
            myremotepost.Add("firstname", firstName);
            myremotepost.Add("phone", phone);
            myremotepost.Add("email", email);
            myremotepost.Add("surl", "http://localhost:3271/Return/Return");
            myremotepost.Add("furl", "http://localhost:3271/Return/Return");
            myremotepost.Add("service_provider", "");

            string hashString = key + "|" + Generatetxnid() + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|||||||||||" + salt;
            //string hashString = "3Q5c3q|2590640|3053.00|OnlineBooking|vimallad|ladvimal@gmail.com|||||||||||mE2RxRwx";
            string hash = Generatehash512(hashString);

            //var hashString = key + "|" + Generatetxnid() + "|" + amount + "|" + productInfo + "|" + firstName + "|" + email + "|||||||||||" + salt;
            //var hash = Generatehash512(hashString);
            myremotepost.Add("hash", hash);
            myremotepost.Post();

        }

        public class RemotePost
        {
            public readonly System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();

            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            public void Post()
            {
                System.Web.HttpContext.Current.Response.Clear();

                System.Web.HttpContext.Current.Response.Write("<html><head>");

                System.Web.HttpContext.Current.Response.Write($"</head><body onload=\"document.{FormName}.submit()\">");
                System.Web.HttpContext.Current.Response.Write($"<form name=\"{FormName}\" method=\"{Method}\" action=\"{Url}\" >");
                for (var i = 0; i < Inputs.Keys.Count; i++)
                {
                    System.Web.HttpContext.Current.Response.Write($"<input name=\"{Inputs.Keys[i]}\" type=\"hidden\" value=\"{Inputs[Inputs.Keys[i]]}\">");
                }
                System.Web.HttpContext.Current.Response.Write("</form>");
                System.Web.HttpContext.Current.Response.Write("</body></html>");

                System.Web.HttpContext.Current.Response.End();
            }
        }


        public string Generatehash512(string text)
        {
            var message = Encoding.UTF8.GetBytes(text);
            var hashString = new SHA512Managed();
            var hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
        }


        public string Generatetxnid()
        {
            var rnd = new Random();
            var strHash = Generatehash512(rnd.ToString() + DateTime.Now);
            var txnid1 = strHash.Substring(0, 20);
            return txnid1;
        }


    }
}
