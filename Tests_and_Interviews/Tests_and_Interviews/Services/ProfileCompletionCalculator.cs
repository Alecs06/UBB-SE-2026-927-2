// <copyright file="ProfileCompletionCalculator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services.Interfaces;
    using Tests_and_Interviews.Validators;
    using System.Diagnostics;
    using Tests_and_Interviews.Dtos;

    public class ProfileCompletionCalculator : IProfileCompletionCalculator
    {
        // Company has:
        //    this.Name = name;
        //    this.AboutUs = aboutus;
        //    this.Pfp_url = pfp_url;
        //    this.Logo_url = logo_url;
        //    this.Location = location;
        //    this.Email = email;
        private const int TotalRequiredTasksCount = 5;
        private const int MinimumRequiredPostedJobs = 5;
        private const int MinimumRequiredCollaborators = 2;
        private const int PercentageMultiplier = 100;
        private const int TopSkillsLimit = 3;
        private const int DaysToLookBack = -7;
        private const int EmptyCount = 0;

        private const string TaskUploadPictureText = "Upload company picture";
        private const string TaskAddDescriptionText = "Add company description";
        private const string TaskPostJobsText = "Post at least 5 jobs";
        private const string TaskAddCollaboratorsText = "Add 2 collaborators";
        private const string TaskCompleteMiniGameText = "Complete mini-game";

        private const string MessageNoApplicantsText = "No applicants yet. Start posting jobs!";
        private const string MessageGreatStartPrefix = "Great start! You have ";
        private const string MessageGreatStartSuffix = " new applicants.";
        private const string MessageFewerApplicantsPrefix = "You have ";
        private const string MessageFewerApplicantsSuffix = "% fewer applicants than last week.";
        private const string MessageMoreApplicantsPrefix = "Congrats! You have ";
        private const string MessageMoreApplicantsSuffix = "% more applicants than last week.";

        private readonly IJobsService jobsService;
        private readonly IApplicantService applicantService;
        //private readonly IJobsRepository jobsRepository;
        //private readonly IApplicantRepository applicantRepository;

        public ProfileCompletionCalculator(IJobsService jobsService, IApplicantService applicantService)
        {
            this.jobsService = jobsService;
            this.applicantService = applicantService;
        }

        public (int percentage, List<string> remainingTasks) Calculate(Company company)
        {
            int completedTasksCount = EmptyCount;
            var remainingTasksList = new List<string>();

            if (!string.IsNullOrEmpty(company.ProfilePicturePath))
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskUploadPictureText);
            }

            if (!string.IsNullOrEmpty(company.AboutUs))
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskAddDescriptionText);
            }
            if (company.PostedJobsCount >= MinimumRequiredPostedJobs)
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskPostJobsText);
            }
            if (company.CollaboratorsCount >= MinimumRequiredCollaborators || company.CollaboratorsCount >= MinimumRequiredCollaborators)
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskAddCollaboratorsText);
            }
            if (IsMiniGameComplete(company.Game))
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskCompleteMiniGameText);
            }
            return ((completedTasksCount * PercentageMultiplier) / TotalRequiredTasksCount, remainingTasksList);
        }

        private static bool IsMiniGameComplete(Game game)
        {
            return game != null && game.IsPublished;
        }

        public async Task<(List<string> skillNames, List<int> percents)> GetSkillsTop3Async(int companyId)
        {
            var allJobs = await jobsService.GetAllJobsAsync();
            var companyJobsList = allJobs
                .Where(job => job.CompanyId == companyId)
                .ToList();

            var skillCountsDictionary = new Dictionary<string, int>();
            int totalRequiredPercentage = EmptyCount;

            foreach (var job in companyJobsList)
            {
                if (job.JobSkills == null || job.JobSkills.Count == 0)
                {
                    continue;
                }
                foreach (var jobSkill in job.JobSkills)
                {
                    var skillName = jobSkill.Skill?.SkillName;
                    if (string.IsNullOrEmpty(skillName))
                    {
                        continue;
                    }
                    if (!skillCountsDictionary.ContainsKey(skillName))
                    {
                        skillCountsDictionary[skillName] = EmptyCount;
                    }
                    skillCountsDictionary[skillName] += jobSkill.RequiredPercentage;
                    totalRequiredPercentage += jobSkill.RequiredPercentage;
                }
            }

            var topSkillNamesList = new List<string>();
            var topSkillPercentagesList = new List<int>();

            if (totalRequiredPercentage == EmptyCount)
            {
                return (topSkillNamesList, topSkillPercentagesList);
            }
            var topThreeSkills = skillCountsDictionary
                .OrderByDescending(skillEntry => skillEntry.Value)
                .Take(TopSkillsLimit);

            foreach (var skillEntry in topThreeSkills)
            {
                topSkillNamesList.Add(skillEntry.Key);
                topSkillPercentagesList.Add((int)Math.Round((double)skillEntry.Value * PercentageMultiplier / totalRequiredPercentage));
            }

            return (topSkillNamesList, topSkillPercentagesList);
        }

        public (List<string> skillNames, List<int> percents) GetSkillsTop3(int companyId)
        {
            var companyJobsList = jobsService
                .GetAllJobsAsync().Result
                .Where(job => job.Company != null && job.Company.CompanyId == companyId)
                .ToList();

            var skillCountsDictionary = new Dictionary<string, int>();
            int totalRequiredPercentage = EmptyCount;

            foreach (var job in companyJobsList)
            {
                if (job.JobSkills == null)
                {
                    continue;
                }
                foreach (var jobSkill in job.JobSkills)
                {
                    var skillName = jobSkill.Skill?.SkillName;
                    if (string.IsNullOrEmpty(skillName))
                    {
                        continue;
                    }
                    if (!skillCountsDictionary.ContainsKey(skillName))
                    {
                        skillCountsDictionary[skillName] = EmptyCount;
                    }
                    skillCountsDictionary[skillName] += jobSkill.RequiredPercentage;
                    totalRequiredPercentage += jobSkill.RequiredPercentage;
                }
            }

            var topSkillNamesList = new List<string>();
            var topSkillPercentagesList = new List<int>();

            if (totalRequiredPercentage == EmptyCount)
            {
                return (topSkillNamesList, topSkillPercentagesList);
            }
            var topThreeSkills = skillCountsDictionary
                .OrderByDescending(skillEntry => skillEntry.Value)
                .Take(TopSkillsLimit);

            foreach (var skillEntry in topThreeSkills)
            {
                topSkillNamesList.Add(skillEntry.Key);
                topSkillPercentagesList.Add((int)Math.Round((double)skillEntry.Value * PercentageMultiplier / totalRequiredPercentage));
            }

            return (topSkillNamesList, topSkillPercentagesList);
        }

        public async Task<string> ApplicantsMessage(int companyId)
        {
            var companyApplicantsList = await applicantService.GetApplicantsByCompany(companyId);

            int currentWeekApplicantsCount = companyApplicantsList
                .Count(applicant => applicant.AppliedAt >= DateTime.Now.AddDays(DaysToLookBack));

            int previousWeekApplicantsCount = companyApplicantsList
                .Count(applicant => applicant.AppliedAt < DateTime.Now.AddDays(DaysToLookBack));

            if (previousWeekApplicantsCount == EmptyCount)
            {
                if (currentWeekApplicantsCount == EmptyCount)
                {
                    return MessageNoApplicantsText;
                }
                return $"{MessageGreatStartPrefix}{currentWeekApplicantsCount}{MessageGreatStartSuffix}";
            }

            double percentageChange = ((double)(currentWeekApplicantsCount - previousWeekApplicantsCount) / previousWeekApplicantsCount) * PercentageMultiplier;

            if (percentageChange < EmptyCount)
            {
                return $"{MessageFewerApplicantsPrefix}{Math.Abs((int)percentageChange)}{MessageFewerApplicantsSuffix}";
            }
            else
            {
                return $"{MessageMoreApplicantsPrefix}{(int)percentageChange}{MessageMoreApplicantsSuffix}";
            }
        }
    }
}