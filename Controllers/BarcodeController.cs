using EnergyScanApi.Attributes;
using EnergyScanApi.Models;
using EnergyScanApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace EnergyScanApi.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class BarcodeController : ControllerBase
    {

        private readonly ILogger<BarcodeController> _logger;
        private readonly AppDb Db;
        public BarcodeController(ILogger<BarcodeController> logger, AppDb dbContext)
        {
            _logger = logger;
            Db = dbContext;
            _logger.LogDebug(1, "NLog injected into HomeController");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postbarcode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/barcode")]
        [ValidateModelState]
        [SwaggerOperation("AddBarcode")]
        [Authorize(Policy = "create")]
        public async Task<IActionResult> PostBarcode([FromBody] Barcode postbarcode)
        {
            postbarcode.Id = Guid.NewGuid().ToString();
            postbarcode.CreationDate = DateTime.Now;
            postbarcode.ChangedLast = DateTime.Now;
            postbarcode.ChangedById = new Utilities(Db).GetCurrentUserId(User);
            Db.Barcodes.Add(postbarcode);
            int entrieswrittentodb = await Db.SaveChangesAsync();
            if (entrieswrittentodb > 0)
            {
                return new OkObjectResult(postbarcode);
            } else
            {
                return new BadRequestResult();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="postbarcode"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("/barcode/{id}")]
        [Authorize(Policy = "update")]
        public async Task<IActionResult> PutBarcode([FromRoute] string id, [FromBody] Barcode postbarcode)
        {
            Barcode barcode = Db.Barcodes.Find(id);
            if (barcode != null)
            {
                if (postbarcode.Id.Equals(barcode.Id))
                {
                    barcode.Name = postbarcode.Name;
                    barcode.ChangedLast = DateTime.Now;
                    barcode.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;

                    if (0 < await Db.SaveChangesAsync())
                    {
                        return new OkObjectResult(barcode);
                    }
                }
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="did"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("/barcode/{id}")]
        [Authorize(Policy = "delete")]
        public async Task<IActionResult> DeleteBarcode([FromRoute] string id)
        {
            Barcode barcode = Db.Barcodes.Find(id);
            Db.Barcodes.Remove(barcode);
            IQueryable<BarcodeCan> bcs = Db.BarcodeCans.Where(i => i.BarcodeId.Equals(barcode.Id));
            Db.BarcodeCans.RemoveRange(bcs);
            if (0 < await Db.SaveChangesAsync())
            {
                return new OkResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/barcode")]
        [SwaggerOperation("GetBarcodes")]
        public IActionResult GetBarcodes()
        {
            System.Collections.Generic.List<Barcode> result = Db.Barcodes.ToList();
            if (result.Count > 0)
            {
                return new OkObjectResult(result);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/barcode/{id}")]
        [SwaggerOperation("GetBarcodeById")]
        public IActionResult GetBarcodeById([FromRoute] string id)
        {
            Barcode result = Db.Barcodes.Where(i => i.Id.Equals(id)).FirstOrDefault();
            if (result != null)
            {
                return new OkObjectResult(result);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/barcode/byCode/{code}")]
        [SwaggerOperation("GetBarcodeByCode")]
        public IActionResult GetBarcodeByCode([FromRoute] string code)
        {
            Barcode result = Db.Barcodes.Where(i => i.Name.Equals(code)).FirstOrDefault();
            if (result != null)
            {
                return new OkObjectResult(result);
            }
            return new NotFoundResult();
        }
    }
}
