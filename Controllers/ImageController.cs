using EnergyScanApi.Attributes;
using EnergyScanApi.Models;
using EnergyScanApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnergyScanApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly AppDb Db;
        ILogger<ImageController> Log;
        const String folderName = "uploads";
        readonly String folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public ImageController(AppDb db, ILogger<ImageController> logger)
        {
            Db = db;
            Log = logger;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// Add a new image
        /// </summary>
        /// <param name="body">image object that needs to be added to the store</param>
        /// <response code="405">Invalid input</response>
        /*
        [HttpPost]
        [Route("/image")]
        [ValidateModelState]
        [SwaggerOperation("Addimage")]
        public virtual async Task<IActionResult> Addimage([FromBody] Image body)
        {
            Image duplicate = Db.Images.Where(i => i.Name.Equals(body.Name) && i.Url.Equals(body.Url)).FirstOrDefault();
            if (duplicate == null)
            {
                body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
                body.CreationDate = DateTime.Now;
                body.ChangedLast = DateTime.Now;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                Db.Images.Add(body);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(body);
                }
                return new BadRequestResult();
            }
            return new OkObjectResult(duplicate);
        }
        */

        /// <summary>
        /// Update an existing image
        /// </summary>
        /// <remarks>Update image</remarks>
        /// <param name="imageId">image object that needs to be updated </param>
        /// <param name="body"></param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">image not found</response>
        /// <response code="405">Validation exception</response>
        [HttpPut]
        [Route("/image/{imageId}")]
        [ValidateModelState]
        [SwaggerOperation("Updateimage")]
        [Authorize(Policy = "update")]
        public virtual async Task<IActionResult> Updateimage([FromRoute][Required] string imageId, [FromBody] Image body)
        {
            Image t = Db.Images.Find(imageId);
            if (t != null)
            {
                t.Name = body.Name;
                t.Url = body.Url;
                t.Description = body.Description;
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
        /// Deletes a image
        /// </summary>
        /// <param name="imageId">image id to delete</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">image not found</response>
        [HttpDelete]
        [Route("/image/{imageId}")]
        [ValidateModelState]
        [SwaggerOperation("Deleteimage")]
        [Authorize(Policy = "delete")]
        public virtual async Task<IActionResult> Deleteimage([FromRoute][Required] string imageId)
        {
            Image t = Db.Images.Find(imageId);
            string fullfilename = t.Url.Split("/").Last();
            if (t != null)
            {
                Db.Images.Remove(t);
                IOrderedQueryable<ImageCan> ics = Db.ImageCans.Where(i => i.ImageId.Equals(t.Id)).OrderBy(i => i.Id);
                Db.ImageCans.RemoveRange(ics);
                if (0 < await Db.SaveChangesAsync())
                {
                    try
                    {
                        System.IO.File.Delete(Path.Combine(folderPath, fullfilename));
                    } catch (Exception e)
                    {
                        Log.LogWarning(e.Message);
                    }
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find image by ID
        /// </summary>
        /// <remarks>Returns a single image</remarks>
        /// <param name="imageId">ID of image to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Image not found</response>
        [HttpGet]
        [Route("/image/{imageId}")]
        [ValidateModelState]
        [SwaggerOperation("GetimageById")]
        [SwaggerResponse(statusCode: 200, type: typeof(Image), description: "successful operation")]
        public IActionResult GetimageById([FromRoute][Required] string imageId)
        {
            Image t = Db.Images.Find(imageId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find image by Name
        /// </summary>
        /// <remarks>Returns a single image</remarks>
        /// <param name="imagename">Name of image to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Image not found</response>
        [HttpGet]
        [Route("/image/byName/{imagename}")]
        [ValidateModelState]
        [SwaggerOperation("GetimageByName")]
        [SwaggerResponse(statusCode: 200, type: typeof(Image), description: "successful operation")]
        public IActionResult GetimageByName([FromRoute][Required] string imagename)
        {
            Image t = Db.Images.Where(i => i.Name.Equals(imagename)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find image by Url
        /// </summary>
        /// <remarks>Returns a single image</remarks>
        /// <param name="url">ID of image to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Image not found</response>
        [HttpGet]
        [Route("/image/byUrl/{url}")]
        [ValidateModelState]
        [SwaggerOperation("GetImageByName")]
        [SwaggerResponse(statusCode: 200, type: typeof(Image), description: "successful operation")]
        public IActionResult GetImageByUrl([FromRoute][Required] string url)
        {
            Image t = Db.Images.Where(i => i.Url.Equals(url)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// List images
        /// </summary>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Images not found</response>
        [HttpGet]
        [Route("/image/")]
        [ValidateModelState]
        [SwaggerOperation("GetimageList")]
        [SwaggerResponse(statusCode: 200, type: typeof(Image), description: "successful operation")]
        public IActionResult GetimageList()
        {
            List<Image> t = Db.Images.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        [HttpPost]
        [Route("/image/file/")]
        [Authorize(Policy = "create")]
        public async Task<ActionResult> UploadDocument([FromHeader] string shortname, [FromForm(Name = "upfile")] IFormFile upfile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(shortname))
                {
                    shortname = "somename";
                }
                string Id = Guid.NewGuid().ToString();
                string filetype = upfile.FileName.Split(".").Last();
                string fullfilename = Id + "." + filetype;

                using (MemoryStream fileContentStream = new MemoryStream())
                {
                    await upfile.CopyToAsync(fileContentStream);
                    await System.IO.File.WriteAllBytesAsync(Path.Combine(folderPath, fullfilename), fileContentStream.ToArray());
                }
                Image im = new Image()
                {
                    Id = Id,
                    Name = shortname,
                    CreationDate = DateTime.Now,
                    ChangedLast = DateTime.Now,
                    ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                    Url = "image/file/" + fullfilename
                };
                Db.Images.Add(im);
                await Db.SaveChangesAsync();
                return new OkObjectResult(im);
            } catch (Exception e)
            {
                Log.LogError(e.Message);
                Log.LogDebug(e.Message + "\n" + e.StackTrace);
                return new BadRequestObjectResult(e.Message);
            }


        }

        [HttpGet]
        [Route("/image/file/{filename}")]
        public async Task<IActionResult> GetImageFile([FromRoute] String filename)
        {

            string filePath = Path.Combine(folderPath, filename);
            if (System.IO.File.Exists(filePath))
            {
                return File(await System.IO.File.ReadAllBytesAsync(filePath), "application/octet-stream", filename);
            }
            return NotFound();
        }
    }
}
