using CvBuilder.Models;
using CvBuilder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CvBuilder.Services
{
    public class ResumeManager
    {
        private readonly CvBuilderContext _db;

        public ResumeManager(CvBuilderContext db)
        {
            _db = db;
        }

        public void CreateResume(User loggedInUser)
        {
            Console.Write("\nEnter resume title: ");
            string title = Console.ReadLine()!;

            Console.Write("Enter summary: ");
            string summary = Console.ReadLine()!;

            Resume newResume = new Resume
            {
                Title = title,
                Summary = summary,
                User = loggedInUser,
                UserId = loggedInUser.UserId,
                Educations = new List<Education>(),
                WorkExperiences = new List<WorkExperience>(),
                Skills = new List<Skill>(),
                Languages = new List<Language>()
            };

            GetEducationEntries(newResume);
            GetWorkExperienceEntries(newResume);
            GetSkills(newResume);
            GetLanguages(newResume);

            SaveResume(newResume);
        }
        private void GetEducationEntries(Resume resume)
        {
            while (true)
            {
                Console.Write("\nEnter school name (or press Enter to stop adding education): ");
                string school = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(school)) break;

                Console.Write("Enter city: ");
                string city = Console.ReadLine()!;

                Console.Write("Enter description: ");
                string description = Console.ReadLine()!;

                Console.Write("Enter start date (yyyy-MM): ");
                DateTime startDate = DateTime.ParseExact(Console.ReadLine()!, "yyyy-MM", null);

                Console.Write("Enter end date (yyyy-MM, or leave empty if still studying): ");
                string endDateInput = Console.ReadLine()!;
                DateTime? endDate = string.IsNullOrWhiteSpace(endDateInput) ? null : DateTime.ParseExact(endDateInput, "yyyy-MM", null);

                resume.Educations.Add(new Education
                {
                    School = school,
                    City = city,
                    Description = description,
                    StartDate = startDate,
                    EndDate = endDate,
                    ResumeId = resume.ResumeId,
                    Resume = resume
                });
            }
        }
        private void GetWorkExperienceEntries(Resume resume)
        {
            while (true)
            {
                Console.Write("\nEnter job title (or press Enter to stop adding work exprecience): ");
                string newJobTitle = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(newJobTitle)) break;

                Console.Write("Enter employer: ");
                string newEmployer = Console.ReadLine()!;

                Console.Write("Enter city: ");
                string newCity = Console.ReadLine()!;

                Console.Write("Enter job description: ");
                string jobDescription = Console.ReadLine()!;

                Console.Write("Enter start date (yyyy-MM): ");
                DateTime newStartDate = DateTime.ParseExact(Console.ReadLine()!, "yyyy-MM", null);

                Console.Write("Enter end date (yyyy-MM, or leave empty if still working): ");
                string newEndDateInput = Console.ReadLine()!;
                DateTime? newEndDate = string.IsNullOrWhiteSpace(newEndDateInput) ? null : DateTime.ParseExact(newEndDateInput, "yyyy-MM", null);

                resume.WorkExperiences.Add(new WorkExperience
                {
                    JobTitle = newJobTitle,
                    Employer = newEmployer,
                    City = newCity,
                    StartDate = newStartDate,
                    EndDate = newEndDate,
                    ResumeId = resume.ResumeId,
                    Resume = resume,
                    Description = jobDescription 
                });

                Console.WriteLine("Work experience entry added.");
            }
        }

        private void GetSkills(Resume resume)
        {
            Console.Write("\nEnter skills (comma-separated, or press Enter to skip): ");
            string skillsInput = Console.ReadLine()!;

            if (!string.IsNullOrWhiteSpace(skillsInput))
            {
                foreach (var skill in skillsInput.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s))) // Ensure no empty skills
                {
                    try
                    {
                        var newSkill = new Skill(skill, resume);
                        resume.Skills.Add(newSkill);
                        Console.WriteLine($"Skill '{newSkill.SkillName}' added successfully.");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error adding skill: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No skills entered.");
            }
        }

        private void GetLanguages(Resume resume)
        {
            while (true)
            {
                Console.Write("\nEnter language (or press Enter to stop adding languages): ");
                string languageName = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(languageName)) break;

                string proficiencyLevel = string.Empty;
                while (true)
                {
                    Console.Write("Enter proficiency level (Beginner, Intermediate, Fluent): ");
                    proficiencyLevel = Console.ReadLine()!;

                    if (string.IsNullOrWhiteSpace(proficiencyLevel))
                    {
                        Console.WriteLine("Proficiency level cannot be empty.");
                    }
                    else if (!new[] { "Beginner", "Intermediate", "Fluent" }.Contains(proficiencyLevel, StringComparer.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Invalid proficiency level. Please choose from 'Beginner', 'Intermediate', or 'Fluent'.");
                    }
                    else
                    {
                        break;
                    }
                }
                try
                {
                    var newLanguage = new Language(languageName, proficiencyLevel, resume);
                    resume.Languages.Add(newLanguage);
                    Console.WriteLine($"Language '{newLanguage.LanguageName}' with proficiency '{newLanguage.ProficiencyLevel}' added successfully.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error adding language: {ex.Message}");
                }
            }
        }

        private void SaveResume(Resume resume)
        {
            _db.Resumes.Add(resume);
            _db.SaveChanges();
            Console.WriteLine("\nResume created successfully!");
        }

        public List<Resume> GetResumesByUser(User user)
        {
            return _db.Resumes
                .Where(r => r.UserId == user.UserId) 
                .Include(r => r.WorkExperiences)
                .ToList();
        }

        public void ViewResume(Resume resume)
        {
            Console.WriteLine($"\nViewing Resume: {resume.Title}");
            Console.WriteLine($"Summary: {resume.Summary}");
            Console.WriteLine("\nWork Experience:");
            foreach (var job in resume.WorkExperiences)
            {
                Console.WriteLine($"- {job.JobTitle} at {job.Employer} ({job.StartDate:yyyy-MM} - {(job.EndDate.HasValue ? job.EndDate.Value.ToString("yyyy-MM") : "Present")})");
                Console.WriteLine($"City: {job.City}");
                Console.WriteLine($"Description: {job.Description}\n");
            }

            Console.WriteLine("\nEducation:");
            if (resume.Educations.Count > 0)
            {
                foreach (var education in resume.Educations)
                {
                    Console.WriteLine($"- {education.School} ({education.StartDate:yyyy-MM} - {(education.EndDate.HasValue ? education.EndDate.Value.ToString("yyyy-MM") : "Present")})");
                    Console.WriteLine($"City: {education.City}");
                    Console.WriteLine($"Description: {education.Description}\n");
                }
            }
            else
            {
                Console.WriteLine("No education entries found.\n");
            }

            Console.WriteLine("\nSkills:");
            if (resume.Skills.Count > 0)
            {
                foreach (var skill in resume.Skills)
                {
                    Console.WriteLine($"- {skill.SkillName}");
                }
            }
            else
            {
                Console.WriteLine("No skills found.\n");
            }

            Console.WriteLine("\nLanguages:");
            if (resume.Languages.Count > 0)
            {
                foreach (var language in resume.Languages)
                {
                    Console.WriteLine($"- {language.LanguageName} (Proficiency: {language.ProficiencyLevel})");
                }
            }
            else
            {
                Console.WriteLine("No languages found.\n");
            }
            Console.WriteLine("\nPress any key to return to the main menu.");
            Console.ReadKey(); 
        }

        public void UpdateResume(Resume resumeToUpdate)
        {
            Console.WriteLine($"\nEditing Resume: {resumeToUpdate.Title}");

            while (true)
            {
                Console.WriteLine("\nWhich section would you like to update?");
                Console.WriteLine("1. Title");
                Console.WriteLine("2. Summary");
                Console.WriteLine("3. Add/Edit Education");
                Console.WriteLine("4. Add/Edit Work Experience");
                Console.WriteLine("5. Add/Edit Skills");
                Console.WriteLine("6. Add/Edit Languages");
                Console.WriteLine("7. Exit and Save Changes");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        Console.Write("\nEnter new title: ");
                        resumeToUpdate.Title = Console.ReadLine()!;
                        Console.WriteLine("Title updated successfully!");
                        break;
                    case "2":
                        Console.Write("\nEnter new summary: ");
                        resumeToUpdate.Summary = Console.ReadLine()!;
                        Console.WriteLine("Summary updated successfully!");
                        break;
                    case "3":
                        Console.WriteLine("\nUpdating Education...");
                        GetEducationEntries(resumeToUpdate);
                        Console.WriteLine("Education updated successfully!");
                        break;
                    case "4":
                        Console.WriteLine("\nUpdating Work Experience...");
                        GetWorkExperienceEntries(resumeToUpdate);
                        Console.WriteLine("Work experience updated successfully!");
                        break;
                    case "5":
                        Console.WriteLine("\nUpdating Skills...");
                        GetSkills(resumeToUpdate);
                        Console.WriteLine("Skills updated successfully!");
                        break;
                    case "6":
                        Console.WriteLine("\nUpdating Languages...");
                        GetLanguages(resumeToUpdate);
                        Console.WriteLine("Languages updated successfully!");
                        break;
                    case "7":
                        Console.WriteLine("\nSaving changes...");
                        _db.SaveChanges();
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }
            }
        }
        public void DeleteResume(Resume resumeToDelete)
        {
            Console.Write($"Are you sure you want to delete '{resumeToDelete.Title}'? (y/n): ");
            string confirm = Console.ReadLine()!.ToLower();

            if (confirm == "y")
            {
                _db.Resumes.Remove(resumeToDelete);
                _db.SaveChanges();
                Console.WriteLine("\nResume deleted successfully.");
            }
            else
            {
                Console.WriteLine("\nDeletion canceled.");
            }
        }
    }
}