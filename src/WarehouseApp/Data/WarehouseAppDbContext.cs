using System.Data.Entity;
using System.Diagnostics;

namespace WarehouseApp.Data
{
    public class WarehouseAppDbContext : DbContext
    {
        public WarehouseAppDbContext()
            : base("name=WarehouseAppDbContext")
        {
#if DEBUG
            Database.Log = message => Debug.WriteLine(message);
#endif
        }

        public virtual DbSet<Customer> Customers { get; set; }
    }
}
