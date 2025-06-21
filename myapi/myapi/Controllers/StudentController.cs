using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using myapi.Models;
using myapi.Repository;

namespace myapi.Controllers
{
    [Route("apiv1/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentRepository studentRepository;

        public StudentController(StudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            try
            {
                var slist = studentRepository.GetStudentData();

                if (slist.IsNullOrEmpty())
                {
                    return NotFound(new { status = "Failure", message = "No students found." });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Students retrieved successfully.", data = slist });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while retrieving students.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentByID(int id)
        {
            try
            {
                var student = studentRepository.GetStudentByID(id);

                if (student == null || student.StudentID == 0)
                {
                    return NotFound(new { status = "Failure", message = $"Student with ID {id} not found." });
                }
                else
                {
                    return Ok(new { status = "Success", message = "Student retrieved successfully.", data = student });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while retrieving the student.", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult InsertStudent(StudentModel studentModel)
        {
            try
            {
                if (studentModel == null)
                {
                    return BadRequest(new { status = "Failure", message = "Invalid student data." });
                }
                bool isInserted = studentRepository.InsertStudent(studentModel);
                if (isInserted)
                {
                    return Ok(new { status = "Success", message = "Student inserted successfully." });
                }
                else
                {
                    return StatusCode(500, new { status = "Failure", message = "An error occurred while inserting the student." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while inserting the student.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, StudentModel studentModel)
        {
            try
            {
                if (studentModel == null)
                {
                    return BadRequest(new { status = "Failure", message = "Invalid student data." });
                }
                bool isUpdated = studentRepository.UpdateStudent(id, studentModel);
                if (isUpdated)
                {
                    return Ok(new { status = "Success", message = "Student updated successfully." });
                }
                else
                {
                    return StatusCode(500, new { status = "Failure", message = "An error occurred while updating the student." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while updating the student.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            try
            {
                var student = studentRepository.DeleteStudent(id);
                if (student)
                {
                    return Ok(new { status = "Success", message = "Student deleted successfully." });
                }
                else
                {
                    return NotFound(new { status = "Failure", message = $"Student with ID {id} not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while deleting the student.", error = ex.Message });
            }
        }
    }
}