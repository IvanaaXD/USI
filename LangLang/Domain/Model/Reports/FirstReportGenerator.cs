using LangLang.Controller;

namespace LangLang.Domain.Model.Reports
{
    public class FirstReportGenerator : IReportGenerator
    {
        CourseController _courseController = Injector.CreateInstance<CourseController>();
        StudentsController _studentController = Injector.CreateInstance<StudentsController>();

        public void GenerateReport()
        {

            PdfGenerator pdfGenerator = new PdfGenerator("..\\..\\..\\Data\\report1.pdf");
            pdfGenerator.AddTitle("Number of penalty points in the last year");
            pdfGenerator.AddNewLine();
            pdfGenerator.AddTable(_courseController.GetPenaltyPointsLastYearPerCourse(), "Course", "Penalties");
            pdfGenerator.AddNewPage();
            pdfGenerator.AddTitle("Average points of students by penalties");
            pdfGenerator.AddNewLine();
            for (int i = 0; i <= 3; i++)
            {
                pdfGenerator.AddSubtitle("Number of penalty points: " + i);
                pdfGenerator.AddTable(_studentController.GetStudentsAveragePointsPerPenalty()[i], "Student", "Average points");
            }
            pdfGenerator.SaveAndClose();
            SendEmail();

        }
        private void SendEmail()
        {
            EmailSender emailSender = new EmailSender("smtp.gmail.com", 587, "diirrektorr@gmail.com", "dvwa dbkw bzyl cauy");
            emailSender.SendEmail("diirrektorr@gmail.com", "diirrektorr@gmail.com", "Report 1", "Report 1 body",
                                    "..\\..\\..\\Data\\report1.pdf");
        }

    }
}
