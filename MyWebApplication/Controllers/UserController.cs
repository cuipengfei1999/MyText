using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Contexts;
using MyWebApplication.Services;
using NuGet.Protocol;

namespace MyWebApplication.Controllers
{
    /// <summary>
    /// UserController
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public UserController(DataContext context, ILogger<WeatherForecastController> logger)
        {
            _userService = new UserService(context);
            _logger = logger;
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost("AddUsers")]
        public async Task<int> AddUsers([FromBody] List<User> para)
        {
           return await _userService.AddUsers(para);
        }

        /// <summary>
        /// GET
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUsers")]
        public async Task<List<User>> GetDbUser()
        {
            return await _userService.GetDbUser();
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteUsers")]
        public async Task<int> DeleteUsers()
        {
            return await _userService.DeleteUsers();
        }

    }
}
