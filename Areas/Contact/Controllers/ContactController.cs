using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models.Contacts;
using Microsoft.AspNetCore.Authorization;
using App.Models;
// có thể set lại alias 
using ContactModel = App.Models.Contacts.Contact;
using App.Data;

namespace App.Areas.Contact.Controllers
{
    [Area("Contact")]
    [Authorize(Roles = RoleName.Administrator)]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Contact
        [HttpGet("/admin/contact")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contacts.ToListAsync());
        }

        // GET: Contact/Details/5
        [HttpGet("/admin/contact/details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contact/SendContact
        //Message gửi liên hệ
        [TempData]
        public string StatusMessage { get; set; }
        [HttpGet("/contact/")]
        //ko cần đăng nhập vẫn có thể gửi liên hệ
        [AllowAnonymous]
        public IActionResult SendContact()
        {
            return View();
        }

        // POST: Contact/SendContact
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/contact/")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact([Bind("FullName,Email,Message,Phone")] ContactModel contact)
        {
            if (ModelState.IsValid)
            {
                contact.DateSent = DateTime.Now;
                _context.Add(contact);
                await _context.SaveChangesAsync();
                StatusMessage = "Đã gửi liên hệ thành công. Chúng tôi sẽ phản hồi sớm nhất có thể.";
                return RedirectToAction("Index", "Home");
            }
            return View(contact);
        }

        // GET: Contact/Delete/5
        [HttpGet("/admin/contact/delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost("/admin/contact/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
