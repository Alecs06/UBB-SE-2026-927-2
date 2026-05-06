using Moq;
using Tests_and_Interviews.Models.Core;
using Tests_and_Interviews.Services.Interfaces;
using Tests_and_Interviews.ViewModels;

namespace TestsAndInterviews.Tests.ViewModels
{
    public class InterviewInterviewerViewModelTests
    {
        [Fact]
        public async Task SubmitScoreSuccessfully()
        {
            var mockSessionService = new Mock<IInterviewSessionService>();
            var mockNotificationService = new Mock<INotificationService>();
            
            var session = new InterviewSession { Id = 1 };
            mockSessionService.Setup(s => s.GetSessionAsync(1)).ReturnsAsync(session);

            var vm = new InterviewInterviewerViewModel(mockSessionService.Object, mockNotificationService.Object);
            vm.InitializeSession(1);
            await Task.Delay(100); // Wait for async initialization to complete
           
            vm.Score = 4.5f;
            vm.SubmitScoreCommand.Execute(null);
            await Task.Delay(100); // Wait for async submit to complete

            mockSessionService.Verify(s => s.SubmitScoreAsync(1, 4.5f), Times.Once);
            mockNotificationService.Verify(n => n.ShowSimpleNotification("Score submitted", It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public void SettingScore_RaisesPropertyChanged()
        {
            var mockSessionService = new Mock<IInterviewSessionService>();
            var mockNotificationService = new Mock<INotificationService>();
           
            var vm = new InterviewInterviewerViewModel(mockSessionService.Object, mockNotificationService.Object);
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
        public async Task RecordingUri_SetFromUrl()
        {
            var mockSessionService = new Mock<IInterviewSessionService>();
            var mockNotificationService = new Mock<INotificationService>();
           
            var session = new InterviewSession { Id = 1, Video = "http://localhost/InterviewSessions/videos/video.mp4" };
            mockSessionService.Setup(s => s.GetSessionAsync(1)).ReturnsAsync(session);

            var vm = new InterviewInterviewerViewModel(mockSessionService.Object, mockNotificationService.Object);
            vm.InitializeSession(1);
            await Task.Delay(100); // Wait for async initialization to complete

            Assert.Equal(new Uri(session.Video), vm.RecordingUri);
        }

        [Fact]
        public async Task VideoShouldLoadEvenIfServiceThrowsError() 
        {
            var mockSessionService = new Mock<IInterviewSessionService>();
            var mockNotificationService = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1, Video = "" };
            mockSessionService.Setup(s => s.GetSessionAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var vm = new InterviewInterviewerViewModel(mockSessionService.Object, mockNotificationService.Object);
            vm.InitializeSession(1);
            await Task.Delay(100); // Wait for async initialization to complete

            Assert.Equal(new Uri("about:blank"), vm.RecordingUri);
        }

        [Fact]
        public async Task SubmitScore_WhenNotificationFails_StillUpdatesService()
        {
            var mockSessionService = new Mock<IInterviewSessionService>();
            var mockNotif = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1 };

            mockSessionService.Setup(s => s.GetSessionAsync(It.IsAny<int>())).ReturnsAsync(session);

            mockNotif.Setup(n => n.ShowSimpleNotification(It.IsAny<string>(), It.IsAny<string>()))
                     .Throws(new Exception("Notification Crash"));

            var vm = new InterviewInterviewerViewModel(mockSessionService.Object, mockNotif.Object);
            vm.InitializeSession(1);
            await Task.Delay(50);

            vm.SubmitScore();
            await Task.Delay(50);
            mockSessionService.Verify(s => s.SubmitScoreAsync(1, It.IsAny<float>()), Times.Once);
        }
        [Fact]
        public async Task SubmitScore_WhenServiceFails_HandlesExceptionGracefully()
        {
            var mockSessionService = new Mock<IInterviewSessionService>();
            var mockNotif = new Mock<INotificationService>();

            mockSessionService.Setup(s => s.GetSessionAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database Offline"));

            var vm = new InterviewInterviewerViewModel(mockSessionService.Object, mockNotif.Object);
            vm.InitializeSession(1);
            await Task.Delay(50);

            vm.SubmitScore();
            await Task.Delay(50);

            mockNotif.Verify(n => n.ShowSimpleNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
