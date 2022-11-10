using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double MaxHourlyConsumption { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

    }
}
