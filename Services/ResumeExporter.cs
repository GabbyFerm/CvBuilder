using System;
using System.IO;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout;
using iText.Layout.Element;
using CvBuilder.Models;
using CvBuilder.Data;


namespace CvBuilder.Services
{
    public class ResumeExporter
    {
        private readonly CvBuilderContext _db;

        public ResumeExporter(CvBuilderContext db)
        {
            _db = db;
        }

        public void ExportResume(User user)
        {
            var resumes = _db.Resumes.Where(r => r.UserId == user.UserId).ToList();

            if (!resumes.Any())
            {
                Console.WriteLine("\nYou have no resumes to export.");
                return;
            }

            Console.WriteLine("\nSelect a resume to export:");
            for (int i = 0; i < resumes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {resumes[i].Title}");
            }

            Console.Write("Enter resume number: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= resumes.Count)
            {
                ResumeExporter.ExportResumeToPdf(resumes[choice - 1]);
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
        public static void ExportResumeToPdf(Resume resume)
        {
            Console.Write("Enter the full path to save the PDF (e.g., C:\\Users\\YourName\\Documents\\Resume.pdf): ");
            string filePath = Console.ReadLine()!;

            try
            {
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
                {
                    Console.WriteLine("The directory does not exist. Creating it...");
                    Directory.CreateDirectory(directory);
                }

                using (var writer = new PdfWriter(filePath))
                using (var pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    // Title & Summary
                    document.Add(new Paragraph($"Resume: {resume.Title}")
                        .SetFontSize(18)
                        .SetFont(boldFont));
                    document.Add(new Paragraph($"Summary: {resume.Summary}\n")
                        .SetFont(normalFont));

                    // Work Experience
                    if (resume.WorkExperiences != null && resume.WorkExperiences.Count > 0)
                    {
                        document.Add(new Paragraph("Work Experience:")
                            .SetFontSize(14)
                            .SetFont(boldFont));

                        foreach (var job in resume.WorkExperiences)
                        {
                            document.Add(new Paragraph($"{job.JobTitle} at {job.Employer} ({job.StartDate:yyyy-MM} - {(job.EndDate.HasValue ? job.EndDate.Value.ToString("yyyy-MM") : "Present")})")
                                .SetFont(boldFont));
                            document.Add(new Paragraph($"{job.City}")
                                .SetFont(normalFont));
                            document.Add(new Paragraph($"{job.Description}\n")
                                .SetFont(normalFont));
                        }
                    }

                    // Education
                    if (resume.Educations != null && resume.Educations.Count > 0)
                    {
                        document.Add(new Paragraph("Education:")
                            .SetFontSize(14)
                            .SetFont(boldFont));

                        foreach (var edu in resume.Educations)
                        {
                            document.Add(new Paragraph($"{edu.School}, {edu.City}")
                                .SetFont(boldFont));
                            document.Add(new Paragraph($"{edu.StartDate:yyyy-MM} - {(edu.EndDate.HasValue ? edu.EndDate.Value.ToString("yyyy-MM") : "Present")}")
                                .SetFont(normalFont));
                            document.Add(new Paragraph($"{edu.Description}\n")
                                .SetFont(normalFont));
                        }
                    }

                    // Skills
                    if (resume.Skills != null && resume.Skills.Count > 0)
                    {
                        document.Add(new Paragraph("Skills:")
                            .SetFontSize(14)
                            .SetFont(boldFont));

                        document.Add(new Paragraph(string.Join(", ", resume.Skills.Select(s => s.SkillName)) + "\n")
                            .SetFont(normalFont));
                    }

                    // Languages
                    if (resume.Languages != null && resume.Languages.Count > 0)
                    {
                        document.Add(new Paragraph("Languages:")
                            .SetFontSize(14)
                            .SetFont(boldFont));

                        foreach (var lang in resume.Languages)
                        {
                            document.Add(new Paragraph($"{lang.LanguageName} - {lang.ProficiencyLevel}")
                                .SetFont(normalFont));
                        }
                    }

                    Console.WriteLine($"\nResume exported to {filePath}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Error: You do not have permission to save to this location. Try another folder.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}