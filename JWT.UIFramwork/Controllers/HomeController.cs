using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JWT.UIFramwork.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HttpClient client = new HttpClient();
            var token = "";
            HttpCookie cookie = Request.Cookies["webToken"];
            if(cookie != null)
            {
                token = cookie.Value;
                JwtSecurityToken cookieToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var validToken = cookieToken.ValidTo > DateTime.UtcNow;
                if (!validToken)
                {
                    var byteArray = Encoding.ASCII.GetBytes("Admin:pass");
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    var responseMessage = client.PostAsync("http://localhost:11790/api/auth/token", new StringContent(""));


                    token = responseMessage.Result.Content.ReadAsStringAsync().Result;
                    JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                    cookie.Expires = jwtToken.ValidTo;
                    cookie.Name = "webToken";
                    cookie.Value = token;
                    Response.Cookies.Add(cookie);

                }
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var result = client.GetAsync("http://localhost:10493/api/values");
                var stringResult = result.Result.Content.ReadAsStringAsync().Result;
            }
            else
            {
                var byteArray = Encoding.ASCII.GetBytes("Admin:pass");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var responseMessage = client.PostAsync("http://localhost:11790/api/auth/token", new StringContent(""));


                token = responseMessage.Result.Content.ReadAsStringAsync().Result;
                JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                cookie = new HttpCookie("webToken");
                cookie.Expires = jwtToken.ValidTo;
                cookie.Value = token;
                Response.Cookies.Add(cookie);

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var result = client.GetAsync("http://localhost:10493/api/values");
                var stringResult = result.Result.Content.ReadAsStringAsync().Result;
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}