using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace CallApiTest.Controllers
{   
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        
        private readonly HttpClient _httpClient;
        
        public TestController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        
        // GET api/proxy
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Get()
        {
            // Replace 'external-api-url' with the actual URL of the web API you want to call
            string externalApiUrl = "https://jsonplaceholder.typicode.com/todos";
            
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(externalApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Ok(content); // Returns the content as a JSON object
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                }
            }
            catch(HttpRequestException e)
            {
                // Handle the exception
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(string data)
        {


            return StatusCode(200, new { name = data });

        }
        
    }
}
