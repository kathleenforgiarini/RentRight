﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.ComponentModel;

namespace RentRight.Models
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string StNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public User Owner { get; set; }
        public User Manager { get; set; }
        public byte[] Photo { get; set; } = new byte[0];
        [NotMapped]
        [Required(ErrorMessage = "Please select a photo.")]
        public IFormFile PhotoFile { get; set; }

    }
}
