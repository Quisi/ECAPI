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
    /// API-Controller: Policy
    /// </summary>
    [ApiController]
    public class PolicyController : ControllerBase
    {
        private readonly AppDb Db;
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public PolicyController(AppDb db)
        {
            Db = db;
        }

        /// <summary>
        /// Add a new policy
        /// </summary>
        /// <param name="body">Policy object that needs to be added to the store</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Policy not found</response>
        /// <response code="405">Invalid input</response>
        [HttpPost]
        [Route("/policy")]
        [ValidateModelState]
        [SwaggerOperation("Addpolicy")]
        [Authorize(Policy = "policymanager")]
        public virtual async Task<IActionResult> Addpolicy([FromBody] Policy body)
        {
            Policy duplicate = Db.Policies.Where(i => i.Name.Equals(body.Name)).FirstOrDefault();
            if (duplicate == null)
            {
                body.Id = Guid.NewGuid().ToString(); // Generate UUId for new content
                body.CreationDate = DateTime.Now;
                body.ChangedLast = DateTime.Now;
                body.ChangedById = new Utilities(Db).GetCurrentUser(User).Id;
                Db.Policies.Add(body);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkObjectResult(body);
                }
                return new BadRequestResult();
            }
            return new OkObjectResult(duplicate);
        }

        /// <summary>
        /// Update an existing policy
        /// </summary>
        /// <remarks>Update policy</remarks>
        /// <param name="policyId">Policy object that needs to be updated </param>
        /// <param name="body"></param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Policy not found</response>
        /// <response code="405">Validation exception</response>
        [HttpPut]
        [Route("/policy/{policyId}")]
        [ValidateModelState]
        [SwaggerOperation("Updatepolicy")]
        [Authorize(Policy = "policymanager")]
        public virtual async Task<IActionResult> Updatepolicy([FromRoute][Required] string policyId, [FromBody] Policy body)
        {
            Policy t = Db.Policies.Find(policyId);
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
        /// Deletes a policy
        /// </summary>
        /// <param name="policyId">Policy id to delete</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Policy not found</response>
        [HttpDelete]
        [Route("/policy/{policyId}")]
        [ValidateModelState]
        [SwaggerOperation("Deletepolicy")]
        [Authorize(Policy = "policymanager")]
        public virtual async Task<IActionResult> Deletepolicy([FromRoute][Required] string policyId)
        {
            Policy t = Db.Policies.Find(policyId);
            if (t != null)
            {
                IQueryable<UserPolicy> ups = Db.UserPolicies.Where(i => i.PolicyId.Equals(policyId));
                Db.UserPolicies.RemoveRange(ups);
                Db.Policies.Remove(t);
                if (0 < await Db.SaveChangesAsync())
                {
                    return new OkResult();
                }
                return new BadRequestResult();
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find policy by ID
        /// </summary>
        /// <remarks>Returns a single policy</remarks>
        /// <param name="policyId">ID of policy to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Policy not found</response>
        [HttpGet]
        [Route("/policy/{policyId}")]
        [ValidateModelState]
        [SwaggerOperation("GetpolicyById")]
        [SwaggerResponse(statusCode: 200, type: typeof(Policy), description: "successful operation")]
        public IActionResult GetpolicyById([FromRoute][Required] string policyId)
        {
            Policy t = Db.Policies.Find(policyId);
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Find policy by Name
        /// </summary>
        /// <remarks>Returns a single policy</remarks>
        /// <param name="policy">ID of policy to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Policy not found</response>
        [HttpGet]
        [Route("/policy/byName/{policy}")]
        [ValidateModelState]
        [SwaggerOperation("GetpolicyByName")]
        [SwaggerResponse(statusCode: 200, type: typeof(Policy), description: "successful operation")]
        public IActionResult GetpolicyByName([FromRoute][Required] string policy)
        {
            Policy t = Db.Policies.Where(i => i.Name.Equals(policy)).FirstOrDefault();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// List policys
        /// </summary>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Policys not found</response>
        [HttpGet]
        [Route("/policy/")]
        [ValidateModelState]
        [SwaggerOperation("GetpolicyList")]
        [SwaggerResponse(statusCode: 200, type: typeof(Policy), description: "successful operation")]
        public IActionResult GetpolicyList()
        {
            List<Policy> t = Db.Policies.ToList();
            if (t != null)
            {
                return new OkObjectResult(t);
            }
            return new NotFoundResult();
        }
    }
}
