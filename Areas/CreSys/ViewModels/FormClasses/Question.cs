namespace ResearchManagementSystem.Areas.CreSys.ViewModels.FormClasses
{
    public enum AnswerType
    {
        YesNo,
        YesNoFollowUp,
        Text,
        MultiDropDown,
        MultipleChoice,
        MultipleChoiceWithText,
        MultipleChoiceWithTextNo
    }

    public class Question
    {
        public string QuestionText { get; set; }
        public string Answer { get; set; }
        public AnswerType AnswerType { get; set; }  // Using the enum instead of a string
        public List<string> Options { get; set; }
        public string FollowUpText { get; set; }
        public string FollowUpAnswer { get; set; }

        // Constructor for simple yes/no or text questions
        public Question()
        {
            Options = new List<string>(); // Initialize lists to prevent null issues
        }

        public Question(string questionText, AnswerType answerType)
        {
            QuestionText = questionText;
            AnswerType = answerType;
            Options = new List<string>();
        }

        // Constructor for multiple-choice questions
        public Question(string questionText, AnswerType answerType, List<string> options, string followUpText = "")
        {
            QuestionText = questionText;
            AnswerType = answerType;
            Options = options;
            FollowUpText = followUpText;
        }
    }

}
