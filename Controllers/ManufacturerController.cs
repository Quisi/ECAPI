using EnergyScanApi.Attributes;
using EnergyScanApi.Models;
using EnergyScanApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnergyScanApi.Controllers
{
    /// <summary>
    /// API-Controller: Manufacturer
    /// </summary>
    [ApiController]
    public class ManufacturerController : ControllerBase
    {
        private readonly AppDb Db;
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public ManufacturerController(AppDb db)
        {
            Db = db;
        }

        /// <summary>
        /// Add a new manufacturer
        /// </summary>
        /// <param name="body">manufacturer object that needs to be added to the store</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">can not found</response>
        /// <response code="405">Invalid input</response>
        [HttpPost]
        [Route("/manufacturer")]
        [ValidateModelState]
        [SwaggerOperation("Addmanufacturer")]
        [Authorize(Policy = "create")]
        public virtual async Task<IActionResult> Addmanufacturer([FromBody] Manufacturer body)
        {
            Manufacturer duplicate = Db.Manufacturers.Where(i => i.Name.Equals(body.Name)).FirstOrDefault();
            if (duplicate == null)
            {
                body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
                body.CreationDate = DateTime.Now;
                body.ChangedLast = DateTime.Now;
                System.Security.Claims.ClaimsPrincipal c = User;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                Db.Manufacturers.Add(body);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(body);
                }
                return new BadRequestResult();
            }
            return new OkObjectResult(duplicate);
        }

        /// <summary>
        /// Update an existing manufacturer
        /// </summary>
        /// <remarks>Update manufacturer</remarks>
        /// <param name="manufacturerId">manufacturer object that needs to be updated </param>
        /// <param name="body"></param>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">manufacturer not found</response>
        /// <response code="405">Validation exception</response>
        [HttpPut]
        [Route("/manufacturer/{manufacturerId}")]
        [ValidateModelState]
        [SwaggerOperation("Updatemanufacturer")]
        [Authorize(Policy = "update")]
        public virtual async Task<IActionResult> Updatemanufacturer([FromRoute][Required] string manufacturerId, [FromBody] Manufacturer body)
        {
            Manufacturer t = Db.Manufacturers.Find(manufacturerId);
            if (t != null)
            {
                t.Name = body.Name;
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
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturerId">manufacturer id to delete</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">manufacturer not found</response>
        [HttpDelete]
        [Route("/manufacturer/{manufacturerId}")]
        [ValidateModelState]
        [SwaggerOperation("Deletemanufacturer")]
        [Authorize(Policy = "delete")]
        public virtual async Task<IActionResult> Deletemanufacturer([FromRoute][Required] string manufacturerId)
        {
            Manufacturer t = Db.Manufacturers.Find(manufacturerId);
            if (t != null)
            {
                Db.Manufacturers.Remove(t);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find Manufacturer by ID
        /// </summary>
        /// <remarks>Returns a single Manufacturer</remarks>
        /// <param name="manufacturerId">ID of manufacturer to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">manufacturer not found</response>
        [HttpGet]
        [Route("/manufacturer/{manufacturerId}")]
        [ValidateModelState]
        [SwaggerOperation("GetmanufacturerById")]
        [SwaggerResponse(statusCode: 200, type: typeof(Manufacturer), description: "successful operation")]
        public IActionResult GetmanufacturerById([FromRoute][Required] string manufacturerId)
        {
            Manufacturer t = Db.Manufacturers.Find(manufacturerId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find Manufacturer by ID
        /// </summary>
        /// <remarks>Returns a single Manufacturer</remarks>
        /// <param name="manufacturer">ID of Manufacturer to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">can not found</response>
        [HttpGet]
        [Route("/manufacturer/byName/{manufacturer}")]
        [ValidateModelState]
        [SwaggerOperation("GetmanufacturerByName")]
        [SwaggerResponse(statusCode: 200, type: typeof(Manufacturer), description: "successful operation")]
        public IActionResult GetmanufacturerByName([FromRoute][Required] string manufacturer)
        {
            Manufacturer t = Db.Manufacturers.Where(i => i.Name.Equals(manufacturer)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// List Manufacturer
        /// </summary>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">can not found</response>
        [HttpGet]
        [Route("/manufacturer/")]
        [ValidateModelState]
        [SwaggerOperation("GetmanufacturerList")]
        [SwaggerResponse(statusCode: 200, type: typeof(Manufacturer), description: "successful operation")]
        public IActionResult GetmanufacturerList()
        {
            List<Manufacturer> t = Db.Manufacturers.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }
    }
}
