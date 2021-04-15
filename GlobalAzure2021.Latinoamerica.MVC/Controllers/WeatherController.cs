using GlobalAzure2021.Latinoamerica.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GlobalAzure2021.Latinoamerica.MVC.Controllers
{
    [Authorize]
    public class WeatherController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        public WeatherController(
            ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                await PrepareAuthenticatedClientAsync();

                var response = await _httpClient.GetAsync("weatherforecast");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<IEnumerable<WeatherForecastViewModel>>(content);

                    return View(values);
                }

                return View(Array.Empty<WeatherForecastViewModel>());
            }
            catch (Exception ex) when (ex.InnerException is MsalUiRequiredException)
            {
                var appBaseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                var azureLogInUrl = $"{appBaseUrl}/MicrosoftIdentity/Account/SignIn";
                await Request.HttpContext.SignOutAsync();
                return Redirect(azureLogInUrl);
            }
        }

        private async Task PrepareAuthenticatedClientAsync()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _configuration["Api:Scope"] });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.BaseAddress = new Uri(_configuration["Api:BaseUrl"]);
        }
    }
}
