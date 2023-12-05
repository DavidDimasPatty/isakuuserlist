using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ListUserIsaku.Models;

namespace ListUserIsaku.Controllers
{
    public class UserController : Controller
    {
        private readonly MySqlConnection mySqlConnection;

        public UserController(MySqlConnection mySqlConnection)
        {
            this.mySqlConnection = mySqlConnection;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userList = new List<Models.User>();
            await mySqlConnection.OpenAsync();
            var command = new MySqlCommand("select * from userisaku.user", mySqlConnection);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var user = new User
                {
                    id = reader.GetInt32("id"),
                    name = reader.GetString("name"),
                    status = reader.GetInt32("status"),
                    joinedDate = reader.GetDateTime("joinedDate")
                };
                userList.Add(user);
            }
            return View(userList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(User addUser)
        {

            await mySqlConnection.OpenAsync();
            
            var commandTop = new MySqlCommand("SELECT id FROM userisaku.user ORDER BY id DESC LIMIT 1", mySqlConnection);

            var idTop=0;
            var reader = await commandTop.ExecuteReaderAsync();
            while (reader.Read())
            {
                idTop = reader.GetInt32("id");
            }

            await mySqlConnection.CloseAsync();
            await mySqlConnection.OpenAsync();
            var query = "INSERT INTO user (id, name, status, joinedDate) VALUES (@id, @name, @status, @joinedDate)";
            using (var command = new MySqlCommand(query, mySqlConnection))
            {
                command.Parameters.AddWithValue("@id", idTop+1);
                command.Parameters.AddWithValue("@name", addUser.name);
                command.Parameters.AddWithValue("@status", addUser.status);
                command.Parameters.AddWithValue("@joinedDate", DateTime.Today);
                await command.ExecuteNonQueryAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await mySqlConnection.OpenAsync();
            var query = "Delete from user where id=@id";
            using (var command = new MySqlCommand(query, mySqlConnection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var userList = new User();
            await mySqlConnection.OpenAsync();
            var command = new MySqlCommand("SELECT * FROM userisaku.user WHERE id = @id", mySqlConnection);
            command.Parameters.AddWithValue("@id", id);

            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                userList = new User
                {
                    id = reader.GetInt32("id"),
                    name = reader.GetString("name"),
                    status = reader.GetInt32("status"),
                    joinedDate = reader.GetDateTime("joinedDate")
                };
    
            }

            return View(userList);
        }

        [HttpPost]
        public async Task<IActionResult> Update (User user)
        {
            await mySqlConnection.OpenAsync();
            var query = "update user set name=@name, status=@status where id=@id";
            using (var command = new MySqlCommand(query, mySqlConnection))
            {
                command.Parameters.AddWithValue("@id", user.id);
                command.Parameters.AddWithValue("@name", user.name);
                command.Parameters.AddWithValue("@status", user.status);
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
