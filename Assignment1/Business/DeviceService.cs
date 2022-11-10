using Assignment1.Business.Interfaces;
using Assignment1.Models;
using Assignment1.Repository.Interfaces;
using Assignment1.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Assignment1.Business
{
    public class DeviceService : IDeviceService
    {
        private readonly IGenericRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public DeviceService(IGenericRepository repository, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public DeviceViewModel CreateDevice(DeviceViewModel device)
        {
            _repository.Add<Device>(new Device { Address = device.Address, Description = device.Description, MaxHourlyConsumption = device.MaxHourlyConsumption, UserId = device.UserId});

            return device;
        }

        public DeviceViewModel DeleteDevice(int id)
        {
            var device = _repository.Get<Device>(id);
            
            if(device == null)
            {
                return null;
            }
            
            _repository.Delete<Device>(device);
            return new DeviceViewModel { Address = device.Address, Description = device.Description, Id = device.Id, MaxHourlyConsumption = device.MaxHourlyConsumption, UserId = device.UserId};
        }

        public DeviceViewModel GetDevice(int id)
        {
            var device = _repository.Get<Device>(id);

            return new DeviceViewModel { Address = device.Address, Description = device.Description, Id = device.Id, MaxHourlyConsumption = device.MaxHourlyConsumption, UserId = device.UserId };
        }

        public List<DeviceViewModel> GetDevices()
        {
            return _repository.GetAll<Device>().Select(d => new DeviceViewModel
            {
                Address = d.Address,
                Description = d.Description,
                Id = d.Id,
                MaxHourlyConsumption = d.MaxHourlyConsumption,
                UserId = d.UserId
            }).ToList();
        }

        public DeviceViewModel UpdateDevice(DeviceViewModel device)
        {
            var existingDevice = _repository.Get<Device>(device.Id);
            
            if(existingDevice == null)
            {
                return null;
            }

            if(device.MaxHourlyConsumption > existingDevice.MaxHourlyConsumption)
            {
                existingDevice.MaxHourlyConsumption = device.MaxHourlyConsumption;
            }

            existingDevice.Address = device.Address;
            existingDevice.Description = device.Description;
            _repository.Update<Device>(existingDevice);

            return device;
        }

        public DeviceViewModel DesignateUserToDevice(int deviceId, Guid userId)
        {
            var existingDevice = _repository.Get<Device>(deviceId);

            if(existingDevice == null)
            {
                return null;
            }

            existingDevice.UserId = userId.ToString();
            _repository.Update<Device>(existingDevice);

            return new DeviceViewModel { Address = existingDevice.Address, Description = existingDevice.Description, Id = existingDevice.Id, MaxHourlyConsumption = existingDevice.MaxHourlyConsumption, UserId = existingDevice.UserId };
        }

        public DeviceViewModel RemoveUserOwnershipOfDevice(int deviceId)
        {
            var existingDevice = _repository.Get<Device>(deviceId);

            if(existingDevice == null)
            {
                return null;
            }

            var none = _userManager.FindByNameAsync("none").Result;

            existingDevice.UserId = none.Id;
            _repository.Update<Device>(existingDevice);

            return new DeviceViewModel { Address = existingDevice.Address, Description = existingDevice.Description, Id = existingDevice.Id, MaxHourlyConsumption = existingDevice.MaxHourlyConsumption, UserId = existingDevice.UserId };
        }

        public List<DeviceViewModel> GetAllDevicesForUser(Guid userId)
        {
            var devices = _repository.GetAll<Device>();

            if(devices == null)
            {
                return new List<DeviceViewModel>();
            }

            var loggedUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");

            if (loggedUserId.Equals(userId.ToString()) || isAdmin)
            {
                return devices.FindAll(e => e.UserId.ToString().Equals(userId.ToString())).Select(d => new DeviceViewModel
                {
                    Address = d.Address,
                    Description = d.Description,
                    Id = d.Id,
                    MaxHourlyConsumption = d.MaxHourlyConsumption,
                    UserId = d.UserId
                }).ToList();
            }

            return new List<DeviceViewModel>();
        }

        public DeviceViewModel UpdateMaxHourlyConsumption(int deviceId, double consumption)
        {
            var device = _repository.Get<Device>(deviceId);

            if(device == null)
            {
                return null;
            }

            if(consumption > device.MaxHourlyConsumption)
            {
                device.MaxHourlyConsumption = consumption;
                _repository.Update<Device>(device);

                return new DeviceViewModel { Address = device.Address, Description = device.Description, Id = device.Id, MaxHourlyConsumption = consumption, UserId = device.UserId };

            }

            return new DeviceViewModel { Address = device.Address, Description = device.Description, Id = device.Id, MaxHourlyConsumption = device.MaxHourlyConsumption, UserId = device.UserId };
        }
    }
}
