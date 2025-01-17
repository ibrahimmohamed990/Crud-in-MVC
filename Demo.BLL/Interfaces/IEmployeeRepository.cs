﻿using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        //Employee GetById(int id);
        //IEnumerable<Employee> GetAll();
        //int Add(Employee employee);
        //int Update(Employee employee);
        //int Delete(Employee employee);
        public IEnumerable<Employee> Search(string word);
        public IEnumerable<Employee> GetByDepartmentName(string departmentName);
    }
}
