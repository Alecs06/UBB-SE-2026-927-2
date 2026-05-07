namespace Tests_and_Interviews_API.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Tests_and_Interviews_API.Models;
    using Tests_and_Interviews_API.Repositories.Interfaces;
    using Tests_and_Interviews_API.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing companies.
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepo _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access company data. Cannot be null.</param>
        public CompanyService(ICompanyRepo repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Retrieves all companies.
        /// </summary>
        /// <returns>A list of all companies.</returns>
        public List<Company> GetAll()
        {
            return this._repository.GetAll().ToList();
        }

        /// <summary>
        /// Retrieves the company with the specified identifier.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <returns>The company corresponding to the specified identifier, or null if not found.</returns>
        public Company? GetById(int companyId)
        {
            return this._repository.GetById(companyId);
        }

        /// <summary>
        /// Retrieves the company with the specified name.
        /// </summary>
        /// <param name="companyName">The name of the company.</param>
        /// <returns>The company corresponding to the specified name, or null if not found.</returns>
        public Company? GetCompanyByName(string companyName)
        {
            return this._repository.GetCompanyByName(companyName);
        }

        /// <summary>
        /// Adds a new company to the data store.
        /// </summary>
        /// <param name="company">The company to add. Cannot be null.</param>
        public void Add(Company company)
        {
            this._repository.Add(company);
        }

        /// <summary>
        /// Updates an existing company in the data store.
        /// </summary>
        /// <param name="company">The company with updated values. Cannot be null.</param>
        public void Update(Company company)
        {
            this._repository.Update(company);
        }

        /// <summary>
        /// Removes the company with the specified identifier from the data store.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company to remove.</param>
        public void Remove(int companyId)
        {
            this._repository.Remove(companyId);
        }
    }
}