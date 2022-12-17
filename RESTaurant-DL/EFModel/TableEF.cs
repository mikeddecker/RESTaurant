using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_DL.EFModel {
    [Table("Table")]
    public class TableEF {
        public TableEF()
        {
        }

        public TableEF(int restaurantId, int tableNumber, int seats)
        {
            RestaurantId = restaurantId;
            Tablenumber = tableNumber;
            Seats = seats;
        }

        [Key]
        public int UnusedFakeTableId { get; set; }

        [Required]
        [ForeignKey("RestaurantId")]
        [Column(TypeName = "INT")]
        public int RestaurantId { get; set; }

        [Required]
        [Column(TypeName = "INT")]
        public int Tablenumber { get; set; }
        [Required]
        [Column(TypeName = "INT")]
        public int Seats { get; set; }

        [Required]
        [Column(TypeName = "BIT")]
        public bool IsDeleted { get; set; }

    }
}
