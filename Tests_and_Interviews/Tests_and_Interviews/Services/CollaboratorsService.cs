// <copyright file="CollaboratorsService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Services.Interfaces;

    public class CollaboratorsService : ICollaboratorsService
    {
        /// <summary>
        /// Collaborators service constructor
        /// </summary>
        public CollaboratorsService()
        {
        }

        /// <summary>
        /// Function that adds a collaborator to a company's collaborator list
        /// </summary>
        /// <param name="eventToBeCollaboratedOn"> the event the company is invited to collaborate on </param>
        /// <param name="companyInvitedToCollaborate"> the company to be added to the list </param>
        /// <param name="loggedInUserID"></param>
        public async Task AddCollaborator(Event eventToBeCollaboratedOn, Company companyInvitedToCollaborate, int loggedInUserID)
        {
            CollaboratorDto dto = new CollaboratorDto
            {
                EventId = eventToBeCollaboratedOn.Id,
                CompanyId = companyInvitedToCollaborate.CompanyId,
            };
            HttpResponseMessage response = await ApiClient.Http.PostAsJsonAsync(
                $"collaborators?loggedInUserID={loggedInUserID}",
                dto);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Function that returns a list of all the collaborators of the user company
        /// </summary>
        /// <param name="loggedInCompanyId"> the ID of the user company that is currently logged in </param>
        /// <returns> a list of all its collaborators </returns>
        public async Task<List<Company>> GetAllCollaborators(int loggedInCompanyId)
        {
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"collaborators/{loggedInCompanyId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<Company>();
            }

            response.EnsureSuccessStatusCode();
            List<CompanyDto>? dtos = await response.Content.ReadFromJsonAsync<List<CompanyDto>>();
            return dtos?.Select(dto => dto.ToEntity()).ToList() ?? new List<Company>();
        }
    }
}