using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Notes;
using Notes.Controllers;
using Notes.Data;
using Notes.Models;
using Notes.Services;
using Xunit;

namespace Notes_Test
{
    public class NoticeControllerServiceTest 
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly NoticeService _noticeService;

        public NoticeControllerServiceTest()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("Notes"));

            var tempDataFactory = new Mock<ITempDataDictionaryFactory>();
            tempDataFactory.Setup(x => x.GetTempData(It.IsAny<HttpContext>()))
                .Returns(new Mock<ITempDataDictionary>().Object);
            services.AddSingleton(tempDataFactory.Object);

            _serviceProvider = services.BuildServiceProvider();
            _dbContext = _serviceProvider.GetService<ApplicationDbContext>();
            _noticeService = new NoticeService(_dbContext);

        }
     
        /// <summary>
        /// Prüfung, wenn zwei Notizen hinzugefügt werden und die Menge gleich 2 ist.
        /// </summary>
        [Fact]
        private void CreateNoticeAndCheckCount()
        {
            var mockRepo = new Mock<INoticeService>();
            mockRepo.Setup(repo => repo.All).Returns(GetTestNotices());
            var controller = new NoticesController(_dbContext);

            var result = controller.Index("default", null);
            result.Wait();

            var viewResult = result.Result as ViewResult;
            if (viewResult != null)
            {
                var model = Assert.IsAssignableFrom<IEnumerable<Notice>>(
                    viewResult.ViewData.Model);
                Assert.Equal(2, model.Count());
            }
        }

        private IEnumerable<Notice> GetTestNotices()
        {
            var notes = new List<Notice>();
            var user1 = new ApplicationUser() { FirstName = "TestName1", SecondName = "TestSecondName1" };
            notes.Add(new Notice()
            {
                Title = "TestTitel",
                Creator = user1,
                Date = DateTime.Now,
                Importance = 3,
                Description = "TestingTestingTesting",
                State = false,
                CreatorId = null
            });
            var user2 = new ApplicationUser() {FirstName = "TestName2", SecondName = "TestSecondName2"};
            notes.Add(new Notice()
            {
                Title = "TestTitel",
                Creator = user2,
                Date = DateTime.Now,
                Importance = 3,
                Description = "TestingTestingTesting",
                State = false,
                CreatorId = null
            });

            foreach (var item in notes)
            {
                _noticeService.Add(item);
                _noticeService.Save();
            }

            return notes;
        }

        /// <summary>
        /// Prüfung, wenn Id = null ist. Keine verfügbare Notiz vorhanden.
        /// </summary>
        [Fact]
        public void CheckIfNotNoticeContains()
        {
            var controller = new NoticesController(_dbContext);

            var result = controller.Edit(null);
            result.Wait();

            var viewResult = result.Result as NotFoundResult;
            if (viewResult != null)
            {
                Assert.Equal(viewResult.StatusCode, 404);
            }
        }

        /// <summary>
        /// Prüfung, wenn eine ungültige Id angegeben wird.
        /// </summary>
        [Fact]
        public void CheckNoticeIsNotAvailable()
        {
            var mockRepo = new Mock<INoticeService>();
            mockRepo.Setup(repo => repo.All).Returns(GetTestNotices());
            var controller = new NoticesController(_dbContext);

            var resultIndex = controller.Index("default", null);
            resultIndex.Wait();
            var viewResultIndex = resultIndex.Result as ViewResult;

            int tempId = 0;
            if (viewResultIndex != null)
            {
                var model = Assert.IsAssignableFrom<IEnumerable<Notice>>(
                    viewResultIndex.ViewData.Model);
               
                foreach (var item in model)
                {
                    tempId = (int)item.Id;
                }
                tempId += 1;
            }

            var resultEdit = controller.Edit(tempId);
            resultEdit.Wait();

            var viewResultEdit = resultEdit.Result as NotFoundResult;
            if (viewResultEdit != null)
            {
                Assert.Equal(viewResultEdit.StatusCode, 404);
            }
        }

        /// <summary>
        /// Prüfung, ob das Update einer Notiz funktioniert.
        /// </summary>
        [Fact]
        public void CheckEditUpdate()
        {
            var mockRepo = new Mock<INoticeService>();
            mockRepo.Setup(repo => repo.All).Returns(GetTestNotices());
            var controller = new NoticesController(_dbContext);

            if (_noticeService.DbContext.Notices.Any())
            {
                var notice = _noticeService.Query.FirstOrDefault();
                if (notice != null)
                {
                    notice.Title = "NeuerTitel";
                    notice.Description = "Neue Bechreibung";
                    notice.Importance = 1;
                    notice.State = true;

                    var resultEdit = controller.Edit(notice.Id, notice);
                    resultEdit.Wait();
                }

                var noticeRequest = _noticeService.Query.SingleOrDefault(x => x.Id == notice.Id);
                Assert.Equal(noticeRequest.Title, "NeuerTitel");
                Assert.Equal(noticeRequest.Description, "Neue Bechreibung");
                Assert.Equal(noticeRequest.Importance, 1);
                Assert.Equal(noticeRequest.State, true);
            }         
        }
    }
}
