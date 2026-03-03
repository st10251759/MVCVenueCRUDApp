using System.ComponentModel.DataAnnotations;

namespace MVCFirstBasicApp.Models
{
    /// <summary>
    /// m - model
    /// class = table in db
    /// </summary>
    public class Venue
    {

        // id = primary key
        //writes to + reads from the database

        public int id { get; set; }

        ///Venue name:
        ///[required] = not null
        ///string length prevent unrealistic data

        [Required(ErrorMessage = "Venue name is a required field.")]
        [StringLength(50, ErrorMessage = "Venue name cannot exceed 50 characters.")]
        public string name { get; set; } = string.Empty;

        //venue location
        [StringLength(100, ErrorMessage = "Venue location cannot exceed 100 characters.")]
        public string Address { get; set; } = string.Empty;

        //venue capacity
        //number of people the venue can accommodate
        //add a range
        [Range(1, 20000, ErrorMessage = "Venue capacity must be between 1 and 20,000.")]

        public int venueCapacity { get; set; }



    }
}