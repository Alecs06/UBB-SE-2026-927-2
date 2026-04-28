using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Tests_and_Interviews.Models.Core;
using Tests_and_Interviews.Repositories.Interfaces;
using Tests_and_Interviews.Services.Interfaces;
using Tests_and_Interviews.ViewModels;

namespace TestsAndInterviews.Tests.ViewModels
{
    public class InterviewInterviewerViewModelTests
    {
        [Fact]
        public async Task SubmitScoreSuccessfully()
        {
            var mockSessionRepository = new Mock<IInterviewSessionRepository>();
            var mockNotificationService = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1 };
            mockSessionRepository.Setup(repo => repo.GetInterviewSessionByIdAsync(1)).ReturnsAsync(session);

            var vm = new InterviewInterviewerViewModel(mockSessionRepository.Object, mockNotificationService.Object, "");
            vm.InitializeSession(1);
            await Task.Delay(100); // Wait for async initialization to complete
            vm.Score = 4.5f;

            vm.SubmitScoreCommand.Execute(null);
            await Task.Delay(100); // Wait for async submit to complete
            mockSessionRepository.Verify(r => r.UpdateInterviewSessionAsync(It.Is<InterviewSession>(s =>
        s.Score == 4.5m && s.Status == "Completed")), Times.Once);

            mockNotificationService.Verify(n => n.ShowSimpleNotification("Score submitted", It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public void SettingScore_RaisesPropertyChanged()
        {
            var mockSessionRepository = new Mock<IInterviewSessionRepository>();
            var mockNotificationService = new Mock<INotificationService>();
            var vm = new InterviewInterviewerViewModel(mockSessionRepository.Object, mockNotificationService.Object, "");
            bool propertyChangedRaised = false;
            vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(InterviewInterviewerViewModel.Score))
                {
                    propertyChangedRaised = true;
                }
            };
            vm.Score = 3.0f;
            Assert.True(propertyChangedRaised, "Setting Score should raise PropertyChanged event.");
        }

        [Fact]
        public async Task LocalFolderPath_IsConvertedToMSAppDataURI()
        {
            var mockSessionRepository = new Mock<IInterviewSessionRepository>();
            var mockNotificationService = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1, Video = @"Users\Test\Videos\video.mp4" };
            mockSessionRepository.Setup(repo => repo.GetInterviewSessionByIdAsync(1)).ReturnsAsync(session);

            var vm = new InterviewInterviewerViewModel(mockSessionRepository.Object, mockNotificationService.Object, @"Users\Test\Videos\");
            vm.InitializeSession(1);

            await Task.Delay(100); // Wait for async initialization to complete

            Assert.Equal(new Uri("ms-appdata:///local/video.mp4"), vm.RecordingUri);
        }

        [Fact]
        public async Task VideoPathDoesNotStartWithLocalPath()
        {
            var mockSessionRepository = new Mock<IInterviewSessionRepository>();
            var mockNotificationService = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1, Video = "Videos\\video.mp4" };
            mockSessionRepository.Setup(repo => repo.GetInterviewSessionByIdAsync(1)).ReturnsAsync(session);

            var vm = new InterviewInterviewerViewModel(mockSessionRepository.Object, mockNotificationService.Object, "C:\\Users\\Test\\Videos");
            vm.InitializeSession(1);

            await Task.Delay(100); // Wait for async initialization to complete

            Assert.Equal(new Uri("ms-appdata:///local/Videos/video.mp4"), vm.RecordingUri);
        }

        [Fact]
        public async Task VideoShouldLoadEvenIfRepoThrowsError() 
        {
            var mockSessionRepository = new Mock<IInterviewSessionRepository>();
            var mockNotificationService = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1, Video = "C:\\Users\\Test\\Videos\\video.mp4" };
            mockSessionRepository.Setup(repo => repo.GetInterviewSessionByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var vm = new InterviewInterviewerViewModel(mockSessionRepository.Object, mockNotificationService.Object, "C:\\Users\\Test\\Videos");
            vm.InitializeSession(1);

            await Task.Delay(100); // Wait for async initialization to complete

            Assert.Equal(new Uri("about:blank"), vm.RecordingUri);

        }

        [Fact]
        public async Task SubmitScore_WhenNotificationFails_StillUpdatesRepository()
        {
            var mockRepo = new Mock<IInterviewSessionRepository>();
            var mockNotif = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1 };

            mockRepo.Setup(r => r.GetInterviewSessionByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync(session);

            mockNotif.Setup(n => n.ShowSimpleNotification(It.IsAny<string>(), It.IsAny<string>()))
                     .Throws(new Exception("Notification Crash"));

            var vm = new InterviewInterviewerViewModel(mockRepo.Object, mockNotif.Object, "C:\\Test");
            vm.InitializeSession(1);
            await Task.Delay(50);

            vm.SubmitScore();
            await Task.Delay(50);
            mockRepo.Verify(r => r.UpdateInterviewSessionAsync(It.IsAny<InterviewSession>()), Times.Once);
        }
        [Fact]
        public async Task SubmitScore_WhenRepositoryFails_HandlesExceptionGracefully()
        {
            var mockRepo = new Mock<IInterviewSessionRepository>();
            var mockNotif = new Mock<INotificationService>();

            mockRepo.Setup(r => r.GetInterviewSessionByIdAsync(It.IsAny<int>()))
                    .ThrowsAsync(new Exception("Database Offline"));

            var vm = new InterviewInterviewerViewModel(mockRepo.Object, mockNotif.Object, "C:\\Test");
            vm.InitializeSession(1);
            await Task.Delay(50);

            vm.SubmitScore();
            await Task.Delay(50);

            mockNotif.Verify(n => n.ShowSimpleNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        [Fact]
        public async Task InitializeSession_WhenVideoPathIsRaw()
        {
            var mockSessionRepository = new Mock<IInterviewSessionRepository>();
            string externalPath = "D:\\ExternalVideos\\video.mp4";
            var session = new InterviewSession { Id = 1, Video = externalPath };
            mockSessionRepository.Setup(repo => repo.GetInterviewSessionByIdAsync(1)).ReturnsAsync(session);
            var vm = new InterviewInterviewerViewModel(mockSessionRepository.Object, Mock.Of<INotificationService>(), "C:\\Users\\Test\\Videos");
            vm.InitializeSession(1);
            await Task.Delay(50);
            Assert.Contains("video.mp4", vm.RecordingUri.ToString());
            Assert.False(vm.RecordingUri.ToString().StartsWith("ms-appdata:///local/"), "External video paths should not be converted to ms-appdata URIs.");
        }
    }
}
