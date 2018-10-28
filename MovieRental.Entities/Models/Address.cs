using System.ComponentModel.DataAnnotations;

namespace MovieRental.Entities.Models
{
	/// <summary>
	/// Address object. Used to represent a kiosk location, account home, or payment billing address
	/// </summary>
	public class Address
	{
		public int ID { get; set; }
		public string Name { get; set; }
		[Required]
		public string StreetAddress1 { get; set; }
		public string StreetAddress2 { get; set; }
		[Required]
		public string City { get; set; }
		public string StateProvince { get; set; }
		[Required]
		public string Country { get; set; }
		public string PostalCode { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }

		///// <summary>
		///// Fill Latitude/Longitude for address
		///// TODO: Enhancement option: Use Google Maps API or other option to improve geocoding
		///// </summary>
		//public void Geocode()
		//{
		//	if (string.IsNullOrWhiteSpace(PostalCode))
		//	{
		//		throw new ArgumentException("Cannot geocode without postal code");
		//	}
		//	var coords = ZipCode.Spatial.Search(PostalCode);
		//}
	}
}
