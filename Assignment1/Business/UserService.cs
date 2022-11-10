using Assignment1.Business.Interfaces;
using Assignment1.Models;
using Microsoft.AspNetCore.Identity;

namespace Assignment1.Business
{
    public class UserService : IUserService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDeviceService _deviceService;

        public UserService(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IDeviceService deviceService)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._httpContextAccessor = httpContextAccessor;
            this._deviceService = deviceService;
        }

        public async Task<IdentifyUser> Register(RegisterUser newUser)
        {
            try
            {
                var roleFindResult = await _roleManager.FindByNameAsync("User");
                if (roleFindResult == null) return null;

                if (newUser != null)
                {
                    var user = new User
                    {
                        UserName = newUser.Username,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Email = newUser.Username,
                        Address = newUser.Address
                    };

                    var result = await _userManager.CreateAsync(user, newUser.Password);
                    if (result != IdentityResult.Success)
                    {
                        return null;
                    }

                    var registeredUser = await _userManager.FindByNameAsync(user.UserName);
                    var roleAddResult = await _userManager.AddToRoleAsync(registeredUser, "User");
                    if (roleAddResult != IdentityResult.Success)
                    {
                        return null;
                    }

                    var roles = new List<string>();
                    roles.Add(roleFindResult.Name);

                    var createdUser = new IdentifyUser
                    {
                        Id = registeredUser.Id,
                        Username = newUser.Username,
                        Address = newUser.Address,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Roles = roles
                    };
                    return createdUser;
                }
                else
                {

                    return null;

                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public async Task<string> Login(LoginUser user)
        {
            try
            {
                if (user != null)
                {
                    var existingUser = await _userManager.FindByNameAsync(user.Username);

                    if (existingUser != null)
                    {
                        var result = await _signInManager.PasswordSignInAsync(existingUser, user.Password, false, false);

                        if (result.Succeeded)
                        {

                            var userRoles = await _userManager.GetRolesAsync(existingUser);
                            var rolesList = new List<string>();
                            foreach (var role in userRoles)
                            {
                                var roleEntity = await _roleManager.FindByNameAsync(role);
                                rolesList.Add(roleEntity.Name);
                            }

                            var returnUrl = "";

                            if (userRoles.Contains("Admin"))
                            {
                                returnUrl = "/AdminView";
                            }
                            else
                            {
                                returnUrl = "/UserView";
                            }

                            return returnUrl;

                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<IdentifyUser>> GetUsers()
        {
            return _userManager.Users.Select(u => new IdentifyUser
            {
                Id = u.Id,
                Address = u.Address,
                Username = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Roles = _userManager.GetRolesAsync(u).Result.ToList()
            }).ToList();
        }

        public IdentifyUser GetUser(Guid id)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id.Equals(id.ToString()));

            return user == null ? null : new IdentifyUser
            {
                Id = user.Id,
                Address = user.Address,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = _userManager.GetRolesAsync(user).Result.ToList()
            };
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if(user == null)
            {
                return false;
            }

            var devices = _deviceService.GetAllDevicesForUser(id);

            if(devices.Count > 0) {
                var none = await _userManager.FindByNameAsync("none");

                foreach (var device in devices)
                {
                    _deviceService.DesignateUserToDevice(device.Id, Guid.Parse(none.Id));
                }
            }

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<IdentifyUser> UpdateUser(IdentifyUser user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            
            if(existingUser == null)
            {
                return null;
            }

            existingUser.Address = user.Address;
            existingUser.UserName = user.Username;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;

            var res = await _userManager.UpdateAsync(existingUser);

            if (!res.Succeeded)
            {
                return null;
            }

            return new IdentifyUser
            {
                Id = existingUser.Id,
                Address = user.Address,
                Username = user.Username,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName
            };
        }

        public string GetUserName(Guid id)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id.Equals(id.ToString()));
            return user == null ? "??" : user.UserName;
        }

        public Guid GetIdFromUsername(string username)
        {
            var user = _userManager.Users.FirstOrDefault(u => username.Equals(u.UserName));

            return user == null ? Guid.Empty : Guid.Parse(user.Id);
        }

        public async Task<IdentifyUser> GetLoggedInUser()
        {
            var loggedUsername = _httpContextAccessor.HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(loggedUsername);

            return new IdentifyUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Username = user.UserName,
                Roles = _userManager.GetRolesAsync(user).Result.ToList()
            };
        }
    }
}
