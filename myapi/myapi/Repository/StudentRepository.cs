using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.SqlClient;
using MongoDB.Bson;
using MongoDB.Driver;
using myapi.Models;
using myapi.Services;
using System.Data;
using System.Threading.Tasks;

namespace myapi.Repository
{
    public class StudentRepository
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<StudentAttendanceModel> collection;

        public StudentRepository(IConfiguration config,MongoServices services)
        {
            this._config = config;
            this.collection = services.GetCollection<StudentAttendanceModel>("students");
        }

        public SqlCommand Connection(CommandType command)
        {
            SqlConnection cons = new SqlConnection(this._config.GetConnectionString("ConnectionString"));
            cons.Open();
            SqlCommand cmd = cons.CreateCommand();
            cmd.CommandType = command;
            return cmd;
        }

        public IEnumerable<StudentModel> GetStudentData()
        {
            SqlCommand cmd = Connection(CommandType.StoredProcedure);
            cmd.CommandText = "PR_Student_SelectAll";
            List<StudentModel> slist = new List<StudentModel>();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                StudentModel sm = new StudentModel();
                sm.StudentID = reader.GetInt32("StudentID");
                sm.StudentName = reader.GetString("StudentName");
                sm.EnrollmentNumber = reader.GetString("EnrollmentNumber");
                sm.Semester = reader.GetInt32("Semester");
                sm.CGPA = reader.GetDecimal("CGPA");
                slist.Add(sm);
            }

            return slist;
        }

        public StudentModel GetStudentByID(int id)
        {
            SqlCommand cmd = Connection(CommandType.StoredProcedure);
            cmd.CommandText = "PR_Student_SelectByPK";
            cmd.Parameters.AddWithValue("@StudentID", id);

            SqlDataReader reader = cmd.ExecuteReader();
            StudentModel sm = new StudentModel();

            while (reader.Read())
            {
                sm.StudentID = reader.GetInt32("StudentID");
                sm.StudentName = reader.GetString("StudentName");
                sm.EnrollmentNumber = reader.GetString("EnrollmentNumber");
                sm.Semester = reader.GetInt32("Semester");
                sm.CGPA = reader.GetDecimal("CGPA");
            }

            return sm;
        }

        public Boolean InsertStudent(StudentModel sm)
        {
            SqlCommand cmd = Connection(CommandType.StoredProcedure);
            cmd.CommandText = "PR_Student_Insert";
            cmd.Parameters.AddWithValue("@StudentName", sm.StudentName);
            cmd.Parameters.AddWithValue("@EnrollmentNumber", sm.EnrollmentNumber);
            cmd.Parameters.AddWithValue("@Semester", sm.Semester);
            cmd.Parameters.AddWithValue("@CGPA", sm.CGPA);
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        public Boolean UpdateStudent(int id, StudentModel sm)
        {
            SqlCommand cmd = Connection(CommandType.StoredProcedure);
            cmd.CommandText = "PR_Student_UpdateByPK";
            cmd.Parameters.AddWithValue("@StudentID", id);
            cmd.Parameters.AddWithValue("@StudentName", sm.StudentName);
            cmd.Parameters.AddWithValue("@EnrollmentNumber", sm.EnrollmentNumber);
            cmd.Parameters.AddWithValue("@Semester", sm.Semester);
            cmd.Parameters.AddWithValue("@CGPA", sm.CGPA);
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        public Boolean DeleteStudent(int id)
        {
            SqlCommand cmd = Connection(CommandType.StoredProcedure);
            cmd.CommandText = "PR_Student_DeleteByPK";
            cmd.Parameters.AddWithValue("@StudentID", id);            
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }

        public Boolean UserLogin(UserLoginModel lm)
        {
            SqlCommand cmd = Connection(CommandType.Text);
            cmd.CommandText = "Select Count(*) from Login where Username = @UserName and Password = @Password";
            cmd.Parameters.AddWithValue("@Username", lm.UserName);
            cmd.Parameters.AddWithValue("@Password", lm.Password); 
            int result = Convert.ToInt32(cmd.ExecuteScalar());

            return result > 0;
        }

        public async Task<IEnumerable<StudentAttendanceModel>> GetStudentAttendances()
        {            
            var slist = await collection.Find(_=> true).ToListAsync();
            return slist;
        }

        public async Task<StudentAttendanceModel> GetStudentByID(string id)
        {
            return await collection.Find(x => x.StudentId == id).FirstOrDefaultAsync();
        }

        public async Task<bool> InsertStudent(StudentAttendanceModel sm)
        {
            await collection.InsertOneAsync(sm);
            return true;
        }

        public async Task<bool> UpdateStudent(string id,StudentAttendanceModel sm)
        {
            var result = await collection.ReplaceOneAsync(x => x.StudentId == id, sm);
            return result.ModifiedCount > 0;
        }

        public bool DeleteStudent(string id)
        {
            var result = collection.DeleteOne(x => x.StudentId == id);
            return result.DeletedCount > 0;
        }
    }
}
