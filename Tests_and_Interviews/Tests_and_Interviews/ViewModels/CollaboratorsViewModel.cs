using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Tests_and_Interviews.Models;
using Tests_and_Interviews.Repositories.Interfaces;
using Tests_and_Interviews.Services;
using Tests_and_Interviews.Services.Interfaces;
using Tests_and_Interviews.Validators;
using Tests_and_Interviews.ViewModels;

namespace Tests_and_Interviews.ViewModels
{
    public partial class CollaboratorsViewModel : ObservableObject
    {
        public List<Company> AllCollaborators { get; }
        private readonly ICollaboratorsService collaboratorsService;
        private readonly SessionService sessionService;

        /// <summary>
        /// Collaborators view model constructor that populates the list of all the collaborators
        /// </summary>
        /// <param name="collaboratorsService"></param>
        /// <param name="sessionService"></param>
        public CollaboratorsViewModel(ICollaboratorsService collaboratorsService, SessionService sessionService)
        {
            this.collaboratorsService = collaboratorsService;
            this.sessionService = sessionService;

            this.AllCollaborators = collaboratorsService.GetAllCollaborators(sessionService.LoggedInUser.CompanyId);
        }
    }
}
