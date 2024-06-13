using LangLang.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Domain.IRepository
{
    public interface IExamTermDbRepository
    {
        Task<IEnumerable<ExamTerm>> GetAllAsync();
        Task<ExamTerm> GetByIdAsync(int id);
        Task AddAsync(ExamTerm examTerm);
        Task UpdateAsync(ExamTerm examTerm);
        Task DeleteAsync(int id);
    }
}
