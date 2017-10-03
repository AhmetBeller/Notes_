using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.Data;
using Notes.Models;
using Notes.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace Notes.Controllers
{
    internal enum FilterName
    {
        Abgeschlossen,
        Datum,
        Wichtigkeit,
        Unbeendet,
        Alle
    }
    public class NoticesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static bool _isCreateOrEditMode = false;
        private static string _tmpSaveStyler = string.Empty;

        private readonly INoticeService _noticeService;

        public NoticesController(ApplicationDbContext context)
        {          
            _context = context;        
            _noticeService = new NoticeService(_context);
        }
      
        // GET: Notices
        public async Task<IActionResult> Index(string orderBy = null, string styler = null)
        {
            if (styler != null && styler.Equals("Styler"))
            {
                Styler();
            }
            
            ViewBag.Styler = _tmpSaveStyler;

            if (_isCreateOrEditMode)
                orderBy = GetOrderParameter(FilterParameter.Parameter);

            switch (orderBy)
            {
                case "ByFinished":
                    SetParameterNames(FilterName.Abgeschlossen);
                    return View(await _noticeService.Query.Include(x => x.Creator).Where(x=>x.State == true).OrderBy(x=>x.Date).ToListAsync());
                case "ByDate":
                    SetParameterNames(FilterName.Datum);
                    return View(await _noticeService.Query.Include(x => x.Creator).OrderBy(x=>x.Date).ToListAsync());
                case "ByImportance":
                    SetParameterNames(FilterName.Wichtigkeit);
                    return View(await _noticeService.Query.Include(x => x.Creator).OrderByDescending(x=>x.Importance).ToListAsync());
                case "HiddenFinished":
                    SetParameterNames(FilterName.Unbeendet);
                    return View(await _noticeService.Query.Include(x => x.Creator).Where(x=>x.State == false).ToListAsync());
                default:
                    SetParameterNames(FilterName.Alle);
                    return View(await _noticeService.Query.Include(x => x.Creator).ToListAsync());
            }            
        }

       
        private string GetOrderParameter(string filterParameter)
        {
            FilterName filterName;
            if (Enum.TryParse<FilterName>(filterParameter, out filterName))
            {
                switch (filterName)
                {
                    case FilterName.Abgeschlossen:                 
                        return filterParameter = "ByFinished";
                    case FilterName.Datum:
                        return filterParameter = "ByDate";
                    case FilterName.Wichtigkeit:
                        return filterParameter = "ByImportance";
                    case FilterName.Unbeendet:
                        return filterParameter = "HiddenFinished";
                    default:
                        return filterParameter = "Alle";
                }
            }
            return null;
        }

        private void SetParameterNames(FilterName filterName)
        {           
            FilterParameter.Parameter = filterName.ToString();
            ViewBag.Filter = filterName.ToString();
            _isCreateOrEditMode = false;
        }

        // GET: Notices
        public async Task<IActionResult> IndexNotesList(string orderBy)
        {
            ViewBag.Styler = _tmpSaveStyler;
            _isCreateOrEditMode = true;

            switch (orderBy)
            {
                case "Title":
                    return View(await _noticeService.Query.Include(x => x.Creator).OrderBy(x => x.Title).ToListAsync());
                case "Date":
                    return View(await _noticeService.Query.Include(x => x.Creator).OrderBy(x => x.Date).ToListAsync());
                case "Importance":
                    return View(await _noticeService.Query.Include(x => x.Creator).OrderBy(x => x.Importance).ToListAsync());
                case "State":
                    return View(await _noticeService.Query.Include(x => x.Creator).OrderBy(x => x.State).ToListAsync());
                case "Creator":
                    return View(await _noticeService.Query.Include(x => x.Creator).OrderBy(x => x.Creator.FirstName).ToListAsync());
                default:
                    return View(await _noticeService.Query.Include(x => x.Creator).ToListAsync());
            }            
        }

        // GET: Notices/Create
        public IActionResult Create()
        {
            ViewBag.Styler = _tmpSaveStyler;

            var notice = new Notice();
            notice.Importance = 1;
            notice.Date = DateTime.Now;
            _isCreateOrEditMode = true;

            return View(notice);
        }

        // POST: Notices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notice notice)
        {
            ViewBag.Styler = _tmpSaveStyler;

            var user = _noticeService.DbContext.ApplicationUsers.SingleOrDefault(x => x.Email == User.Identity.Name);
            notice.Creator = user;
            notice.CreatorId = user.Id;

            if (ModelState.IsValid)
            {
                _noticeService.Add(notice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notice);
        }

        // GET: Notices/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            ViewBag.Styler = _tmpSaveStyler;

            if (id == null)
            {
                return NotFound();
            }

            var notice =  _noticeService.All.SingleOrDefault(m => m.Id == id);
            if (notice == null)
            {
                return NotFound();
            }

            _isCreateOrEditMode = true;
            return View(notice);
        }

        // POST: Notices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Title,Description,Importance,Date,State, Creator, CreatorId")] Notice tmpNotice)
        {
            ViewBag.Styler = _tmpSaveStyler;

            var notice = _noticeService.Query.Include("Creator").SingleOrDefault(m => m.Id == id);
            if (notice != null)
            {
                notice.Title = tmpNotice.Title;
                notice.Description = tmpNotice.Description;
                notice.Date = tmpNotice.Date;
                notice.Importance = tmpNotice.Importance;
                notice.State = tmpNotice.State;
            }
            _isCreateOrEditMode = true;

            if (id != notice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _noticeService.Update(notice);
                    await _noticeService.DbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoticeExists(notice.Id))
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
            return View(notice);
        }
       
        // POST: Notices/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long? id)
        {
            ViewBag.Styler = _tmpSaveStyler;

            var notice = await _noticeService.DbContext.Notices.SingleOrDefaultAsync(m => m.Id == id);
            _context.Notices.Remove(notice);
            await _noticeService.DbContext.SaveChangesAsync();
            _isCreateOrEditMode = true;
            return RedirectToAction(nameof(Index));
        }

        private bool NoticeExists(long id)
        {
            return _noticeService.DbContext.Notices.Any(e => e.Id == id);
        }

        private const string SesssionKey = "Key";
        [HttpPost]
        public void Styler()
        {            
            var value = (HttpContext.Session.GetInt32(SesssionKey) ?? 0) + 1;
            HttpContext.Session.SetInt32(SesssionKey, value);

            _tmpSaveStyler = value % 2 == 0 ? "/lib/bootstrap/dist/css/bootstrap.css" : "/myLib/css/bootStrapOtherStyle.css";                
        }
    }
}

