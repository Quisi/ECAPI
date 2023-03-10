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
    /// API-Controller: Country
    /// </summary>
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly AppDb Db;
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public CountryController(AppDb db)
        {
            Db = db;
        }

        /// <summary>
        /// Add a new country
        /// </summary>
        /// <param name="body">country object that needs to be added to the store</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">can not found</response>
        /// <response code="405">Invalid input</response>
        [HttpPost]
        [Route("/country")]
        [ValidateModelState]
        [SwaggerOperation("Addcountry")]
        [Authorize(Policy = "create")]
        public virtual async Task<IActionResult> Addcountry([FromBody] Country body)
        {
            Country duplicate = Db.Countries.Where(i => i.Name.Equals(body.Name)).FirstOrDefault();
            if (duplicate == null)
            {
                body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
                body.CreationDate = DateTime.Now;
                body.ChangedLast = DateTime.Now;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                Db.Countries.Add(body);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(body);
                }
                return new BadRequestResult();
            }
            return new OkObjectResult(duplicate);
        }

        /// <summary>
        /// Update an existing country
        /// </summary>
        /// <remarks>Update country</remarks>
        /// <param name="countryId">country object that needs to be updated </param>
        /// <param name="body"></param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">country not found</response>
        /// <response code="405">Validation exception</response>
        [HttpPut]
        [Route("/country/{countryId}")]
        [ValidateModelState]
        [SwaggerOperation("Updatecountry")]
        [Authorize(Policy = "update")]
        public virtual async Task<IActionResult> Updatecountry([FromRoute][Required] string countryId, [FromBody] Country body)
        {
            Country t = Db.Countries.Find(countryId);
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
        /// Deletes a country
        /// </summary>
        /// <param name="countryId">country id to delete</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">country not found</response>
        [HttpDelete]
        [Route("/country/{countryId}")]
        [ValidateModelState]
        [SwaggerOperation("Deletecountry")]
        [Authorize(Policy = "delete")]
        public virtual async Task<IActionResult> Deletecountry([FromRoute][Required] string countryId)
        {
            Country t = Db.Countries.Find(countryId);
            if (t != null)
            {
                Db.Countries.Remove(t);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find country by ID
        /// </summary>
        /// <remarks>Returns a single can</remarks>
        /// <param name="countryId">ID of country to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">country not found</response>
        [HttpGet]
        [Route("/country/{countryId}")]
        [ValidateModelState]
        [SwaggerOperation("GetcountryById")]
        [SwaggerResponse(statusCode: 200, type: typeof(Country), description: "successful operation")]
        public IActionResult GetcountryById([FromRoute][Required] string countryId)
        {
            Country t = Db.Countries.Find(countryId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find Country by Name
        /// </summary>
        /// <remarks>Returns a single Country</remarks>
        /// <param name="country">ID of Country to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">can not found</response>
        [HttpGet]
        [Route("/country/byName/{country}")]
        [ValidateModelState]
        [SwaggerOperation("GetcountryByName")]
        [SwaggerResponse(statusCode: 200, type: typeof(Country), description: "successful operation")]
        public IActionResult GetcountryByName([FromRoute][Required] string country)
        {
            Country t = Db.Countries.Where(i => i.Name.Equals(country)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// List Country
        /// </summary>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Country not found</response>
        [HttpGet]
        [Route("/country/")]
        [ValidateModelState]
        [SwaggerOperation("GetcountryList")]
        [SwaggerResponse(statusCode: 200, type: typeof(Country), description: "successful operation")]
        public IActionResult GetcountryList()
        {
            List<Country> t = Db.Countries.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }
    }
}
