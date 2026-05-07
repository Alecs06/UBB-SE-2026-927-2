namespace Tests_and_Interviews_API.Services
{
    using System.Collections.Generic;
    using Tests_and_Interviews_API.Models;
    using Tests_and_Interviews_API.Repositories.Interfaces;
    using Tests_and_Interviews_API.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing job payments.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access payment data. Cannot be null.</param>
        public PaymentService(IPaymentRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Updates the payment amount for the specified job.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <param name="paymentAmount">The new payment amount to apply.</param>
        public void UpdateJobPayment(int jobId, int paymentAmount)
        {
            this._repository.UpdateJobPayment(jobId, paymentAmount);
        }

        /// <summary>
        /// Retrieves all paid jobs matching the specified job type and experience level.
        /// </summary>
        /// <param name="jobType">The type of the job.</param>
        /// <param name="experienceLevel">The experience level required for the job.</param>
        /// <returns>A list of job payment information matching the specified criteria.</returns>
        public List<JobPaymentInfo> GetPaidJobs(string jobType, string experienceLevel)
        {
            return this._repository.GetPaidJobs(jobType, experienceLevel);
        }

        /// <summary>
        /// Retrieves the email addresses of companies to notify about a new payment amount for the specified job.
        /// </summary>
        /// <param name="currentJobId">The unique identifier of the current job.</param>
        /// <param name="newPaymentAmount">The new payment amount to compare against.</param>
        /// <returns>A list of email addresses of companies to notify.</returns>
        public List<string> GetCompaniesToNotify(int currentJobId, int newPaymentAmount)
        {
            return this._repository.GetCompaniesToNotify(currentJobId, newPaymentAmount);
        }
    }
}