using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace ExposureToAPI.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class SpriteController : ControllerBase {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<SpriteController> _logger;

        public SpriteController(ILogger<SpriteController> logger) {
            _logger = logger;
        }

        [HttpPost(Name = "GetSpriteCoordinates")]
        public async Task<ActionResult<SpriteInfo>> Get(IFormFile image) {

            var stream = new MemoryStream();
            image.CopyTo(stream);
            var bmp = new Bitmap(stream);

            var imageUnpacker = new ASU.BO.ImageUnpacker(bmp, image.FileName, false);
            List<SpriteInfo> sprites = new List<SpriteInfo>();

            imageUnpacker.StartUnpacking();
            while (imageUnpacker.IsUnpacked() == false) {
                await Task.Delay(100);
            }

            imageUnpacker.GetBoxes().ForEach((box) => {
                Console.WriteLine("Box being processed");
                sprites.Add(new SpriteInfo(box.X, box.Y, box.Width, box.Height));
            });

            return Ok(sprites);
            
        }
    }
}