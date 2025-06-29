﻿using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Data.SqlClient;
using myapi.Models;
using System.Data;

namespace myapi.Repository
{
    public class StudentRepository
    {
        private readonly IConfiguration _config;

        public StudentRepository(IConfiguration config)
        {
            this._config = config;
        }

        public SqlCommand Connection()
        {
            SqlConnection cons = new SqlConnection(this._config.GetConnectionString("ConnectionString"));
            cons.Open();
            SqlCommand cmd = cons.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        public IEnumerable<StudentModel> GetStudentData()
        {
            SqlCommand cmd = Connection();
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
            SqlCommand cmd = Connection();
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
            SqlCommand cmd = Connection();
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
            SqlCommand cmd = Connection();
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
            SqlCommand cmd = Connection();
            cmd.CommandText = "PR_Student_DeleteByPK";
            cmd.Parameters.AddWithValue("@StudentID", id);            
            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }
    }
}
