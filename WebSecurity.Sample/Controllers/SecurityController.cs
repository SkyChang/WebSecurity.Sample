using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WebSecurity.Sample.Infrastructures;
using WebSecurity.Sample.Models;

namespace WebSecurity.Sample.Controllers
{
    public class SecurityController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SkySecurityContext _db;

        public SecurityController(
            ILogger<HomeController> logger,
            SkySecurityContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult SQLInject()
        {
            var users = _db.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult SQLInject(string userId)
        {
            using (SqlConnection connection = new SqlConnection(
                "Server=(localdb)\\mssqllocaldb;Database=SkySecurityDB;Trusted_Connection=True;MultipleActiveResultSets=true"))
            {
                // SELECT * FROM Users Where UserId like 'Sky'
                SqlCommand command = new SqlCommand("select * from users where UserId like '" + userId + "'", connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                }
                reader.Close();
            }

            return View(new List<User>());
        }

        public async Task<IActionResult> CreateUser(string userId, string password)
        {
            await _db.AddAsync(new User() { UserId = userId, Password = password });
            await _db.SaveChangesAsync();
            return RedirectToAction("SQLInject");
        }
    }
}
