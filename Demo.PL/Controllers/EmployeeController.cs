using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using Demo.PL.Helpers;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppDBContext context;
        private readonly IMapper mapper;

        public EmployeeController(IUnitOfWork _unitOfWork, AppDBContext _context, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            context = _context;
            mapper = _mapper;
        }
        public IActionResult Index(string SearchValue = "")
        {
            IEnumerable<Employee> employees;
            IEnumerable<EmployeeViewModel> employeesViewModel;

            if (string.IsNullOrEmpty(SearchValue))
                employees = unitOfWork.EmployeeRepository.GetAll();
             else
                employees = unitOfWork.EmployeeRepository.Search(SearchValue);
            employeesViewModel = mapper.Map<IEnumerable<EmployeeViewModel>>(employees);

            return View(employeesViewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departments = unitOfWork.EmployeeRepository.GetAll();
            return View(new EmployeeViewModel());
        }
        [HttpPost]
        public IActionResult Create(EmployeeViewModel employeeView)
        {
            //ModelState["Department"].ValidationState = ModelValidationState.Valid;
            if (ModelState.IsValid)
            {
                // Manual Mapping
                //Employee employee = new Employee
                //{
                //    Name = employeeView.Name,
                //    Address = employeeView.Address,
                //    DepartmentId = employeeView.DepartmentId,
                //    Email = employeeView.Email,
                //    HireDate = employeeView.HireDate,
                //    IsActive = employeeView.IsActive,
                //    Salary = employeeView.Salary
                //};
                var employee = mapper.Map<Employee>(employeeView); // Mapping
                employee.ImageUrl = DocumentSettings.UploadFile(employeeView.Image, "Images");

                unitOfWork.EmployeeRepository.Add(employee);
                TempData["SuccessMessage"] = "This Employee was added Sucessfully!";
                unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();
            return View(employeeView);
        }
        [HttpGet]
        public IActionResult Details(int? id)
        {
            try
            {
                if (id is null)
                {
                    TempData["WarningMessage"] = "There is No Employee with this Id";
                    return RedirectToAction(nameof(Index));
                }
                var employee = unitOfWork.EmployeeRepository.GetById(id);
                if (employee is null)
                {
                    TempData["WarningMessage"] = "There is No Employee with this Id";
                    return RedirectToAction(nameof(Index));
                }
                var department = context.Departments.Find(employee.DepartmentId);
                
                ViewData["DepartmentName"] = department?.Name;
                EmployeeViewModel employeeViewModel = mapper.Map<EmployeeViewModel>(employee);
                return View(employeeViewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpGet]
        public IActionResult Update(int? id)
        {
            var employee = unitOfWork.EmployeeRepository.GetById(id);
            //EmployeeViewModel employeeViewModel = new EmployeeViewModel
            //{
            //    Name = employee.Name,
            //    Address = employee.Address,
            //    DepartmentId = employee.DepartmentId,
            //    Email = employee.Email,
            //    HireDate = employee.HireDate,
            //    IsActive = employee.IsActive,
            //    Salary = employee.Salary
            //};

            ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();

            var employeeViewModel = mapper.Map<EmployeeViewModel>(employee);
            var imageUrl = employee.ImageUrl;
            if (imageUrl != null)
                DocumentSettings.DeleteFile(imageUrl, "Images");

            return View(employeeViewModel);
        }
        [HttpPost]
        public IActionResult Update(EmployeeViewModel employeeView)
        {
            //Employee employee = new Employee
            //{
            //    Name = employeeView.Name,
            //    Address = employeeView.Address,
            //    DepartmentId = employeeView.DepartmentId,
            //    Email = employeeView.Email,
            //    HireDate = employeeView.HireDate,
            //    IsActive = employeeView.IsActive,
            //    Salary = employeeView.Salary
            //};

            var employee = mapper.Map<Employee>(employeeView);
            
            employee.ImageUrl = DocumentSettings.UploadFile(employeeView.Image, "Images");

            unitOfWork.EmployeeRepository.Update(employee);
            unitOfWork.Complete();

            TempData["SuccessMessage"] = "Update was successful.";
            ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(EmployeeViewModel employeeViewModel)
        {
            var employee = mapper.Map<Employee>(employeeViewModel);
            employee.ImageUrl = context.Employees.Where(x => x.Id == employee.Id).Select(x => x.ImageUrl).FirstOrDefault();
            var imageUrl = employee.ImageUrl;
           

            unitOfWork.EmployeeRepository.Delete(employee);
            unitOfWork.Complete();
            if (imageUrl != null)
                DocumentSettings.DeleteFile(imageUrl, "Images");
            TempData["SuccessMessage"] = "Delete was successful.";
            return RedirectToAction(nameof(Index));
        }

    }
}
