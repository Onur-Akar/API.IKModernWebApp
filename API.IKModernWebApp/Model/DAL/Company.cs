using System;

namespace API.IKModernWebApp.Model.DAL
{
    public partial class Company
    {
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Extension { get; set; }
        public string SystemPassword { get; set; }
        public string WorkPassword { get; set; }
        public string RegistryNumber { get; set; }
        public string Address { get; set; }
        public int CompanyGroupId { get; set; }
        public string Sgm { get; set; }
        public string Location { get; set; }
        public byte[] Salt { get; set; }
        public bool IsActive { get; set; }
        public bool IsTenderFirm { get; set; }
        public Nullable<int> MasterCompanyId { get; set; }
        public string CompanyCityOrder { get; set; }

        public virtual CompanyGroup CompanyGroup { get; set; }
    }
}
