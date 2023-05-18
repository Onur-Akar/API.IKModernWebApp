using API.IKModernWebApp.Model.DAL;
using API.IKModernWebApp.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace API.IKModernWebApp.Helpers
{
    public class DBHelper
    {
        public static List<Company> GetCompanies(IKMDbContext ikmDbContext, int groupId, int? companyId, out string groupName)
        {
            List<Company> companies = new List<Company>();
            if (companyId != null)
            {
                companies.Add(ikmDbContext.Company.FirstOrDefault(c => c.CompanyID == companyId && c.IsActive));
                groupName = ikmDbContext.CompanyGroup.FirstOrDefault(g => g.CompanyGroupID == groupId).Name;
            }
            else
            {
                companies = ikmDbContext.Company.Where(c => c.CompanyGroupId == groupId && c.IsActive).ToList();
                groupName = ikmDbContext.CompanyGroup.FirstOrDefault(g => g.CompanyGroupID == groupId).Name;
            }

            return companies;
        }

        public static CompanyViewModel GetViewModelForCompany(Company company, string groupName)
        {
            CompanyViewModel companyView = new CompanyViewModel()
            {
                CompanyId = company.CompanyID,
                CompanyName = company.Location,
                RegistryNumber = company.RegistryNumber,
                GroupId = company.CompanyGroupId,
                GroupName = groupName,
                UserName = CyrptoHelper.DecryptPassword(company.UserName, company.Salt),
                Extension = CyrptoHelper.DecryptPassword(company.Extension, company.Salt),
                SystemPassword = CyrptoHelper.DecryptPassword(company.SystemPassword, company.Salt),
                WorkPassword = CyrptoHelper.DecryptPassword(company.WorkPassword, company.Salt)
            };

            return companyView;
        }
    }
}
