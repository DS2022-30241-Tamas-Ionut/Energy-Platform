using Assignment1.Business.Interfaces;
using Assignment1.Models;
using Assignment1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;

        public DeviceController(IDeviceService deviceService, IUserService userService)
        {
            _deviceService = deviceService;
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetDevices")]
        public IActionResult GetDevices()
        {
            var devices = _deviceService.GetDevices();

            return devices == null || devices.Count == 0 ? NotFound(new List<DeviceViewModel>()) : Ok(devices);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetDevice/{id}")]
        public IActionResult GetDevice([FromRoute] int id)
        {
            var device = _deviceService.GetDevice(id);

            return device == null ? NotFound("The device does not exist") : Ok(device);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("GetDevicesForUser/{user}")]
        public IActionResult GetDevicesForUser([FromRoute] string user)
        {
            var userId = _userService.GetIdFromUsername(user);

            if (userId == Guid.Empty)
            {
                return NotFound("User does not exist");
            }

            var devices = _deviceService.GetAllDevicesForUser(userId);

            if (devices.Count == 0)
            {
                return NoContent();
            }

            return Ok(devices);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateDevice")]
        public IActionResult CreateDevice([FromBody] DeviceViewModel device)
        {
            var createdDevice = _deviceService.CreateDevice(device);

            return createdDevice == null ? BadRequest("Could not create device") : Ok(device);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteDevice/{id}")]
        public IActionResult DeleteDevice([FromRoute] int id)
        {
            var device = _deviceService.DeleteDevice(id);

            return device == null ? NotFound("Device not found") : Ok(device);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateDevice")]
        public IActionResult UpdateDevice([FromBody] DeviceViewModel device)
        {
            var existingDevice = _deviceService.UpdateDevice(device);

            return existingDevice == null ? NotFound("Device not found") : Ok(device);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("DesignateUserToDevice")]
        public IActionResult DesignateUserToDevice([FromBody] UserToDeviceModel userToDevice)
        {
            var userId = _userService.GetIdFromUsername(userToDevice.UserName);

            if (userId == Guid.Empty)
            {
                return NotFound("User does not exist");
            }

            var device = _deviceService.DesignateUserToDevice(userToDevice.DeviceId, userId);
            if (device == null)
            {
                return NotFound("Device does not exist");
            }

            return Ok(device);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RemoveUserOwnershipOfDevice/{id}")]
        public IActionResult RemoveUserOwnershipOfDevice([FromRoute] int deviceId)
        {
            var device = _deviceService.RemoveUserOwnershipOfDevice(deviceId);

            if (device == null)
            {
                return NotFound("Device does not exist");
            }

            return Ok(device);
        }
    }
}
