using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly AppDBContext context;
        public EmployeeRepository(AppDBContext _context) : base(_context)
        {
            context = _context;
        }

        public IEnumerable<Employee> GetByDepartmentName(string departmentName)
        {
            var employees = context.Employees.Include(x => x.Department).Where(x => x.Department.Name == departmentName);
            return employees;
        }

        public IEnumerable<Employee> Search(string word)
        {
            var employee = context.Employees.Where(x => 
                                  x.Name.Trim().ToLower().Contains(word.Trim().ToLower()) ||
                                  x.Email.Trim().ToLower().Contains(word.Trim().ToLower()) ||
                                  x.Address.Trim().ToLower().Contains(word.Trim().ToLower())).Include(e => e.Department);
            return employee;
        }
        public new IEnumerable<Employee> GetAll()
        {
            return context.Employees.Include(e => e.Department).ToList();
        }
        //public int Add(Employee employee)
        //{
        //    context.Employees.Add(employee);
        //    return context.SaveChanges();
        //}
        //public int Delete(Employee employee)
        //{
        //    context.Employees.Remove(employee);
        //    return context.SaveChanges();
        //}
        //public IEnumerable<Employee> GetAll()
        //{
        //    return context.Employees.ToList();
        //}
        //public Employee GetById(int id)
        //{
        //    return context.Employees.FirstOrDefault(x => x.Id == id);
        //}
        //public int Update(Employee employee)
        //{
        //    context.Employees.Remove(employee);
        //    return context.SaveChanges();
        //}
    }
}
