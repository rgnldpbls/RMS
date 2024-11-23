using CrdlSys.Data;
using CrdlSys.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CrdlSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MimeKit;
using Microsoft.AspNetCore.Identity;
using ResearchManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;


namespace CrdlSys.Controllers
{
    [Area("CrdlSys")]
    public class ResearcherController : Controller
    {
        private readonly CrdlDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResearcherController(CrdlDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles ="Faculty")]
        public IActionResult ResearcherHomePage()
        {
            return View();
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> ViewEvents()
        {
            var user = await _userManager.GetUserAsync(User);

            var events = _context.ResearchEvent.ToList();

            var eventViewModels = events.Select(e => new ResearchEventViewModel
            {
                ResearchEventId = e.ResearchEventId,
                EventName = e.EventName,
                EventDescription = e.EventDescription,
                EventLocation = e.EventLocation,
                EventType = e.EventType,
                EventDate = e.EventDate,
                EndTime = e.EndTime,
                RegistrationOpen = e.RegistrationOpen,
                RegistrationDeadline = e.RegistrationDeadline,
                EventStatus = e.EventStatus,
                RegistrationType = e.RegistrationType,
                ParticipantsSlot = e.ParticipantsSlot,
                ParticipantsCount = e.ParticipantsCount,
                EventThumbnail = e.EventThumbnail,
                UpdatedAt = e.UpdatedAt,
                IsArchived = e.IsArchived,
                IsUserRegistered = _context.ResearchEventRegistration
                .Any(r => r.UserId == user.Id && r.ResearchEventId == e.ResearchEventId),
                IsInvited = _context.ResearchEventInvitation
                .Any(i => i.UserId == user.Id && i.ResearchEventId == e.ResearchEventId),
                    InvitationStatus = _context.ResearchEventInvitation
                .Where(i => i.UserId == user.Id && i.ResearchEventId == e.ResearchEventId)
                .Select(i => i.InvitationStatus)
                .FirstOrDefault()
            }).ToList();

            return View(eventViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterForEvent(string eventId)
        {
            var researchEvent = await _context.ResearchEvent.FirstOrDefaultAsync(e => e.ResearchEventId == eventId);

            if (researchEvent == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("ViewEvents");
            }

            var user = await _userManager.GetUserAsync(User);

            if (researchEvent.RegistrationType == "Invitational")
            {
                var invitation = await _context.ResearchEventInvitation
                    .FirstOrDefaultAsync(i => i.ResearchEventId == eventId && i.UserId == user.Id && i.InvitationStatus == "Accepted");

                if (invitation == null)
                {
                    TempData["ErrorMessage"] = "You must be invited to register for this event.";
                    return RedirectToAction("ViewEvents");
                }
            }

            if (researchEvent.ParticipantsCount >= researchEvent.ParticipantsSlot)
            {
                TempData["ErrorMessage"] = "No available slots for this event.";
                return RedirectToAction("ViewEvents");
            }

            var existingRegistration = await _context.ResearchEventRegistration
                .FirstOrDefaultAsync(r => r.ResearchEventId == eventId && r.UserId == user.Id);

            if (existingRegistration != null)
            {
                TempData["ErrorMessage"] = "You have already registered for this event.";
                return RedirectToAction("ViewEvents");
            }

            var registration = new ResearchEventRegistration
            {
                ResearchEventId = eventId,
                UserName = $"{user.FirstName} {user.LastName}",
                UserEmail = user.Email,
                UserId = user.Id, 
                RegistrationDate = DateTime.Now
            };

            _context.ResearchEventRegistration.Add(registration);

            researchEvent.ParticipantsCount += 1;

            await _context.SaveChangesAsync();


            SendEmailRegistrationConfirmation(user.Email, $"{user.FirstName} {user.LastName}", researchEvent.EventName);

            TempData["SuccessMessage"] = "Successfully registered for the event!";
            return RedirectToAction("ViewEvents");
        }

        private void SendEmailRegistrationConfirmation(string recipientEmail, string userName, string eventName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RMO CRDL Notification", "rmocrdlnotification@gmail.com"));
            message.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
            message.Subject = $"Registration Confirmation for {eventName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
        <html>
            <body>
                <p>Dear {userName},</p>
                <p>Thank you for registering for the event <strong>{eventName}</strong>.</p>
                <p>We look forward to your participation. Please check your portal for more event details.</p>
            </body>
        </html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("rmocrdlnotification@gmail.com", "uhfl pded bety whxu");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RespondToInvitation(string eventId, string response)
        {
            var user = await _userManager.GetUserAsync(User);

            var invitation = await _context.ResearchEventInvitation
                .FirstOrDefaultAsync(i => i.ResearchEventId == eventId && i.UserId.ToString() == user.Id);

            if (invitation != null)
            {
                invitation.InvitationStatus = response == "Accept" ? "Accepted" : "Declined";
                _context.Update(invitation);

                if (response == "Accept")
                {

                    var existingRegistration = await _context.ResearchEventRegistration
                        .FirstOrDefaultAsync(r => r.ResearchEventId == eventId && r.UserId == user.Id);

                    if (existingRegistration == null)
                    {
                        var registration = new ResearchEventRegistration
                        {
                            ResearchEventId = eventId,
                            UserName = $"{user.FirstName} {user.LastName}",
                            UserEmail = user.Email,
                            UserId = user.Id,
                            RegistrationDate = DateTime.Now
                        };
                        _context.ResearchEventRegistration.Add(registration);

                        var researchEvent = await _context.ResearchEvent
                            .FirstOrDefaultAsync(e => e.ResearchEventId == eventId);
                        if (researchEvent != null)
                        {
                            researchEvent.ParticipantsCount += 1;
                            _context.Update(researchEvent);

                            SendEmailAcceptanceConfirmation(user.Email, $"{user.FirstName} {user.LastName}", researchEvent.EventName);
                        }

                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = response == "Accept"
                    ? "You have successfully accepted the invitation and been registered for the event."
                    : "You have declined the invitation.";
            }
            else
            {
                TempData["ErrorMessage"] = "Invitation not found.";
            }
            return RedirectToAction("ViewEvents");
        }

        private void SendEmailAcceptanceConfirmation(string recipientEmail, string userName, string eventName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("RMO CRDL Notification", "rmocrdlnotification@gmail.com"));
            message.To.Add(new MailboxAddress(recipientEmail, recipientEmail));
            message.Subject = $"Invitation Accepted for {eventName}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
        <html>
            <body>
                <p>Dear {userName},</p>
                <p>We are pleased to confirm your acceptance for the event <strong>{eventName}</strong>.</p>
                <p>You have been successfully registered. Please check your portal for more details.</p>
            </body>
        </html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("rmocrdlnotification@gmail.com", "uhfl pded bety whxu");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
