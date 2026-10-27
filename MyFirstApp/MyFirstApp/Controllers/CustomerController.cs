using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyFirstApp.Models;
using MyFirstApp.ViewModels;

namespace MyFirstApp.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class CustomerController : Controller
    {
        private const string CustomerSessionKey = "CustomerId";
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Customer> _passwordHasher;

        public CustomerController(AppDbContext context, IPasswordHasher<Customer> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32(CustomerSessionKey).HasValue)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View(new CustomerRegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerRegisterViewModel model)
        {
            if (HttpContext.Session.GetInt32(CustomerSessionKey).HasValue)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim().ToLowerInvariant();
            if (await _context.Customers.AnyAsync(item => item.Email == email))
            {
                ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
                return View(model);
            }

            var customer = new Customer
            {
                Name = model.Name.Trim(),
                Email = email
            };
            customer.PasswordHash = _passwordHasher.HashPassword(customer, model.Password);
            _context.Customers.Add(customer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
                when (exception.InnerException is SqlException { Number: 2601 or 2627 })
            {
                ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Your account has been created. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32(CustomerSessionKey).HasValue)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View(new CustomerLoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(CustomerLoginViewModel model)
        {
            if (HttpContext.Session.GetInt32(CustomerSessionKey).HasValue)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim().ToLowerInvariant();
            var customer = await _context.Customers.SingleOrDefaultAsync(item => item.Email == email);
            var verification = customer == null
                ? PasswordVerificationResult.Failed
                : _passwordHasher.VerifyHashedPassword(customer, customer.PasswordHash, model.Password);

            if (customer == null || verification == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            if (verification == PasswordVerificationResult.SuccessRehashNeeded)
            {
                customer.PasswordHash = _passwordHasher.HashPassword(customer, model.Password);
                await _context.SaveChangesAsync();
            }

            // Keep only the customer ID in session, never the password or password hash.
            HttpContext.Session.Clear();
            HttpContext.Session.SetInt32(CustomerSessionKey, customer.Id);
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var customerId = HttpContext.Session.GetInt32(CustomerSessionKey);
            if (!customerId.HasValue)
            {
                return RedirectToAction(nameof(Login));
            }

            var customer = await _context.Customers
                .AsNoTracking()
                .Where(item => item.Id == customerId.Value)
                .Select(item => new CustomerDashboardViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email
                })
                .SingleOrDefaultAsync();

            if (customer == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
