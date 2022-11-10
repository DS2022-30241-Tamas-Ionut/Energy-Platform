using Assignment1.Models;
using Assignment1.ViewModels;

namespace Assignment1.Business.Interfaces
{
    public interface IDeviceService
    {
        DeviceViewModel GetDevice(int id);
        List<DeviceViewModel> GetDevices();
        DeviceViewModel UpdateDevice(DeviceViewModel device);
        DeviceViewModel DeleteDevice(int id);
        DeviceViewModel CreateDevice(DeviceViewModel device);
        DeviceViewModel DesignateUserToDevice(int deviceId, Guid userId);
        DeviceViewModel RemoveUserOwnershipOfDevice(int deviceId);
        List<DeviceViewModel> GetAllDevicesForUser(Guid userId);
        DeviceViewModel UpdateMaxHourlyConsumption(int deviceId, double consumption);
    }
}
