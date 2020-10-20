using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Onbox.GridGenerator.Web.Models.Adsk;
using Onbox.GridGenerator.Web.Services;
using System.Threading.Tasks;

namespace Onbox.GridGenerator.Web.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class DesignAutoController : ControllerBase
    {
        private readonly DesignAutoService designAutoService;
        private readonly RevitFileService revitFileService;
        private readonly IConfiguration config;

        public DesignAutoController(
            DesignAutoService designAutoService,
            RevitFileService revitFileService,
            IConfiguration config)
        {
            this.designAutoService = designAutoService;
            this.revitFileService = revitFileService;
            this.config = config;
        }

        [HttpGet]
        public IActionResult ForgeInformation()
        {
            var forgeClientId = config["Forge:ClientId"];
            var forgeBaseUrl = config["Forge:BaseUrl"];
            var forgeEnvironment = config["Forge:Environment"];

            var result = new
            {
                forgeClientId,
                forgeBaseUrl,
                forgeEnvironment
            };

            return Ok(result);
        }

        [HttpGet("bundles")]
        public async Task<IActionResult> GetAppBundles()
        {
            var bundles = await this.designAutoService.GetAllBundlesAsync();
            return Ok(bundles);
        }

        [HttpPost("bundles")]
        public async Task<IActionResult> PostAppBundle(DesignAutoBundleInput input)
        {
            if (string.IsNullOrWhiteSpace(input.FileName))
            {
                input.FileName = this.config["Forge:DA:App"];
            }

            // Default Engine
            if (string.IsNullOrWhiteSpace(input.Engine))
            {
                input.Engine = this.config["Forge:DA:Engine"];
            }

            var bundle = await this.designAutoService.CreateAppBundleAsync(input.FileName, input.Engine);
            return Ok(bundle);
        }

        [HttpGet("activities")]
        public async Task<IActionResult> GetActivities()
        {
            var activities = await this.designAutoService.GetActivitiesAsync();

            return Ok(activities);
        }

        [HttpPost("activities")]
        public async Task<IActionResult> PostActivity(DesignAutoActivityInput input)
        {
            if (string.IsNullOrWhiteSpace(input.ActivityName))
            {
                input.ActivityName = this.config["Forge:DA:App"];
            }

            // Default Engine
            if (string.IsNullOrWhiteSpace(input.Engine))
            {
                input.Engine = this.config["Forge:DA:Engine"];
            }

            var activitiy = await this.designAutoService.CreateActivityAsync(input.ActivityName, input.Engine);

            return Ok(activitiy);
        }

        [HttpPost("workitems")]
        public async Task<IActionResult> PostWorkItem(DesignAutoWorkItemInput input)
        {
            if (string.IsNullOrWhiteSpace(input.ActivityName))
            {
                input.ActivityName = this.config["Forge:DA:App"];
            }

            var callback = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/designauto/results";

            var status = await this.designAutoService.CreateWorkItemAsync(input.ActivityName, input.GridCollection, callback);
            return Ok(status);
        }

        [HttpGet("workitems")]
        public async Task<IActionResult> GetWorkItem(string id)
        {
            var status = await this.designAutoService.CheckWorkItemAsync(id);
            return Ok(status);
        }

        [HttpDelete("accounts")]
        public async Task<IActionResult> ClearAccount()
        {
            await this.designAutoService.ClearAccount();

            return Ok();
        }

        [HttpPut("results")]
        public async Task<IActionResult> ResultsCallback()
        {
            await this.revitFileService.SaveLocal(this.Request.Body);
            return Ok();
        }

    }
}
