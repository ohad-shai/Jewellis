﻿using Jewellis.App_Custom.Helpers.ViewModelHelpers;
using Jewellis.Areas.Admin.ViewModels.Newsletter;
using Jewellis.Data;
using Jewellis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewellis.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsletterController : Controller
    {
        private readonly JewellisDbContext _dbContext;

        public NewsletterController(JewellisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Admin/Newsletter
        public async Task<IActionResult> Index(IndexVM model)
        {
            List<NewsletterSubscriber> subscribers = await _dbContext.NewsletterSubscribers
                .Where(s => (model.Query == null) || s.EmailAddress.Contains(model.Query))
                .OrderByDescending(s => s.DateJoined)
                .ToListAsync();

            #region Pagination...

            Pagination pagination = new Pagination(subscribers.Count, model.PageSize, model.Page);
            if (pagination.HasPagination())
            {
                if (pagination.PageSize.HasValue)
                {
                    subscribers = subscribers
                        .Skip(pagination.GetRecordsSkipped())
                        .Take(pagination.PageSize.Value)
                        .ToList();
                }
            }
            ViewData["Pagination"] = pagination;

            #endregion

            ViewData["SubscribersModel"] = subscribers;
            return View(model);
        }

        // GET: /Admin/Newsletter/Remove/{id}
        public async Task<IActionResult> Remove(int id)
        {
            NewsletterSubscriber subscriber = await _dbContext.NewsletterSubscribers.FirstOrDefaultAsync(s => s.Id == id);
            if (subscriber == null)
                return NotFound();
            else
                return View(subscriber);
        }

        // POST: /Admin/Newsletter/Remove/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Remove")]
        public async Task<IActionResult> Remove_POST(int id)
        {
            NewsletterSubscriber subscriber = await _dbContext.NewsletterSubscribers.FindAsync(id);
            _dbContext.NewsletterSubscribers.Remove(subscriber);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
