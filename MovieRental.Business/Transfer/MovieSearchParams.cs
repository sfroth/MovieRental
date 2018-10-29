using System;

namespace MovieRental.Business.Transfer
{
	public class MovieSearchParams
	{
		public string SearchTerm { get; set; }
		public DateTime? ReleaseFrom { get; set; }
		public DateTime? ReleaseTo { get; set; }

		public override int GetHashCode()
		{
			return string.Format("{0}-{1}-{2}", SearchTerm, ReleaseFrom == null ? "n" : ReleaseFrom.Value.ToString("yyyyMMdd"), ReleaseTo == null ? "n" : ReleaseTo.Value.ToString("yyyyMMdd")).GetHashCode();
		}
	}
}
