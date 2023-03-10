using EnergyScanApi.Attributes;
using EnergyScanApi.Models;
using EnergyScanApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnergyScanApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class BarcodeCanController : ControllerBase
    {
        private readonly AppDb Db;
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public BarcodeCanController(AppDb db)
        {
            Db = db;
        }

        /// <summary>
        /// Add a new BarcodeCan
        /// </summary>
        /// <param name="body">BarcodeCan object that needs to be added to the store</param>
        /// <response code="200">successful operation</response>
        /// <response code="500">Internal Error, could not create Db-Entry</response>
        /// <response code="405">Invalid input</response>
        [HttpPost]
        [Route("/barcodeCan")]
        [ValidateModelState]
        [SwaggerOperation("AddBarcodeCan")]
        [Authorize(Policy = "create")]
        public virtual async Task<IActionResult> AddBarcodeCan([FromBody] BarcodeCan body)
        {
            BarcodeCan duplicate = Db.BarcodeCans.Where(i => i.BarcodeId.Equals(body.BarcodeId) && i.CanId.Equals(body.CanId)).FirstOrDefault();
            if (duplicate == null)
            {
                body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
                body.CreationDate = DateTime.Now;
                body.ChangedLast = DateTime.Now;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                Db.BarcodeCans.Add(body);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(body);
                }
                return new BadRequestResult();
            }
            return new OkObjectResult(duplicate);
        }

        /// <summary>
        /// Update an existing BarcodeCan
        /// </summary>
        /// <remarks>Update BarcodeCan</remarks>
        /// <param name="barcodeCanId">BarcodeCan object that needs to be updated </param>
        /// <param name="body"></param>
        /// <response code="200">successful operation</response>
        /// <response code="404">BarcodeCan not found</response>
        [HttpPut]
        [Route("/barcodeCan/{barcodeCanId}")]
        [ValidateModelState]
        [SwaggerOperation("UpdateBarcodeCan")]
        [Authorize(Policy = "update")]
        public virtual async Task<IActionResult> UpdateBarcodeCan([FromRoute][Required] string barcodeCanId, [FromBody] BarcodeCan body)
        {
            BarcodeCan t = Db.BarcodeCans.Find(barcodeCanId);
            if (t != null)
            {
                t.BarcodeId = body.BarcodeId;
                t.CanId = body.CanId;
                t.ChangedLast = DateTime.Now;
                t.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;

                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(t);
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }
        /// <summary>
        /// Deletes a BarcodeCan
        /// </summary>
        /// <param name="barcodeCanId">can id to delete</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">BarcodeCan not found</response>
        [HttpDelete]
        [Route("/barcodeCan/{barcodeCanId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteBarcodeCan")]
        [Authorize(Policy = "delete")]
        public virtual async Task<IActionResult> DeleteBarcodeCan([FromRoute][Required] string barcodeCanId)
        {
            BarcodeCan t = Db.BarcodeCans.Find(barcodeCanId);
            if (t != null)
            {
                Db.BarcodeCans.Remove(t);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find BarcodeCan by ID
        /// </summary>
        /// <remarks>Returns a single barcodecan</remarks>
        /// <param name="barcodeCanId">ID of BarcodeCan to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">BarcodeCan not found</response>
        [HttpGet]
        [Route("/barcodeCan/{barcodeCanId}")]
        [ValidateModelState]
        [SwaggerOperation("GetBarcodeCanById")]
        [SwaggerResponse(statusCode: 200, type: typeof(BarcodeCan), description: "successful operation")]
        public IActionResult GetBarcodeCanById([FromRoute][Required] string barcodeCanId)
        {
            BarcodeCan t = Db.BarcodeCans.Find(barcodeCanId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds BarcodeCans by CanID
        /// </summary>
        /// <remarks>BarcodeCanId by CanId</remarks>
        /// <param name="canId">Can Id</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">CanId not found</response>
        [HttpGet]
        [Route("/barcodeCan/findByCanId/{canId}")]
        [ValidateModelState]
        [SwaggerOperation("FindBarcodeCansByCanId")]
        [SwaggerResponse(statusCode: 200, type: typeof(BarcodeCan), description: "successful operation")]
        public IActionResult FindBarcodeCansByCanId([FromRoute][Required] string canId)
        {
            BarcodeCan t = Db.BarcodeCans.Where(i => i.CanId.Equals(canId)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds BarcodeCans by barcodeID
        /// </summary>
        /// <remarks>BarcodeCanId by barcodeID</remarks>
        /// <param name="barcodeId">barcode Id</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid canId value</response>
        [HttpGet]
        [Route("/barcodeCan/findByBarcodeId/{barcodeId}")]
        [ValidateModelState]
        [SwaggerOperation("FindBarcodeCansByBarcodeId")]
        [SwaggerResponse(statusCode: 200, type: typeof(BarcodeCan), description: "successful operation")]
        public IActionResult FindBarcodeCansByBarcodeId([FromRoute][Required] string barcodeId)
        {
            BarcodeCan t = Db.BarcodeCans.Where(i => i.BarcodeId.Equals(barcodeId)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// List BarcodeCans
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/barcodeCan/")]
        [ValidateModelState]
        [SwaggerOperation("GetbarcodeCanList")]
        [SwaggerResponse(statusCode: 200, type: typeof(BarcodeCan), description: "successful operation")]
        public IActionResult GetbarcodeCanList()
        {
            List<BarcodeCan> t = Db.BarcodeCans.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }
    }
}
