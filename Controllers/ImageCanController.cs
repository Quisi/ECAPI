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
    public class ImageCanController : ControllerBase
    {
        private readonly AppDb Db;
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public ImageCanController(AppDb db)
        {
            Db = db;
        }

        /// <summary>
        /// Add a new ImageCan
        /// </summary>
        /// <param name="body">ImageCan object that needs to be added to the store</param>
        /// <response code="200">successful operation</response>
        /// <response code="500">Internal Error, could not create Db-Entry</response>
        /// <response code="405">Invalid input</response>
        [HttpPost]
        [Route("/imageCan")]
        [ValidateModelState]
        [SwaggerOperation("AddImageCan")]
        [Authorize(Policy = "create")]
        public virtual async Task<IActionResult> AddImageCan([FromBody] ImageCan body)
        {
            ImageCan duplicate = Db.ImageCans.Where(i => i.ImageId.Equals(body.ImageId) && i.CanId.Equals(body.CanId)).FirstOrDefault();
            if (duplicate == null)
            {
                body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
                body.CreationDate = DateTime.Now;
                body.ChangedLast = DateTime.Now;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                Db.ImageCans.Add(body);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(body);
                }
                return new BadRequestResult();
            }
            return new OkObjectResult(duplicate);
        }

        /// <summary>
        /// Update an existing ImageCan
        /// </summary>
        /// <remarks>Update ImageCan</remarks>
        /// <param name="imageCanId">ImageCan object that needs to be updated </param>
        /// <param name="body"></param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">ImageCan not found</response>
        /// <response code="405">Validation exception</response>
        [HttpPut]
        [Route("/imageCan/{imageCanId}")]
        [ValidateModelState]
        [SwaggerOperation("UpdateImageCan")]
        [Authorize(Policy = "update")]
        public virtual async Task<IActionResult> UpdateImageCan([FromRoute][Required] string imageCanId, [FromBody] ImageCan body)
        {
            ImageCan t = Db.ImageCans.Find(imageCanId);
            if (t != null)
            {
                t.CanId = body.CanId;
                t.ImageId = body.ImageId;
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
        /// Deletes a ImageCan
        /// </summary>
        /// <param name="ImageCanId">can id to delete</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">ImageCan not found</response>
        [HttpDelete]
        [Route("/imageCan/{imageCanId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteImageCan")]
        [Authorize(Policy = "delete")]
        public virtual async Task<IActionResult> DeleteImageCan([FromRoute][Required] string imageCanId)
        {
            ImageCan t = Db.ImageCans.Find(imageCanId);
            if (t != null)
            {
                Db.ImageCans.Remove(t);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find ImageCan by ID
        /// </summary>
        /// <remarks>Returns a single tagcan</remarks>
        /// <param name="ImageCanId">ID of ImageCan to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">ImageCan not found</response>
        [HttpGet]
        [Route("/imageCan/{imageCanId}")]
        [ValidateModelState]
        [SwaggerOperation("GetImageCanById")]
        [SwaggerResponse(statusCode: 200, type: typeof(ImageCan), description: "successful operation")]
        public IActionResult GetImageCanById([FromRoute][Required] string imageCanId)
        {
            ImageCan t = Db.ImageCans.Find(imageCanId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds ImageCans by CanID
        /// </summary>
        /// <remarks>ImageCanId by CanID</remarks>
        /// <param name="canId">Can Id</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid canId value</response>
        [HttpGet]
        [Route("/imageCan/findByCanId/{canId}")]
        [ValidateModelState]
        [SwaggerOperation("FindImageCansByCanId")]
        [SwaggerResponse(statusCode: 200, type: typeof(ImageCan), description: "successful operation")]
        public IActionResult FindImageCansByCanId([FromRoute][Required] string canId)
        {
            ImageCan t = Db.ImageCans.Where(i => i.CanId.Equals(canId)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }


        /// <summary>
        /// Finds ImageCans by ImageID
        /// </summary>
        /// <remarks>ImageCanId by ImageID</remarks>
        /// <param name="imageId">Image Id</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid canId value</response>
        [HttpGet]
        [Route("/imageCan/findByImageId/{imageId}")]
        [ValidateModelState]
        [SwaggerOperation("FindImageCansByImageId")]
        [SwaggerResponse(statusCode: 200, type: typeof(ImageCan), description: "successful operation")]
        public IActionResult FindImageCansByImageId([FromRoute][Required] string imageId)
        {
            ImageCan t = Db.ImageCans.Where(i => i.ImageId.Equals(imageId)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// List imageCans
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/imageCan/")]
        [ValidateModelState]
        [SwaggerOperation("GetimageCanList")]
        [SwaggerResponse(statusCode: 200, type: typeof(ImageCan), description: "successful operation")]
        public IActionResult GetimageCanList()
        {
            List<ImageCan> t = Db.ImageCans.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }
    }
}
