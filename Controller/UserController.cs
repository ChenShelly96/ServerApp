using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using MySqlConnector;
using ServerApp.Model;
using Microsoft.Extensions.Configuration;
using static Hangfire.Storage.JobStorageFeatures;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace ServerApp.Controller
{
	public class UserController
	{
		private readonly string connectionString;
		public MySqlConnection conn;
		public UserController()
		{

			IConfigurationRoot configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();
			this.connectionString = configuration["ConnectionString"];
		}
		

		// method to add a new user to the database
		public async Task<User> CreateUserAsync(User newUser)
		{
			if (newUser == null)
			{
				return null;
			}
			using MySqlConnection conn = new MySqlConnection(connectionString);
			await conn.OpenAsync();
			
			string query = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password)";
			using MySqlCommand command = new MySqlCommand(query, conn);

			command.Parameters.AddWithValue("@Name", newUser.Name);
			command.Parameters.AddWithValue("@Email", newUser.Email);
			command.Parameters.AddWithValue("@Password", newUser.Password);

			await command.ExecuteNonQueryAsync();
			newUser.ID = Convert.ToInt32(command.LastInsertedId);

			return newUser;

		}
		[HttpPost("user/add")]
		public void CreateUser(User user)
		{
			using (conn = new MySqlConnection(connectionString))
			{
				conn.Open();

				string query = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password)";
				using (MySqlCommand command = new MySqlCommand(query, conn))
				{
					command.Parameters.AddWithValue("@Name", user.Name);
					command.Parameters.AddWithValue("@Email", user.Email);
					command.Parameters.AddWithValue("@Password", user.Password);

					command.ExecuteNonQuery();
				}



				Console.WriteLine("Create user succeeded in userController! ");
				conn.Close();
			}
		}
		[HttpPut("user/update")]
		public void UpdateUser(User user, int id)
		{
			user.ID = id;
			using (conn = new MySqlConnection(connectionString))
			{
				conn.Open();

				string query = "UPDATE Users SET Name = @Name, Email = @Email, Password = @Password WHERE ID = @ID";
				using (MySqlCommand command = new MySqlCommand(query, conn))
				{
					command.Parameters.AddWithValue("@ID", user.ID);
					command.Parameters.AddWithValue("@Name", user.Name);
					command.Parameters.AddWithValue("@Email", user.Email);
					command.Parameters.AddWithValue("@Password", user.Password);

					command.ExecuteNonQuery();

					
					
				}
			}
		}
		public async Task<User> GetUserByIdAsync(int userId)
		{
			
			using MySqlConnection conn = new MySqlConnection(connectionString);
			await conn.OpenAsync();
			string query = "SELECT * FROM mydatabase.users  WHERE ID = @UserID";
			MySqlCommand cmd = new MySqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@UserID", userId);
			using MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
			if (reader.Read())
			{
				
				Console.WriteLine("Connection Open ! ");
				int Id = Convert.ToInt32(reader["ID"]);
				string _Name = reader["Name"].ToString();
				string _Email = reader["Email"].ToString();
				string _Password = reader["Password"].ToString();

				return new User
				{
					ID = Id,
					Name = _Name,
					Email = _Email,
					Password = _Password
				};
				
				
			}
			
			return null;
		}
		public User ReadUserById(int userId)
		{
			MySqlConnection conn = new MySqlConnection(connectionString);
		
			conn.Open();
			string query = "SELECT * FROM mydatabase.users  WHERE ID = @UserID";
			MySqlCommand cmd = new MySqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@UserID", userId);
			MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
			if(reader.Read())
			{
				
				Console.WriteLine("Connection Open ! ");
				int Id = Convert.ToInt32(reader["ID"]);
				string _Name = reader["Name"].ToString();
				string _Email = reader["Email"].ToString();
				string _Password = reader["Password"].ToString();


				return new User
				{
					ID = Id,
					Name = _Name,
					Email = _Email,
					Password = _Password
				};

			
			
			}
			else 
			{
				Console.WriteLine("Connection Failed ! ");
			
				conn.Close();

			}
			return null;
		}

		public async Task UpdateUserAsync(User user)
		{
			
			using MySqlConnection conn = new MySqlConnection(connectionString);
			await conn.OpenAsync();
			using MySqlCommand command = conn.CreateCommand();
			command.CommandText = "UPDATE Users SET Name = @Name, Email = @Email, Password = @Password WHERE ID = @ID";
			command.Parameters.AddWithValue("@ID", user.ID);
			command.Parameters.AddWithValue("@Name", user.Name);
			command.Parameters.AddWithValue("@Email", user.Email);
			command.Parameters.AddWithValue("@Password", user.Password);
			await command.ExecuteNonQueryAsync();
		}

		public async Task DeleteUserAsync(int userId)
		{
			using MySqlConnection conn = new MySqlConnection(connectionString);
			await conn.OpenAsync();
			string query = "DELETE FROM Users WHERE ID = @ID";
			using MySqlCommand command = new MySqlCommand(query, conn);
			
				command.Parameters.AddWithValue("@ID", userId);
				await command.ExecuteNonQueryAsync();

		}

		public async Task<Object> GetAllUsersAsync()
		{
			List<User> userList = new List<User>();
			using MySqlConnection conn = new MySqlConnection(connectionString);
			await conn.OpenAsync();
			string query = "SELECT * FROM Users";
			using MySqlCommand command = new MySqlCommand(query, conn);
			using MySqlDataReader reader = await command.ExecuteReaderAsync();
			while (reader.Read())
			{
				userList.Add(new User
				{
					ID = Convert.ToInt32(reader["ID"]),
					Name = reader["Name"].ToString(),
					Email = reader["Email"].ToString(),
					Password = reader["Password"].ToString()

				});
			}

			return userList;
		}
		public async Task<List<User>> GetAllUsersList()
		{
			List<User> userList = new List<User>();

			using MySqlConnection conn = new MySqlConnection(connectionString);
			await conn.OpenAsync();

			string query = "SELECT * FROM Users";
			using MySqlCommand command = new MySqlCommand(query, conn);

			using MySqlDataReader reader = await command.ExecuteReaderAsync();

			while (reader.Read())
			{
				userList.Add(new User
				{
					ID = Convert.ToInt32(reader["ID"]),
					Name = reader["Name"].ToString(),
					Email = reader["Email"].ToString(),
					Password = reader["Password"].ToString()

				});
			}

			return userList;
		}

		public List<User> GetAllUsers()
		{
			List<User> userList = new List<User>();
			using MySqlConnection conn = new MySqlConnection(connectionString);

			conn.Open();
			string query = "SELECT * FROM Users";
			using MySqlCommand command = new MySqlCommand(query, conn);
				
					using (MySqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							userList.Add(new User
							{
								ID = Convert.ToInt32(reader["ID"]),
								Name = reader["Name"].ToString(),
								Email = reader["Email"].ToString(),
								Password = reader["Password"].ToString()

							});



						}
					}
				
			

			return userList;
		}
		public async Task PrintAllUsers()
		{
			try
			{
				var userList = await GetAllUsersList();

				if (userList.Count > 0)
				{
					Console.WriteLine("List of all users:");
					foreach (var user in userList)
					{
						Console.WriteLine($"ID: {user.ID}, Name: {user.Name}, Email: {user.Email}, Password: {user.Password}");
					}
				}
				else
				{
					Console.WriteLine("No users found.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}

	}
}
