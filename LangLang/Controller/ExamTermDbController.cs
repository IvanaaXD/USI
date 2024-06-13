using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Controller
{
    public class ExamTermDbController
    {
        private readonly IExamTermDbRepository _examTermRepository;

        public ExamTermDbController(IExamTermDbRepository examTermRepository)
        {
            _examTermRepository = examTermRepository;
        }

       
        public ExamTerm GetExamTermById(int id)
        {
            return  _examTermRepository.GetById(id);
        }

        public void AddExamTerm(ExamTerm examTerm)
        {
            _examTermRepository.Add(examTerm);
        }

        public void UpdateExamTerm(ExamTerm examTerm)
        {
            _examTermRepository.Update(examTerm);
        }

        public void DeleteExamTerm(int id)
        {
            _examTermRepository.Delete(id);
        }
    }
}
