using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieRental.API.Models
{
    public class KioskModel
    {
        // ID exists here for output to user only
        public int? ID { get; set; }
        [Required]
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
    }
}