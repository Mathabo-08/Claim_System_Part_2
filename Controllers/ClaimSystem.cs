using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ClaimManagementSystem.Models;
using ClaimManagementSystem.Context;
using Microsoft.Data.SqlClient;

namespace ClaimManagementSystem.Controllers
{
    public class ClaimSystem : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimSystem(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View(); // This view should contain buttons for Lecturer and Contractor
        }

        // POST: Redirect to the corresponding login form based on role
        [HttpPost]
        public IActionResult SelectRole(string role)
        {
            if (role == "Lecturer")
            {
                return RedirectToAction("Login_Lecturer");
            }
            else if (role == "Contractor")
            {
                // Directly redirect to the Login_Contractor view
                return RedirectToAction("Login_Contractor");
            }

            ModelState.AddModelError("", "Invalid user type selected.");
            return View("Login");
        }

        // GET: Login_Lecturer
        public IActionResult Login_Lecturer()
        {
            return View(); // This view should contain the login form for lecturers
        }

        // POST: Login_Lecturer
        [HttpPost]
        public async Task<IActionResult> Login_Lecturer(Lecturer lecturer)
        {
            var existingLecturer = await _context.Lecturer
                .FirstOrDefaultAsync(u => u.LecturerEmail == lecturer.LecturerEmail);

            if (existingLecturer != null)
            {
                HttpContext.Session.SetInt32("UserId", existingLecturer.EmployeeNumber);
                HttpContext.Session.SetString("Role", "Lecturer");
                return RedirectToAction("SubmitClaim");
            }
            else
            {
                _context.Lecturer.Add(new Lecturer
                {
                    LecturerEmail = lecturer.LecturerEmail,
                    LecturerName = "Default Name",
                    LecturerSurname = "Default Surname",
                    LecturerPassword = "Default Password",
                    ModuleCode = "WEB51",
                    Course = "Default Course",
                    MonthlyHoursWorked = 0,
                    HourlyRate = 0.0m
                });

                await _context.SaveChangesAsync();

                var savedLecturer = await _context.Lecturer
                    .FirstOrDefaultAsync(u => u.LecturerEmail == lecturer.LecturerEmail);

                if (savedLecturer != null)
                {
                    HttpContext.Session.SetInt32("UserId", savedLecturer.EmployeeNumber);
                    HttpContext.Session.SetString("Role", "Lecturer");
                    return RedirectToAction("SubmitClaim");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(lecturer);
        }
        // GET: Login_Contractor
        public IActionResult Login_Contractor()
        {
            return View(); // This view should contain the login form for contractors
        }

        // POST: Login_Contractor
        [HttpPost]
        public async Task<IActionResult> Login_Contractor(Contractor contractor)
        {
            var existingContractor = await _context.Contractors
                .FirstOrDefaultAsync(u => u.ContractorEmail == contractor.ContractorEmail);

            if (existingContractor != null)
            {
                HttpContext.Session.SetInt32("UserId", existingContractor.ContractorID);
                HttpContext.Session.SetString("Role", "Contractor");
                return RedirectToAction("PendingClaims");
            }
            else
            {
                // Create a new contractor with default values
                _context.Contractors.Add(new Contractor
                {
                    ContractorUserName = "Default UserName",
                    ContractorEmail = contractor.ContractorEmail,
                    TypeOfContractor = contractor.TypeOfContractor,
                    ContractorPassword = "Default Password", // Set this to a secure value in production
                    Role = "Project Coordinator", // Set a default role if needed
                    ContractorWorkCampus = "Default Campus" // Set a default work campus if needed
                });

                await _context.SaveChangesAsync();

                var savedContractor = await _context.Contractors
                    .FirstOrDefaultAsync(u => u.ContractorEmail == contractor.ContractorEmail);

                if (savedContractor != null)
                {
                    HttpContext.Session.SetInt32("UserId", savedContractor.ContractorID);
                    HttpContext.Session.SetString("Role", "Contractor");
                    return RedirectToAction("PendingClaims");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(contractor);
        }


        // GET: Submit Claim
        public IActionResult SubmitClaim()
        {
            return View(new ClaimSubmission());
        }

        // POST: Submit Claim
        [HttpPost]
        public async Task<IActionResult> SubmitClaim(ClaimSubmission claim, IFormFile suppDocFile)
        {
            if (ModelState.IsValid && suppDocFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await suppDocFile.CopyToAsync(memoryStream);
                    claim.SuppDocFileContent = memoryStream.ToArray();
                }

                claim.ClaimDate = DateTime.Now;
                claim.EmployeeNumber = HttpContext.Session.GetInt32("UserId").Value;
                claim.SuppDocUploadDate = DateTime.Now;

                _context.ClaimSubmission.Add(claim);
                await _context.SaveChangesAsync();
                return RedirectToAction("Claim_Status");
            }

            return View(claim);
        }

        // GET: Pending Claims for Contractors
        public async Task<IActionResult> PendingClaims()
        {
            var pendingClaims = await _context.ClaimSubmission
                .Where(c => c.ClaimStatus == "Pending")
                .ToListAsync();

            return View(pendingClaims);
        }

        // POST: Respond to Claim
        [HttpPost]
        public async Task<IActionResult> RespondToClaim(int claimId, string response, string feedback)
        {
            var claim = await _context.ClaimSubmission.FindAsync(claimId);

            if (claim != null)
            {
                // Call stored procedure to update claim status
                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateClaimStatus @ClaimID, @NewStatus, @ContractorID, @Feedback",
                    new SqlParameter("@ClaimID", claimId),
                    new SqlParameter("@NewStatus", response),
                    new SqlParameter("@ContractorID", HttpContext.Session.GetInt32("UserId").Value),
                    new SqlParameter("@Feedback", feedback));

                // Redirect to PendingClaims after updating
                return RedirectToAction("PendingClaims");
            }

            // Handle the case where the claim doesn't exist
            ModelState.AddModelError("", "Claim not found.");
            return RedirectToAction("PendingClaims");
        }

        // GET: Claim_Status Page
        public IActionResult Claim_Status()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

