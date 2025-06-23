using System.ComponentModel.DataAnnotations;

namespace myapp.Models
{
    public class StudentModel
    {      
        public int StudentID { get; set; }
        [Required(ErrorMessage = "Student Name is required")]
        public string StudentName { get; set; }
        [Required(ErrorMessage = "Enrollment Number is required")]
        public string EnrollmentNumber { get; set; }
        [Required(ErrorMessage = "Semester is required")]
        public int Semester { get; set; }
        [Required(ErrorMessage = "CGPA is required")]
        [Range(0,10,ErrorMessage ="CGPA Should in between 0 - 10")]
        public decimal CGPA { get; set; }
    }
}
