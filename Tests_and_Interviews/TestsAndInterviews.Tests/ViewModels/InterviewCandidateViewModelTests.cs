using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Tests_and_Interviews.Models.Core;
using Tests_and_Interviews.Models.Enums;
using Tests_and_Interviews.Repositories;
using Tests_and_Interviews.Repositories.Interfaces;
using Tests_and_Interviews.Services.Interfaces;
using Tests_and_Interviews.ViewModels;

namespace TestsAndInterviews.Tests.ViewModels
{
    public class InterviewCandidateViewModelTests
    {
        [Fact]
        public async Task InitializeAsync_WhenSessionExists_UpdatesStartTimeAndLoadsQuestions()
        {
            var sessionId = 10;
            var positionId = 5;

            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();

            var fakeSession = new InterviewSession { Id = sessionId, PositionId = positionId };
            mockSessionRepo.Setup(repo => repo.GetInterviewSessionByIdAsync(sessionId))
                           .ReturnsAsync(fakeSession);

            var fakeQuestions = new List<Question> { new Question(), new Question() };
            mockQuestionRepo.Setup(repo => repo.GetInterviewQuestionsByPositionAsync(positionId))
                            .ReturnsAsync(fakeQuestions);

            var mockNotificationService = new Mock<INotificationService>();
            var sut = new InterviewCandidateViewModel(mockSessionRepo.Object, mockQuestionRepo.Object, mockNotificationService.Object);

            await sut.LoadData(sessionId);

            Assert.True((DateTime.UtcNow - fakeSession.DateStart).TotalSeconds < 3);
            mockSessionRepo.Verify(r => r.UpdateInterviewSessionAsync(fakeSession), Times.Once);

            mockQuestionRepo.Verify(r => r.GetInterviewQuestionsByPositionAsync(positionId), Times.Once);
        }
        [Fact]
        public async Task NextQuestion_IncrementsIndex_AndUpdatesQuestionText()
        {
            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var mockNotification = new Mock<INotificationService>();

            var session = new InterviewSession { PositionId = 101 };
            var questions = new List<Question>
        {
            new Question { QuestionText = "What is C#?" },
            new Question { QuestionText = "What is MVVM?" }
        };

            mockSessionRepo.Setup(r => r.GetInterviewSessionByIdAsync(1))
            .ReturnsAsync(session);
            mockQuestionRepo.Setup(r => r.GetInterviewQuestionsByPositionAsync(101))
            .ReturnsAsync(questions);

            var vm = new InterviewCandidateViewModel(
            mockSessionRepo.Object,
            mockQuestionRepo.Object,
            mockNotification.Object);

            await vm.LoadData(1);

            vm.StartQuestions();

            Assert.Equal("What is C#?", vm.QuestionText);

            vm.NextQuestionCommand.Execute(null);

            Assert.Equal("What is MVVM?", vm.QuestionText);

        }
        [Fact]
        public async Task CompletingQuestions_ShowsCompletionMessage()
        {
            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var mockNotification = new Mock<INotificationService>();

            var questions = new List<Question>
                {
                new Question { QuestionText = "Q1" },
                new Question { QuestionText = "Q2" }
            };
            mockSessionRepo.Setup(r => r.GetInterviewSessionByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new InterviewSession { PositionId = 20 });
            mockQuestionRepo.Setup(r => r.GetInterviewQuestionsByPositionAsync(20))
            .ReturnsAsync(questions);

            var vm = new InterviewCandidateViewModel(
                mockSessionRepo.Object,
                mockQuestionRepo.Object,
                mockNotification.Object);

            await vm.LoadData(1);

            vm.StartQuestions();
            vm.NextQuestionCommand.Execute(null);
            vm.NextQuestionCommand.Execute(null);
            Assert.Contains("Congratulation", vm.QuestionText);
        }
        [Fact]
        public async Task ResetQuestionsTest()
        {
            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var mockNotification = new Mock<INotificationService>();
            var questions = new List<Question>
            {
                new Question { QuestionText = "Q1" },
                new Question { QuestionText = "Q2" }
            };
            mockSessionRepo.Setup(r => r.GetInterviewSessionByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new InterviewSession { PositionId = 30 });

            mockQuestionRepo.Setup(r => r.GetInterviewQuestionsByPositionAsync(30))
                .ReturnsAsync(questions);
            var vm = new InterviewCandidateViewModel(
                mockSessionRepo.Object,
                mockQuestionRepo.Object,
                mockNotification.Object);
            await vm.LoadData(1);
            vm.StartQuestions();
            vm.NextQuestionCommand.Execute(null);
            Assert.Equal("Q2", vm.QuestionText);
            vm.ResetQuestions();
            Assert.Equal("Questions will start after starting recording", vm.QuestionText);
            vm.NextQuestionCommand.Execute(null);
            Assert.Equal("Q1", vm.QuestionText);

        }
        [Fact]
        public async Task LoadData_WhenRepositoryThrows_ShowsGenericErrorMessage()
        {
            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var mockNotification = new Mock<INotificationService>();

            mockSessionRepo.Setup(repo => repo.GetInterviewSessionByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database connection failed"));
            var vm = new InterviewCandidateViewModel(
                mockSessionRepo.Object,
                mockQuestionRepo.Object,
                mockNotification.Object);

            await vm.LoadData(1);
            Assert.Equal("An error occurred while loading the session.", vm.QuestionText);
        }
        [Fact]
        public async Task SubmitRecordingSuccessfully()
        {
            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var mockNotification = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1, PositionId = 101 };
            var questions = new List<Question>
        {
            new Question { QuestionText = "What is C#?" },
            new Question { QuestionText = "What is MVVM?" }
        };
            mockSessionRepo.Setup(r => r.GetInterviewSessionByIdAsync(1))
            .ReturnsAsync(session);
            mockQuestionRepo.Setup(r => r.GetInterviewQuestionsByPositionAsync(101))
            .ReturnsAsync(questions);
            var vm = new InterviewCandidateViewModel(
            mockSessionRepo.Object,
            mockQuestionRepo.Object,
            mockNotification.Object);

            await vm.LoadData(1);
            vm.RecordingFilePath = "path/to/video.mp4";
            vm.StartQuestions();
            vm.NextQuestionCommand.Execute(null);
            vm.NextQuestionCommand.Execute(null);
            vm.SubmitRecordingCommand.Execute(null);

            await Task.Delay(100);

            mockSessionRepo.Verify(r => r.UpdateInterviewSessionAsync(It.Is<InterviewSession>(s =>
                    s.Video == "path/to/video.mp4" &&
                    s.Status == InterviewStatus.InProgress.ToString())), Times.Exactly(2));

            mockNotification.Verify(n => n.ShowSimpleNotification(
                "Video uploaded",
                "Your interview video was uploaded successfully."), Times.Once);
        }

