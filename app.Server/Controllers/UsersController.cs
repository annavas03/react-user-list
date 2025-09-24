using app.Server.Data;
using app.Server.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _context.Users
                   .Include(u => u.UserRoles)
                       .ThenInclude(ur => ur.Role)
                   .OrderByDescending(u => u.CreatedAt) 
                   .ThenByDescending(u => u.Id);       

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserItemModel
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Image = u.Image,
                    LoginType = u.LoginType,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToArray()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                users
            });
        }
    }
}
