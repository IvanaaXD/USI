using LangLang.Domain.IUtility;
using LangLang.Domain.Model.Enums;
using LangLang.Domain.Utility;
using System;

public static class MailStrategyFactory
{
    public static IMailStrategy GetStrategy(TypeOfMessage messageType)
    {
        return messageType switch
        {
            TypeOfMessage.TopStudentsMessage => new TopStudentsMailStrategy(),
            TypeOfMessage.PenaltyPointMessage => new PenaltyPointMailStrategy(),
            TypeOfMessage.DenyEnterCourseRequestMessage => new DenyEnterCourseStrategy(),
            TypeOfMessage.TeacherGradeStudentMessage => new TeacherGradeStudentStrategy(),
            _ => throw new ArgumentException("Invalid email type")
        };
    }
}

