using System.ComponentModel.DataAnnotations;

namespace MovieRental.Entities.Models
{
	/// <summary>
	/// Kiosk Details
	/// </summary>
	public class Kiosk
	{
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public virtual Address Address { get; set; }
	}
}
