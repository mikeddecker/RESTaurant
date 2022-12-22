using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.EFModel {
    [Table("Reservation")]
    public class ReservationEF {
        public ReservationEF() {
        }

        public ReservationEF(RestaurantEF restaurant, CustomerEF customer, TableEF table, int seats, DateTime date) {
            Restaurant = restaurant;
            Customer = customer;
            Table = table;
            Seats = seats;
            Date = date;
        }

        public ReservationEF(int reservationId, RestaurantEF restaurant, CustomerEF customer, TableEF table, int seats, DateTime date, bool isCanceled) {
            ReservationId = reservationId;
            Restaurant = restaurant;
            Customer = customer;
            Table = table;
            Seats = seats;
            Date = date;
            IsCanceled = isCanceled;
        }

        [Key]
        [Column(TypeName = "INT")]
        public int ReservationId { get; set; }

        [Required]
        //[ForeignKey("RestaurantId")]
        public RestaurantEF Restaurant { get; set; }

        [Required]
        //[ForeignKey("CustomerId")]
        public CustomerEF Customer { get; set; }

        [Required]
        [ForeignKey("UnusedFakeTableId")]
        public TableEF Table { get; set; }

        [Required]
        public int Seats { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsDeleted { get; set; } // Default false

        [Required]
        public bool IsCanceled { get; set; } // Default false
    }
}
