using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_DL.EFModel {
    public class RestaurantEF {
        public RestaurantEF() {
        }

        public RestaurantEF(int restaurantId, string name, LocationEF location, string kitchen, string email, string phone) {
            RestaurantId = restaurantId;
            Name = name;
            Location = location;
            Kitchen = kitchen;
            Email = email;
            Phone = phone;
        }

        [Key]
        public int RestaurantId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required]
        public LocationEF Location { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Kitchen { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(320)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Phone { get; set; }

        
    }
}
