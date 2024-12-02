using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels.FormClasses;

namespace ResearchManagementSystem.Areas.CreSys.ViewModels.ListViewModels
{
    public class ProtocolReviewFormViewModel : EvaluationDataViewModel
    {
        public List<FormClasses.Question> Questions { get; set; } = new List<FormClasses.Question>();


        public ProtocolReviewFormViewModel()
        {
            // Using enums instead of strings for better type safety
            Questions.Add(new Question(
                "1. Is/Are the research question(s) reasonable?",
                AnswerType.MultipleChoiceWithText,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question(
                "2. Are the study objectives specific, measurable, attainable, and reasonable?",
                AnswerType.MultipleChoiceWithText,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question(
                "3. Does the research need to be carried out with human participants?",
                AnswerType.MultipleChoiceWithText,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question(
                "4. Does the protocol present sufficient background information or results of previous studies prior to human experiment?",
                AnswerType.MultipleChoiceWithText,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question(
                "5. Does the study involve individuals who are vulnerable?",
                AnswerType.MultipleChoiceWithText,
               new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If YES or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question(
                "6. Are appropriate mechanisms in place to protect the vulnerable potential participants?",
                AnswerType.MultipleChoiceWithText,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question(
                "7. Are there probable risks to the human participants in the study?",
                AnswerType.MultipleChoice,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" }));

            Questions.Add(new Question(
                "8. Does the protocol adequately address the risk/benefit balance?",
                AnswerType.MultipleChoiceWithText,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question(
                "9. Are toxicological and pharm acological data adequate?",
                AnswerType.MultipleChoiceWithTextNo,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO, please explain."));

            Questions.Add(new Question(
                "10. Is the informed consent procedure/form adequate and culturally appropriate?",
                AnswerType.MultipleChoiceWithTextNo,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO, please explain."));

            Questions.Add(new Question(
                "11. Are the proponents adequately trained and do they have sufficient experience?",
                AnswerType.MultipleChoiceWithText,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question(
                "12. Is the research facility appropriate?",
                AnswerType.MultipleChoiceWithText,
                new List<string> { "YES", "NO", "NOT APPLICABLE", "UNABLE TO ASSESS" },
                "If NO or UNABLE TO ASSESS, please explain."));

            Questions.Add(new Question("13. Do you have any other concerns?", AnswerType.YesNo));

            Questions.Add(new Question(
                "Recommendation:",
                AnswerType.MultiDropDown,
                new List<string> { "Approved", "Major Revisions Required", "Minor Revisions Required", "Disapproved" }));

            Questions.Add(new Question("Remarks/Reasons for unfavorable decision:", AnswerType.Text)
            {
                Answer = " " // Set default answer
            });
        }
    }
}
