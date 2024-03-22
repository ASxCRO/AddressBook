using AddressBook.Data.Entities;
using AddressBook.Data.Repositories.Abstraction;
using AddressBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace AddressBook.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContactsRepository _contactsRepository;

        public ContactsController(IContactsRepository contactsRepository)
        {
            _contactsRepository = contactsRepository;
        }

        // Endpoint: /Contacts
        [HttpGet]
        public IActionResult Index(int pageNumber = 1, int pageSize = 12)
        {
            var totalCount = _contactsRepository.CountAll();

            var contactGridViewModel = new ContactGridViewModel
            {
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Contacts = _contactsRepository.FindAll(pageNumber, pageSize),
                TotalItemsNumber = totalCount,
                PageSize = pageSize
            };
            
            return View("Index", contactGridViewModel);
        }

        [HttpGet]
        public IActionResult Search(string term, int pageNumber = 1, int pageSize = 12)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return RedirectToAction("Index", new { pageNumber, pageSize });
            }

            var totalCount = _contactsRepository.CountByTerm(term);

            var contactGridViewModel = new ContactGridViewModel
            {
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Contacts = _contactsRepository.FindAllByTerm(term,pageNumber, pageSize),
                SearchTerm = term,
                TotalItemsNumber = totalCount,
                PageSize = pageSize
            };

            return View("Index", contactGridViewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var partner = _contactsRepository.FindByID(id);
            if (partner == null)
                return NotFound();
            return View("Details", partner);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Contact());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contact partner)
        {
            if (ModelState.IsValid)
            {
                _contactsRepository.Add(partner);
                return RedirectToAction("Index");
            }
            return View(partner);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var partner = _contactsRepository.FindByID(id);
            if (partner == null)
                return NotFound();
            return View("Edit", partner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Contact partner)
        {
            if (ModelState.IsValid)
            {
                _contactsRepository.Update(partner);
                return RedirectToAction("Index");
            }
            return View(partner);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var partner = _contactsRepository.FindByID(id);
            if (partner == null)
                return NotFound();
            return View("Delete", partner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _contactsRepository.Remove(id);
            return RedirectToAction("Index");
        }
    }
}
