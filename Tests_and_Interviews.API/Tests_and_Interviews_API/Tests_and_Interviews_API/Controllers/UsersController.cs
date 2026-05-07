namespace Tests_and_Interviews_API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Tests_and_Interviews_API.Dtos;
    using Tests_and_Interviews_API.Mappers;
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            this._service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            User? user = await this._service.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.ToDto());
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            List<User> users = await this._service.GetAllAsync();

            return Ok(users.Select(u => u.ToDto()).ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] UserDto dto)
        {
            await this._service.AddAsync(dto.ToEntity());

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UserDto dto)
        {
            User user = dto.ToEntity();
            user.Id = id;
            await this._service.UpdateAsync(user);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await this._service.DeleteAsync(id);

            return Ok(new { message = "User deleted successfully" });
        }
    }
}