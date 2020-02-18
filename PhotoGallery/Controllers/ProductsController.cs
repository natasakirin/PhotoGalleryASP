using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Models;

namespace PhotoGallery.Controllers
{

    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Products.ToListAsync());
        //}


        // Start: ---------------------SEARCHING TITLE & DESCRIPTION + SORTING AUTHOR, PRICE, DIMENSIONS and GENRE -------------
        [Authorize]
        public async Task<IActionResult> Index(
                                        string sortOrder,
                                        string currentFilter,
                                        string searchString,
                                        int? pageNumber)
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["AuthorSortParm"] = sortOrder == "Author" ? "author_desc" : "Author";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["DimensionsSortParm"] = sortOrder == "Dimensions" ? "dimensions_desc" : "Dimensions";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;


            var photos = from p in _context.Products
                        select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                photos = photos.Where(p => p.Title.Contains(searchString)
                                       || p.Description.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "author_desc":
                    photos = photos.OrderByDescending(p => p.Author);
                    break;
                case "Price":
                    photos = photos.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    photos = photos.OrderByDescending(p => p.Price);
                    break;
                case "Author":
                    photos = photos.OrderBy(p => p.Author);
                    break;
                case "Genre":
                    photos = photos.OrderBy(p => p.Genre);
                    break;
                case "genre_desc":
                    photos = photos.OrderByDescending(p => p.Genre);
                    break;
                case "Dimensions":
                    photos = photos.OrderBy(p => p.Dimensions);
                    break;
                case "dimensions_desc":
                    photos = photos.OrderByDescending(p => p.Dimensions);
                    break;
                default:
                    photos = photos.OrderBy(p => p.Id);
                    break;
            }
            int pageSize = 9;
            return View(await PaginatedList<Product>.CreateAsync(photos.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        // End: ---------------------SEARCHING TITLE & DESCRIPTION + SORTING AUTHOR, PRICE, DIMENSIONS and GENRE -------------




        // GET: Products/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }




        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,Dimensions,Price,Photo,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre,Dimensions,Price,Photo,Description")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }



        //Gallery - for unregistred users

        public async Task<IActionResult> Gallery()
        {
            return View(await _context.Products.ToListAsync());
        }

    }
}
