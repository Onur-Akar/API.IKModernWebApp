using System.Collections.Generic;

namespace API.IKModernWebApp.Model.DAL
{
    public partial class CompanyGroup
    {
        public CompanyGroup()
        {
            this.Company = new HashSet<Company>();
        }

        public int CompanyGroupID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Company> Company { get; set; }
    }
}
