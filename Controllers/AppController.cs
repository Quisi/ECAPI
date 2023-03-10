using EnergyScanApi.Attributes;
using EnergyScanApi.DTOs;
using EnergyScanApi.Models;
using EnergyScanApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;

namespace EnergyScanApi.Controllers
{
    /// <summary>
    /// Convinience Endpoints for Mobile and Desktop App
    /// </summary>
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly AppDb Db;
        public IConfiguration Configuration { get; }
        /// <summary>
        /// Constructor with DB-Connection injection
        /// </summary>
        /// <param name="db"></param>
        public AppController(AppDb db, IConfiguration Conf)
        {
            Db = db;
            Configuration = Conf;
        }

        /// <summary>
        /// Create a Can with Fieldinput instead of IDs, therefore search for existing Ids or create new ones
        /// </summary>
        /// <remarks>
        /// Every 'id'-Field will be ignored! Ids are handled by the API.
        /// </remarks>
        /// <param name="body"></param>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="409">Conflict, something already exists</response>
        [Route("/app/can")]
        [ValidateModelState]
        [HttpPost]
        [Authorize(Policy = "create")]
        [SwaggerResponse(statusCode: 200, type: typeof(Can), description: "successful operation")]
        public async Task<IActionResult> Post([FromBody] CanDTO body)
        {
            string userid = new Utilities(Db).GetCurrentUserId(User);
            // check for existing barcode
            Barcode barcode = Db.Barcodes.Where(i => i.Name.Equals(body.Barcodes)).FirstOrDefault();
            if (barcode != null)
            {
                return new ConflictObjectResult("Barcode already exists");
            } else
            {
                // create Barcode 
                barcode = new Barcode()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = body.Barcodes[0].Name,
                    CreationDate = DateTime.Now,
                    ChangedLast = DateTime.Now,
                    ChangedById = userid
                };
                Db.Barcodes.Add(barcode);
            }
            // check for existing manufacturer
            Manufacturer manufacturer = Db.Manufacturers.Where(i => i.Name.Equals(body.Manufacturer.Name)).FirstOrDefault();
            if (manufacturer == null)
            {
                manufacturer = new Manufacturer()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = body.Manufacturer.Name,
                    CreationDate = DateTime.Now,
                    ChangedLast = DateTime.Now,
                    ChangedById = userid
                };
                Db.Manufacturers.Add(manufacturer);
            }
            // check for existing taste
            Taste taste = Db.Tastes.Where(i => i.Name.Equals(body.Taste.Name)).FirstOrDefault();
            if (taste == null)
            {
                taste = new Taste()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = body.Taste.Name,
                    CreationDate = DateTime.Now,
                    ChangedLast = DateTime.Now,
                    ChangedById = userid
                };
                Db.Tastes.Add(taste);
            }
            // check for existing country
            Country country = Db.Countries.Where(i => i.Name.Equals(body.Country.Name)).FirstOrDefault();
            if (country == null && !string.IsNullOrWhiteSpace(body.Country.Name))
            {
                country = new Country()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = body.Country.Name,
                    CreationDate = DateTime.Now,
                    ChangedLast = DateTime.Now,
                    ChangedById = userid
                };
                Db.Countries.Add(country);
            }
            // check for existing State
            Status status = Db.Statuses.Where(i => i.Name.Equals(body.Status.Name)).FirstOrDefault();
            if (status == null && !string.IsNullOrWhiteSpace(body.Status.Name))
            {
                status = new Status()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = body.Status.Name,
                    CreationDate = DateTime.Now,
                    ChangedLast = DateTime.Now,
                    ChangedById = userid
                };
                Db.Statuses.Add(status);
            }
            Can can = new Can()
            {
                Id = Guid.NewGuid().ToString(),
                CreationDate = DateTime.Now,
                ChangedLast = DateTime.Now,
                ChangedById = userid,
                Closure = body.Closure == null ? "-" : body.Closure,
                Contentamount = body.Contentamount == null ? "-" : body.Contentamount,
                Coffeincontent = body.Coffeincontent == null ? "-" : body.Coffeincontent,
                Damaged = body.Damaged,
                Deposit = body.Deposit,
                Description = body.Description is null ? "-" : body.Description,
                Mhd = body.Mhd == null ? "-" : body.Mhd,
                CountryId = country.Id,
                ManufacturerId = manufacturer.Id,
                StatusId = status.Id,
                TasteId = taste.Id
            };
            Db.Cans.Add(can);
            // check tags
            if (body.Tags != null && body.Tags.Count > 0)
            {
                foreach (TagDTO tag in body.Tags)
                {
                    Tag t = Db.Tags.Where(i => i.Name.Equals(tag.Name)).FirstOrDefault();
                    if (t == null)
                    {
                        t = new Tag()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = tag.Name,
                            CreationDate = DateTime.Now,
                            ChangedLast = DateTime.Now,
                            ChangedById = userid
                        };
                        Db.Tags.Add(t);
                    }
                    TagCan tc = new TagCan()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CanId = can.Id,
                        TagId = t.Id,
                        CreationDate = DateTime.Now,
                        ChangedLast = DateTime.Now,
                        ChangedById = userid
                    };
                    Db.TagCans.Add(tc);
                }
            }
            await Db.BarcodeCans.AddAsync(new BarcodeCan() { Id = Guid.NewGuid().ToString(), BarcodeId = barcode.Id, CanId = can.Id });
            await Db.SaveChangesAsync();
            return new OkObjectResult(can);
        }

        /// <summary>
        /// Get Can By Id as DTO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Id not found</response>
        [Route("/app/can/{id}")]
        [ValidateModelState]
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, type: typeof(CanDTO), description: "successful operation")]
        public async Task<IActionResult> GetCanByIdAsync([FromRoute] string id)
        {
            Can can = await Db.Cans.FindAsync(id);
            if (can == null)
            {
                return NotFound();
            } else
            {
                List<BarcodeDTO> barcodes = await getBarcodeDTOs(can.Id);
                List<TagDTO> tags = await getTagDTOs(can.Id);
                CountryDTO countryDTO = null;
                Country country = await Db.Countries.FindAsync(can.CountryId);
                if (country != null)
                {
                    countryDTO = new CountryDTO() { Id = country.Id, Name = country.Name };
                }
                StatusDTO statusDTO = null;
                Status status = await Db.Statuses.FindAsync(can.StatusId);
                if (status != null)
                {
                    statusDTO = new StatusDTO() { Id = status.Id, Name = status.Name };
                }
                TasteDTO tasteDTO = null;
                Taste taste = await Db.Tastes.FindAsync(can.TasteId);
                if (taste != null)
                {
                    tasteDTO = new TasteDTO() { Id = taste.Id, Name = taste.Name };
                }
                ManufacturerDTO manufacturerDTO = null;
                Manufacturer manufacturer = await Db.Manufacturers.FindAsync(can.ManufacturerId);
                if (manufacturer != null)
                {
                    manufacturerDTO = new ManufacturerDTO() { Id = manufacturer.Id, Name = manufacturer.Name };
                }
                CanDTO canDTO = new CanDTO()
                {
                    Id = id,
                    Barcodes = barcodes,
                    Closure = can.Closure,
                    Coffeincontent = can.Coffeincontent,
                    Contentamount = can.Contentamount,
                    Country = countryDTO,
                    Damaged = can.Damaged,
                    Deposit = can.Deposit,
                    Description = can.Description,
                    Mhd = can.Mhd,
                    Status = statusDTO,
                    Taste = tasteDTO,
                    Manufacturer = manufacturerDTO,
                    Tags = tags
                };
                return new OkObjectResult(canDTO);
            }
        }

        /// <summary>
        /// TagDTOs for CanDTO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<TagDTO>> getTagDTOs(string canid)
        {
            List<TagDTO> tagDTOs = new List<TagDTO>();
            List<TagCan> tagcans = await Db.TagCans.Where(i => i.CanId.Equals(canid)).ToListAsync();
            foreach (TagCan tc in tagcans)
            {
                tagDTOs.Add(await GetTagDTO(tc.TagId));
            }
            return tagDTOs;
        }
        /// <summary>
        /// BarcodeDTOs for CanDTO
        /// </summary>
        /// <param name="canId"></param>
        /// <returns></returns>
        private async Task<List<BarcodeDTO>> getBarcodeDTOs(string canId)
        {
            List<BarcodeDTO> barcodeDTOs = new List<BarcodeDTO>();
            List<BarcodeCan> barcodescans = await Db.BarcodeCans.Where(i => i.CanId.Equals(canId)).ToListAsync();
            foreach (BarcodeCan barcodeCan in barcodescans)
            {
                barcodeDTOs.Add(await GetBarcodeDTO(barcodeCan.BarcodeId));
            }
            return barcodeDTOs;
        }

        /// <summary>
        /// Lists Change Requests as DTOs
        /// </summary>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Id not found</response>
        [Route("/app/changeRequest")]
        [ValidateModelState]
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, type: typeof(ChangeRequestDTO), description: "successful operation")]
        public async Task<IActionResult> GetChangeRequestsAsync()
        {
            List<ChangeRequestDTO> dTOs = new List<ChangeRequestDTO>();
            List<ChangeRequest> changeRequests = await Db.ChangeRequests.ToListAsync();
            foreach (ChangeRequest request in changeRequests)
            {
                ChangeRequestDTO dTO = new ChangeRequestDTO();
                dTO.id = request.Id;
                dTO.changedBy = await GetUserDTO(request.ChangedById);
                dTO.changedLast = request.ChangedLast;
                dTO.changenewValue = request.ChangeNewValue;
                dTO.changeoldValue = request.ChangeOldValue;
                dTO.creationDate = request.CreationDate;
                dTO.field = request.Field;
                dTO.pk = request.Pk;
                dTO.pkField = request.PkField;
                dTO.status = await GetStatusDTO(request.StateId);
                dTO.table = request.Table;
                dTO.timestamp = request.Timestamp;
                dTO.user = await GetUserDTO(request.UserId);
            }
            return new OkObjectResult(dTOs);
        }

        private async Task<UserDTO> GetUserDTO(string userId)
        {
            User u = await Db.Users.FindAsync(userId);
            UserDTO userDTO = new UserDTO()
            {
                Id = userId,
                Email = u.Email,
                Username = u.Username,
                Verified = u.Verified,
                Group = await GetGroupDTO(u.GroupId)
            };
            return userDTO;
        }
        private async Task<GroupDTO> GetGroupDTO(string groupId)
        {
            Group g = await Db.Groups.FindAsync(groupId);
            GroupDTO groupDTO = new GroupDTO();
            if (g != null)
            {
                groupDTO.Id = g.Id;
                groupDTO.Name = g.Name;
            } else
            {
                groupDTO.Id = "-1";
                groupDTO.Name = "unknown";
            }
            return groupDTO;
        }
        private async Task<StatusDTO> GetStatusDTO(string statusId)
        {
            Status s = await Db.Statuses.FindAsync(statusId);
            StatusDTO statusDTO = new StatusDTO();
            if (s != null)
            {
                statusDTO.Id = s.Id;
                statusDTO.Name = s.Name;
            }else
            {
                statusDTO.Id = "-1";
                statusDTO.Name = "unknown";
            }
            return statusDTO;
        }
        private async Task<BarcodeDTO> GetBarcodeDTO(string barcodeId)
        {
            Barcode b = await Db.Barcodes.FindAsync(barcodeId);
            BarcodeDTO barcodeDTO = new BarcodeDTO();
            if (b != null)
            {
                barcodeDTO.Id = b.Id;
                barcodeDTO.Name = b.Name;
            }else
            {
                barcodeDTO.Id = "-1";
                barcodeDTO.Name = "unknown";
            }
            return barcodeDTO;
        }
        private async Task<TagDTO> GetTagDTO(string tagId)
        {
            Tag t = await Db.Tags.FindAsync(tagId);
            TagDTO tagDTO = new TagDTO();
            if (t != null)
            {
                tagDTO.Id = t.Id;
                tagDTO.Name = t.Name;
            }else
            {
                tagDTO.Id = "-1";
                tagDTO.Name = "unknown";
            }
            return tagDTO;
        }
        private async Task<CountryDTO> GetCountryDTO(string countryId)
        {
            Country t = await Db.Countries.FindAsync(countryId);
            CountryDTO countryDTO = new CountryDTO();
            if (t != null)
            {
                countryDTO.Id = t.Id;
                countryDTO.Name = t.Name;
            }else
            {
                countryDTO.Id = "-1";
                countryDTO.Name = "unknown";
            }
            return countryDTO;
        }
        private async Task<ManufacturerDTO> GetManufacturerDTO(string manufacturerId)
        {
            Manufacturer t = await Db.Manufacturers.FindAsync(manufacturerId);
            ManufacturerDTO manufacturerDTO = new ManufacturerDTO();
            if (t != null)
            {
                manufacturerDTO.Id = t.Id;
                manufacturerDTO.Name = t.Name;
            }
            else
            {
                manufacturerDTO.Id = "-1";
                manufacturerDTO.Name = "unknown";
            }
            return manufacturerDTO;
        }
        private async Task<TasteDTO> GetTasteDTO(string tasteId)
        {
            Taste t = await Db.Tastes.FindAsync(tasteId);
            TasteDTO tasteDTO = new TasteDTO();
            if (t != null)
            {
                tasteDTO.Id = t.Id;
                tasteDTO.Name = t.Name;
            }else
            {
                tasteDTO.Id = "-1";
                tasteDTO.Name = "unknown";
            }
            return tasteDTO;
        }

        /// <summary>
        /// Get Can List as DTOs
        /// </summary>
        /// <returns></returns>
        /// <response code="200">successful operation</response>
        /// <response code="404">Id not found</response>
        [Route("/app/can/")]
        [ValidateModelState]
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, type: typeof(List<CanDTO>), description: "successful operation")]
        public async Task<IActionResult> GetCansAsync()
        {
            List<CanDTO> dTOs = new List<CanDTO>();
            List<Can> cans = await Db.Cans.ToListAsync();
            foreach(Can can in cans)
            {
                CanDTO dTO = new CanDTO()
                {
                    Id = can.Id,
                    Barcodes = await getBarcodeDTOs(can.Id),
                    Closure = can.Closure,
                    Coffeincontent = can.Coffeincontent,
                    Contentamount = can.Contentamount,
                    Country = await GetCountryDTO(can.CountryId),
                    Damaged = can.Damaged,
                    Deposit = can.Deposit,
                    Description = can.Description,
                    Manufacturer = await GetManufacturerDTO(can.ManufacturerId),
                    Mhd = can.Mhd,
                    Status = await GetStatusDTO(can.StatusId),
                    Tags = await getTagDTOs(can.Id),
                    Taste = await GetTasteDTO(can.TasteId)                    
                };
                dTOs.Add(dTO);
            }
            return new OkObjectResult(dTOs);
        }
    }
}
