using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using LangLang.Domain.Model.Enums;
using System.Linq;
using LangLang.Domain.Model.Reports;

namespace LangLang.Controller
{
    public class ReportController
    {
        public void GenerateReport(IReportGenerator reportGenerator)
        {
            reportGenerator.GenerateReport();
        }
    }
}
