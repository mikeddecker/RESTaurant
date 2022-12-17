using RESTaurant_BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Model {
    public class Table {
        private int tableNumber;
        private int seats;

        public Table(int tableNumber, int seats) {
            TableNumber = tableNumber;
            SetSeats(seats);
        }

        public int TableNumber { get => tableNumber; private set => tableNumber = value; }
        public int Seats { get => seats; private set => SetSeats(value); }

        public override bool Equals(object? obj) {
            return obj is Table table &&
                   TableNumber == table.TableNumber &&
                   Seats == table.Seats;
        }

        public void SetSeats(int seats) {
            if (seats > 0) { this.seats = seats; } else { throw new TableException($"{nameof(SetSeats)} - A table must have more than 0 seats"); }
        }


    }
}
