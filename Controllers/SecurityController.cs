using EnergyScanApi.DTOs;
using EnergyScanApi.Models;
using EnergyScanApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EnergyScanApi.Controllers
{
    /// <summary>
    /// Controller endpoint for authorization, token and security
    /// </summary>
    public class SecurityController : Controller
    {
        readonly IConfiguration config;
        readonly AppDb Db;
        public SecurityController(IConfiguration configuration, AppDb db)
        {
            config = configuration;
            Db = db;
        }

        /// <summary>
        /// Get JWT-Token by Username
        /// </summary>
        /// <param name="username">The user name for login</param>
        /// <param name="password">The password for login in clear text</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid username/password supplied</response>
        [HttpPost]
        [Route("/authorize")]
        [AllowAnonymous]
        public async Task<IActionResult> Authorize([FromQuery][Required()] string username, [FromQuery][Required()] string password)
        {
            List<Claim> claims = new List<Claim>();
            User u = await Db.Users.Where(i => i.Username.Equals(username)).FirstOrDefaultAsync();
            if (u == null)
            {
                return new StatusCodeResult(400);
            } else
            {
                string hash = u.Password;
                (bool Verified, bool NeedsUpgrade) = new PasswordHasher(new HashingOptions()).Check(hash, password);
                if (Verified)
                {
                    /** new way, just claim the user id and do permissioncheck in authorizationhandler **/
                    //************MOST IMPORTANT CLAIM FOR COREAUTHORIZATIONHANDLER***********
                    claims.Add(new Claim("userId", u.Id.ToString()));
                    //All Claims from grouppermission and userpermission collected, make us a token please.
                    string issuer = config["Authentification:Issuer"].ToString();
                    string audience = config["Authentification:Audience"].ToString();
                    byte[] secret = Encoding.UTF8.GetBytes(config["Authentification:Secret"].ToString());
                    SymmetricSecurityKey key = new SymmetricSecurityKey(secret);
                    string algorithm = SecurityAlgorithms.HmacSha256;
                    SigningCredentials signingCredientials = new SigningCredentials(key, algorithm);
                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer,
                        audience,
                        claims,
                        notBefore: DateTime.Now,
                        expires: DateTime.Now.AddDays(1),
                        signingCredientials
                        );
                    string tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
                    TokenDTO result = new TokenDTO() { access_token = tokenJson, Id = u.Id, Username = u.Username, Email = u.Email, Verified = u.Verified }; 
                    Group g = await Db.Groups.FindAsync(u.GroupId);
                    if (g != null)
                    {
                        result.Group = new GroupDTO() { Id = g.Id, Name = g.Name };
                    }
                    return Ok(result);
                }
            }
            return new BadRequestObjectResult("Invalid username/password supplied");
        }

        /// <summary>
        /// Get JWT-Token by Email
        /// </summary>
        /// <param name="email">The user email for login</param>
        /// <param name="password">The password for login in clear text</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid username/password supplied</response>
        [HttpPost]
        [Route("/authorize/byEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthorizeByEmail([FromQuery][Required()] string email, [FromQuery][Required()] string password)
        {
            List<Claim> claims = new List<Claim>();
            User u = await Db.Users.Where(i => i.Email.Equals(email)).FirstOrDefaultAsync();
            if (u == null)
            {
                return new StatusCodeResult(400);
            } else
            {
                string hash = u.Password;
                (bool Verified, bool NeedsUpgrade) = new PasswordHasher(new HashingOptions()).Check(hash, password);
                if (Verified)
                {
                    /** new way, just claim the user id and do permissioncheck in authorizationhandler **/
                    //************MOST IMPORTANT CLAIM FOR COREAUTHORIZATIONHANDLER***********
                    claims.Add(new Claim("userId", u.Id.ToString()));
                    //All Claims from grouppermission and userpermission collected, make us a token please.
                    string issuer = config["Authentification:Issuer"].ToString();
                    string audience = config["Authentification:Audience"].ToString();
                    byte[] secret = Encoding.UTF8.GetBytes(config["Authentification:Secret"].ToString());
                    SymmetricSecurityKey key = new SymmetricSecurityKey(secret);
                    string algorithm = SecurityAlgorithms.HmacSha256;
                    SigningCredentials signingCredientials = new SigningCredentials(key, algorithm);
                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer,
                        audience,
                        claims,
                        notBefore: DateTime.Now,
                        expires: DateTime.Now.AddDays(1),
                        signingCredientials
                        );
                    string tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
                    TokenDTO result = new TokenDTO() { access_token = tokenJson, Id = u.Id, Username = u.Username, Email = u.Email, Verified = u.Verified };
                    Group g = await Db.Groups.FindAsync(u.GroupId);
                    if (g != null)
                    {
                        result.Group = new GroupDTO() { Id = g.Id, Name = g.Name };
                    }
                    return Ok(result);
                }
            }
            return new BadRequestObjectResult("Invalid email/password supplied");
        }
    }
}
