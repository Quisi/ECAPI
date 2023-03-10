using EnergyScanApi.Attributes;
using EnergyScanApi.Models;
using EnergyScanApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;
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
    public class CanController : ControllerBase
    {
        private readonly AppDb Db;
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public CanController(AppDb db)
        {
            Db = db;
        }

        /// <summary>
        /// Creates a can in the store
        /// </summary>
        /// <param name="body">Can object to be created</param>
        /// <response code="405">Invalid input</response>
        [HttpPost]
        [Route("/can/")]
        [ValidateModelState]
        [SwaggerOperation("CreateCan")]
        [Authorize(Policy = "create")]
        public virtual async Task<IActionResult> CreateCan([FromBody] Can body)
        {
            Can duplicate = Db.Cans.Where(i => i.ManufacturerId.Equals(body.ManufacturerId)
                                        && i.TasteId.Equals(body.TasteId)
                                        && i.Closure.Equals(body.Closure)
                                        && i.Coffeincontent.Equals(body.Coffeincontent)
                                        && i.Contentamount.Equals(body.Contentamount)
                                        && i.CountryId.Equals(body.CountryId)
                                        && i.Deposit.Equals(body.Deposit)
                                        ).FirstOrDefault();
            if (duplicate == null)
            {
                body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
                body.CreationDate = DateTime.Now;
                body.ChangedLast = DateTime.Now;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                Db.Cans.Add(body);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(body);
                }
                return new BadRequestResult();
            }
            return new OkObjectResult(duplicate);
        }

        /// <summary>
        /// Updates a can in the store with form data
        /// </summary>
        /// <param name="canId">ID of can that needs to be updated</param>
        /// <param name="name">Updated name of the can</param>
        /// <param name="status">Updated status of the can</param>
        /// <response code="405">Invalid input</response>
        [HttpPut]
        [Route("/can/{canId}")]
        [ValidateModelState]
        [SwaggerOperation("UpdateCanWithForm")]
        [Authorize(Policy = "update")]
        public virtual async Task<IActionResult> UpdateCanWithForm([FromRoute][Required] string canId, [FromForm] Can body)
        {
            Can t = await Db.Cans.FindAsync(canId);
            if (t != null)
            {
                t.Closure = body.Closure;
                t.Coffeincontent = body.Coffeincontent;
                t.Contentamount = body.Contentamount;
                t.CountryId = body.CountryId;
                t.Damaged = body.Damaged;
                t.Deposit = body.Deposit;
                t.Description = body.Description;
                t.ManufacturerId = body.ManufacturerId;
                t.Mhd = body.Mhd;
                t.StatusId = body.StatusId;
                t.TasteId = body.TasteId;
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
        /// Patches a can
        /// </summary>
        /// <param name="canId">can id to be patched</param>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">can not found</response>
        [HttpPatch]
        [Route("/can/{canId}")]
        [SwaggerOperation("Patchcan")]
        [Authorize]
        public async Task<IActionResult> PatchCan([FromRoute][Required] string canId, Can newcan)
        {
            Can t = await Db.Cans.FindAsync(canId);
            if (t != null)
            {
                Status s = await Db.Statuses.Where(i => i.Name.Equals("new")).FirstOrDefaultAsync();
                if (t.Closure != newcan.Closure)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.Closure,
                        ChangeNewValue = newcan.Closure,
                        CreationDate = DateTime.Now,
                        Field = "Closure",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.Closure = newcan.Closure;
                }
                if (t.Coffeincontent != newcan.Coffeincontent)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.Coffeincontent,
                        ChangeNewValue = newcan.Coffeincontent,
                        CreationDate = DateTime.Now,
                        Field = "Coffeincontent",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.Coffeincontent = newcan.Coffeincontent;
                }
                if (t.Contentamount != newcan.Contentamount)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.Contentamount,
                        ChangeNewValue = newcan.Contentamount,
                        CreationDate = DateTime.Now,
                        Field = "Contentamount",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.Contentamount = newcan.Contentamount;
                }
                if (t.CountryId != newcan.CountryId)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.CountryId,
                        ChangeNewValue = newcan.CountryId,
                        CreationDate = DateTime.Now,
                        Field = "CountryId",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.CountryId = newcan.CountryId;
                }
                if (t.Damaged != newcan.Damaged)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.Damaged ? "true" : "false",
                        ChangeNewValue = newcan.Damaged ? "true" : "false",
                        CreationDate = DateTime.Now,
                        Field = "Damaged",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.Damaged = newcan.Damaged;
                }
                if (t.Deposit != newcan.Deposit)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.Deposit ? "true" : "false",
                        ChangeNewValue = newcan.Deposit ? "true" : "false",
                        CreationDate = DateTime.Now,
                        Field = "Deposit",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.Deposit = newcan.Deposit;
                }
                if (t.Description != newcan.Description)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.Description,
                        ChangeNewValue = newcan.Description,
                        CreationDate = DateTime.Now,
                        Field = "Description",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.Description = newcan.Description;
                }
                if (t.ManufacturerId != newcan.ManufacturerId)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.ManufacturerId,
                        ChangeNewValue = newcan.ManufacturerId,
                        CreationDate = DateTime.Now,
                        Field = "ManufacturerId",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.ManufacturerId = newcan.ManufacturerId;
                }
                if (t.Mhd != newcan.Mhd)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.Mhd,
                        ChangeNewValue = newcan.Mhd,
                        CreationDate = DateTime.Now,
                        Field = "Mhd",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.Mhd = newcan.Mhd;
                }
                if (t.StatusId != newcan.StatusId)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.StatusId,
                        ChangeNewValue = newcan.StatusId,
                        CreationDate = DateTime.Now,
                        Field = "StatusId",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.StatusId = newcan.StatusId;
                }
                if (t.TasteId != newcan.TasteId)
                {
                    ChangeRequest changeRequest = new ChangeRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChangedById = new Utilities(Db).GetCurrentUser(User).Id,
                        ChangedLast = DateTime.Now,
                        ChangeOldValue = t.TasteId,
                        ChangeNewValue = newcan.TasteId,
                        CreationDate = DateTime.Now,
                        Field = "TasteId",
                        PkField = "Id",
                        Pk = t.Id,
                        Table = "Can",
                        Timestamp = DateTime.Now,
                        StateId = s.Id,
                        UserId = new Utilities(Db).GetCurrentUser(User).Id,
                    };
                    Db.ChangeRequests.Add(changeRequest);
                    t.TasteId = newcan.TasteId;
                }
                await Db.SaveChangesAsync();
                return new OkResult();
            }
            return new NotFoundResult();
        }


        /// <summary>
        /// Deletes a can
        /// </summary>
        /// <param name="canId">can id to delete</param>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">can not found</response>
        [HttpDelete]
        [Route("/can/{canId}")]
        [ValidateModelState]
        [SwaggerOperation("Deletecan")]
        [Authorize(Policy = "delete")]
        public virtual async Task<IActionResult> Deletecan([FromRoute][Required] string canId)
        {
            Can t = Db.Cans.Find(canId);
            if (t != null)
            {
                IQueryable<TagCan> tcs = Db.TagCans.Where(i => i.CanId.Equals(canId));
                IQueryable<BarcodeCan> bcs = Db.BarcodeCans.Where(i => i.CanId.Equals(canId));
                IQueryable<ImageCan> ics = Db.ImageCans.Where(i => i.CanId.Equals(canId));

                foreach (BarcodeCan bci in bcs)
                {
                    Db.Barcodes.Remove(Db.Barcodes.Where(i => i.Id.Equals(bci.BarcodeId)).FirstOrDefault());
                }
                foreach (ImageCan ici in ics)
                {
                    Image img = Db.Images.Find(ici.Id);
                    string fullfilename = img.Url.Split("/").Last();
                    Db.Images.Remove(img);
                    try
                    {
                        String folderName = "uploads";
                        String folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        System.IO.File.Delete(Path.Combine(folderPath, fullfilename));
                    } catch (Exception)
                    {
                    }
                }
                Db.TagCans.RemoveRange(tcs);
                Db.BarcodeCans.RemoveRange(bcs);
                Db.ImageCans.RemoveRange(ics);
                Db.Cans.Remove(t);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find can by ID
        /// </summary>
        /// <remarks>Returns a single can</remarks>
        /// <param name="canId">ID of can to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">can not found</response>
        [HttpGet]
        [Route("/can/{canId}")]
        [ValidateModelState]
        [Authorize]
        [SwaggerOperation("GetCanById")]
        [SwaggerResponse(statusCode: 200, type: typeof(Can), description: "successful operation")]
        public IActionResult GetCanById([FromRoute][Required] string canId)
        {
            Can t = Db.Cans.Find(canId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by manufacturerid
        /// </summary>
        /// <param name="manufacturerid">Status values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByManufacturerId/{manufacturerid}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByManufacturerId")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByManufacturerId([FromRoute][Required()] string manufacturerid)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.ManufacturerId.Equals(manufacturerid)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by tasteid
        /// </summary>
        /// <param name="tasteid">Status values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByTasteId/{tasteid}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByTasteId")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByTasteId([FromRoute][Required()] string tasteid)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.TasteId.Equals(tasteid)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by countryid
        /// </summary>
        /// <param name="countryid">Status values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByCountryId/{countryid}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByCountryId")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByCountryId([FromRoute][Required()] string countryid)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.CountryId.Equals(countryid)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by status
        /// </summary>
        /// <param name="statusid">Status values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByStatus/{statusid}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByStatus")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByStatus([FromRoute][Required()] string statusid)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.StatusId.Equals(statusid)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by contentAmount
        /// </summary>
        /// <param name="contentAmount">Status values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByContentAmount/{contentAmount}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByContentAmount")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByContentAmount([FromRoute][Required()] string contentAmount)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.Contentamount.Equals(contentAmount)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by mhd
        /// </summary>
        /// <param name="mhd">Status values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByMhd/{mhd}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByMhd")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByMhd([FromRoute][Required()] string mhd)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.Mhd.Equals(mhd)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by deposit
        /// </summary>
        /// <param name="deposit">values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByDeposit/{deposit}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByDeposit")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByDeposit([FromRoute][Required()] string deposit)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.Deposit.Equals(deposit)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by closure
        /// </summary>
        /// <param name="closure">values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByClosure/{closure}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByClosure")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByClosure([FromRoute][Required()] string closure)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.Closure.Equals(closure)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by damaged
        /// </summary>
        /// <param name="damaged">values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByDamaged/{damaged}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByDamaged")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByDamaged([FromRoute][Required()] bool damaged)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.Damaged.Equals(damaged)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Cans by coffeincontent
        /// </summary>
        /// <param name="coffeincontent">values that need to be considered for filter</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid status value</response>
        [HttpGet]
        [Route("/can/findByCoffeincontent/{coffeincontent}")]
        [ValidateModelState]
        [SwaggerOperation("FindCansByCoffeincontent")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Can>), description: "successful operation")]
        public IActionResult FindCansByCoffeincontent([FromRoute][Required()] string coffeincontent)
        {
            IOrderedQueryable<Can> q = Db.Cans.Where(i => i.Coffeincontent.Equals(coffeincontent)).OrderBy(i => i.ChangedLast);
            List<Can> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// List Cans
        /// </summary>
        /// <remarks>Returns all cans</remarks>
        /// <response code="200">successful operation</response>        
        /// <response code="404">can not found</response>
        [HttpGet]
        [Route("/can/")]
        [ValidateModelState]
        [SwaggerOperation("GetCans")]
        [SwaggerResponse(statusCode: 200, type: typeof(Can), description: "successful operation")]
        public IActionResult GetCans()
        {
            List<Can> t = Db.Cans.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }
    }
}
