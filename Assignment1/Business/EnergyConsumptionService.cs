using Assignment1.Business.Interfaces;
using Assignment1.Models;
using Assignment1.Repository.Interfaces;
using Assignment1.ViewModels;
using System.Security.Claims;

namespace Assignment1.Business
{
    public class EnergyConsumptionService : IEnergyConsumptionService
    {
        private readonly IGenericRepository _genericRepository;
        private readonly IDeviceService _deviceService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EnergyConsumptionService(IGenericRepository genericRepository, IDeviceService deviceService, IHttpContextAccessor httpContextAccessor)
        {
            _genericRepository = genericRepository;
            _deviceService = deviceService;
            _httpContextAccessor = httpContextAccessor;
        }

        public EnergyConsumptionViewModel AddEnergyConsumption(EnergyConsumptionViewModel energyConsumption)
        {
            _genericRepository.Add<EnergyConsumption>(new EnergyConsumption { TimeStamp = energyConsumption.TimeStamp, Consumption = energyConsumption.Consumption, DeviceId = energyConsumption.DeviceId });
            _deviceService.UpdateMaxHourlyConsumption(energyConsumption.DeviceId, energyConsumption.Consumption);

            return energyConsumption;
        }

        public List<EnergyConsumptionViewModel> GetEnergyCosumptionForDevicePerDay(int deviceId, DateTime date)
        {
            var loggedUser = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("Admin");
            var isUsersDevice = _deviceService.GetAllDevicesForUser(Guid.Parse(loggedUser)).Exists(e => loggedUser.Equals(e.UserId) && deviceId.Equals(e.Id));
            var energyConsumptionForDay = _genericRepository.GetAll<EnergyConsumption>().FindAll(e => e.DeviceId.Equals(deviceId) && e.TimeStamp.Date.Equals(date));

            if(isAdmin || isUsersDevice)
            {
                return energyConsumptionForDay.Select(e => new EnergyConsumptionViewModel
                {
                    TimeStamp = e.TimeStamp,
                    Consumption = e.Consumption,
                    DeviceId = e.DeviceId,
                    Id = e.Id
                }).ToList();
            }

            return new List<EnergyConsumptionViewModel>();
        }
    }
}
