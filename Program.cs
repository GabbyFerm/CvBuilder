using System;
using System.Linq;
using CvBuilder.Data;
using CvBuilder.Services;

namespace CvBuilder
{
    class Program
    {
        static void Main()
        {
            using var db = new CvBuilderContext();
            var userService = new UserCreation(db);
            var resumeManager = new ResumeManager(db);
            var resumeExporter = new ResumeExporter(db);
            var menuManager = new MenuManager(userService, resumeManager, resumeExporter, db);

            menuManager.ShowStartMenu();
        }
    }
}