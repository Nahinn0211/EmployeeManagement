﻿using System;
using System.Collections.Generic;

namespace EmployeeManagement.Models.Entity
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; } // Được tạo từ FirstName + LastName trong DB
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IDCardNumber { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int DepartmentID { get; set; }
        public int PositionID { get; set; }
        public int? ManagerID { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string TaxCode { get; set; }
        public string InsuranceCode { get; set; }
        public string FaceDataPath { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Department Department { get; set; }
        public Position Position { get; set; }
        public Employee Manager { get; set; }
        public List<Employee> Subordinates { get; set; } = new List<Employee>();
        public User User { get; set; }
        public List<Project> ManagedProjects { get; set; } = new List<Project>();
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Task> Tasks { get; set; } = new List<Task>();
        public List<Document> Documents { get; set; } = new List<Document>();
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
        public List<Salary> Salaries { get; set; } = new List<Salary>();
        public List<Finance> Finances { get; set; } = new List<Finance>();
    }
}