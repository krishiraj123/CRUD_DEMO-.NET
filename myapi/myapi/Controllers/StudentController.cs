using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using myapi.Models;
using myapi.Repository;
using myapi.Services;

namespace myapi.Controllers
{
    [Route("apiv1/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly StudentRepository studentRepository;
        private readonly JwtServices jwtServices;

        public StudentController(StudentRepository studentRepository,JwtServices jwtServices)
        {
            this.studentRepository = studentRepository;
            this.jwtServices = jwtServices;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            try
            {
                var slist = studentRepository.GetStudentData();

                if (slist == null || !slist.Any())
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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult UserLogin(UserLoginModel lm)
        {
            try
            {
                if (lm == null)
                {
                    return Unauthorized(new { status = "Failure", message = "Invalid student data." });
                }
                bool isInserted = studentRepository.UserLogin(lm);
                if (isInserted)
                {
                    var token = jwtServices.GenerateToken(lm);

                    return Ok(new { status = "Success", message = "Student Login successfully.",authtoken = token });
                }
                else
                {
                    return StatusCode(500, new { status = "Failure", message = "Username or Password is incorrect" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while inserting the student.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentAttendances()
        {
            try
            {
                var attendances = await studentRepository.GetStudentAttendances();

                if (attendances == null || !attendances.Any())
                {
                    return NotFound(new { status = "Failure", message = "No attendance records found." });
                }

                return Ok(new { status = "Success", message = "Attendance records retrieved successfully.", data = attendances });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while retrieving attendance records.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentAttendanceByID(string id)
        {
            try
            {
                var record = await studentRepository.GetStudentByID(id);
                if (record == null)
                {
                    return NotFound(new { status = "Failure", message = $"Attendance record with ID {id} not found." });
                }

                return Ok(new { status = "Success", message = "Attendance record retrieved successfully.", data = record });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while retrieving the attendance record.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertAttendance(StudentAttendanceModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { status = "Failure", message = "Invalid attendance data." });
                }

                await studentRepository.InsertStudent(model);
                return Ok(new { status = "Success", message = "Attendance record inserted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while inserting attendance.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(string id, StudentAttendanceModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { status = "Failure", message = "Invalid attendance data." });
                }

                var result = await studentRepository.UpdateStudent(id, model);
                if (result)
                {
                    return Ok(new { status = "Success", message = "Attendance record updated successfully." });
                }

                return NotFound(new { status = "Failure", message = $"Attendance record with ID {id} not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while updating attendance.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAttendance(string id)
        {
            try
            {
                var result = studentRepository.DeleteStudent(id);
                if (result)
                {
                    return Ok(new { status = "Success", message = "Attendance record deleted successfully." });
                }

                return NotFound(new { status = "Failure", message = $"Attendance record with ID {id} not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Failure", message = "An error occurred while deleting attendance.", error = ex.Message });
            }
        }
    }
}