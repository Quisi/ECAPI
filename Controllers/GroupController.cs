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
    /// API-Controller: Group
    /// </summary>
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly AppDb Db;
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public GroupController(AppDb db)
        {
            Db = db;
        }

        /// <summary>
        /// Add a new group
        /// </summary>
        /// <param name="body">Group object that needs to be added to the store</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Group not found</response>
        /// <response code="405">Invalid input</response>
        [HttpPost]
        [Route("/group")]
        [ValidateModelState]
        [SwaggerOperation("Addgroup")]
        [Authorize(Policy = "groupmanager")]
        public virtual async Task<IActionResult> Addgroup([FromBody] Group body)
        {
            Group duplicate = Db.Groups.Where(i => i.Name.Equals(body.Name)).FirstOrDefault();
            if (duplicate == null)
            {
                body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
                body.CreationDate = DateTime.Now;
                body.ChangedLast = DateTime.Now;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                Db.Groups.Add(body);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(body);
                }
                return new BadRequestResult();
            }
            return new OkObjectResult(duplicate);
        }

        /// <summary>
        /// Update an existing group
        /// </summary>
        /// <remarks>Update group</remarks>
        /// <param name="groupId">Group object that needs to be updated </param>
        /// <param name="body"></param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Group not found</response>
        /// <response code="405">Validation exception</response>
        [HttpPut]
        [Route("/group/{groupId}")]
        [ValidateModelState]
        [SwaggerOperation("Updategroup")]
        [Authorize(Policy = "groupmanager")]
        public virtual async Task<IActionResult> Updategroup([FromRoute][Required] string groupId, [FromBody] Group body)
        {
            Group t = Db.Groups.Find(groupId);
            if (t != null)
            {
                t.Name = body.Name;
                body.ChangedLast = DateTime.Now;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(t);
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Deletes a group
        /// </summary>
        /// <param name="groupId">Group id to delete</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Group not found</response>
        [HttpDelete]
        [Route("/group/{groupId}")]
        [ValidateModelState]
        [SwaggerOperation("Deletegroup")]
        [Authorize(Policy = "groupmanager")]
        public virtual async Task<IActionResult> Deletegroup([FromRoute][Required] string groupId)
        {
            Group t = Db.Groups.Find(groupId);
            if (t != null)
            {
                Db.Groups.Remove(t);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find group by ID
        /// </summary>
        /// <remarks>Returns a single group</remarks>
        /// <param name="groupId">ID of group to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Group not found</response>
        [HttpGet]
        [Route("/group/{groupId}")]
        [ValidateModelState]
        [SwaggerOperation("GetgroupById")]
        [SwaggerResponse(statusCode: 200, type: typeof(Group), description: "successful operation")]
        public IActionResult GetgroupById([FromRoute][Required] string groupId)
        {
            Group t = Db.Groups.Find(groupId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find group by Name
        /// </summary>
        /// <remarks>Returns a single group</remarks>
        /// <param name="group">ID of group to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Group not found</response>
        [HttpGet]
        [Route("/group/byName/{group}")]
        [ValidateModelState]
        [SwaggerOperation("GetgroupByName")]
        [SwaggerResponse(statusCode: 200, type: typeof(Group), description: "successful operation")]
        public IActionResult GetgroupByName([FromRoute][Required] string group)
        {
            Group t = Db.Groups.Where(i => i.Name.Equals(group)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// List groups
        /// </summary>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Groups not found</response>
        [HttpGet]
        [Route("/group/")]
        [ValidateModelState]
        [SwaggerOperation("GetgroupList")]
        [SwaggerResponse(statusCode: 200, type: typeof(Group), description: "successful operation")]
        public IActionResult GetgroupList()
        {
            List<Group> t = Db.Groups.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }
    }
}
