using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
        //private readonly IDepartmentRepository departmentRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<DepartmentController> logger;
        private readonly AppDBContext context;
        private readonly IMapper mapper;

        public DepartmentController(
            //IDepartmentRepository _departmentRepository, 
            IUnitOfWork _unitOfWork,
            ILogger<DepartmentController> _logger,
            AppDBContext _context, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            //departmentRepository = _departmentRepository;
            logger = _logger;
            context = _context;
            mapper = _mapper;
        }
        public IActionResult Index(string SearchValue = "")
        {
            IEnumerable<Department> departments;
            

            if ( int.TryParse(SearchValue , out int id))
                departments = unitOfWork.DepartmentRepository.GetDepartmentById(id);
            else if(!string.IsNullOrEmpty(SearchValue))
                departments = unitOfWork.DepartmentRepository.GetDepartmentByNameORCode(SearchValue);
            else
                departments = unitOfWork.DepartmentRepository.GetAll();
            var departmentsViewModel = mapper.Map<IEnumerable<DepartmentViewModel>>(departments);
            return View(departmentsViewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new DepartmentViewModel());
        }
        [HttpPost]
        public IActionResult Create(DepartmentViewModel departmentViewModel)
        {
            if (ModelState.IsValid)
            {
                var department = mapper.Map<Department>(departmentViewModel);
                var IsExist = context.Departments.FirstOrDefault(x => x.Code == department.Code);
                
                if (IsExist != null)
                {
                    TempData["WarningMessage"] = "There is Department with this Data";
                    ModelState.Clear();
                    return View(departmentViewModel);
                    //return RedirectToAction(nameof(Index));
                }
                unitOfWork.DepartmentRepository.Add(department);
                TempData["SuccessMessage"] = "This Department was added Sucessfully!";
                TempData.Keep("SuccessMessage");
                unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentViewModel);
        }
        [HttpGet]
        public IActionResult Details(int? id)
        {
            try
            {
                if (id is null)
                {
                    TempData["WarningMessage"] = "There is No Department with this Id";
                    return RedirectToAction(nameof(Index));
                }
                var department = unitOfWork.DepartmentRepository.GetById(id);
                if (department is null)
                {
                    TempData["WarningMessage"] = "There is No Department with this Id";
                    return RedirectToAction(nameof(Index));
                }
                var departmentViewModel = mapper.Map<DepartmentViewModel>(department);
                return View(departmentViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            } 
        }
        [HttpGet]
        public IActionResult Update(int? id)
        {
            var department = unitOfWork.DepartmentRepository.GetById(id);
            var departmentViewModel = mapper.Map<DepartmentViewModel>(department);
            return View(departmentViewModel);
        }
        [HttpPost]
        public IActionResult Update(DepartmentViewModel departmentViewModel)
        {
            var department = mapper.Map<Department>(departmentViewModel);
            unitOfWork.DepartmentRepository.Update(department);
            unitOfWork.Complete();
            TempData["SuccessMessage"] = "Update was successful.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(DepartmentViewModel departmentViewModel)
        {
            var department = mapper.Map<Department>(departmentViewModel);
            unitOfWork.DepartmentRepository.Delete(department);
            unitOfWork.Complete();
            TempData["SuccessMessage"] = "Delete was successful.";
            return RedirectToAction(nameof(Index));
        }
    }
}
