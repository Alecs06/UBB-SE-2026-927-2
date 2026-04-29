using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests_and_Interviews.Models;

namespace Tests_and_Interviews.Services.Interfaces
{
    public interface ICompanyService
    {
        void AddCompany(string companyName, string aboutUs, string pfpUrl, string logoUrl, string location, string email);
        Company? GetCompanyById(int companyId);
        void UpdateCompany(Company company);
        void RemoveCompany(int companyId);
        void PrintAll();
        Company? GetCompanyByName(string companyName);
    }
}
