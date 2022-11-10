using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public class EnergyConsumption
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Consumption { get; set; }
        [ForeignKey("Device")]
        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }
    }
}
