using Assignment1.Business.Interfaces;
using Assignment1.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyConsumptionController : ControllerBase
    {
        private readonly IEnergyConsumptionService _energyConsumptionService;

        public EnergyConsumptionController(IEnergyConsumptionService energyConsumptionService)
        {
            _energyConsumptionService = energyConsumptionService;
        }

        [HttpGet("GetEnergyConsumption/{deviceId}&{date}")]
        public IActionResult GetEnergyConsumption([FromRoute] int deviceId, DateTime date)
        {
            var energyConsumptionList = _energyConsumptionService.GetEnergyCosumptionForDevicePerDay(deviceId, date);

            if (energyConsumptionList.Count == 0)
            {
                return NotFound("No data available");
            }

            return Ok(energyConsumptionList);
        }

        [HttpPost("AddEnergyConsumption")]
        public IActionResult AddEnergyConsumption([FromBody] EnergyConsumptionViewModel energyConsumption)
        {
            var energyConsumptionAdded = _energyConsumptionService.AddEnergyConsumption(energyConsumption);

            if (energyConsumptionAdded == null)
            {
                return BadRequest($"Could not add consumption for device {energyConsumption.Id}");
            }

            return Ok(energyConsumptionAdded);
        }
    }
}
