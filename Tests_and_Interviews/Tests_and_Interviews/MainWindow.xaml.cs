namespace Tests_and_Interviews
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Repositories;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services;
    using Tests_and_Interviews.Services.Interfaces;
    using Tests_and_Interviews.Validators;
    using Tests_and_Interviews.Views;

    public sealed partial class MainWindow : Window
    {
        private const int DefaultCompanyId = 1;

        public Frame RootFrame => rootFrame;

        public IEventsService EventsService { get; }

        public ICompanyService CompanyService { get; }

        public SessionService SessionService { get; }

        public ICollaboratorsService CollabsService { get; }

        public IJobsRepository JobsRepository { get; }

        public IApplicantRepository ApplicantsRepository { get; }

        public IApplicantService ApplicantService { get; }

        public IGameService GameService { get; }

        public IPaymentService PaymentService { get; }

        public IGameValidator GameValidator { get; }

        public IEventValidator EventValidator { get; }

        public ICompanyValidator CompanyValidator { get; }

        public IPaymentValidator PaymentValidator { get; }

        public MainWindow()
        {
            ICompanyRepo companyRepository = new CompanyRepo();
            this.CompanyService = new CompanyService(companyRepository);
            this.GameService = new GameService(companyRepository);

            ICollaboratorsRepo collaboratorsRepository = new CollaboratorsRepo();
            this.CollabsService = new CollaboratorsService(collaboratorsRepository);

            Company defaultCompany = new Company("ndj", "dnis", "dnjs", "hdjd", "sybau", "dj@");

            this.InitializeComponent();

            IEventsRepo eventsRepository = new EventsRepo();
            this.EventsService = new EventsService(eventsRepository);
            this.SessionService = new SessionService(defaultCompany);
            this.JobsRepository = new JobsRepository();
            this.ApplicantsRepository = new ApplicantRepository();
            this.ApplicantService = new ApplicantService(ApplicantsRepository);
            this.CompanyValidator = new CompanyValidator();
            this.EventValidator = new EventValidator();
            this.PaymentValidator = new PaymentValidator();
            this.GameValidator = new GameValidator();

            IPaymentRepository paymentRepository = new PaymentRepository();
            this.PaymentService = new PaymentService(paymentRepository, this.PaymentValidator);
        }

        public void ReturnToMainMenu()
        {
            this.RootFrame.Content = null;
            this.RootFrame.BackStack.Clear();
            this.MainMenuContainer.Visibility = Visibility.Visible;
        }

        private void NavigateToViewProfile_Click(object sender, RoutedEventArgs e)
        {
            MainMenuContainer.Visibility = Visibility.Collapsed;
            this.RootFrame.Navigate(typeof(ViewProfilePage), DefaultCompanyId);
        }

        private void NavigateToMainTest_Click(object sender, RoutedEventArgs e)
        {
            MainMenuContainer.Visibility = Visibility.Collapsed;
            this.RootFrame.Navigate(typeof(MainTestPage));
        }

        private void NavigateToRecruiter_Click(object sender, RoutedEventArgs e)
        {
            MainMenuContainer.Visibility = Visibility.Collapsed;
            this.RootFrame.Navigate(typeof(RecruiterPage));
        }

        private void NavigateToCandidateHome_Click(object sender, RoutedEventArgs e)
        {
            MainMenuContainer.Visibility = Visibility.Collapsed;
            this.RootFrame.Navigate(typeof(CandidateHomePage));
        }
    }
}