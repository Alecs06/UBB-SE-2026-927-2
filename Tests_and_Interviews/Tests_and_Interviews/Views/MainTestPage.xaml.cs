namespace Tests_and_Interviews.Views
{
    using System;
    using System.Linq;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Navigation;
    using Tests_and_Interviews.Repositories;
    using Tests_and_Interviews.ViewModels;

    /// <summary>
    /// MainTestPage serves as the landing page for candidates, displaying a list of available tests.
    /// </summary>
    public sealed partial class MainTestPage : Page
    {
        public MainTestPage()
        {
            this.InitializeComponent();
            this.ViewModel = new MainTestViewModel(new TestRepository());
        }

        public MainTestViewModel ViewModel { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            await this.ViewModel.LoadTestsAsync();
        }

        private void StartTest_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is not Button button || button.Tag == null)
            {
                return;
            }

            int testId = Convert.ToInt32(button.Tag);
            var selected = this.ViewModel.Tests.FirstOrDefault(test => test.TestId == testId);
            if (selected != null)
            {
                this.ViewModel.SelectedTest = selected;
            }

            this.Frame.Navigate(typeof(TestPage), new TestNavigationArgs
            {
                TestId = testId,
                UserId = App.CurrentUserId,
            });
        }

        private void Card_PointerEntered(object sender, PointerRoutedEventArgs eventArgs)
        {
            if (sender is not Button button || button.Tag == null)
            {
                return;
            }

            int testId = Convert.ToInt32(button.Tag);
            var card = this.ViewModel.Tests.FirstOrDefault(test => test.TestId == testId);
            if (card != null)
            {
                card.IsHovered = true;
            }
        }

        private void Card_PointerExited(object sender, PointerRoutedEventArgs eventArgs)
        {
            if (sender is not Button button || button.Tag == null)
            {
                return;
            }

            int testId = Convert.ToInt32(button.Tag);
            var card = this.ViewModel.Tests.FirstOrDefault(test => test.TestId == testId);
            if (card != null)
            {
                card.IsHovered = false;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            App.MainWindow.ReturnToMainMenu();
        }
    }
}
