using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Threading.Tasks;

namespace GlobalAzure2021.Latinoamerica.MVC.Controllers
{
    public class ErrorController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (context.Error?.InnerException != null && context.Error.InnerException is MsalUiRequiredException)
            {
                var appBaseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                var azureLogInUrl = $"{appBaseUrl}/MicrosoftIdentity/Account/SignIn";
                await Request.HttpContext.SignOutAsync();
                return Redirect(azureLogInUrl);
            }

            return Problem();
        }
    }
}
