using GlobalAzure2021.Latinoamerica.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GlobalAzure2021.Latinoamerica.MVC.Controllers
{
    [Authorize]
    public class WeatherController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherController(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_configuration["Api:BaseUrl"]}weatherforecast");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<IEnumerable<WeatherForecastViewModel>>(content);

                return View(values);
            }

            return View(Array.Empty<WeatherForecastViewModel>());
        }
    }
}
