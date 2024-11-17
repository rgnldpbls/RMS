using CrdlSys.Data;
using CrdlSys.Models;
using OfficeOpenXml; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using ResearchManagementSystem.Models;


namespace CrdlSys.Services
{
    public class ReportService
    {
        private readonly CrdlDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportService(CrdlDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<string> SaveGeneratedReportAsync(string fileName, byte[] reportData, int? year, string typeOfReport, string? researchEventId = null)
        {
            var reportId = GeneratedReport.GenerateReportId();

            var report = new GeneratedReport
            {
                ReportId = reportId,
                FileName = fileName,
                GenerateReportFile = reportData,
                Year = year,
                TypeOfReport = typeOfReport,
                ResearchEventId = researchEventId,
                GeneratedAt = DateTime.Now
            };

            _context.GeneratedReport.Add(report);
            await _context.SaveChangesAsync();

            return reportId;
        }

        public async Task<byte[]> GenerateNewPartnersExcelAsync(int year)
        {
            var currentYear = DateTime.UtcNow.Year;

            /*var newPartners = await _context.UserRoles
                .Where(ur => ur.RoleId == 2
                             && ur.User.AccountCreationDate.Year == year
                             && _context.StakeholderUpload.Any(su => su.StakeholderId == ur.UserId)
                             && _context.StakeholderUpload.Any(c => c.StakeholderId == ur.UserId && c.ContractStatus == "Active")) 
                .Select(ur => new
                {
                    ur.User.UserId,
                    UserName = $"{ur.User.FirstName} {ur.User.LastName}",
                    ur.User.Birthday,
                    ur.User.College,
                    ur.User.Branch,
                    ur.User.Email,
                    ur.User.Webmail
                })
                .ToListAsync();*/

            var newPartners = await _context.StakeholderUpload
                .Where(su => su.CreatedAt.Year == year && su.ContractStatus == "Active")
                .ToListAsync();

            

            if (!newPartners.Any())
            {
                throw new InvalidOperationException($"No new partners found for the year {year}.");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("New Partners");

            worksheet.InsertRow(1, 1);
            var titleCell = worksheet.Cells[1, 1, 1, 7];
            titleCell.Merge = true;
            titleCell.Value = $"New Partners Report for the Year {year}";
            titleCell.Style.Font.Size = 14;
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            titleCell.Style.Font.Color.SetColor(Color.White);
            titleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var headerRow = 2;
            string[] headers = { "UserId", "Stakeholder Name", "Birthday", "College", "Branch", "Email", "Webmail" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[headerRow, i + 1].Value = headers[i];
            }

            var headerRange = worksheet.Cells[headerRow, 1, headerRow, 7];
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            headerRange.Style.Font.Color.SetColor(Color.White);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if (newPartners.Count > 0)
            {
                for (int i = 0; i < newPartners.Count; i++)
                {
                    var partner = newPartners[i];
                    var row = i + headerRow + 1;

                    var partnerInfo = await _userManager.FindByEmailAsync(partner.StakeholdeEmail);

                    worksheet.Cells[row, 1].Value = partnerInfo.Id;
                    worksheet.Cells[row, 2].Value = partner.StakeholderName;
                    worksheet.Cells[row, 3].Value = partnerInfo.Birthday;
                    worksheet.Cells[row, 4].Value = partnerInfo.College;
                    worksheet.Cells[row, 5].Value = partnerInfo.Campus;
                    worksheet.Cells[row, 6].Value = partner.StakeholdeEmail;
                    worksheet.Cells[row, 7].Value = partnerInfo.Webmail;

                    worksheet.Cells[row, 3].Style.Numberformat.Format = "yyyy-MM-dd";

                    if (i % 2 == 0)
                    {
                        worksheet.Cells[row, 1, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                    }
                }

                var dataRange = worksheet.Cells[headerRow, 1, newPartners.Count + headerRow, 7];
                var border = dataRange.Style.Border;
                border.Top.Style = border.Bottom.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(headerRow + 1, 1);

            return package.GetAsByteArray();
        }

        public async Task<byte[]> GenerateEventReportExcelAsync(int year)
        {
            var events = await _context.ResearchEvent
                .Where(e => e.EventDate.Year == year)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();

            AddEventWorksheet(package, "General List", events);

            var preEvents = events
                .Where(e => e.EventDate > DateTime.Now && e.EventStatus.Equals("Scheduled", StringComparison.OrdinalIgnoreCase))
                .ToList();
            AddEventWorksheet(package, "Pre-Event", preEvents);

            var postEvents = events
                .Where(e => e.EndTime <= DateTime.Now && e.EventStatus.Equals("Scheduled", StringComparison.OrdinalIgnoreCase))
                .ToList();
            AddEventWorksheet(package, "Post-Event", postEvents);

            var postponedEvents = events
                .Where(e => e.EventStatus.Equals("Postponed", StringComparison.OrdinalIgnoreCase))
                .ToList();
            AddEventWorksheet(package, "Postponed", postponedEvents);

            var cancelledEvents = events
                .Where(e => e.EventStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                .ToList();
            AddEventWorksheet(package, "Cancelled", cancelledEvents);

            return package.GetAsByteArray();
        }
        private void AddEventWorksheet(ExcelPackage package, string sheetName, List<ResearchEvent> events)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // Add title
            worksheet.InsertRow(1, 1);
            var titleCell = worksheet.Cells[1, 1, 1, 11];
            titleCell.Merge = true;
            titleCell.Value = $"{sheetName} - Events Report";
            titleCell.Style.Font.Size = 14;
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            titleCell.Style.Font.Color.SetColor(Color.White);
            titleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var headerRow = 2;
            string[] headers = {
                "Research Event ID", "Event Name", "Event Description", "Event Location",
                "Event Type", "Registration Type", "Event Date", "End Time", "Event Status",
                "Participant Slot", "Registered Participants"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[headerRow, i + 1].Value = headers[i];
            }

            var headerRange = worksheet.Cells[headerRow, 1, headerRow, headers.Length];
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            headerRange.Style.Font.Color.SetColor(Color.White);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if (events.Any())
            {
                for (int i = 0; i < events.Count; i++)
                {
                    var eventItem = events[i];
                    var row = i + headerRow + 1;

                    worksheet.Cells[row, 1].Value = eventItem.ResearchEventId;
                    worksheet.Cells[row, 2].Value = eventItem.EventName;
                    worksheet.Cells[row, 3].Value = eventItem.EventDescription;
                    worksheet.Cells[row, 4].Value = eventItem.EventLocation;
                    worksheet.Cells[row, 5].Value = eventItem.EventType;
                    worksheet.Cells[row, 6].Value = eventItem.RegistrationType;
                    worksheet.Cells[row, 7].Value = eventItem.EventDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 8].Value = eventItem.EndTime.ToString("hh:mm tt");
                    worksheet.Cells[row, 9].Value = eventItem.EventStatus;
                    worksheet.Cells[row, 10].Value = eventItem.ParticipantsSlot;
                    worksheet.Cells[row, 11].Value = eventItem.ParticipantsCount;

                    worksheet.Cells[row, 7].Style.Numberformat.Format = "yyyy-MM-dd";
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "hh:mm AM/PM";

                    if (i % 2 == 0)
                    {
                        worksheet.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                    }
                }

                var dataRange = worksheet.Cells[headerRow, 1, events.Count + headerRow, headers.Length];
                var border = dataRange.Style.Border;
                border.Top.Style = border.Bottom.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(headerRow + 1, 1);
        }

        public async Task<byte[]> GenerateAttendeesListExcelAsync(string researchEventId)
        {
            var attendees = await _context.ResearchEventRegistration
                .Where(r => r.ResearchEventId == researchEventId)
                .Select(r => new
                {
                    UserId = r.UserId,
                    UserName = r.UserName,
                    Email = r.UserEmail
                })
                .ToListAsync();

            var eventDetails = await _context.ResearchEvent
                .FirstOrDefaultAsync(e => e.ResearchEventId == researchEventId);

            if (eventDetails == null)
            {
                throw new InvalidOperationException("Event details not found.");
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Attendees List");

            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 30;
            worksheet.Column(3).Width = 35;
            worksheet.Column(4).Width = 40;

            worksheet.Row(1).Height = 30;

            string title = $"{eventDetails.ResearchEventId}: {eventDetails.EventName} ({eventDetails.EventDate:yyyy-MM-dd hh:mm tt} to {eventDetails.EndTime:yyyy-MM-dd hh:mm tt})";

            var titleRange = worksheet.Cells["A1:D1"];
            titleRange.Merge = true;
            titleRange.Value = title;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.Size = 14;
            titleRange.Style.Font.Color.SetColor(Color.White);
            titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            titleRange.Style.WrapText = true;

            worksheet.Row(2).Height = 25;
            string[] headers = new string[] { "UserId", "Name", "Email", "Signature" };
            var headerRange = worksheet.Cells["A2:D2"];

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[2, i + 1].Value = headers[i];
            }

            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.Color.SetColor(Color.White);
            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            for (int i = 0; i < attendees.Count; i++)
            {
                var rowIndex = i + 3;
                worksheet.Row(rowIndex).Height = 25;
                var attendee = attendees[i];

                worksheet.Cells[rowIndex, 1].Value = attendee.UserId;
                worksheet.Cells[rowIndex, 2].Value = attendee.UserName;
                worksheet.Cells[rowIndex, 3].Value = attendee.Email;
                worksheet.Cells[rowIndex, 4].Value = "";

                if (i % 2 == 0)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[rowIndex, 1, rowIndex, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                }

                var rowRange = worksheet.Cells[rowIndex, 1, rowIndex, 4];
                rowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            var dataRange = worksheet.Cells[1, 1, attendees.Count + 2, 4];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            var signatureColumn = worksheet.Cells[3, 4, attendees.Count + 2, 4];
            signatureColumn.Style.Border.Left.Style = ExcelBorderStyle.Medium;
            signatureColumn.Style.Border.Right.Style = ExcelBorderStyle.Medium;

            worksheet.View.FreezePanes(3, 1);

            return package.GetAsByteArray();
        }

        public Task<IActionResult> GenerateSentimentReport(double averageScore, double degreePercentage, string tone, List<string> positives, List<string> negatives, string fileName)
        {
            using var stream = new MemoryStream();
            using var workbook = new ExcelPackage(stream);

            var worksheet = workbook.Workbook.Worksheets.Add("Sentiment Analysis Report");

            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 45;
            worksheet.Column(5).Width = 45;

            using (var titleRange = worksheet.Cells["A1:E1"])
            {
                titleRange.Merge = true;
                titleRange.Value = "Sentiment Analysis Report";
                titleRange.Style.Font.Size = 16;
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                titleRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
                titleRange.Style.Font.Color.SetColor(Color.White);
                titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            int headerRow = 3;
            var headers = new[] {
                "Average Score",
                "Degree (%)",
                "Tone",
                "Positive Words",
                "Negative Words"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[headerRow, i + 1];
                cell.Value = headers[i];
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(155, 194, 230));
                cell.Style.Font.Bold = true;
                cell.Style.WrapText = true;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            worksheet.Cells[headerRow + 1, 1].Value = averageScore;
            worksheet.Cells[headerRow + 1, 2].Value = $"{degreePercentage}%";
            worksheet.Cells[headerRow + 1, 3].Value = tone;

            var dataRange = worksheet.Cells[headerRow + 1, 1, headerRow + 1, 3];
            dataRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            dataRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(221, 235, 247));
            dataRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            for (int i = 0; i < Math.Max(positives.Count, negatives.Count); i++)
            {
                if (i < positives.Count)
                {
                    var cell = worksheet.Cells[headerRow + 1 + i, 4];
                    cell.Value = positives[i];
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(198, 239, 206));
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }

                if (i < negatives.Count)
                {
                    var cell = worksheet.Cells[headerRow + 1 + i, 5];
                    cell.Value = negatives[i];
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 199, 206));
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }
            }

            var usedRange = worksheet.Cells[worksheet.Dimension.Address];
            usedRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            usedRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            usedRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            usedRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Row(1).Height = 30;
            worksheet.Row(headerRow).Height = 35;

            using var resultStream = new MemoryStream();
            {
                workbook.SaveAs(resultStream);
                resultStream.Position = 0;
                return Task.FromResult<IActionResult>(new FileContentResult(resultStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName + ".xlsx"
                });
            }
        }

        public async Task<byte[]> GenerateRenewalHistoryExcelAsync(int year)
        {
            var renewalHistory = await _context.RenewalHistory
                .Include(r => r.StakeholderUpload)
                .Where(r => r.RenewalDate.Year == year)
                .Select(r => new
                {
                    UserId = r.StakeholderUpload.StakeholderId,
                    StakeholderName = r.StakeholderUpload.StakeholderName,
                    r.DocumentId,
                    r.StakeholderUpload.NameOfDocument,
                    r.PreviousEndDate,
                    r.NewEndDate,
                    r.RenewalDate
                })
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Renewal History");

            var headerStyle = worksheet.Cells[1, 1, 1, 7].Style;
            headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
            headerStyle.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            headerStyle.Font.Color.SetColor(Color.White);
            headerStyle.Font.Bold = true;
            headerStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerStyle.VerticalAlignment = ExcelVerticalAlignment.Center;

            string[] headers = new string[]
            {
                "User ID", "Stakeholder Name", "Document ID", "Name of Document",
                "Previous End Date", "New End Date", "Renewal Date"
                    };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            for (int i = 0; i < renewalHistory.Count; i++)
            {
                var record = renewalHistory[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = record.UserId;
                worksheet.Cells[row, 2].Value = record.StakeholderName;
                worksheet.Cells[row, 3].Value = record.DocumentId;
                worksheet.Cells[row, 4].Value = record.NameOfDocument;
                worksheet.Cells[row, 5].Value = record.PreviousEndDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 6].Value = record.NewEndDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 7].Value = record.RenewalDate.ToString("yyyy-MM-dd HH:mm:ss");

                worksheet.Cells[row, 5].Style.Numberformat.Format = "yyyy-MM-dd";
                worksheet.Cells[row, 6].Style.Numberformat.Format = "yyyy-MM-dd";
                worksheet.Cells[row, 7].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
            }

            var dataRange = worksheet.Cells[2, 1, renewalHistory.Count + 1, 7];
            var dataBorder = dataRange.Style.Border;
            dataBorder.Top.Style = dataBorder.Bottom.Style = dataBorder.Left.Style = dataBorder.Right.Style = ExcelBorderStyle.Thin;

            for (int row = 2; row <= renewalHistory.Count + 1; row++)
            {
                if (row % 2 == 0)
                {
                    worksheet.Cells[row, 1, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                }
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            worksheet.View.FreezePanes(2, 1);

            worksheet.InsertRow(1, 1);
            var titleCell = worksheet.Cells[1, 1, 1, 7];
            titleCell.Merge = true;
            titleCell.Value = $"Renewal History Report for the Year {year}";
            titleCell.Style.Font.Size = 14;
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            titleCell.Style.Font.Color.SetColor(Color.White);
            titleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            return package.GetAsByteArray();
        }

        public async Task<byte[]> GenerateStakeholderReportAsync()
        {
            var stakeholderData = await _context.StakeholderUpload
                .OrderBy(s => s.TypeOfDocument)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();

            AddWorksheet(package, "General List", stakeholderData, s => true, $"MOAs/MOUs Report - {DateTime.UtcNow.Year}", Color.FromArgb(79, 129, 189));

            AddWorksheet(package, "Ongoing Partners", stakeholderData,
                s => s.ContractStatus == "Active" && !s.IsArchived, $"MOAs/MOUs Report - {DateTime.UtcNow.Year}", Color.FromArgb(155, 187, 89));

            AddWorksheet(package, "Expired Contracts", stakeholderData,
                s => s.ContractStatus == "Expired", $"MOAs/MOUs Report - {DateTime.UtcNow.Year}", Color.FromArgb(230, 126, 34));

            AddWorksheet(package, "Terminated Contracts", stakeholderData,
                s => s.ContractStatus == "Terminated", $"MOAs/MOUs Report - {DateTime.UtcNow.Year}", Color.FromArgb(192, 80, 77));

            return package.GetAsByteArray();
        }

        private void AddWorksheet(ExcelPackage package, string sheetName,
            IEnumerable<StakeholderUpload> data, Func<StakeholderUpload, bool> filter, string title, Color headerColor)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            worksheet.InsertRow(1, 1);
            var titleCell = worksheet.Cells[1, 1, 1, 11];
            titleCell.Merge = true;
            titleCell.Value = title;
            titleCell.Style.Font.Size = 14;
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleCell.Style.Fill.BackgroundColor.SetColor(headerColor);
            titleCell.Style.Font.Color.SetColor(Color.White);
            titleCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var headerStyle = worksheet.Cells[2, 1, 2, 11].Style;
            headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
            headerStyle.Fill.BackgroundColor.SetColor(headerColor);
            headerStyle.Font.Color.SetColor(Color.White);
            headerStyle.Font.Bold = true;
            headerStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            string[] headers = new string[] {
                "StakeholderID", "Stakeholder Name", "DocumentID", "Name Of Document",
                "Type Of Document", "Type of MOA", "Document Description", "Status",
                "Contract Start Date", "Contract End Date", "Contract Status"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[2, i + 1].Value = headers[i];
            }

            var filteredData = data.Where(filter).ToList();

            for (int i = 0; i < filteredData.Count; i++)
            {
                var stakeholder = filteredData[i];
                var row = i + 3; 

                worksheet.Cells[row, 1].Value = stakeholder.StakeholderId;
                worksheet.Cells[row, 2].Value = stakeholder.StakeholderName;
                worksheet.Cells[row, 3].Value = stakeholder.DocumentId;
                worksheet.Cells[row, 4].Value = stakeholder.NameOfDocument;
                worksheet.Cells[row, 5].Value = stakeholder.TypeOfDocument;
                worksheet.Cells[row, 6].Value = stakeholder.TypeOfMOA;
                worksheet.Cells[row, 7].Value = stakeholder.DocumentDescription;
                worksheet.Cells[row, 8].Value = stakeholder.Status;
                worksheet.Cells[row, 9].Value = stakeholder.ContractStartDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 10].Value = stakeholder.ContractEndDate?.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 11].Value = stakeholder.ContractStatus;

                worksheet.Cells[row, 9].Style.Numberformat.Format = "yyyy-MM-dd";
                worksheet.Cells[row, 10].Style.Numberformat.Format = "yyyy-MM-dd";
            }

            var dataRange = worksheet.Cells[2, 1, filteredData.Count + 2, 11];
            var border = dataRange.Style.Border;
            border.Top.Style = border.Bottom.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }
    }
}
