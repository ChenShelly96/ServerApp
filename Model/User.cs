using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ServerApp.Model
{
	public class User
	{
		public User(){}
		[Key]
		public int ID { get; set; }
		[Required]
		[Column(TypeName = "nvarchar(255)")]
		public string Name { get; set; }
		[Required]
		[Column(TypeName = "nvarchar(255)")]
		public string Email { get; set; }
		[Required]
		[Column(TypeName = "varchar(255)")]
		public string Password { get; set; }

	}
}
