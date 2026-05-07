// <copyright file="InterviewInterviewerPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Views
{
    using System;
    using System.Diagnostics;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;
    using Tests_and_Interviews.Repositories;
    using Tests_and_Interviews.ViewModels;
    using Windows.Globalization.NumberFormatting;
    using Windows.Media.Core;

    /// <summary>
    /// Interaction logic for InterviewInterviewerPage.xaml.
    /// </summary>
    public sealed partial class InterviewInterviewerPage : Page
    {
        public InterviewInterviewerPage()
        {
            this.InitializeComponent();

            var sessionService = new Services.InterviewSessionService(new QuestionRepository());
            var notificationService = new Services.NotificationService(new Services.Interfaces.WindowsToastNotifier());

            this.ViewModel = new InterviewInterviewerViewModel(sessionService, notificationService);
            this.SetNumberBoxNumberFormatter();
            this.DataContext = this.ViewModel;
        }

        public InterviewInterviewerViewModel ViewModel { get; }

        public MediaSource? CreateMediaSource(Uri uri)
        {
            return uri != null ? MediaSource.CreateFromUri(uri) : null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);

            if (eventArgs.Parameter is int interviewSessionId && interviewSessionId > 0)
            {
                this.ViewModel.InitializeSession(interviewSessionId);
            }
        }

        private void SubmitScore_Click(object sender, RoutedEventArgs eventArgs)
        {
            this.ViewModel.SubmitScore();

            if (this.Tag is Window hostWindow)
            {
                try
                {
                    hostWindow.Close();
                    return;
                }
                catch
                {
                    Debug.WriteLine("Host window close threw an exception, but it will be ignored.");
                }
            }

            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void Skip10_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.InterviewPlayer.MediaPlayer == null)
            {
                return;
            }

            var playbackSession = this.InterviewPlayer.MediaPlayer.PlaybackSession;
            playbackSession.Position += TimeSpan.FromSeconds(10);
        }

        private void SetNumberBoxNumberFormatter()
        {
            var rounder = new IncrementNumberRounder
            {
                Increment = 0.01,
                RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp,
            };

            var formatter = new DecimalFormatter
            {
                IntegerDigits = 1,
                FractionDigits = 2,
                NumberRounder = rounder,
            };

            this.FormattedNumberBox.NumberFormatter = formatter;
            this.FormattedNumberBox.Minimum = 1;
            this.FormattedNumberBox.Maximum = 10;
        }
    }
}
