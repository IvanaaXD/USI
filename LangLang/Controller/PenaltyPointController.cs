using LangLang.Model.DAO;
using LangLang.Model;
using LangLang.Observer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
