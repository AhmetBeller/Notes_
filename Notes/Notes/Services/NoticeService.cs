using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Notes.Data;
using Notes.Models;

namespace Notes.Services
{
    public class NoticeService : INoticeService
    {
        private ApplicationDbContext _dbContext;
      
        public NoticeService(ApplicationDbContext dbContext)
        {
          _dbContext = dbContext;
        }

        public ApplicationDbContext DbContext => _dbContext;
        public IQueryable<Notice> Query => this._dbContext.Notices;
        public IEnumerable<Notice> All => this._dbContext.Notices;


        public void Add(Notice notice)
        {
            _dbContext.Notices.Add(notice);
        }

        public void Update(Notice notice)
        {
            _dbContext.Notices.Update(notice);
        }

        public void Remove(Notice notice)
        {
            this._dbContext.Notices.Remove(notice);
        }

        public Notice FindById(int? id)
        {
            return this._dbContext.Notices.Find(id);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
              
    }

   
}
