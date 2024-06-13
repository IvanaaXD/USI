using LangLang.Domain.Model;
using LangLang.Observer;
using LangLang.Storage;
using System;
using System.Collections.Generic;

namespace LangLang.Domain.IRepository
{
    public interface IPenaltyPointRepository : IObserver
    {
        PenaltyPoint? AddPenaltyPoint(PenaltyPoint point);
        PenaltyPoint? UpdatePenaltyPoint(PenaltyPoint point);
        PenaltyPoint? RemovePenaltyPoint(int id);
        List<PenaltyPoint> GetAllPenaltyPoints();
        PenaltyPoint? GetPenaltyPointById(int id);
        List<PenaltyPoint> GetPenaltyPointsByCourseId(int courseId);
        List<PenaltyPoint> GetPenaltyPointsByStudentId(int studentId);
        List<PenaltyPoint> GetDeletedPenaltyPointsByStudentId(int studentId);
    }
}
