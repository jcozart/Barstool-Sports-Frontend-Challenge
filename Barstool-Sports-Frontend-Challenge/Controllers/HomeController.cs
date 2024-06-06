using Barstool_Sports_Frontend_Challenge.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
namespace Barstool_Sports_Frontend_Challenge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        string Baseurl = "https://union-staging.barstoolsports.com/v2/stories/latest123";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var stories = await GetStoriesAsync();
                var firstFiveStories = stories.Take(5).ToList();
                var model = new StoriesViewModel
                {
                    Stories = firstFiveStories,
                    IsEnd = false,
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading stories.");
                TempData["ErrorMessage"] = "An error occurred while loading more stories. Please try again later.";
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> LoadMoreStories(int storiesLoaded)
        {
            try
            {
                var stories = await GetStoriesAsync();
                var moreStories = stories.Skip(storiesLoaded).Take(5).ToList();
                var model = new StoriesViewModel
                {
                    IsEnd = false,
                };
                if (storiesLoaded + moreStories.Count >= stories.Count)
                {
                    model.IsEnd = true;
                }
                var currentStories = stories.Take(storiesLoaded + moreStories.Count).ToList();
                model.Stories = currentStories;
                return View("Index", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading more stories.");
                TempData["ErrorMessage"] = "An error occurred while loading more stories. Please try again later.";
                return View("Index");
            }
        }

        public async Task<List<Story>> GetStoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(Baseurl);
                var stories = JsonConvert.DeserializeObject<List<Story>>(response);
                return stories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching stories from the API.");
                TempData["ErrorMessage"] = "An error occurred while fetching the stories from the API. Please try again later.";
                return new List<Story>(); // Return an empty list in case of an error
            }
        }
    }
}
