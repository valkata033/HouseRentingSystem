using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.House;
using HouseRentingSystem.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseRentingSystem.Controllers
{
    [Authorize]
    public class HouseController : Controller
    {
        private readonly IHouseService houses;
        private readonly IAgentService agents;

        public HouseController(IHouseService _houses, IAgentService _agents)
        {
            houses = _houses;
            agents = _agents;
        }

        [AllowAnonymous]
        public async Task<IActionResult> All()
        {
            var model = new HousesQueryModel();

            return View(model);
        }

        public async Task<IActionResult> Mine()
        {
            var model = new HousesQueryModel();

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var model = new HouseDetailsModel();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (await agents.ExistById(this.User.GetUserId()) == false)
            {
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }

            return View(new HouseFormModel
            {
                Categories = await houses.AllCategories()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(HouseFormModel model)
        {
            if (await agents.ExistById(this.User.GetUserId()) == false)
            {
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }

            if (!this.houses.CategoryExist(model.CategoryId))
            {
                this.ModelState.AddModelError(nameof(model.CategoryId),
                    "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await houses.AllCategories();

                return View(model);
            }

            var agentId = agents.GetAgentId(this.User.GetUserId());

            var newHouseId = houses.Create(model.Title, model.Address, model.Description,
                model.ImageUrl, model.PricePerMonth, model.CategoryId, agentId);

            return RedirectToAction(nameof(Details), new { id = newHouseId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = new HouseFormModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HouseFormModel model)
        {
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public IActionResult Rent(int id)
        {
            return RedirectToAction(nameof(Mine));
        }

        [HttpPost]
        public IActionResult Leave(int id)
        {
            return RedirectToAction(nameof(Mine));
        }
    }
}
