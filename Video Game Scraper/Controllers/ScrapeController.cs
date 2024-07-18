using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Video_Game_ScraperMVC.Models;

namespace VideoGameScraperMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScraperController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetGames()
        {
            string url = "https://screenscraper.fr"; // Replace with the actual URL
            var games = await ScrapeGamesAsync(url);
            return Ok(games);
        }

        private async Task<List<Game>> ScrapeGamesAsync(string url)
        {
            var games = new List<Game>();

            try
            {
                var htmlContent = await GetHtmlAsync(url);
                var document = new HtmlDocument();
                document.LoadHtml(htmlContent);

                var gameItems = document.DocumentNode.SelectNodes("//div[@class='game-item']");

                if (gameItems != null)
                {
                    foreach (var item in gameItems)
                    {
                        var title = item.SelectSingleNode(".//h2[@class='game-title']")?.InnerText.Trim();
                        var releaseDate = item.SelectSingleNode(".//span[@class='release-date']")?.InnerText.Trim();
                        var description = item.SelectSingleNode(".//p[@class='description']")?.InnerText.Trim();

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(releaseDate) && !string.IsNullOrEmpty(description))
                        {
                            games.Add(new Game
                            {
                                Title = title,
                                ReleaseDate = releaseDate,
                                Description = description
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions (logging, etc.)
            }

            return games;
        }

        private async Task<string> GetHtmlAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}