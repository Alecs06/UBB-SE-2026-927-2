using System.Collections.Generic;
using Tests_and_Interviews.Models;

namespace Tests_and_Interviews.Services.Interfaces
{
    public interface ICollaboratorsService
    {
        void AddCollaborator(Event eventToBeCollaboratedOn, Company companyInvitedToCollaborate, int loggedInUserID);
        List<Company> GetAllCollaborators(int loggedInCompanyId);
    }
}