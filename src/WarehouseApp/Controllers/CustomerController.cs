using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WarehouseApp.Data;

namespace WarehouseApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly WarehouseAppDbContext _context = new WarehouseAppDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View(_context.Customers);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var employee = _context.Customers.FirstOrDefault(e => e.ID == id);
            return View(employee);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return Details(id);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, Customer customerUpdate)
        {
            var customer = _context.Customers.FirstOrDefault(e => e.ID == id);
            customer.FirstName = customerUpdate.FirstName;
            customer.LastName = customerUpdate.LastName;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            return Details(id);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Customer customer)
        {
            var customerToDelete = _context.Customers.FirstOrDefault(e => e.ID == customer.ID);
            _context.Customers.Remove(customerToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
