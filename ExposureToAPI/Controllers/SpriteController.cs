using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace ExposureToAPI.Controllers {
    [ApiController]
    [Route("sprites")]
    public class SpriteController : ControllerBase {

        private readonly ILogger<SpriteController> _logger;

        public SpriteController(ILogger<SpriteController> logger) {
            _logger = logger;
        }

        [HttpPost("single")]
        public async Task<ActionResult<SpriteInfo>> PostSingle(IFormFile image) {

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

        [HttpPost("multiple")]
        public async Task<ActionResult<SpriteInfo>> PostMultiple(IFormFile[] images) {

            MemoryStream stream;
            Bitmap bmp;
            List<ASU.BO.ImageUnpacker> unpackers = new List<ASU.BO.ImageUnpacker>();

            foreach (var image in images) {
                stream = new MemoryStream();
                image.CopyTo(stream);
                bmp = new Bitmap(stream);
                unpackers.Add(new ASU.BO.ImageUnpacker(bmp, image.FileName, false));
            }


            foreach (var unpacker in unpackers) {
                unpacker.StartUnpacking();
            }

            while (unpackers.Exists(unpacker => !unpacker.IsUnpacked())) {
                await Task.Delay(100);
            }

            List<List<SpriteInfo>> sprites = new List<List<SpriteInfo>>();
            List<SpriteInfo> eachImageSprites;

            foreach (var unpacker in unpackers) {

                eachImageSprites = new List<SpriteInfo>();

                unpacker.GetBoxes().ForEach((box) => {
                    Console.WriteLine("Box being processed");
                    eachImageSprites.Add(new SpriteInfo(box.X, box.Y, box.Width, box.Height));
                });

                sprites.Add(eachImageSprites);

            }

            return Ok(sprites);

        }
    }
}