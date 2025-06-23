using Microsoft.AspNetCore.Mvc;
using myapp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace myapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;

        public HomeController()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:44310/apiv1")
            };
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StudentList()
        {
            List<StudentModel> students = new();

            try
            {
                var res = await _client.GetAsync($"{_client.BaseAddress}/Student/GetStudents");

                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<JObject>(data);

                    if (jsonObject?["status"]?.ToString().ToLower() == "success")
                    {
                        students = JsonConvert.DeserializeObject<List<StudentModel>>(jsonObject["data"]?.ToString());
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No student data found.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error: {res.StatusCode} - {res.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception: {ex.Message}";
            }

            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> StudentAddEdit(int? id)
        {
            if (id == null || id == 0)
                return View(new StudentModel());

            StudentModel student = new StudentModel();

            try
            {
                var res = await _client.GetAsync($"{_client.BaseAddress}/Student/GetStudentByID/{id}");

                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<JObject>(data);

                    if (jsonObject?["status"]?.ToString().ToLower() == "success")
                    {
                        student = JsonConvert.DeserializeObject<StudentModel>(jsonObject["data"]?.ToString());
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No student data found.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Error fetching student data.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception: {ex.Message}";
            }

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> StudentSave(StudentModel sm)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(sm);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage res;

                if (sm.StudentID == 0)
                {
                    res = await _client.PostAsync($"{_client.BaseAddress}/Student/InsertStudent", content);
                }
                else
                {
                    res = await _client.PutAsync($"{_client.BaseAddress}/Student/UpdateStudent/{sm.StudentID}", content);
                }

                if (res.IsSuccessStatusCode)
                {
                    TempData["Message"] = $"Student {(sm.StudentID > 0 ? "updated" : "inserted")} successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to {(sm.StudentID == 0 ? "insert" : "update")} student.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception: {ex.Message}";
            }

            return RedirectToAction("StudentList");
        }

        [HttpPost]
        public async Task<IActionResult> StudentDelete(int id)
        {
            try
            {
                var res = await _client.DeleteAsync($"{_client.BaseAddress}/Student/DeleteStudent/{id}");

                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<JObject>(data);

                    if (jsonObject?["status"]?.ToString().ToLower() == "success")
                    {
                        TempData["Message"] = "Student deleted successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to delete the student.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Server error while deleting student.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception: {ex.Message}";
            }

            return RedirectToAction("StudentList");
        }
    }
}
