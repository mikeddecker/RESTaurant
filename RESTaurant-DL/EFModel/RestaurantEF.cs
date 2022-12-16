using RESTaurant_DL.EFModel;
using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.EFModel
{
    [Table("Restaurant")]
    public class RestaurantEF
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RestaurantEF()
        {
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RestaurantEF(string name, string kitchen, string email, string phone, int postalCode, string city)
        {
            Name = name;
            Kitchen = kitchen;
            Email = email;
            Phone = phone;
            PostalCode = postalCode;
            City = city;
        }

        public RestaurantEF(int restaurantId, string name, string kitchen, string email, string phone, int postalCode, string city)
        {
            RestaurantId = restaurantId;
            Name = name;
            Kitchen = kitchen;
            Email = email;
            Phone = phone;
            PostalCode = postalCode;
            City = city;
        }

        [Key]
        public int RestaurantId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Kitchen { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(320)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Phone { get; set; }

        [Required]
        [Column(TypeName = "INT")]
        public int PostalCode { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string City { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Street { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string? HousenumberLabel { get; set; }

        [Column(TypeName = "BIT")]
        public bool? IsDeleted { get; set; } = false; // Default value false is set in OnModelCreating

        [ForeignKey("RestaurantId")]
        public ICollection<TableEF> Tables { get; set; }
    }
}
