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
        public async Task<IActionResult> All([FromQuery] AllHousesQueryModel query)
        {
            var queryResult = await houses.All(
                query.Category,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllHousesQueryModel.HousesPerPage);

            query.TotalHousesCount = queryResult.TotalHouseCount;
            query.Houses = queryResult.Houses;

            var houseCategories = await houses.AllCategoriesNames();
            query.Categories = houseCategories;

            return View(query);
        }

        public async Task<IActionResult> Mine()
        {
            IEnumerable<HouseServiceModel> myHouses = null;

            string userId = User.GetUserId();

            if (await agents.ExistById(userId))
            {
                var currentAgentId = await agents.GetAgentId(userId);

                myHouses = await houses.AllHousesByAgentId(currentAgentId);
            }
            else
            {
                myHouses = await houses.AllHousesByUserId(userId);
            }

            return View(myHouses);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            if (!(await houses.Exists(id)))
            {
                return BadRequest();
            }

            var houseModel = await houses.HouseDetailsById(id);

            return View(houseModel);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if ((await agents.ExistById(this.User.GetUserId())) == false)
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
            if ((await agents.ExistById(this.User.GetUserId())) == false)
            {
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }

            if ((await houses.CategoryExist(model.CategoryId)) == false)
            {
                this.ModelState.AddModelError(nameof(model.CategoryId),
                    "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await houses.AllCategories();

                return View(model);
            }

            int agentId = await agents.GetAgentId(this.User.GetUserId());

            int Id = await houses.Create(model, agentId);

            return RedirectToAction(nameof(Details), new { Id });
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
