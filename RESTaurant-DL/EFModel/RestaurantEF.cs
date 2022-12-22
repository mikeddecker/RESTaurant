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
        public RestaurantEF()
        {
        }
        public RestaurantEF(string name, string kitchen, string email, string phone, LocationEF location)
        {
            Name = name;
            Kitchen = kitchen;
            Email = email;
            Phone = phone;
            Location = location;
        }

        public RestaurantEF(int restaurantId, string name, string kitchen, string email, string phone, LocationEF location) : this (name, kitchen, email, phone, location)
        {
            RestaurantId = restaurantId;
        }

        [Key]
        [Column(TypeName = "INT")]
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
        [ForeignKey("LocationId")]
        public LocationEF Location { get; set; }

        [Required]
        [Column(TypeName = "BIT")]
        public bool? IsDeleted { get; set; } = false; // Default value false is set in OnModelCreating

        [ForeignKey("RestaurantId")]
        public ICollection<TableEF> Tables { get; set; }

        public List<ReservationEF> Reservations { get; set; }
    }
}
