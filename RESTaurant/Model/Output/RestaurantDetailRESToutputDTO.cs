namespace RESTaurant.Model.Output
{
    public class RestaurantDetailRESToutputDTO
    {
        public RestaurantDetailRESToutputDTO(string id, string name, LocationRESToutputDTO location, string kitchen, string email, string phone, Dictionary<int, int> tablenumberSeats)
        {
            Id = id;
            Name = name;
            Location = location;
            Kitchen = kitchen;
            Email = email;
            Phone = phone;
            TablenumberSeats = tablenumberSeats;
            foreach (int seats in tablenumberSeats.Values)
            {
                if (AmountOfTablesWithXSeats.ContainsKey(seats)) { AmountOfTablesWithXSeats[seats]++; } else { AmountOfTablesWithXSeats.Add(seats, 1); }
            }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Kitchen { get; set; }
        public LocationRESToutputDTO Location { get; set; }
        public Dictionary<int, int> AmountOfTablesWithXSeats { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> TablenumberSeats { get; set; }
    }
}
