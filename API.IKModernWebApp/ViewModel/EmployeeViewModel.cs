using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.IKModernWebApp.ViewModel
{
    public class EmployeeViewModel : BaseViewModel
    {
        public string NID { get; set; }
        public string EmployeeRegistryNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FirstLastName { get; set; }
        public string ProfesionCode { get; set; }
        public DateTime EmploymentStartDate { get; set; }
    }
}
