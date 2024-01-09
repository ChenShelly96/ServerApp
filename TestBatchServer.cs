using ServerApp.Controller;
using ServerApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
	 public class TestBatchServer
	{
		public TestBatchServer(){}
		public async Task Start()
		{

			Task thread1 =Task.Run(() => Main2());
			await Task.WhenAll(thread1);

			
				
		}
			public static async Task Main2()
		{
			Console.WriteLine(" Run batch service manually: \n");
			UserController userController = new UserController();
			var userBatchService = new UserBatchService();

			try
			{
				
				userBatchService.Start();

				User newUser = new User { Name = "Clara ", Email = "Clara@gmail.com", Password = "498SKA" };
				
				newUser = await userController.CreateUserAsync(newUser);
				Console.WriteLine($"Test Batch Server1 : User added successfully \n");
				Console.WriteLine($"----------Test Batch Server1 : End test Add Users---------- \n");

				Console.WriteLine($"----------Test Batch Server1 : Send email----------\n");

				var ans1 = await userBatchService.SendEmail(newUser);
				Console.WriteLine(ans1);

				Console.WriteLine($"----------Test Batch Server1 : Update the Record user Batch----------\n");

				var ans2 = await userBatchService.UpdateUserRecords(newUser);
				Console.WriteLine(ans2);



				await Task.WhenAll();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Test Batch Server1 : An error occurred: {ex.Message}");
			}
			Console.WriteLine("Batch service testing completed.");

			Console.WriteLine("Press Enter to stop the batch service.");
			userBatchService.StopBatchProcessing();
			Console.ReadLine();
			
		}





	}
}
