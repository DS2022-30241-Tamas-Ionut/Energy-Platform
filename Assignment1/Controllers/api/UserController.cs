using Assignment1.Business.Interfaces;
using Assignment1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        public UsersController(SignInManager<User> signInManager,
                               RoleManager<IdentityRole> roleManager,
                               IUserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
            _roleManager = roleManager;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUser user)
        {
            var result = await _userService.Login(user);
            return result == null ? NotFound("Wrong credentials") : Ok(result);
        }

        [HttpPost("Logout")]
        [Authorize]
        public async void Logout()
        {
            await _signInManager.SignOutAsync();
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUser user)
        {
            var result = await _userService.Register(user);
            return result == null ? BadRequest("Error while creating user") : Ok(result);
        }

        [HttpGet("GetUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<List<IdentifyUser>> GetUsers()
        {
            var result = await _userService.GetUsers();
            return result;
        }

        [HttpGet("GetUser/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUser([FromRoute] string id)
        {
            var user = _userService.GetUser(Guid.Parse(id));
            return user == null ? NotFound() : Ok(user);
        }

        [HttpDelete("DeleteUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            var result = await _userService.DeleteUser(Guid.Parse(id));
            return result ? Ok(id.ToString()) : NotFound("The user does not exist!");
        }

        [HttpPut("UpdateUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser([FromBody] IdentifyUser user)
        {
            var res = await _userService.UpdateUser(user);
            return res == null ? NotFound("The user does not exist") : Ok(res);
        }

        [HttpPost("CreateRole")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> CreateRole([FromBody] Role roleModel)
        {
            var newRole = new IdentityRole(roleModel.RoleName);

            var result = await _roleManager.CreateAsync(newRole);
            return result != null;
        }

        [HttpGet("Roles")]
        [Authorize(Roles = "Admin")]
        public async Task<List<Role>> GetRoles()
        {
            var rolesList = new List<Role>();
            foreach (var role in _roleManager.Roles.ToList())
            {
                var roleModel = new Role
                {
                    Id = role.Id,
                    RoleName = role.Name
                };
                rolesList.Add(roleModel);
            }
            return rolesList;
        }

        [HttpGet("GetLoggedUser")]
        [Authorize]
        public async Task<IActionResult> GetLoggedUser()
        {
            var user = await _userService.GetLoggedInUser();

            return user == null ? NotFound("Session expired. Please log back in!") : Ok(user);
        }

        [HttpGet("GetUsername/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsername([FromRoute]string id)
        {
            var response = _userService.GetUserName(Guid.Parse(id));

            return String.IsNullOrEmpty(response) ? NotFound() : Ok(response);
        }
    }
}
