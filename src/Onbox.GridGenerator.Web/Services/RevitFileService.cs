using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace Onbox.GridGenerator.Web.Services
{
    public class RevitFileService
    {
        private readonly IWebHostEnvironment env;

        public RevitFileService(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async Task SaveLocal(Stream requestStream)
        {
            var stream = new MemoryStream();
            await requestStream.CopyToAsync(stream);
            stream.Position = 0;

            var rootFolder = Path.Combine(env.WebRootPath, "DesignAuto");
            var resultFolder = Path.Combine(rootFolder, "Results");
            if (!Directory.Exists(resultFolder))
            {
                Directory.CreateDirectory(resultFolder);
            }

            var resultsFile = Path.Combine(resultFolder, "Results.rvt");
            using (var fileStream = new FileStream(resultsFile, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

    }
}
