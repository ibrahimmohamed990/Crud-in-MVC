﻿using Demo.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Address { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType("money")]
        public double Salary { get; set; }
        public bool IsActive { get; set; }
        public DateTime HireDate { get; set; } = DateTime.Now;
        public IFormFile Image { get; set; }
        public string? ImageUrl { get; set; }
        public int DepartmentId { get; set; }
    }
}
