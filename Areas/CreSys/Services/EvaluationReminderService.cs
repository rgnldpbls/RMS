using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RemcSys.Models;
using ResearchManagementSystem.Areas.CreSys.Data;
using ResearchManagementSystem.Areas.CreSys.Interfaces;
using ResearchManagementSystem.Models;

namespace ResearchManagementSystem.Areas.CreSys.Services
{
    public class EvaluationReminderService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public EvaluationReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run the reminder task every day (24 hours)
            _timer = new Timer(SendEvaluationReminderEmails, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void SendEvaluationReminderEmails(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<CreDbContext>();
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var _emailService = scope.ServiceProvider.GetRequiredService<IEthicsEmailService>();

                var currentDate = DateTime.UtcNow;

                // Get all accepted evaluations
                var evaluationsToRemind = await _context.CRE_EthicsEvaluation
                    .Where(e => e.EvaluationStatus == "Accepted" &&
                                e.StartDate.HasValue &&
                                e.StartDate.Value.AddDays(5) <= currentDate) // Checks for evaluations that have been assigned for at least 5 days
                    .ToListAsync();

                foreach (var evaluation in evaluationsToRemind)
                {
                    var daysSinceAssignment = (DateTime.UtcNow - evaluation.StartDate.Value).Days;

                    // Continue sending reminders if the evaluation is not yet "Evaluated"
                    if (evaluation.EvaluationStatus != "Evaluated")
                    {
                        // Fetch evaluator
                        var evaluator = await _context.CRE_EthicsEvaluator
                            .FirstOrDefaultAsync(e => e.EthicsEvaluatorId == evaluation.EthicsEvaluatorId);

                        if (evaluator != null)
                        {
                            var evaluatorUser = await _userManager.FindByIdAsync(evaluator.UserID);
                            if (evaluatorUser != null && !string.IsNullOrEmpty(evaluatorUser.Email))
                            {
                                string evaluatorFullName = $"{evaluatorUser.FirstName} {evaluatorUser.MiddleName?.FirstOrDefault()}. {evaluatorUser.LastName}";
                                string evaluatorEmail = evaluatorUser.Email;

                                // Check how many days have passed and adjust the message accordingly
                                string subject = "Reminder: Complete Your Ethics Application Evaluation";
                                string daysRemaining = Math.Max(0, 10 - daysSinceAssignment).ToString(); // Ensure days remaining is not negative

                                string body = $@"
                                    <p>You were assigned to evaluate the Ethics Application with UREC No: <strong>{evaluation.UrecNo}</strong>.</p>
                                    <p>It's been {daysSinceAssignment} days since you accepted the assignment. You have <strong>{daysRemaining} days</strong> remaining to complete the evaluation.</p>
                                    <p>If you do not complete the evaluation within <strong>10 days</strong>, the assignment may be auto-declined.</p>
                                    <p>Please complete the evaluation as soon as possible to avoid delays in processing.</p>";
                                // Send email
                                await _emailService.SendEmailAsync(evaluatorEmail, evaluatorFullName, subject, body);
                            }
                        }
                    }
                }

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }


}
