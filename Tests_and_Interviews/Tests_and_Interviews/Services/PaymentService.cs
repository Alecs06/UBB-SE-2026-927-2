// <copyright file="PaymentService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Services.Interfaces;
    using Tests_and_Interviews.Validators;

    public class PaymentService : IPaymentService
    {
        private const int EmptyCollectionCount = 0;
        private const string AdminEmailAddress = "carla.draghiciu@cnglsibiu.ro";
        private const string AdminEmailDisplayName = "Job Portal Admin";
        private const string AdminEmailPassword = "[REDACTED_PASSWORD]";
        private const string SmtpHostAddress = "smtp.gmail.com";
        private const int SmtpHostPort = 587;
        private const int SmtpTimeoutMilliseconds = 60000;
        private const string NotificationEmailSubject = "Job Promotion Alert!";
        private const string DatabaseErrorMessagePrefix = "Database Error: ";
        private const string EmailSentDebugMessagePrefix = "Email sent to ";
        private const string EmailFailedDebugMessagePrefix = "Failed to send email: ";
        private readonly IPaymentValidator validator;
        private readonly HttpClient http;

        public PaymentService(IPaymentValidator paymentValidator)
        {
            this.validator = paymentValidator;
            this.http = ApiClient.Http;
        }

        public PaymentService(IPaymentValidator paymentValidator, HttpClient httpClient)
        {
            this.validator = paymentValidator;
            this.http = httpClient ?? ApiClient.Http;
        }

        public async Task<string> ProcessPaymentAsync(int jobId, int amount, string name, string cardNum, string exp, string cvv)
        {
            string validationError = this.validator.ValidatePaymentDetails(name, cardNum, exp, cvv);
            if (!string.IsNullOrEmpty(validationError))
            {
                return validationError;
            }
            try
            {
                // 1. Save to database
                HttpResponseMessage updateResponse = await this.http.PutAsJsonAsync(
                    $"payment/{jobId}?paymentAmount={amount}",
                    new { });
                updateResponse.EnsureSuccessStatusCode();

                // 2. Fetch emails to notify
                HttpResponseMessage notifyResponse = await this.http.GetAsync(
                    $"payment/notify/{jobId}?newPaymentAmount={amount}");
                notifyResponse.EnsureSuccessStatusCode();
                List<string>? emailsToNotify = await notifyResponse.Content.ReadFromJsonAsync<List<string>>();

                // 3. Send Emails
                if (emailsToNotify != null && emailsToNotify.Count > EmptyCollectionCount)
                {
                    await this.SendNotificationEmailsAsync(emailsToNotify, amount);
                }
                return string.Empty;
            }
            catch (Exception exception)
            {
                return $"{DatabaseErrorMessagePrefix}{exception.Message}";
            }
        }

        public async Task<List<JobPaymentInfo>> GetPaidJobsInfo(string jobType, string expLevel)
        {
            HttpResponseMessage response = await this.http.GetAsync(
                $"payment/paid?jobType={jobType}&experienceLevel={expLevel}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<JobPaymentInfo>();
            }

            response.EnsureSuccessStatusCode();
            List<JobPaymentInfoDto>? dtos = await response.Content.ReadFromJsonAsync<List<JobPaymentInfoDto>>();
            return dtos?.Select(dto => dto.ToEntity()).ToList() ?? new List<JobPaymentInfo>();
        }

        private async Task SendNotificationEmailsAsync(List<string> emails, int newAmount)
        {
            try
            {
                var fromAddress = new MailAddress(AdminEmailAddress, AdminEmailDisplayName);
                using (var smtpClient = new SmtpClient
                {
                    Host = SmtpHostAddress,
                    Port = SmtpHostPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, AdminEmailPassword),
                    Timeout = SmtpTimeoutMilliseconds
                })
                {
                    foreach (string email in emails)
                    {
                        var toAddress = new MailAddress(email);
                        string notificationBody = $"Hello, \n\nJust letting you know that a competitor has placed a bid of ${newAmount} on a job that shares the same Type and Experience Level as yours. Consider increasing your budget to stay competitive!";
                        using (var mailMessage = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = NotificationEmailSubject,
                            Body = notificationBody
                        })
                        {
                            await smtpClient.SendMailAsync(mailMessage);
                            System.Diagnostics.Debug.WriteLine($"{EmailSentDebugMessagePrefix}{email}!");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"{EmailFailedDebugMessagePrefix}{exception.Message}");
            }
        }
    }
}