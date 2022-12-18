using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.EFModel {
    [Table("Customer")]
    public class CustomerEF {
        [Key]
        [Column(TypeName = "INT")]
        public int CustomerId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(320)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public string Phone { get; set; }

        [Required]
        [ForeignKey("LocationId")]
        [Column(TypeName = "INT")]
        public LocationEF Location { get; set; }

        [Required]
        [Column(TypeName = "BIT")]
        public bool? IsDeleted { get; set; } = false;
    }
}
