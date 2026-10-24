
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApp.Models;
using MyFirstApp;

public class SchoolController : Controller
{
    private readonly AppDbContext _context;

    public SchoolController(AppDbContext context)
    {
        _context = context;
    }

    // GET: SCHOOLS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Schools.ToListAsync());
    }

    // GET: SCHOOLS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var school = await _context.Schools
            .FirstOrDefaultAsync(m => m.Id == id);
        if (school == null)
        {
            return NotFound();
        }

        return View(school);
    }

    // GET: SCHOOLS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SCHOOLS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Domain,Students")] School school)
    {
        if (ModelState.IsValid)
        {
            _context.Add(school);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(school);
    }

    // GET: SCHOOLS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var school = await _context.Schools.FindAsync(id);
        if (school == null)
        {
            return NotFound();
        }
        return View(school);
    }

    // POST: SCHOOLS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Domain,Students")] School school)
    {
        if (id != school.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(school);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SchoolExists(school.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(school);
    }

    // GET: SCHOOLS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var school = await _context.Schools
            .FirstOrDefaultAsync(m => m.Id == id);
        if (school == null)
        {
            return NotFound();
        }

        return View(school);
    }

    // POST: SCHOOLS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var school = await _context.Schools.FindAsync(id);
        if (school != null)
        {
            _context.Schools.Remove(school);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SchoolExists(int? id)
    {
        return _context.Schools.Any(e => e.Id == id);
    }
}
