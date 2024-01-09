using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServerApp;
using ServerApp.Controller;
using ServerApp.Model;


	public class Server
	{
	public static bool _keepRunning = true;

	UserController userController = new UserController();


		public int Port = 8080;
		public static string url = @"http://localhost:8080/";
		private HttpListener _listener;

		public void Start()
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add(url);
			_listener.Start();
			Receive();
		}

		public void Stop()
		{
			_listener.Stop();
		}

		private void Receive()
		{
			_listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);

		}
		/// <summary>
		/// Callback method for handling HTTP requests.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation.</param>
		private void ListenerCallback(IAsyncResult result)
		{
			if (_listener.IsListening)
			{
				string jsonResp = "";
				var context = _listener.EndGetContext(result);
				var request = context.Request;
				var response = context.Response;
				int userId;
				Stream output_stream;
				byte[] buffer;
				Console.WriteLine($"{request.Url}");
				Console.WriteLine($"{request.HttpMethod} {request.Url}");


				// Use switch case to handle different HTTP methods
				switch (request.HttpMethod)
				{
					case "POST":
						Console.WriteLine("request HttpMethod : \"POST\" \n");
						if (request.HasEntityBody)
						{
							var body = request.InputStream;
							var encoding = request.ContentEncoding;
							var reader = new StreamReader(body, encoding);
							if (request.ContentType != null)
							{
								Console.WriteLine("Client data content type {0}", request.ContentType);
							}
							Console.WriteLine("Client data content length {0}", request.ContentLength64);

							string s = reader.ReadToEnd();
							User newUser = new User();
							newUser =ParseJsonString(s);
							userController.CreateUser(newUser);
							HandleResponse(response, s);


							reader.Close();
							body.Close();
							Receive();
						}
						break;
					case "DELETE":
						Console.WriteLine("request HttpMethod : \"DELETE\" \n");
						userId = ExtractUserIdFromUrl(request.Url.AbsolutePath);
						Console.WriteLine("user id is: " + userId);

						userController.DeleteUserAsync(userId);
						 HandleResponse(response, "User deleted successfully.");

						break;

					case "GET":
					Console.WriteLine("request HttpMethod : \"GET\" \n");
					List<User> userList = userController.GetAllUsers();
					if (userList == null)
					{
						jsonResp = "No users exist in the dataBase";
					}
					jsonResp = JsonConvert.SerializeObject(userList);
					response.ContentType = "application/json";
					buffer = System.Text.Encoding.UTF8.GetBytes(jsonResp);
					response.ContentLength64 = buffer.Length;
					output_stream = response.OutputStream;
					output_stream.Write(buffer, 0, buffer.Length);

					break;

					case "PUT":
						Console.WriteLine("request HttpMethod : \"PUT\" \n");
						User userToUpdate = ExtractUserFromUrl(request.Url.AbsolutePath);
						userId = userToUpdate.ID;
						Console.WriteLine("user id is: " + userId);

						if (userToUpdate != null)
						{
							userController.UpdateUser(userToUpdate,userId);
							HandleResponse(response, jsonResp);
						}
						else
						{
							Console.WriteLine("Error to extract user");
						}
						break;

					default:
						// Handle unsupported HTTP methods
						jsonResp = $"Unsupported HTTP method: {request.HttpMethod}";
						response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
						response.ContentType = "application/json";
						buffer = System.Text.Encoding.UTF8.GetBytes(jsonResp);
						response.ContentLength64 = buffer.Length;
						output_stream = response.OutputStream;
						output_stream.Write(buffer, 0, buffer.Length);
						output_stream.Close();

						break;
				}
				Receive();

			}

		}
		/// <summary>
		/// Parses a JSON string into a User object.
		/// </summary>
		/// <param name="jsonString">The JSON string to parse.</param>
		/// <returns>The User object parsed from the JSON string.</returns>
		private User ParseJsonString(string jsonString)
		{
			try
			{

				User newUser = JsonConvert.DeserializeObject<User>(jsonString);
				return newUser;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error parsing JSON string: {ex.Message}");
				return null;
			}
		}
		/// <summary>
		/// Handles the response by writing content to the output stream.
		/// </summary>
		/// <param name="response">The HTTP response object.</param>
		/// <param name="content">The content to be written to the response stream.</param>
		private void HandleResponse(HttpListenerResponse response, string content)
		{
		var responseData = System.Text.Encoding.UTF8.GetBytes(content);
		response.ContentLength64 = responseData.Length;
		response.OutputStream.Write(responseData, 0, responseData.Length);
		response.Close();

		}
		
		/// <summary>
		/// Extracts the user ID from the URL path.
		/// </summary>
		/// <param name="urlPath">The URL path.</param>
		/// <returns>The extracted user ID.</returns>
		private int ExtractUserIdFromUrl(string urlPath)
		{
			//URL is in the format "/user/delete/{userId}"
			var segments = urlPath.Split('/');
			if (segments.Length >= 2 && int.TryParse(segments[1], out var userId))
			{
				return userId;
			}
			return 0;



		}
		/// <summary>
		/// Extracts user information from the URL path.
		/// </summary>
		/// <param name="urlPath">The URL path.</param>
		/// <returns>The extracted User object.</returns>
		private User ExtractUserFromUrl(string urlPath)
		{
			//URL is in the format "/user/myPath/{ID}/{Name}/{Email}/{Password}"

			var segments = urlPath.Split('/');

			if (segments.Length >= 6)
			{
				if (int.TryParse(segments[3], out var userId))
				{
					//ID is at index 3, Name at index 4, Email at index 5, and Password at index 6
					var nameWithSpaces = Uri.UnescapeDataString(segments[4]);
					var emailWithSpaces = Uri.UnescapeDataString(segments[5]);
					return new User
					{
						ID = userId,
						Name = nameWithSpaces,
						Email = emailWithSpaces,
						Password = segments[6]
					};
				}
			}

			return null;
		}
	}
