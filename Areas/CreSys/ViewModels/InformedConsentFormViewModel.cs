using DocumentFormat.OpenXml.Office.SpreadSheetML.Y2023.MsForms;
using Microsoft.Identity.Client;
using ResearchManagementSystem.Areas.CreSys.Models;
using ResearchManagementSystem.Areas.CreSys.ViewModels.FormClasses;
namespace ResearchManagementSystem.Areas.CreSys.ViewModels
{
    public class InformedConsentFormViewModel : EvaluationDataViewModel
    {
        public List<FormClasses.Question> Questions { get; set; } = new List<FormClasses.Question>();


        public InformedConsentFormViewModel()
        {
            Questions.Add(new FormClasses.Question(
                "Is it necessary to seek the informed consent of the participants?",
                AnswerType.YesNoFollowUp)
            {
                FollowUpText = "If NO, please explain.",
                FollowUpAnswer = null
            });

            // Regular Yes/No type questions
            Questions.Add(new FormClasses.Question("Background of the study", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Purpose of the study?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Procedures of the study?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Benefits to the participants?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Risk?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Cost of participation?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Payment or Renumeration?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Extent of confidentiality?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Does the protocol include an adequate process for ensuring that consent is voluntary?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Who to contact for pertinent questions and/or for assistance in a research-related injury?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Is the informed consent written or presented in lay language that participants can understand?", AnswerType.YesNo));
            Questions.Add(new FormClasses.Question("Do you have any other concerns?", AnswerType.YesNo));

            // MultiDropDown question
            Questions.Add(new FormClasses.Question(
                "Recommendation:",
                AnswerType.MultiDropDown,
                new List<string> { "Exempt from Review", "Approved", "Major Revisions Required", "Minor Revisions Required", "Disapproved" }
            ));

            // Text question
            Questions.Add(new FormClasses.Question("Remarks/Reasons for unfavorable decision:", AnswerType.Text)
            {
                Answer = null
            });
        }
    }
}
