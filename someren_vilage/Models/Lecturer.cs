using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace someren_vilage.Models
{
    public class Lecturer
    {
        public int LecturerId { get; set; } // incremented by DB

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public int Age { get; set; }

        public int? RoomId { get; set; }
        //public Room Room { get; set; }

        //public Lecturer() 
        //{
        //}

        //public Lecturer(int lecturerId, string firstName, string lastName, string phoneNumber, int age, int roomId)
        //{
        //    //
        //    LecturerId = lecturerId;
        //    FirstName = firstName;
        //    LastName = lastName;
        //    PhoneNumber = phoneNumber;
        //    Age = age;
        //    RoomId = roomId;
        //}
    }
}
