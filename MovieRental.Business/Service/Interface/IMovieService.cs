using MovieRental.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental.Business.Service.Interface
{
	public interface IMovieService
	{
		/// <summary>
		/// Get Movie by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Movie Get(int id);
	}
}
