using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly AppDBContext context;
        public DepartmentRepository(AppDBContext _context) : base(_context)
        {
            context = _context;
        }

        public IEnumerable<Department> GetDepartmentById(int id)
        {
            var department = context.Departments.Where(x => x.Id == id);
            return department;
        }

        public IEnumerable<Department> GetDepartmentByNameORCode(string nameORcode)
        {
            var department = context.Departments.Where(x => x.Name.Trim().ToLower().Contains(nameORcode.Trim().ToLower()) ||
                                                x.Code.Trim().ToLower() == nameORcode.Trim().ToLower()).ToList();
            return department;
        }
        //public int Add(Department department)
        //{
        //    if(context.Departments.FirstOrDefault(x => x.Code == department.Code) == null)
        //    {
        //        context.Departments.Add(department);
        //        return context.SaveChanges();
        //    }
        //    return -1;
        //}

        //public int Delete(Department department)
        //{
        //    context.Departments.Remove(department);
        //    return context.SaveChanges();
        //}

        //public IEnumerable<Department> GetAll()
        //{
        //    return context.Departments.ToList();
        //}

        //public Department GetById(int? id)
        //{
        //    return context.Departments.Find(id);
        //}

        //public int Update(Department department)
        //{
        //    context.Departments.Update(department);
        //    return context.SaveChanges();
        //}
    }
}
