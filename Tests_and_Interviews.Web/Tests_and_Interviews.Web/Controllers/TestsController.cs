using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tests_and_Interviews.Web.Clients;
using Tests_and_Interviews.Web.Dtos;
using Tests_and_Interviews.Web.Models;

namespace Tests_and_Interviews.Web.Controllers
{
    public class TestsController : Controller
    {
        private readonly TestsApiClient _api;

        public TestsController(TestsApiClient api)
        {
            this._api = api;
        }

        public async Task<IActionResult> Index()
        {
            List<string> categories = await this._api.GetCategories();
            TestsViewModel viewModel = new TestsViewModel();

            foreach (string category in categories) {
                List<TestDto> tests = await _api.GetByCategory(category);

                foreach (TestDto test in tests) {
                    viewModel.Tests.Add(new TestCardViewModel
                    {
                        TestId = test.Id,
                        Title = test.Title,
                        Category = test.Category,
                        QuestionTypeLabel = test.QuestionTypeLabel
                    });
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            TestDto? test = await this._api.GetById(id);
            if (test == null) 
            {
                return NotFound();
            }

            return View(test);
        }

        //[Authorize(Roles = Roles.Candidate)]
        public IActionResult Start(int id)
        {
            return RedirectToAction("Take", new { id });
        }

        //[Authorize(Roles = Roles.Recruiter)]
        public IActionResult Create()
        {
            return View();
        }

        //[Authorize(Roles = Roles.Recruiter)]
        [HttpPost]
        public async Task<IActionResult> Create(TestDto dto)
        {
            if (!ModelState.IsValid) 
            {
                return View(dto);
            }

            await this._api.Create(dto);
            return RedirectToAction("Index");
        }

        //[Authorize(Roles = Roles.Recruiter)]
        public async Task<IActionResult> Edit(int id)
        {
            TestDto? test = await this._api.GetById(id);
            if (test == null) {
                return NotFound();
            }

            return View(test);
        }

        //[Authorize(Roles = Roles.Recruiter)]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TestDto dto)
        {
            if (!ModelState.IsValid) {
                return View(dto);
            }

            await this._api.Update(id, dto);
            return RedirectToAction("Index");
        }

        //[Authorize(Roles = Roles.Recruiter)]
        public async Task<IActionResult> Delete(int id)
        {
            TestDto? test = await this._api.GetById(id);
            if (test == null) {
                return NotFound();
            }

            return View(test);
        }

        //[Authorize(Roles = Roles.Recruiter)]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await this._api.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
