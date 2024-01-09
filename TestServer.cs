
using ServerApp.Controller;
using ServerApp.Model;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using  ServerApp;
using static Server;
using static ServerApp.Program;
public class ServerTest
{
	//connection string to connect database - MyDatabase in MySQL server
	public static string connString = "server=localhost;port=3307;user id=root;pwd=password;database=mydatabase";
	public static string serverUrl = "http://localhost:8080/";
	public static void MainServer()
	{
		var httpServer = new Server();
		httpServer.Start();
		Console.WriteLine("----------Start Test Server----------");
		UserController userController = new UserController();

		

		MainAsync(userController);



	}

	/// <summary>
	/// Asynchronous main function to start the test server.
	/// </summary>
	/// <param name="userController">The user controller instance for database interactions.</param>
	static async Task MainAsync(UserController userController)
	{
	
		Console.WriteLine($"----------Start test Post User----------");
		User user1 = new User { Name = "Tani", Email = "Tani@gmail.com", Password = "1t2AD" };
		User user2 = new User { Name = "Emily ", Email = "Emily@gmail.com", Password = "3fD24" };
		User user3 = new User { Name = "Bob ", Email = "Bob@gmail.com", Password = "jk43" };
		User user4 = new User { Name = "Momo ", Email = "momo@gmail.com", Password = "JKDS46" };


		await PostUserAsync(user1);
		await PostUserAsync(user2);
		await PostUserAsync(user3);
		await PostUserAsync(user4);
		Console.WriteLine($"----------End test Post Users---------- \n");


		Console.WriteLine($"----------Start test Get Users----------\n");
		await GetUsersListAsync(serverUrl);
		Console.WriteLine($"----------End test Get Users----------\n");

		Console.WriteLine($"----------Start test Update User----------\n");

		var res = updateUser(user1, user1.ID);
		Console.WriteLine(res);

		Console.WriteLine($"----------End test Update User----------\n");


		Console.WriteLine($"----------Start test Delete User----------\n");

		await deleteUser(user1.ID);
		await deleteUser(user2.ID);
		await deleteUser(user3.ID);
		await deleteUser(user4.ID);
		// Stop the server
		Console.WriteLine("Press Enter to stop the server...");
		Console.ReadLine();

	}
	
	/// <summary>
	///  Sends a POST request to the server to add a new user.
	/// </summary>
	/// <param name="user">The user object to be added.</param>
	/// <param name="url">The URL for the POST request.</param>
	/// <returns>A string indicating the completion of the POST function test.</returns>
	static string PostUser(User user, string url)
	{

		using (var request = new HttpRequestMessage(HttpMethod.Post, url))
		{

			var json = JsonConvert.SerializeObject(user);
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");
			// Create an HttpClient to send the request
			using (var httpClient = new HttpClient())
			{

				// Send the HTTP request and get the response
				var response = httpClient.SendAsync(request).Result;

				if (response.IsSuccessStatusCode)
				{

					var responseBody = response.Content.ReadAsStringAsync().Result;
					Console.WriteLine($"POST /adduser: User added successfully \n");
					Console.WriteLine($"Status code fron the server: {response.StatusCode}");

				}
				else
				{
					Console.WriteLine($"Failed to add user. Status code: {response.StatusCode}");
				}
			}
		}
		return "POST function test end";
	}



	static async Task PostUserAsync(User user)
	{
		string url = $"{serverUrl}user/add";
		string res = PostUser(user, url);
		Console.WriteLine($"{res}");
	}




	/// <summary>
	/// Sends a GET request to the server to retrieve the list of all users.
	/// </summary>
	/// <param name="url">The URL for the GET request.</param>
	/// <returns>A string indicating the completion of the GET function test.</returns>
	static string GetUsers(string url)
	{

		using (var request = new HttpRequestMessage(HttpMethod.Get, url))
		{

			var json = JsonConvert.SerializeObject("");
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");

			using (var httpClient = new HttpClient())
			{
				var newUser = JsonConvert.SerializeObject("");
				var response = httpClient.GetAsync(url).Result;

				if (response.IsSuccessStatusCode)
				{

					var responseBody = response.Content.ReadAsStringAsync().Result;
					var jDoc = JsonDocument.Parse(responseBody);
					Console.WriteLine($"Status code: {response.StatusCode}");
					string jsonPretty = System.Text.Json.JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
					Console.WriteLine($"GET : Users get successfully - List of All Users: \n {jsonPretty}");
				}
				else
				{

					Console.WriteLine($"Failed to get users. Status code: {response.StatusCode} \n");
				}
			}
		}
		return "Get users";
	}



	/// <summary>
	/// Asynchronously retrieves the list of all users from the server.
	/// </summary>
	/// <param name="serverUrl">The URL of the server.</param>
	static async Task GetUsersListAsync(string serverUrl)
	{


		string url = $"{serverUrl}api/allUsers";
		string postResponse = GetUsers(url);

		Console.WriteLine($"Get Response: {postResponse}");

	}
	/// <summary>
	/// Asynchronously updates a user on the server.
	/// </summary>
	/// <param name="user">The updated user object.</param>
	/// <param name="userId">The ID of the user to be updated.</param>
	static async Task updateUser(User user, int userId)
	{
		Console.WriteLine($"update User");
		string url = $"{serverUrl}user/update";

		var uri = Path.Combine(url, userId.ToString(), user.Name, user.Email, user.Password);

		using (var request = new HttpRequestMessage(HttpMethod.Put, uri))
		{
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			var json = JsonConvert.SerializeObject(uri);
			request.Content = new StringContent(json, Encoding.UTF8);
			request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			using (var httpClient = new HttpClient())
			{

				var response = httpClient.SendAsync(request).Result;

				if (response.IsSuccessStatusCode)
				{

					var responseBody = response.Content.ReadAsStringAsync().Result;

					Console.WriteLine($"Status code: {response.StatusCode}");
					Console.WriteLine($"UPDATE : Update user by id successfully \n");
				}
				else
				{

					Console.WriteLine($"Failed to Update user. Status code: {response.StatusCode} \n");
				}
			}

		}
	}

	/// <summary>
	/// Asynchronously deletes a user from the server.
	/// </summary>
	/// <param name="userId">The ID of the user to be deleted.</param>
	static async Task deleteUser(int userId)
	{
		string url = $"{serverUrl}{userId}";

		using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
		{

			var json = JsonConvert.SerializeObject(userId);
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");

			using (var httpClient = new HttpClient())
			{
				var response = httpClient.DeleteAsync(url).Result;
				if (response.IsSuccessStatusCode)
				{

					var responseBody = response.Content.ReadAsStringAsync().Result;

					Console.WriteLine($"Status code: {response.StatusCode}");
					Console.WriteLine($"DELETE : Delete user by id successfully \n");
				}
				else
				{

					Console.WriteLine($"Failed to Delete users. Status code: {response.StatusCode} \n");
				}
			}
		}
	}
}



