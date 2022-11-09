using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.Agent;
using HouseRentingSystem.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseRentingSystem.Controllers
{
    [Authorize]
    public class AgentController : Controller
    {
        private readonly IAgentService service;

        public AgentController(IAgentService _service)
        {
            service = _service;
        }

        [HttpGet]
        public IActionResult Become()
        {
            if (this.service.ExistById(this.User.GetUserId()))
            {
                return BadRequest();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Become(BecomeAgentModel model)
        {
            var userId = this.User.GetUserId();

            if (service.ExistById(userId))
            {
                return BadRequest();
            }

            if (service.UserWithPhoneNumberExist(model.PhoneNumber))
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), 
                    "Phone number already exist. Enter another one.");
            }

            if (service.UserHasRents(userId))
            {
                ModelState.AddModelError("Error",
                    "You should have no rents to become an agent.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            service.Create(userId, model.PhoneNumber);

            return RedirectToAction(nameof(HouseController.All), "House");
        }

    }
}
