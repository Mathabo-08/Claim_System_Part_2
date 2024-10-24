using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For async operations
using System;
using System.Linq;
using System.Threading.Tasks;
using ClaimManagementSystem.Models;
using ClaimManagementSystem.Context;

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
        public async Task<IActionResult> Login_Lecturer(int employeeNumber, string lecturerEmail)
        {
            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(u => u.EmployeeNumber == employeeNumber && u.LecturerEmail == lecturerEmail);

            if (lecturer != null)
            {
                HttpContext.Session.SetInt32("UserId", lecturer.EmployeeNumber);
                HttpContext.Session.SetString("Role", "Lecturer");
                return RedirectToAction("SubmitClaim");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        // GET: Login_Contractor
        public IActionResult Login_Contractor()
        {
            return View(); // This view should contain the login form for contractors
        }

        // POST: Login_Contractor
        [HttpPost]
        public async Task<IActionResult> Login_Contractor(int contractorID, string contractorEmail, string typeOfContractor)
        {
            var contractor = await _context.Contractors
                .FirstOrDefaultAsync(u => u.ContractorID == contractorID && u.ContractorEmail == contractorEmail && u.TypeOfContractor == typeOfContractor);

            if (contractor != null)
            {
                HttpContext.Session.SetInt32("UserId", contractor.ContractorID);
                HttpContext.Session.SetString("Role", "Contractor");
                return RedirectToAction("PendingClaims");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        // GET: Submit Claim
        public IActionResult SubmitClaim()
        {
            return View(new ClaimSubmission());
        }

        // POST: Submit Claim
        [HttpPost]
        public async Task<IActionResult> SubmitClaim(ClaimSubmission claim)
        {
            if (ModelState.IsValid)
            {
                claim.ClaimDate = DateTime.Now;
                claim.EmployeeNumber = HttpContext.Session.GetInt32("UserId").Value;
                claim.SuppDocUploadDate = DateTime.Now; // Set upload date

                _context.ClaimSubmission.Add(claim);
                await _context.SaveChangesAsync();
                return RedirectToAction("Success");
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
                claim.ClaimStatus = response; // Update claim status
                await _context.SaveChangesAsync();

                // Update ClaimStatus table if needed
                var claimStatus = new ClaimStatus
                {
                    StatusDate = DateTime.Now,
                    ContractorFeedback = feedback,
                    Claim_Status = response,
                    EmployeeNumber = claim.EmployeeNumber,
                    ClaimID = claimId
                };
                _context.ClaimStatuses.Add(claimStatus);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PendingClaims");
        }

        // GET: Success Page
        public IActionResult Success()
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
