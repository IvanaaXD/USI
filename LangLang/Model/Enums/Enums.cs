
namespace LangLang.Model.Enums
{
    public enum Gender
    {
        Male,
        Female
    }

    public enum EducationLevel
    {
        NoEducation,
        ElementarySchool,
        HighSchool,
        HigherSchool,
        PostgraduateStudies
    }

    public enum SortDirection
    {
        ASC,
        DESC
    }

    public enum LanguageLevel
    {
        A1,
        A2,
        B1,
        B2,
        C1,
        C2,
        NULL
    }

    public enum Language
    {
        English,
        Spanish,
        German,
        Japanese,
        Russian,
        French,
        NULL
    }

    public enum TypeOfMessage
    {
        AcceptEnterCourseRequestMessage,
        DenyEnterCourseRequestMessage,

        QuitCourseRequest,
        AcceptQuitCourseRequestMessage,
        DenyQuitCourseRequestMessage,

        PenaltyPointMessage,
        NULL
    }
}
