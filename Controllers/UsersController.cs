using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Examinationsuppgift.Entities;
using WebAPI_Examinationsuppgift.Filters;
using WebAPI_Examinationsuppgift.Models;

namespace WebAPI_Examinationsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UseApiKey]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(CreateUserModel model)
        {
            User user = new User
            {
                Address = model.Address,
                City = model.City,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                PostalCode = model.PostalCode,

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new OkObjectResult($"{model.FirstName} {model.LastName} has been created sucessfully!");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers() => await _context.Users.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUserById(CreateUserModel model, int id)
        {
            var user = await _context.Users.FindAsync(id);

            if(user == null)
            {
                return BadRequest();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Password = model.Password;
            user.Email = model.Email;
            user.Address = model.Address;
            user.PostalCode = model.PostalCode;
            user.City = model.City;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new OkObjectResult(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
