using System;
using CvBuilder.Data;
using CvBuilder.Models;
using CvBuilder.Services;

namespace CvBuilder
{
    public class MenuManager
    {
        private readonly UserCreation _userCreation;
        private readonly ResumeManager _resumeManager;
        private readonly ResumeExporter _resumeExporter;
        private readonly CvBuilderContext _db;

        public MenuManager(UserCreation userCreation, ResumeManager resumeManager, ResumeExporter resumeExporter, CvBuilderContext db)
        {
            _userCreation = userCreation;
            _resumeManager = resumeManager;
            _resumeExporter = resumeExporter;
            _db = db;
        }

        public void ShowStartMenu()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n1. Register User");
                Console.WriteLine("2. Login User");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()?.Trim()!;

                switch (choice)
                {
                    case "1":
                        _userCreation.RegisterUser();
                        break;

                    case "2":
                        Login();
                        break;

                    case "3":
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        private void Login()
        {
            string email = "";
            string password = "";

            while (string.IsNullOrWhiteSpace(email))
            {
                Console.Write("Enter email: ");
                email = Console.ReadLine()?.Trim()!;
            }

            while (string.IsNullOrWhiteSpace(password))
            {
                Console.Write("Enter password: ");
                password = Console.ReadLine()!;
            }

            var user = _userCreation.LoginUser(email, password);
            if (user != null)
            {
                Console.WriteLine($"\nWelcome, {user.FullName}!");
                ShowMainMenu(user);
            }
            else
            {
                Console.WriteLine("Invalid credentials.");
            }
        }

        private void ShowMainMenu(User user)
        {
            while (true)
            {
                Console.WriteLine("\n======== Main Menu ========");
                Console.WriteLine("1. Create Resume");
                Console.WriteLine("2. Edit Resume");
                Console.WriteLine("3. View Resumes");
                Console.WriteLine("4. Delete Resume");
                Console.WriteLine("5. Export Resume to PDF");
                Console.WriteLine("6. Edit user");
                Console.WriteLine("7. Logout");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        _resumeManager.CreateResume(user);
                        break;
                    case "2":
                        Resume? resumeToEdit = SelectResume(user);
                        if (resumeToEdit != null)
                        {
                            _resumeManager.UpdateResume(resumeToEdit);
                        }
                        else
                        {
                            Console.WriteLine("No resume selected.");
                        }
                        break;
                    case "3":
                        Resume? selectedResume = SelectResume(user);

                        if (selectedResume != null)
                        {
                            // Display the selected resume's details
                            _resumeManager.ViewResume(selectedResume);
                        }
                        else
                        {
                            Console.WriteLine("No resume selected.");
                        }
                        break;
                    case "4":
                        Resume? resumeToDelete = SelectResume(user);
                        if (resumeToDelete != null)
                        {
                            _resumeManager.DeleteResume(resumeToDelete);
                        }
                        else
                        {
                            Console.WriteLine("No resume selected.");
                        }
                        break;
                    case "5":
                        //Resume? resumeToExport = SelectResume(user);
                        //if (resumeToExport != null)
                        //{
                        //    _resumeExporter.ExportResume(user);
                        //}
                        //else
                        //{
                        //    Console.WriteLine("No resume selected.");
                        //}
                        _resumeExporter.ExportResume(user);
                        break;
                    case "6":
                        Console.WriteLine("Edit user not implemented yet.");
                        break;
                    case "7":
                        Console.WriteLine("\nLogging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
        private Resume? SelectResume(User user)
        {
            List<Resume> resumes = _resumeManager.GetResumesByUser(user);

            if (resumes.Count == 0)
            {
                Console.WriteLine("No resumes found.");
                return null;
            }

            Console.WriteLine("\nSelect a resume:");
            for (int i = 0; i < resumes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {resumes[i].Title}");
            }

            Console.Write("Enter the number of the resume: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= resumes.Count)
            {
                return resumes[index - 1];
            }

            Console.WriteLine("Invalid selection.");
            return null;
        }
    }
}