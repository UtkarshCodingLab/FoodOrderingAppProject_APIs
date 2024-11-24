using Microsoft.AspNetCore.Mvc;
using Restaurant_API.Models;
using Restaurant_API.Models.Dto;
using Restaurant_API.Repository.IRepository;
using System.Net;
using System.Threading.Tasks;

namespace Restaurant_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemRepository _menuItemRepo;
        private readonly ApiResponse _response;

        public MenuItemController(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepo = menuItemRepository;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            _response.Result = await _menuItemRepo.GetAllAsync();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{id:int}", Name = "GetMenuItem")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var menuItem = await _menuItemRepo.GetAsync(u => u.Id == id);

            if (menuItem == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            _response.Result = menuItem;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        public async Task<ActionResult<MenuItem>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDTO)
        {
            if (menuItemCreateDTO.File == null)
            {
                ModelState.AddModelError("CustomError", "Please upload an image.");
                return BadRequest(ModelState);
            }

            var imageName = await _menuItemRepo.SaveImageAsync(menuItemCreateDTO.File);

            if (imageName == null)
            {
                ModelState.AddModelError("CustomError", "Invalid image file or size exceeds 1 MB.");
                return BadRequest(ModelState);
            }

            var menuItem = new MenuItem
            {
                Name = menuItemCreateDTO.Name,
                Description = menuItemCreateDTO.Description,
                SpecialTag = menuItemCreateDTO.SpecialTag,
                Category = menuItemCreateDTO.Category,
                Price = menuItemCreateDTO.Price,
                Image = imageName
            };

            await _menuItemRepo.CreateAsync(menuItem);
            return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, menuItem);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            if (id != menuItemUpdateDTO.Id || menuItemUpdateDTO == null)
                return BadRequest();

            var menuItemFromDB = await _menuItemRepo.GetAsync(u => u.Id == id);
            if (menuItemFromDB == null)
                return NotFound();

            menuItemFromDB.Name = menuItemUpdateDTO.Name;
            menuItemFromDB.Description = menuItemUpdateDTO.Description;
            menuItemFromDB.SpecialTag = menuItemUpdateDTO.SpecialTag;
            menuItemFromDB.Category = menuItemUpdateDTO.Category;
            menuItemFromDB.Price = menuItemUpdateDTO.Price;

            if (menuItemUpdateDTO.File != null)
            {
                _menuItemRepo.DeleteImage(menuItemFromDB.Image);
                var newImageName = await _menuItemRepo.SaveImageAsync(menuItemUpdateDTO.File);

                if (newImageName == null)
                {
                    ModelState.AddModelError("CustomError", "Invalid image file or size exceeds 1 MB.");
                    return BadRequest(ModelState);
                }

                menuItemFromDB.Image = newImageName;
            }

            await _menuItemRepo.UpdateAsync(menuItemFromDB);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            if (id == 0)
                return BadRequest();

            var menuItem = await _menuItemRepo.GetAsync(u => u.Id == id);
            if (menuItem == null)
                return NotFound();

            _menuItemRepo.DeleteImage(menuItem.Image);
            await _menuItemRepo.RemoveAsync(menuItem);

            return NoContent();
        }
    }
}
