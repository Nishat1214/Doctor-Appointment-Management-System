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
    public class AppointmentController : Controller
    {
        private readonly Test1Context _context;

        public AppointmentController(Test1Context context)
        {
            _context = context;
        }

        // GET: Appointment/SelectSpecialty
        public async Task<IActionResult> SelectSpeciality()
        {
            var specialties = await _context.Speciality.ToListAsync();
            return View(specialties);
        }

        // GET: Appointment/SelectDoctor
        public async Task<IActionResult> SelectDoctor(int specialtyId)
        {
            var doctors = await _context.Doctor
                .Include(d => d.Specialty)
                .Where(d => d.SpecialtyId == specialtyId && d.IsApproved)
                .ToListAsync();

            ViewBag.SpecialtyId = specialtyId;
            return View(doctors);
        }

        // GET: Appointment/Create
        public async Task<IActionResult> Create(int specialtyId)
        {
            if (specialtyId <= 0)
            {
                return RedirectToAction("SelectSpecialty");
            }

            ViewData["DoctorId"] = new SelectList(
                await _context.Doctor
                    .Where(d => d.SpecialtyId == specialtyId && d.IsApproved)
                    .ToListAsync(), "Id", "Email");
            return View();
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int specialtyId, AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Save the patient
                    var patient = new Patient
                    {
                        Name = model.PatientName,
                        Age = model.PatientAge,
                        Sex = "Unknown",
                        Mobile = model.Mobile,
                        Email = "auto@system.com",
                        Description = "Created during appointment",
                        Date = DateTime.Today
                    };

                    _context.Patient.Add(patient);
                    await _context.SaveChangesAsync();

                    // 2. Save the appointment
                    var appointment = new Appointment
                    {
                        DoctorId = model.DoctorId,
                        PatientId = patient.Id,
                        AppointmentDate = model.AppointmentDate,
                        Status = "Pending",
                        Notes = model.Notes
                    };

                    _context.Appointment.Add(appointment);
                    await _context.SaveChangesAsync();

                    // 3. Store data in TempData for the Confirmation page
                    TempData["PatientId"] = patient.Id;
                    TempData["DoctorId"] = model.DoctorId;
                    TempData["AppointmentDate"] = model.AppointmentDate.ToString("yyyy-MM-dd");
                    TempData["PatientName"] = model.PatientName;

                    // Redirect to Confirmation page
                    return RedirectToAction("Confirmation");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error saving appointment: {ex.Message}");
                }
            }

            // If we reach here, there was a validation error or an exception
            ViewData["DoctorId"] = new SelectList(
                await _context.Doctor.Where(d => d.SpecialtyId == specialtyId && d.IsApproved).ToListAsync(),
                "Id", "Email", model.DoctorId
            );

            return View(model);
        }

        // Confirmation Page
        public IActionResult Confirmation()
        {
            return View();
        }

        // GET: Appointment/AddPatientDetails
        public async Task<IActionResult> AddPatientDetails(int doctorId, int specialtyId)
        {
            if (doctorId <= 0 || specialtyId <= 0)
            {
                return RedirectToAction("SelectSpecialty");
            }

            var doctorExists = await _context.Doctor.AnyAsync(d => d.Id == doctorId && d.SpecialtyId == specialtyId && d.IsApproved);
            if (!doctorExists)
            {
                return RedirectToAction("SelectDoctor", new { specialtyId });
            }

            var viewModel = new PatientAppointmentViewModel
            {
                Patient = new Patient(),
                DoctorId = doctorId,
                SpecialtyId = specialtyId,
                AppointmentDate = DateTime.Now.AddDays(1) // Default to tomorrow
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPatientDetails(PatientAppointmentViewModel viewModel)
        {
            var doctorExists = await _context.Doctor.AnyAsync(d => d.Id == viewModel.DoctorId && d.SpecialtyId == viewModel.SpecialtyId && d.IsApproved);
            if (!doctorExists)
            {
                ModelState.AddModelError("", "Invalid doctor or specialty selection.");
                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                _context.Add(viewModel.Patient);
                await _context.SaveChangesAsync();

                var appointment = new Appointment
                {
                    PatientId = viewModel.Patient.Id,
                    DoctorId = viewModel.DoctorId,
                    AppointmentDate = viewModel.AppointmentDate,
                    Status = "Pending",
                    Notes = viewModel.AppointmentNotes
                };
                _context.Add(appointment);
                await _context.SaveChangesAsync();

                TempData["PatientId"] = viewModel.Patient.Id;
                TempData["DoctorId"] = viewModel.DoctorId;
                TempData["AppointmentDate"] = viewModel.AppointmentDate.ToString("yyyy-MM-dd HH:mm");
                TempData["PatientName"] = viewModel.Patient.Name;
                return RedirectToAction("Confirmation");
            }

            return View(viewModel);
        }

        // GET: Appointment
        public async Task<IActionResult> Index()
        {
            var test1Context = _context.Appointment.Include(a => a.Doctor).Include(a => a.Patient);
            return View(await test1Context.ToListAsync());
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Approved";
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Appointment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "Id", "Email", appointment.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Set<Patient>(), "Id", "Mobile", appointment.PatientId);
            return View(appointment);
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,DoctorId,AppointmentDate,Status,Notes")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            ViewData["DoctorId"] = new SelectList(_context.Set<Doctor>(), "Id", "Email", appointment.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Set<Patient>(), "Id", "Mobile", appointment.PatientId);
            return View(appointment);
        }

        // GET: Appointment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointment.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.Id == id);
        }
    }
}