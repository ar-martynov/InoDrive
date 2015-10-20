using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;



namespace WebUI.Infrastructure
{
    public class ApplicationAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            string responseMsg = new JavaScriptSerializer().Serialize(new { message = Resources.Language.NotAuthenticated});
            actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            actionContext.Response.Content = new StringContent(responseMsg, Encoding.UTF8, "application/json");
        }
    }
}
