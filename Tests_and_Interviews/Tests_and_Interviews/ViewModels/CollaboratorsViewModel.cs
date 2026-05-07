namespace Tests_and_Interviews.ViewModels
{
    using System.Collections.Generic;
    using CommunityToolkit.Mvvm.ComponentModel;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Services;
    using Tests_and_Interviews.Services.Interfaces;

    /// <summary>
    /// ViewModel for managing collaborators. This partial class contains logic for retrieving and exposing all collaborators for the current session's company.
    /// </summary>
    public partial class CollaboratorsViewModel : ObservableObject
    {
        private readonly ICollaboratorsService collaboratorsService;

        private readonly SessionService sessionService;

        /// <summary>
        /// Gets the list of all collaborators for the current session's company.
        /// </summary>
        public List<Company> AllCollaborators { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollaboratorsViewModel"/> class.
        /// Collaborators view model constructor that populates the list of all the collaborators.
        /// </summary>
        /// <param name="collaboratorsService">The service used to manage collaborator data.</param>
        /// <param name="sessionService">The service providing information about the current session and logged-in user.</param>
        public CollaboratorsViewModel(ICollaboratorsService collaboratorsService, SessionService sessionService)
        {
            this.collaboratorsService = collaboratorsService;
            this.sessionService = sessionService;

            this.AllCollaborators = collaboratorsService.GetAllCollaborators(sessionService.LoggedInUser.CompanyId);
        }
    }
}
