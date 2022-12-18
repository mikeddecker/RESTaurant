using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.EFModel {
    [Table("Location")]
    public class LocationEF {
        public LocationEF() {
        }

        public LocationEF(int postalCode, string city, string? street, string? housenumberLabel) {
            PostalCode = postalCode;
            City = city;
            Street = street;
            HousenumberLabel = housenumberLabel;
        }

        public LocationEF(int locationId, int postalCode, string city, string? street, string? housenumberLabel) : this(postalCode, city, street, housenumberLabel) {
            LocationId = locationId;

        }

        [Key]
        [Column(TypeName = "INT")]
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

        [Column(TypeName = "BIT")]
        public bool? IsDeleted { get; set; } = false; // Default value false is set in OnModelCreating
    }
}
