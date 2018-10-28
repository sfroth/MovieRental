using MovieRental.Entities;
using MovieRental.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRental.Business.Service
{
	public class MovieService
	{
		private readonly DataContext _context;

		public MovieService(DataContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Get Movie by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Movie Get(int id)
		{
			return _context.Movies.FirstOrDefault(m => m.ID == id);
		}
	}
}
