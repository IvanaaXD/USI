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

        public async Task<IEnumerable<ExamTerm>> GetAllExamTerms()
        {
            return await _examTermRepository.GetAllAsync();
        }

        public async Task<ExamTerm> GetExamTermById(int id)
        {
            return await _examTermRepository.GetByIdAsync(id);
        }

        public async Task AddExamTerm(ExamTerm examTerm)
        {
            await _examTermRepository.AddAsync(examTerm);
        }

        public async Task UpdateExamTerm(ExamTerm examTerm)
        {
            await _examTermRepository.UpdateAsync(examTerm);
        }

        public async Task DeleteExamTerm(int id)
        {
            await _examTermRepository.DeleteAsync(id);
        }
    }
}
