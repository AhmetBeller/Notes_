using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notes.Models;

namespace Notes.Data
{
    public class DataService
    {
        private readonly IServiceProvider _provider;

        public DataService(IServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
        }

        public async Task<Task> CreateTestData()
        {
            var defaultPasswordForAll = "Meier01!";

            using (var serviceScope = _provider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                CreateRoles(serviceScope);
                var listTestNotes =  CreateTestUser(userManager, defaultPasswordForAll, context).Result;
                SetNoticeForUser(context, listTestNotes);

                return Task.CompletedTask;
            }
            
        }

        private async Task<List<Notice>> CreateTestUser(UserManager<ApplicationUser> userManager, string defaultPasswordForAll, ApplicationDbContext context)
        {
            var listUserTest = new List<Person>();
            var listTestNotes = ListTestNotes(out listUserTest);

            foreach (var user in listUserTest)
            {
                var testMember = new ApplicationUser();

                testMember.UserName = string.Concat(user.Vorname, ".", user.Name, "@hsr.ch");
                testMember.Email = string.Concat(user.Vorname, ".", user.Name, "@hsr.ch");
                testMember.FirstName = user.Name;
                testMember.SecondName = user.Vorname;

                await userManager.CreateAsync(testMember, defaultPasswordForAll);
                await userManager.AddToRoleAsync(testMember, "User");
                await userManager.IsEmailConfirmedAsync(testMember);
                context.SaveChanges();
            }
                      

            if (await userManager.FindByNameAsync("admin@admin.ch") == null)
            {
                var userAdmin = new ApplicationUser
                {
                    UserName = "admin@admin.ch",
                    Email = "admin@admin.ch",
                    FirstName = "Admin",
                    SecondName = "Admin"
                };

                await userManager.CreateAsync(userAdmin, defaultPasswordForAll);
                await userManager.AddToRoleAsync(userAdmin, "Administrator");
            }
            
            return listTestNotes;
        }

        private void SetNoticeForUser(ApplicationDbContext context, List<Notice> listTestNotes)
        {
            int index = 0;
            foreach (var notice in listTestNotes)
            {
                var testMemberNotice = new Models.Notice();

                testMemberNotice.Title = notice.Title;
                testMemberNotice.Description = notice.Description;
                testMemberNotice.Importance = notice.Importance;
                testMemberNotice.Date = notice.Date;
                testMemberNotice.State = notice.State;

                context.Notices.Add(testMemberNotice);
                context.SaveChanges();

                for (int i = index; index < context.ApplicationUsers.Count();)
                {
                    testMemberNotice.CreatorId = context.ApplicationUsers.ToArray()[i].Id;
                    testMemberNotice.Creator = context.ApplicationUsers.ToArray()[i];
                    context.SaveChanges();
                    ++index;
                    break;
                }
            }
        }

        private struct Notice
        {
            public string Title;
            public string Description;
            public int Importance;
            public DateTime Date;
            public bool State;
        }

        private List<Notice> ListTestNotes(out List<Person> listTestUser)
        {
            string fixDescription = "Asp macht Spass! Mein Name ist ";
            int index = 1;
            int dateAddDays = index;

            var listTestNotes = new List<Notice>();
            listTestUser = ListTestUser();

            foreach (var user in listTestUser)
            {
                var newNotices = new Notice();

                newNotices.Title = string.Concat("Test", user.Name);
                newNotices.Description = fixDescription + user.Vorname;
                newNotices.Importance = index;
                newNotices.Date = DateTime.Now.AddDays(dateAddDays);
                newNotices.State = false;

                listTestNotes.Add(newNotices);

                index = index >= 5 ? 1 : ++index;
                dateAddDays += index;
            }

            return listTestNotes;
        }
        private struct Person
        {
            public string Name;
            public string Vorname;
        }

        private List<Person> ListTestUser()
        {
            var listTestUser = new List<Person>();

            listTestUser.Add(new Person { Name = "Meier", Vorname = "Hans" });
            listTestUser.Add(new Person { Name = "Fritz", Vorname = "Kurt" });
            listTestUser.Add(new Person { Name = "Muster", Vorname = "Emil" });
            listTestUser.Add(new Person { Name = "Roger", Vorname = "Daniel" });
            listTestUser.Add(new Person { Name = "Meier", Vorname = "Mateo" });
            listTestUser.Add(new Person { Name = "Colin", Vorname = "Sergio" });
            listTestUser.Add(new Person { Name = "Sebastian", Vorname = "Ivan" });
            listTestUser.Add(new Person { Name = "Kilian", Vorname = "Peter" });
            listTestUser.Add(new Person { Name = "Lars", Vorname = "Marcel" });
            listTestUser.Add(new Person { Name = "Nikita", Vorname = "Alfred" });
            listTestUser.Add(new Person { Name = "Theodor", Vorname = "Fabian" });

            return listTestUser;
        }

        private async void CreateRoles(IServiceScope serviceScope)
        {
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            string[] roleList = { "Administrator", "User" };

            foreach (var roleName in roleList)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new IdentityRole() { Name = roleName };
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}
