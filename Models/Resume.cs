using System;
using System.Collections.Generic;

namespace CvBuilder.Models
{
    public class Resume
    {
        public int ResumeId { get; set; }
        public required string Title { get; set; } 
        public required string Summary { get; set; } // User's background summary
        public int UserId { get; set; }
        public required User User { get; set; }

        public ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<Language> Languages { get; set; } = new List<Language>();

        public Resume() { } // Parameterless constructor requred by EF core

        // Constructor requires a logged-in User
        public Resume(string title, string summary, User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            Title = title;
            Summary = summary;
            User = user;
            UserId = user.UserId;
        }
    }
}