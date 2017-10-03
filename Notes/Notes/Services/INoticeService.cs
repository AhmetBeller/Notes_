using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Notes.Data;
using Notes.Models;

namespace Notes.Services
{
    public interface INoticeService
    {
        ApplicationDbContext DbContext { get; }

        IQueryable<Notice> Query { get; }

        IEnumerable<Notice> All { get; }

        void Add(Notice notice);

        void Update(Notice notice);

        void Remove(Notice notice);

        Notice FindById(int? id);

        void Save();

    }
}
