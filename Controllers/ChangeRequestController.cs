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
    public class ChangeRequestController : ControllerBase
    {
        private readonly AppDb Db;
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public ChangeRequestController(AppDb db)
        {
            Db = db;
        }

        /// <summary>
        /// List changeRequests
        /// </summary>
        /// <remarks>Returns all changeRequests</remarks>
        /// <response code="200">successful operation</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequest")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequest()
        {
            List<ChangeRequest> t = Db.ChangeRequests.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Add a new changeRequest
        /// </summary>
        /// <param name="body">changeRequest object that needs to be added to the store</param>
        /// <response code="405">Invalid input</response>
        [HttpPost]
        [Route("/changeRequest")]
        [ValidateModelState]
        [SwaggerOperation("AddchangeRequest")]
        [Authorize(Policy = "create")]
        public virtual async Task<IActionResult> AddchangeRequest([FromBody] ChangeRequest body)
        {
            body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
            body.CreationDate = DateTime.Now;
            body.ChangedLast = DateTime.Now;
            body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
            Db.ChangeRequests.Add(body);
            if (0 < await Db.SaveChangesAsync())
            {
                return new OkObjectResult(body);
            }
            return new BadRequestResult();
        }

        /// <summary>
        /// Update an existing changeRequest
        /// </summary>
        /// <remarks>Update changeRequest</remarks>
        /// <param name="changeRequestId">changeRequest object that needs to be updated </param>
        /// <param name="body"></param>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">changeRequest not found</response>
        /// <response code="405">Validation exception</response>
        [HttpPut]
        [Route("/changeRequest/{changeRequestId}")]
        [ValidateModelState]
        [SwaggerOperation("UpdatechangeRequest")]
        [Authorize(Policy = "requestmanager")]
        public virtual async Task<IActionResult> UpdatechangeRequest([FromRoute][Required] string changeRequestId, [FromBody] ChangeRequest body)
        {
            ChangeRequest t = Db.ChangeRequests.Find(changeRequestId);
            if (t != null)
            {
                t.ChangeNewValue = body.ChangeNewValue;
                t.ChangeOldValue = body.ChangeOldValue;
                t.Field = body.Field;
                t.Pk = body.Pk;
                t.PkField = body.PkField;
                t.StateId = body.StateId;
                t.Table = body.Table;
                t.Timestamp = body.Timestamp;
                t.UserId = body.UserId;
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
        /// Deletes a changeRequest
        /// </summary>
        /// <param name="changeRequestId">changeRequest id to delete</param>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpDelete]
        [Route("/changeRequest/{changeRequestId}")]
        [ValidateModelState]
        [SwaggerOperation("DeletechangeRequest")]
        [Authorize(Policy = "requestmanager")]
        public virtual async Task<IActionResult> DeletechangeRequest([FromRoute][Required] string changeRequestId)
        {
            ChangeRequest t = Db.ChangeRequests.Find(changeRequestId);
            if (t != null)
            {
                Db.ChangeRequests.Remove(t);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find changeRequests by ID
        /// </summary>
        /// <remarks>Returns a single can</remarks>
        /// <param name="changeRequestId">ID of changeRequest to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/{changeRequestId}")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequestById")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequestById([FromRoute][Required] string changeRequestId)
        {
            ChangeRequest t = Db.ChangeRequests.Find(changeRequestId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find changeRequests by Table
        /// </summary>
        /// <remarks>Returns a List of changeRequests</remarks>
        /// <param name="table">Table of changeRequests to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/byTable/{table}")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequestByTable")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequestByTable([FromRoute][Required] string table)
        {
            IOrderedQueryable<ChangeRequest> q = Db.ChangeRequests.Where(i => i.Table.Equals(table)).OrderBy(i => i.ChangedLast);
            List<ChangeRequest> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find changeRequests by Field
        /// </summary>
        /// <remarks>Returns a List of changeRequests</remarks>
        /// <param name="field">Field of changeRequests to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid Field supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/byField/{field}")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequestByField")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequestByField([FromRoute][Required] string field)
        {
            IOrderedQueryable<ChangeRequest> q = Db.ChangeRequests.Where(i => i.Field.Equals(field)).OrderBy(i => i.ChangedLast);
            List<ChangeRequest> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find changeRequests by Pkfield
        /// </summary>
        /// <remarks>Returns a List of changeRequests</remarks>
        /// <param name="pkfield">Pkfield of changeRequests to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid Pkfield supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/byPkField/{pkfield}")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequestByPkField")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequestByPkField([FromRoute][Required] string pkfield)
        {
            IOrderedQueryable<ChangeRequest> q = Db.ChangeRequests.Where(i => i.PkField.Equals(pkfield)).OrderBy(i => i.ChangedLast);
            List<ChangeRequest> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find changeRequests by Pk
        /// </summary>
        /// <remarks>Returns a List of changeRequests</remarks>
        /// <param name="pk">Pk of changeRequests to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid Pk supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/byPk/{pk}")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequestByPk")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequestByPk([FromRoute][Required] string pk)
        {
            IOrderedQueryable<ChangeRequest> q = Db.ChangeRequests.Where(i => i.Pk.Equals(pk)).OrderBy(i => i.ChangedLast);
            List<ChangeRequest> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find changeRequests by UserId
        /// </summary>
        /// <remarks>Returns a List of changeRequests</remarks>
        /// <param name="userId">UserId of changeRequests to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid UserId supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/byUserId/{userId}")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequestByUserId")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequestByUserId([FromRoute][Required] string userId)
        {
            IOrderedQueryable<ChangeRequest> q = Db.ChangeRequests.Where(i => i.UserId.Equals(userId)).OrderBy(i => i.ChangedLast);
            List<ChangeRequest> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find changeRequests by Timestamp
        /// </summary>
        /// <remarks>Returns a List of changeRequests</remarks>
        /// <param name="timestamp">Timestamp of changeRequests to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid Timestamp supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/byTimestamp/{timestamp}")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequestByTimestamp")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequestByTimestamp([FromRoute][Required] DateTime timestamp)
        {
            IOrderedQueryable<ChangeRequest> q = Db.ChangeRequests.Where(i => i.Timestamp.Equals(timestamp)).OrderBy(i => i.ChangedLast);
            List<ChangeRequest> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find changeRequests by StateId
        /// </summary>
        /// <remarks>Returns a List of changeRequests</remarks>
        /// <param name="stateId">StateId of changeRequests to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid StateId supplied</response>
        /// <response code="404">changeRequest not found</response>
        [HttpGet]
        [Route("/changeRequest/byStateId/{stateId}")]
        [ValidateModelState]
        [SwaggerOperation("GetChangeRequestByStateId")]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequest), description: "successful operation")]
        [Authorize(Policy = "requestmanager")]
        public IActionResult GetChangeRequestByStateId([FromRoute][Required] string stateId)
        {
            IOrderedQueryable<ChangeRequest> q = Db.ChangeRequests.Where(i => i.StateId.Equals(stateId)).OrderBy(i => i.ChangedLast);
            List<ChangeRequest> t = q.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

    }
}
