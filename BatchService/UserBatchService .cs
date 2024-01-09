using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using ServerApp.Model;
using ServerApp.Controller;


public class UserBatchService
	{

	private readonly string connectionString;
	private static System.Timers.Timer timer;
	public UserBatchService()
	{
		IConfigurationRoot configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();
		timer = new System.Timers.Timer();
		this.connectionString = configuration["ConnectionString"];
	}
	public void Start()
	{
		
		this.ProcessBatch();
	}
	public void StopBatchProcessing()
	{
	
		Console.WriteLine("Batch service stopped.");
	}
	
	private async void ProcessBatch()
	{
		string res1 = "";
		string res2 = "";
		List<User> users = await GetAllUsers();
		foreach (var user in users)
		{
			
			// Send email to the user
			res1 = await SendEmail(user);
			Console.WriteLine(res1);
			// Update the user records Batch
			res2 = await UpdateUserRecords(user);
			Console.WriteLine(res2);
		}
	}

	
	
	private async Task<List<User>> GetAllUsers()
	{
		List<User> users = new List<User>();
		using MySqlConnection connection = new MySqlConnection(connectionString);
		await connection.OpenAsync();
		using MySqlCommand command = connection.CreateCommand();
		command.CommandText = "SELECT * FROM Users";
		using MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync();
		while (reader.Read())
		{
			User user = new User
			{
				ID = Convert.ToInt32(reader["ID"]),
				Name = reader["Name"].ToString(),
				Email = reader["Email"].ToString(),
				Password = reader["Password"].ToString()

			};
			users.Add(user);
		}
		return users;
	}
	

	public async Task<string> SendEmail(User user)
	{
		if (user.Email == null)
		{
			return ($"Failed to send email to {user.Name}");
		}
		else
		{

			return ($"Weekly Newsletter email sending to {user.Email}");
		}
	}
	


	public async Task<string> UpdateUserRecords(User user)
	{
		try
		{
			using MySqlConnection connection = new MySqlConnection(connectionString);
			await connection.OpenAsync();
			using MySqlCommand command = connection.CreateCommand();
			command.CommandText = "UPDATE Users SET Processed = 1 WHERE ID = @ID";
			command.Parameters.AddWithValue("@ID", user.ID);

			 await command.ExecuteNonQueryAsync();
			return ($"{user.Name} record updated successfully");
		}
		catch (Exception error)
		{
			return "Falied to update";
		}
	}
}

