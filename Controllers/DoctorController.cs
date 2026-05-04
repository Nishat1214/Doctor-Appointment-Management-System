using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Test1.Data;
using Test1.Models;

namespace Test1.Controllers
{
    public class DoctorController : Controller
    {
        private readonly Test1Context _context;

        public DoctorController(Test1Context context)
        {
            _context = context;
        }
        // GET: Doctor/Login
        // GET: Appointment/SelectSpecialty
        public async Task<IActionResult> SelectSpecialty()
        {
            var specialties = await _context.Speciality.ToListAsync();
            return View(specialties);
        }

        public IActionResult ViewAppointments()
        {
           
            return View();
        }
       
      
        // GET: Appointment/SelectDoctor
        public async Task<IActionResult> SelectDoctor(int specialtyId)
        {
            // Validate specialtyId
            if (specialtyId <= 0)
            {
                return RedirectToAction("SelectSpecialty");
            }

            // Fetch doctors for the given specialty, including only approved ones
            var doctors = await _context.Doctor
                .Include(d => d.Specialty)
                .Where(d => d.SpecialtyId == specialtyId && d.IsApproved)
                .ToListAsync();

            if (doctors == null || !doctors.Any())
            {
                ViewBag.SpecialtyId = specialtyId;
                return View(new List<Doctor>()); // Return empty list to trigger no-data message
            }

            ViewBag.SpecialtyId = specialtyId;
            return View(doctors);
        }

        // Other actions (e.g., AddPatientDetails, Confirmation) remain unchanged
        // ...
    
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }


        // POST: Doctor/Login (Email and Password)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Email and password are required.";
                return View();
            }

            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(d => d.Email == email && d.Password == password);

            if (doctor == null)
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View();
            }

            if (!doctor.IsApproved)
            {
                ViewBag.ErrorMessage = "Your account has not been approved by the admin.";
                return View();
            }

            TempData["DoctorId"] = doctor.Id;
            return RedirectToAction("Dashboard", "Doctor");
        }


        // GET: Doctor
        public async Task<IActionResult> Index()
        {
            var test1Context = _context.Doctor.Include(d => d.Specialty);
            return View(await test1Context.ToListAsync());
        }

        // GET: Doctor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctor/Create
        public IActionResult Create()
        {
            ViewData["SpecialtyId"] = new SelectList(_context.Set<Speciality>(), "Id", "Name");
            return View();
        }

        // POST: Doctor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,SpecialtyId,IsApproved")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialtyId"] = new SelectList(_context.Set<Speciality>(), "Id", "Name", doctor.SpecialtyId);
            return View(doctor);
        }

        // GET: Doctor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["SpecialtyId"] = new SelectList(_context.Set<Speciality>(), "Id", "Name", doctor.SpecialtyId);
            return View(doctor);
        }

        // POST: Doctor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,SpecialtyId,IsApproved")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialtyId"] = new SelectList(_context.Set<Speciality>(), "Id", "Name", doctor.SpecialtyId);
            return View(doctor);
        }

        //my work
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["SpecialtyId"] = new SelectList(_context.Set<Speciality>(), "Id", "Name", doctor.SpecialtyId);
            return View(doctor);
        }

        // POST: Doctor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,Name,Email,Password,SpecialtyId,IsApproved")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialtyId"] = new SelectList(_context.Set<Speciality>(), "Id", "Name", doctor.SpecialtyId);
            return View(doctor);
        }
        //done

        // GET: Doctor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctor.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctor.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Appointments(int doctorId)
        {
            if (doctorId == 0)
            {
                return RedirectToAction("Login"); // Redirect if no DoctorId provided
            }

            var approvedAppointments = await _context.Appointment
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.Status == "Approved")
                .ToListAsync();

            ViewBag.DoctorId = doctorId;

            return View(approvedAppointments);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteAppointment(int id, int doctorId)
        {
            var appointment = await _context.Appointment.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();

            // Redirect back to the appointment list (assuming method name is ViewAppointments)
            return RedirectToAction("ViewAppointments", new { id = doctorId });
        }

    }
}
