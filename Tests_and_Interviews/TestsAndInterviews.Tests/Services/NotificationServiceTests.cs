
namespace TestsAndInterviews.Tests.Services
{
    using System;
    using Microsoft.Toolkit.Uwp.Notifications;
    using Moq;
    using Tests_and_Interviews.Services;
    using Tests_and_Interviews.Services.Interfaces;
    using Xunit;

    /// <summary>
    /// Tests for the <see cref="NotificationService"/> class.
    /// </summary>
    public class NotificationServiceTests
    {
        [Fact]
        public void ShowBookingConfirmed_WithValidInputs_DoesNotThrow()
        {
            var svc = new NotificationService(new WindowsToastNotifier());
            var company = "Bosch";
            var title = "Software Engineer Intern";
            var start = new DateTime(2026, 5, 1, 14, 0, 0);
            var end = new DateTime(2026, 5, 1, 15, 0, 0);

            var ex = Record.Exception(() => svc.ShowBookingConfirmed(company, title, start, end));
            Assert.Null(ex);
        }

        [Fact]
        public void ShowSimpleNotification_WithValidInputs_DoesNotThrow()
        {
            var svc = new NotificationService(new WindowsToastNotifier());
            var title = "Reminder";
            var message = "This is a test notification.";

            var ex = Record.Exception(() => svc.ShowSimpleNotification(title, message));
            Assert.Null(ex);
        }

        [Fact]
        public void ShowBookingConfirmed_WithNullOrEmptyInputs_DoesNotThrow()
        {
            var svc = new NotificationService(new WindowsToastNotifier());
            string company = null;
            string title = string.Empty;
            var start = DateTime.Now;
            var end = DateTime.Now.AddMinutes(30);

            var ex = Record.Exception(() => svc.ShowBookingConfirmed(company, title, start, end));
            Assert.Null(ex);
        }

        [Fact]
        public void ShowSimpleNotification_WithNullValues_DoesNotThrow()
        {
            var svc = new NotificationService(new WindowsToastNotifier());
            string title = null;
            string message = null;

            var ex = Record.Exception(() => svc.ShowSimpleNotification(title, message));
            Assert.Null(ex);
        }

        [Fact]
        public void ShowBookingConfirmed_WhenEndTimeIsBeforeStartTime_DoesNotThrow()
        {
            var svc = new NotificationService(new WindowsToastNotifier());
            var start = new DateTime(2026, 12, 31, 23, 0, 0);
            var end = new DateTime(2026, 1, 1, 1, 0, 0);

            var ex = Record.Exception(() => svc.ShowBookingConfirmed("Test", "Test", start, end));
            Assert.Null(ex);
        }

        [Fact]
        public async Task ShowSimpleNotification_CalledFromDifferentThread_DoesNotThrow()
        {
            var svc = new NotificationService(new WindowsToastNotifier());

            var ex = await Record.ExceptionAsync(async () =>
            {
                await Task.Run(() => svc.ShowSimpleNotification("Thread Test", "From Background"));
            });

            Assert.Null(ex);
        }

        [Fact]
        public void ShowSimpleNotification_WhenToastFails_ExecutesCatchBlock()
        {
            var mockNotifier = new Mock<IToastNotifier>();

            mockNotifier.Setup(n => n.Show(It.IsAny<ToastContentBuilder>()))
                .Throws(new Exception("Windows Notification Service Unavailable"));

            var service = new NotificationService(mockNotifier.Object);

            var ex = Record.Exception(() => service.ShowSimpleNotification("Title", "Message"));

            Assert.Null(ex); 
            mockNotifier.Verify(n => n.Show(It.IsAny<ToastContentBuilder>()), Times.Once);
        }

        [Fact]
        public void ShowBookingConfirmed_WhenToastFails_ExecutesCatchBlock()
        {
            var mockNotifier = new Mock<IToastNotifier>();
            mockNotifier.Setup(n => n.Show(It.IsAny<ToastContentBuilder>()))
                .Throws(new Exception("OS Error"));

            var service = new NotificationService(mockNotifier.Object);

            var ex = Record.Exception(() =>
                service.ShowBookingConfirmed("Company", "Job", DateTime.Now, DateTime.Now.AddHours(1)));

            Assert.Null(ex);
            mockNotifier.Verify(n => n.Show(It.IsAny<ToastContentBuilder>()), Times.Once);
        }
    }
}
