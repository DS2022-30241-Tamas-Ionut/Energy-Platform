using Assignment1.ViewModels;

namespace Assignment1.Business.Interfaces
{
    public interface IEnergyConsumptionService
    {
        List<EnergyConsumptionViewModel> GetEnergyCosumptionForDevicePerDay(int deviceId, DateTime date);
        EnergyConsumptionViewModel AddEnergyConsumption(EnergyConsumptionViewModel energyConsumption);
    }
}
