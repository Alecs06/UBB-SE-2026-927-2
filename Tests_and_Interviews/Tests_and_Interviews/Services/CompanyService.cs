// <copyright file="CompanyService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Services.Interfaces;
    using Tests_and_Interviews.Validators;

    public class CompanyService : ICompanyService
    {
        private readonly ICompanyValidator companyValidator;

        public CompanyService()
        {
            this.companyValidator = new CompanyValidator();
        }

        private void ValidateCompany(Company company)
        {
            this.companyValidator.ValidateName(company.Name);
            this.companyValidator.ValidateAboutUs(company.AboutUs);
            this.companyValidator.ValidateProfilePicture(company.ProfilePicturePath);
            this.companyValidator.ValidateLogo(company.CompanyLogoPath);
            this.companyValidator.ValidateLocation(company.Location);
            this.companyValidator.ValidateEmail(company.Email);
        }

        public async Task AddCompany(string companyName, string aboutUs, string pfpUrl, string logoUrl, string location, string email)
        {
            Company companyToBeAdded = new Company(companyName, aboutUs, pfpUrl, logoUrl, location, email);
            this.ValidateCompany(companyToBeAdded);
            HttpResponseMessage response = await ApiClient.Http.PostAsJsonAsync("companies", companyToBeAdded.ToDto());
            response.EnsureSuccessStatusCode();
        }

        public async Task<Company?> GetCompanyById(int companyId)
        {
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"companies/{companyId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            CompanyDto? dto = await response.Content.ReadFromJsonAsync<CompanyDto>();
            return dto?.ToEntity();
        }

        public async Task UpdateCompany(Company company)
        {
            this.ValidateCompany(company);
            HttpResponseMessage response = await ApiClient.Http.PutAsJsonAsync(
                $"companies/{company.CompanyId}",
                company.ToDto());
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveCompany(int companyId)
        {
            HttpResponseMessage response = await ApiClient.Http.DeleteAsync($"companies/{companyId}");
            response.EnsureSuccessStatusCode();
        }

        // PrintAll() omitted — was debug/console output only, no API equivalent

        /// <summary>
        /// Function that searches a company by name and returns it
        /// </summary>
        /// <param name="companyName"> the name of the company being searched </param>
        /// <returns> the company if found, else null </returns>
        public async Task<Company?> GetCompanyByName(string companyName)
        {
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"companies/byname/{companyName}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            CompanyDto? dto = await response.Content.ReadFromJsonAsync<CompanyDto>();
            return dto?.ToEntity();
        }
    }
}