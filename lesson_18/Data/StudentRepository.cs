using System;
using System.Collections.Generic;
using College_DB.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Linq;

namespace College_DB.Data
{
    class StudentRepository
    {
        private readonly string _conn_str;
        public StudentRepository(string connection_string)
        {
            _conn_str = connection_string;
        }

        public List<Student> GetAllStudents()
        {
            using (var connection = new SqlConnection(_conn_str))
            {
                return connection.Query<Student>("SELECT StudentId, FirstName, LastName, Age FROM Students").ToList();
            }
        }

        public Student GetStudentById(string Id)
        {
            using (var connection = new SqlConnection(_conn_str))
            {
                return connection.QueryFirstOrDefault<Student>("SELECT FirstName, LastName, Age, GroupId, FROM Students WHERE StudentId = @id", new { @id = Id });
            }
        }

        public int CreateStudent(Student student)
        {
            using (var connection = new SqlConnection(_conn_str))
            {
                string sql = "INSERT INTO Students(FirstName, LastName, Age, GroupId) OUTPUT INSERTED.StudentId VALUES(@FirstName, @LastName, @Age, @GroupId);";

                return connection.QuerySingle<int>(sql, student);
            }
        }

        public void AddStudent(string first_name, string last_name, int age, int group_id)
        {
            var student = new Student();
            student = new Student
            {
                FirstName = first_name,
                LastName = last_name,
                Age = age,
                GroupId = group_id
            };

            CreateStudent(student);
        }

        public void UpdateStudent(int id, Student student)
        {
            using (var connection = new SqlConnection(_conn_str))
            {

            }
        }
    }

}