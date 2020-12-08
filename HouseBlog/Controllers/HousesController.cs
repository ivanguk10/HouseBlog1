using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HouseBlog.Models;
using HouseBlog.Service;

namespace HouseBlog.Controllers
{
    public class HousesController : Controller
    {
        private readonly BlogContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly ImageService _imageService;
        public HousesController(BlogContext context, IWebHostEnvironment appEnvironment, ImageService imageService)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _imageService = imageService;
        }



        public async Task<IActionResult> Index()
        {
            return View(await _context.Houses.ToListAsync());
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.Houses
                .FirstOrDefaultAsync(m => m.HouseId == id);
            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }


        public IActionResult Create()
        {
            return View();
        }



        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HouseID,HouseName,HouseDescription")] House house, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadedFile != null && uploadedFile.ContentType.ToLower().Contains("image"))
                {

                    house.HouseImage = await _imageService.SaveImageAsync(uploadedFile, 0);
                    _context.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("Image", "Некорректный формат");
                    return View(house);
                }
                _context.Add(house);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(house);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.Houses.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }
            return View(house);
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HouseId,HouseName,HouseDescription,HouseImage")] House house, IFormFile uploadedFile)
        {
            if (id != house.HouseId)
            {
                return NotFound();
            }

            House house1 = await _context.Houses
                .FirstOrDefaultAsync(m => m.HouseId == id);
            if (ModelState.IsValid)
            {
                try
                {
                    if (house.HouseDescription != house1.HouseDescription && house1.HouseDescription != null)
                        house1.HouseDescription = house.HouseDescription;
                    if (house.HouseName != house1.HouseName && house1.HouseName != null)
                        house1.HouseName = house.HouseName;
                    if (uploadedFile != null && uploadedFile.ContentType.ToLower().Contains("image"))
                    {
                        house1.HouseImage = await _imageService.SaveImageAsync(uploadedFile, 0);
                    }
                    _context.Update(house1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseExists(house.HouseId))
                    {
                        return RedirectPermanent("~/Error/Index?statusCode=404");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectPermanent("~/Houses");
            }
            return View(house);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.Houses
                .FirstOrDefaultAsync(m => m.HouseId == id);
            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseExists(int id)
        {
            return _context.Houses.Any(e => e.HouseId == id);
        }
    }
}