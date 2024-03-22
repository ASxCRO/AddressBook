using AddressBook.Data.Entities;
using AddressBook.Data.Repositories.Abstraction;
using AddressBook.Localization;
using AddressBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace AddressBook.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContactsRepository _contactsRepository;
        private readonly LanguageService _languageService;

        public ContactsController(IContactsRepository contactsRepository, LanguageService languageService)
        {
            _contactsRepository = contactsRepository;
            _languageService = languageService;
        }

        [HttpGet]
        public IActionResult Index(string term = "", int pageNumber = 1, int pageSize = 12, string sortField = "FirstNameAsc")
        {
            int totalCount = 0;

            if(string.IsNullOrWhiteSpace(term)) totalCount = _contactsRepository.CountAll();
            else totalCount = _contactsRepository.CountByTerm(term);

            if (totalCount < ((pageSize * (pageNumber - 1))))  pageNumber = 1;
            else if (pageNumber > (int)Math.Ceiling((double)totalCount / pageSize)) pageNumber = 1;

            var contactGridViewModel = new ContactGridViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Sort = sortField,
                SearchTerm = term,
                TotalItemsNumber = totalCount,
                Contacts = _contactsRepository.FindAll(pageNumber, pageSize, sortField, term)
            };
            
            return View("Index", contactGridViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Contact());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contact contact)
        {
            if (ModelState.IsValid)
            {
                var id = _contactsRepository.Add(contact);
                TempData["SuccessMessage"] = _languageService.Getkey("SUCCESS_ADDED_CONTACT").Value;
                return RedirectToAction("Edit", new { id });
            }
            return View(contact);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var contact = _contactsRepository.FindByID(id);
            if (contact == null)
                return RedirectToAction("Index", "Error");
            return View("Edit", contact);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _contactsRepository.Update(contact);
                TempData["SuccessMessage"] = _languageService.Getkey("SUCCESS_EDITED_CONTACT").Value;
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var contact = _contactsRepository.FindByID(id);
            if (contact == null)
                return RedirectToAction("Index", "Error");
            return View("Delete", contact);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _contactsRepository.Remove(id);
            TempData["SuccessMessage"] = _languageService.Getkey("SUCCESS_DELETED_CONTACT").Value;
            return RedirectToAction("Index");
        }
    }
}
