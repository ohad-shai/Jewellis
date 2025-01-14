﻿using Jewellis.App_Custom.ActionFilters;
using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using Jewellis.Areas.Admin.ViewModels.ProductTypes;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductTypesController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public ProductTypesController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/ProductTypes
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<ProductType> types = await _dbContext.ProductTypes
                .Where(t => (model.Query == null) || t.Name.Contains(model.Query))
                .OrderBy(t => t.Name)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(types.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    types = types
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["ProductTypesModel"] = types;
            return View(model);
        }

        // GET: /Admin/ProductTypes/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            ProductType types = await _dbContext.ProductTypes.FirstOrDefaultAsync(t => t.Id == id);
            if (types == null)
                return NotFound();
            else
                return View(types);
        }

        // GET: /Admin/ProductTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/ProductTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ProductType type = new ProductType()
            {
                Name = model.Name
            };
            _dbContext.ProductTypes.Add(type);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ProductTypes/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            ProductType type = await _dbContext.ProductTypes.FindAsync(id);
            if (type == null)
                return NotFound();
            else
                return View(new EditVM()
                {
                    Id = type.Id,
                    CurrentName = type.Name,
                    Name = type.Name
                });
        }

        // POST: /Admin/ProductTypes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVM model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            ProductType type = await _dbContext.ProductTypes.FirstOrDefaultAsync(c => c.Id == id);
            if (type == null)
                return NotFound();

            // Binds the view model:
            type.Name = model.Name;
            type.DateLastModified = DateTime.Now;

            _dbContext.ProductTypes.Update(type);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/ProductTypes/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            ProductType type = await _dbContext.ProductTypes.FirstOrDefaultAsync(c => c.Id == id);
            if (type == null)
                return NotFound();
            else
                return View(type);
        }

        // POST: /Admin/ProductTypes/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete_POST(int id)
        {
            ProductType type = await _dbContext.ProductTypes.FindAsync(id);
            _dbContext.ProductTypes.Remove(type);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region AJAX Actions

        [AjaxOnly]
        public async Task<JsonResult> CheckNameAvailability(string name)
        {
            bool isNameAvailable = (await _dbContext.ProductTypes.AnyAsync(t => t.Name.Equals(name)) == false);
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
                bool isNameAvailable = (await _dbContext.ProductTypes.AnyAsync(t => t.Name.Equals(name)) == false);
                return Json(isNameAvailable);
            }
        }

        #endregion

    }
}
