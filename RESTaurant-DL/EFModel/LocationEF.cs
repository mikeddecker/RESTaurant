using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RESTaurant_DL.EFModel {
    [Table("Location")]
    public class LocationEF {
        public LocationEF() {
        }

        public LocationEF(int locationId, int postalCode, string city) {
            LocationId = locationId;
            PostalCode = postalCode;
            City = city;
        }

        [Key]
        public int LocationId { get; set; }

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
    }
}