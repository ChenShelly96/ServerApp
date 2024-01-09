using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Model;
using ServerApp.Controller;
using ServerApp;
using Microsoft.Extensions.Configuration;
using static Server;

namespace ServerApp;
class Program
{
	private readonly string connectionString;
	
	public static async Task Main(string[] args)
	{

		Console.WriteLine("----------Start Test Server----------");
		UserController userController = new UserController();

		ServerTest.MainServer();

		Console.WriteLine("----------Start Test the batch service----------");
		
		Task thread1 =Task.Run(() => Task1());
		Task thread2 = Task.Run(() => Task2());
		await Task.WhenAll(thread1,thread2);


		Task serverThread = Task.Run(() => RunServerThread());
		ValueTask userBatchThread = ScheduleBatchThread();
		await Task.WhenAll(serverThread, userBatchThread.AsTask());


		// Run the batch service manually ///
		Console.WriteLine("----------batch service manually----------");
		TestBatchServer testBatchServer = new TestBatchServer();
		await testBatchServer.Start();

	}
	static void RunServerThread()
	{
		
		
		var userController = new UserController();
		var server = new Server();
		server.Start();
	}
	static void RunUserBatchThread(object state)
	{
		var userBatchService = new UserBatchService();
		userBatchService.Start();
		ScheduleBatchThread().ConfigureAwait(false);
	}
	static async ValueTask ScheduleBatchThread()
	{

		// Calculate the delay until the next Sunday at 8:00 PM
		TimeSpan delay = CalculateDelayUntilNextSundayAt8PM();

		// Schedule the second thread to run on the next Sunday at 8:00 PM
		Timer secondThreadTimer = new Timer(RunUserBatchThread, null, delay, Timeout.InfiniteTimeSpan);


		await Task.Delay(-1);
	}
	static TimeSpan CalculateDelayUntilNextSundayAt8PM()
	{
		DayOfWeek today = DateTime.Now.DayOfWeek;
		int daysUntilNextSunday = ((int)DayOfWeek.Sunday - (int)today + 7) % 7;
		TimeSpan delay = TimeSpan.FromDays(daysUntilNextSunday);
		DateTime nextSundayAt8PM = DateTime.Today.AddDays(daysUntilNextSunday).AddHours(20);
		if (nextSundayAt8PM < DateTime.Now)
		{
			// If today is Sunday and it's already past 8:00 PM, schedule for next week
			delay += TimeSpan.FromDays(7);
		}
		return delay;
	}

	static async Task Task1()
	{

		UserController userController = new UserController();

		try
		{
			Console.WriteLine($"----------TASK 1: Add, Get, Udate, Delete users ----------\n");
			

			User newUser = new User { Name = "Emily ", Email = "Emily@gmail.com", Password = "3fD24" };
			newUser = await userController.CreateUserAsync(newUser);
			Console.WriteLine($"TASK 1: User added successfully \n");
			Console.WriteLine($"----------TASK 1: End test Add Users---------- \n");

			Console.WriteLine($"----------TASK 1: Start test Get Users----------\n");
			var getUsers = await userController.GetAllUsersList();
			foreach (var user in getUsers)
			{
				Console.WriteLine($"ID: {user.ID}, Name: {user.Name}, Email: {user.Email}, Password: {user.Password}");
			}
			Console.WriteLine($"----------TASK 1: End test Get Users----------\n");


			Console.WriteLine($"----------TASK 1: Start test Update User----------\n");
			int userIdToUdate = newUser.ID;
			User userToUdate = await userController.GetUserByIdAsync(userIdToUdate);
			userToUdate.Name = "Bob-UPDATE";
			userToUdate.Email = "Bob-UPDATE@gmail.com";
			await userController.UpdateUserAsync(userToUdate);
			Console.WriteLine($"UPDATE : Update user by id updated successfully: \n");
			Console.WriteLine($"ID: {userToUdate.ID}, Name: {userToUdate.Name}, Email: {userToUdate.Email}, Password: {userToUdate.Password}");
			Console.WriteLine($"----------TASK 1: End test Update User----------\n");


			Console.WriteLine($"----------TASK 1: Start test Delete User----------\n");
			int userIdToDelete = newUser.ID;
			await userController.DeleteUserAsync(userIdToDelete);
			Console.WriteLine($"DELETE : Delete user by id deleted successfully \n");
			Console.WriteLine($"----------TASK 1: End test Delete User----------\n");
			Console.WriteLine($"\n");
			Console.WriteLine($"---------- TASK 1: Users Data after tests: ----------\n");
			 await userController.PrintAllUsers();


			await Task.WhenAll();
		}
		catch (Exception ex)
		{
			Console.WriteLine("TASK 2: Error occurred: " + ex.Message);
		}

		
		Console.WriteLine("Press any key to exit...");
		Console.ReadKey();






	}

	static async Task Task2()
	{
		UserController userController = new UserController();
		var userBatchService = new UserBatchService();

		try
		{
			Console.WriteLine($"----------TASK 2:Add user, Send Email, Update the Record ----------\n");
			

			User newUser = new User { Name = "Olivia ", Email = "Olivia@gmail.com", Password = "425tFDC1" };
			newUser = await userController.CreateUserAsync(newUser);
			Console.WriteLine($"TASK 2: User added successfully \n");
			Console.WriteLine($"----------TASK 2: End test Add Users---------- \n");

			Console.WriteLine($"----------TASK 2: Send email----------\n");
			
			var ans1 = await userBatchService.SendEmail(newUser);
			Console.WriteLine(ans1);

			Console.WriteLine($"----------TASK 2: Update the Record user Batch----------\n");
			
			var ans2 = await userBatchService.UpdateUserRecords(newUser);
			Console.WriteLine(ans2);



			await Task.WhenAll();
		}
		catch (Exception ex)
		{
			Console.WriteLine("TASK 2: Error occurred: " + ex.Message);
		}



		// Wait for user input before exiting the console application
		Console.WriteLine("Press any key to exit...");
		Console.ReadKey();



	}
}