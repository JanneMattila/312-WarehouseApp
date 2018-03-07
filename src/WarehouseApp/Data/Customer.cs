using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Data
{
    public class Customer
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }
    }
}
