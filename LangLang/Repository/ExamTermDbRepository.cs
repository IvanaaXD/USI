using LangLang.Data;
using LangLang.Domain.IRepository;
using LangLang.Domain.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LangLang.Repository
{
    public class ExamTermDbRepository : IExamTermDbRepository
    {
        private readonly AppDbContext _context;

        public ExamTermDbRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamTerm>> GetAllAsync()
        {
            return await _context.ExamTerms.ToListAsync();
        }

        public async Task<ExamTerm> GetByIdAsync(int id)
        {
            return await _context.ExamTerms.FindAsync(id);
        }

        public async Task AddAsync(ExamTerm examTerm)
        {
            await _context.ExamTerms.AddAsync(examTerm);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExamTerm examTerm)
        {
            _context.ExamTerms.Update(examTerm);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var examTerm = await _context.ExamTerms.FindAsync(id);
            if (examTerm != null)
            {
                _context.ExamTerms.Remove(examTerm);
                await _context.SaveChangesAsync();
            }
        }
    }
}