using System;
using System.Collections.Generic;

namespace CvBuilder.Models
{
    public class Education
    {
        public int EducationId { get; set; }
        public required string School { get; set; }
        public required string City { get; set; }
        public required string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int ResumeId { get; set; }
        public Resume? Resume { get; set; }

        public string StartDateFormatted => StartDate.ToString("yyyy-MM");
        public string EndDateFormatted => EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM") : "Present"; // ✅ Fix

        public Education() { }

        public Education(string school, string city, DateTime startDate, DateTime? endDate, string description, Resume resume)
        {
            if (string.IsNullOrWhiteSpace(school))
                throw new ArgumentException("School name cannot be empty.", nameof(school));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty.", nameof(city));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.", nameof(description));

            if (endDate.HasValue && endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            School = school;
            City = city;
            StartDate = startDate;
            EndDate = endDate; 
            Description = description;
            Resume = resume ?? throw new ArgumentNullException(nameof(resume));
            ResumeId = resume.ResumeId;
        }
    }
}