        [Fact]
        public async Task FailedToShowNotificationTest()
        {
            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var mockNotification = new Mock<INotificationService>();
            var questions = new List<Question>();
            mockNotification.Setup(n => n.ShowSimpleNotification(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Notification system failed"));
            var session = new InterviewSession { Id = 1, PositionId = 101 };
            mockSessionRepo.Setup(r => r.GetInterviewSessionByIdAsync(1))
            .ReturnsAsync(session);
            mockQuestionRepo.Setup(r => r.GetInterviewQuestionsByPositionAsync(101))
            .ReturnsAsync(questions);
            var vm = new InterviewCandidateViewModel(
                mockSessionRepo.Object,
                mockQuestionRepo.Object,
                mockNotification.Object);
            await vm.LoadData(1);
            vm.StartQuestions();
            vm.RecordingFilePath = "path/to/video.mp4";
            vm.SubmitRecordingCommand.Execute(null);
            await Task.Delay(100);
            Assert.Equal("Video uploaded, but failed to show notification.", vm.QuestionText);
        }
        [Fact]
        public void SubmitRecording_WhenSessionNull()
        {
            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var mockNotification = new Mock<INotificationService>();
            var vm = new InterviewCandidateViewModel(
                mockSessionRepo.Object,
                mockQuestionRepo.Object,
                mockNotification.Object);
            vm.SubmitRecordingCommand.Execute(null);
            Assert.Equal("No session loaded. Cannot submit recording.", vm.QuestionText);
        }

        [Fact]
        public async Task SubmitRecordingWhenRepositoryThrowsError()
        {
            var mockSessionRepo = new Mock<IInterviewSessionRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var mockNotification = new Mock<INotificationService>();
            var session = new InterviewSession { Id = 1, PositionId = 101 };
            var questions = new List<Question>();
            mockSessionRepo.Setup(r => r.GetInterviewSessionByIdAsync(1))
            .ReturnsAsync(session);
            mockQuestionRepo.Setup(r => r.GetInterviewQuestionsByPositionAsync(101))
            .ReturnsAsync(questions);
            mockSessionRepo.Setup(r => r.UpdateInterviewSessionAsync(It.IsAny<InterviewSession>()))
                .ThrowsAsync(new Exception("Database update failed"));
            var vm = new InterviewCandidateViewModel(
                mockSessionRepo.Object,
                mockQuestionRepo.Object,
                mockNotification.Object);
            await vm.LoadData(1);
            vm.StartQuestions();
            vm.RecordingFilePath = "path/to/video.mp4";
            vm.SubmitRecordingCommand.Execute(null);
            await Task.Delay(100);
            Assert.Equal("Failed to upload video. Please try again.", vm.QuestionText);
        }
    }
}
