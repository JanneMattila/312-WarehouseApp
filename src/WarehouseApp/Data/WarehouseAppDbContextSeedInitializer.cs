using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace WarehouseApp.Data
{
    public class WarehouseAppDbContextSeedInitializer : DropCreateDatabaseIfModelChanges<WarehouseAppDbContext>
    {
        protected override void Seed(WarehouseAppDbContext context)
        {
            context.Database.Log = message => Debug.WriteLine(message);

            if (context.Customers.Count() == 0)
            {
                // No customers so let's create some data
                var random = new Random();
                var firstNames = new string[] { "John", "Jane", "Mike", "Michael", "David", "Sal", "Sonja", "Alf", "Greg" };
                var lastNames = new string[] { "Doe", "Mathews", "Clinton", "Winter", "Autumn", "Rogers", "Moore", "Alfonson", };

                for (int i = 0; i < 100; i++)
                {
                    var customer = new Customer();
                    customer.ID = i;
                    customer.FirstName = firstNames[random.Next(firstNames.Length)];
                    customer.LastName = lastNames[random.Next(lastNames.Length)];
                    context.Customers.Add(customer);
                }

                context.SaveChanges();
            }

            base.Seed(context);
        }
    }
}
