using LangLang.Repository;
using LangLang.Domain.Model;
using System.Collections.Generic;

namespace LangLang.Controller
{
    public class PenaltyPointController
    {
        private readonly PenaltyPointDAO _points;

        public PenaltyPointController()
        {
            _points = new PenaltyPointDAO();
        }
        public void Add(PenaltyPoint penaltyPoint)
        {
            _points.AddPenaltyPoint(penaltyPoint);
        }

        public void Update(PenaltyPoint penaltyPoint)
        {
            _points.UpdatePenaltyPoint(penaltyPoint);
        }
        public void Delete(int penaltyPointId)
        {
            _points.RemovePenaltyPoint(penaltyPointId);
        }
        public List<PenaltyPoint> GetAllPenaltyPoints()
        {
            return _points.GetAllPenaltyPoints();
        }
        public List<PenaltyPoint> GetPointsByCourseId(int courseId)
        {
            return _points.GetPenaltyPointsByCourseId(courseId);
        }
        public List<PenaltyPoint> GetPenaltyPointsByStudentId(int studentId)
        {
            return _points.GetPenaltyPointsByStudentId(studentId);
        } 
    }
}
