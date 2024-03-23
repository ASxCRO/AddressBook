using AddressBook.Data.Entities;
using AddressBook.Data.Repositories.Abstraction;
using AddressBook.Localization;
using AddressBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            int totalCount;

            if (string.IsNullOrWhiteSpace(term)) totalCount = _contactsRepository.CountAll();
            else totalCount = _contactsRepository.CountByTerm(term);

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (pageNumber > totalPages) pageNumber = 1;

            var contactGridViewModel = new ContactGridViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Sort = sortField,
                SearchTerm = term,
                TotalItemsNumber = totalCount,
                Contacts = _contactsRepository.FindAll(pageNumber, pageSize, sortField, term)
            };

            
            TempData["ContactGridPager"] = JsonSerializer.Serialize(contactGridViewModel);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _contactsRepository.Update(contact);
                TempData["SuccessMessage"] = _languageService.Getkey("SUCCESS_EDITED_CONTACT").Value;

                var contactGridPager = JsonSerializer.Deserialize<ContactGridViewModel>(TempData["ContactGridPager"].ToString());

                return RedirectToAction("Index", new
                {
                    term = contactGridPager.SearchTerm,
                    sortField = contactGridPager.Sort,
                    pageNumber = contactGridPager.PageNumber,
                    pageSize = contactGridPager.PageSize,
                });
            }

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _contactsRepository.Remove(id);
            TempData["SuccessMessage"] = _languageService.Getkey("SUCCESS_DELETED_CONTACT").Value;

            var contactGridPager = JsonSerializer.Deserialize<ContactGridViewModel>(TempData["ContactGridPager"].ToString());

            return RedirectToAction("Index", new {
                term = contactGridPager.SearchTerm,
                sortField = contactGridPager.Sort,
                pageNumber = contactGridPager.PageNumber,
                pageSize = contactGridPager.PageSize,
            });
        }
    }
}
