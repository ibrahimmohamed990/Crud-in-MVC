using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Dapartment Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Dapartment Code is Required")]
        public string Code { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

    }
}
