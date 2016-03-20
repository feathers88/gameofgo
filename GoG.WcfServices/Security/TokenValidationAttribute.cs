using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GoG.Services.Security
{
    //public class TokenValidationAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(HttpActionContext actionContext)
    //    {
    //        string token;

    //        try
    //        {
    //            token = actionContext.Request.Headers.GetValues("Authorization-Token").First();
    //        }
    //        catch (Exception)
    //        {
    //            actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
    //            {
    //                Content = new StringContent("Missing Authorization-Token")
    //            };
    //            return;
    //        }

    //        try
    //        {
    //            AuthorizedUserRepository.GetUsers().First(x => x.Name == RSAClass.Decrypt(token));
    //            base.OnActionExecuting(actionContext);
    //        }
    //        catch (Exception)
    //        {
    //            actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
    //            {
    //                Content = new StringContent("Unauthorized User")
    //            };
    //            return;
    //        }
    //    }
    //}
}