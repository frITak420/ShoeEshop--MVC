using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using Web.Models;

namespace Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyDbContext _context;
        
        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Products

        public async Task<IActionResult> Index()
        {
            ViewBag.Photos = _context.Photos.ToList();
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,ProductDetailName,Price,OldPrice,InfoText,InfoText2,Type,Material,Sex,ProductCode")] Product product,IFormFile photoMain,IList<IFormFile>photosDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                Directory.CreateDirectory($"wwwroot/images/Product{product.Id}");
                SavePhotos(photoMain, photosDetail, product.Id);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Photos = _context.Photos.ToList();
            if (id == null || _context.Products == null)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,ProductDetailName,Price,OldPrice,InfoText,InfoText2,Type,Material,Sex,ProductCode")] Product product)
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'MyDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                foreach (Photo photo in _context.Photos.ToList())
                {
                    if (photo.ProductId == id)
                    {
                        System.IO.File.Delete(photo.ImgPath);
                        _context.Photos.Remove(photo);
                        _context.SaveChanges();
                    }
                }
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


       

        //--------------------------------------práce s fotkama----------------------------------------

        private void SavePhotosXX(IFormFile photoMain, IList<IFormFile> photosDetail, int idProduct)
        {
            Random random = new Random();
            int randomNumber = random.Next(100, 999);
            if (photoMain !=null)
            {
                var photoMainPath = Path.Combine("wwwroot/images/mainPhotos", $"product{idProduct}mainPhoto.jpg");
                if (System.IO.File.Exists(photoMainPath))
                {
                    photoMainPath = Path.Combine("wwwroot/images/mainPhotos", $"product{idProduct}mainPhoto{randomNumber}.jpg");
                }
                using (Stream stream = new FileStream(photoMainPath, FileMode.Create))
                {
                    photoMain.CopyTo(stream);
                    _context.Add(new Photo { ImgPath = photoMainPath, ProductId = idProduct, Priority = 1 });
                }
            }

            if (photosDetail.Count >=1)
            {
                int i = 1;
                foreach (IFormFile item in photosDetail)
                {
                    var photoDetailPath = Path.Combine("wwwroot/images/detailPhotos", $"product{idProduct}detailPhoto{i}.jpg");
                    if (System.IO.File.Exists(photoDetailPath))
                    {
                        photoDetailPath = Path.Combine("wwwroot/images/detailPhotos", $"product{idProduct}detailPhoto{i + randomNumber}.jpg");
                    }

                    using (Stream stream = new FileStream(photoDetailPath, FileMode.Create))
                    {
                        item.CopyTo(stream);
                        _context.Add(new Photo { ImgPath = photoDetailPath, ProductId = idProduct, Priority = 2 });
                    }

                    i++;
                }
            }
            

        }
        public void SavePhotos(IFormFile photoMain, IList<IFormFile> photosDetail, int idProduct)
        {
            Guid guid;
            if (photoMain != null)
            {
                guid = new Guid();
                var path = Path.Combine($"wwwroot/images/Product{idProduct}", $"{guid}.jpg");
              
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    photoMain.CopyTo(stream);
                    _context.Add(new Photo { ImgPath = path, ProductId = idProduct, Priority = 1 });
                }
            }
            if (photosDetail.Count >= 1)
            {
                foreach (IFormFile item in photosDetail)
                {
                   guid = Guid.NewGuid();
                    var path = Path.Combine($"wwwroot/images/Product{idProduct}", $"{guid}.jpg");

                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        item.CopyTo(stream);
                        _context.Add(new Photo { ImgPath = path, ProductId = idProduct, Priority = 2 });
                    }
                }
            }
        }
        public IActionResult DeletePhoto(int id, int idProduct)
        {
            Photo del = _context.Photos.First(e => e.Id == id);
            System.IO.File.Delete(del.ImgPath);
            _context.Remove(del);
            _context.SaveChanges();
            return RedirectToAction("Photos", "Products", new { id = idProduct });

        }

        //GET
        public async Task<IActionResult> Photos(int? id)
        {
            ViewBag.Photos = _context.Photos.ToList();
            ViewBag.Id = id;
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Photos(int id, IFormFile photoMain, IList<IFormFile> photosDetail)
        {
            SavePhotos(photoMain, photosDetail, id);
            _context.SaveChanges();
            return RedirectToAction("Photos", "Products", new { id });
        }

        public IActionResult SetMainPhoto(int id, int idPhoto)
        {
            _context.Products.Include(x => x.Photos).ToList();
            var product = _context.Products.Find(id);

            foreach (Photo p in product.Photos)
            {
                if (p.Id == idPhoto)
                {
                    p.Priority = 1;
                }
                else
                {
                    p.Priority=2;
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Photos", "Products", new { id });
        }

       
    }
}
