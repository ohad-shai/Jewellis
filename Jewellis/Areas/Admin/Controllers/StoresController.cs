﻿using Jewellis.App_Custom.ActionFilters;
using Jewellis.Areas.Admin.ViewModels.Stores;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StoresController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public StoresController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /admin/stores
        public async Task<IActionResult> Index(string query)
        {
            List<Branch> branches = await _dbContext.Branches.Where(b => (query == null) || b.Name.Contains(query)).OrderBy(b => b.Name).ToListAsync();
            ViewData["SearchQuery"] = query;
            return View(branches);
        }

        // GET: /admin/stores/details/{id}
        public async Task<IActionResult> Details(int id)
        {
            Branch branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
                return NotFound();
            else
                return View(branch);
        }

        // GET: /admin/stores/create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /admin/stores/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Branch branch = new Branch()
            {
                Name = model.Name,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                OpeningHours = model.OpeningHours,
                LocationLatitude = model.LocationLatitude,
                LocationLongitude = model.LocationLongitude
            };
            _dbContext.Branches.Add(branch);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /admin/stores/edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            Branch branch = await _dbContext.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = branch.Id,
                    CurrentName = branch.Name,
                    Name = branch.Name,
                    Address = branch.Address,
                    PhoneNumber = branch.PhoneNumber,
                    OpeningHours = branch.OpeningHours,
                    LocationLatitude = branch.LocationLatitude,
                    LocationLongitude = branch.LocationLongitude
                });
        }

        // POST: /admin/stores/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            Branch branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
                return NotFound();

            // Binds the view model:
            branch.Name = model.Name;
            branch.Address = model.Address;
            branch.PhoneNumber = model.PhoneNumber;
            branch.OpeningHours = model.OpeningHours;
            branch.LocationLatitude = model.LocationLatitude;
            branch.LocationLongitude = model.LocationLongitude;
            branch.DateLastModified = DateTime.Now;

            _dbContext.Branches.Update(branch);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /admin/stores/delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            Branch branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
                return NotFound();
            else
                return View(branch);
        }

        // POST: /admin/stores/delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            Branch branch = await _dbContext.Branches.FindAsync(id);
            _dbContext.Branches.Remove(branch);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.Branches.AnyAsync(b => b.Name.Equals(name)) == false);
            return Json(isNameAvailable);
        }

        [AjaxOnly]
        public async Task<JsonResult> CheckNameEditAvailability(string name, string currentName)
        {
            // Checks if the name did not change in the edit:
            if (string.Equals(name, currentName, StringComparison.OrdinalIgnoreCase))
            {
                return Json(true);
            }
            // Otherwise, name was changed so checks availability:
            else
            {
                bool isNameAvailable = (await _dbContext.Branches.AnyAsync(b => b.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
