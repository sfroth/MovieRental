using System.ComponentModel.DataAnnotations;

namespace MovieRental.Entities.Models
{
	/// <summary>
	/// Movie in-stock at a kiosk
	/// </summary>
	public class KioskMovie
	{
		public int ID { get; set; }
		[Required]
		public virtual Kiosk Kiosk { get; set; }
		[Required]
		public virtual Movie Movie { get; set; }
		public int Stock { get; set; }
	}
}
