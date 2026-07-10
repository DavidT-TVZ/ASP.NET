using DnD_Character_Sheet_Creator.Data;
using DnD_Character_Sheet_Creator.Models;
using DnD_Character_Sheet_Creator.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnD_Character_Sheet_Creator.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    [Route("[controller]")]
    public class EquipmentController : Controller
    {
        private readonly DnDDbContext _dbContext;
        private readonly ILogger<EquipmentController> _logger;

        public EquipmentController(DnDDbContext dbContext, ILogger<EquipmentController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index(string? query)
        {
            var normalizedQuery = query?.Trim() ?? string.Empty;
            var model = new EquipmentManagementViewModel
            {
                Query = normalizedQuery,
                EquipmentItems = LoadEquipment(normalizedQuery)
            };

            ViewData["Title"] = "Equipment";
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipmentManagementViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.EquipmentItems = LoadEquipment(vm.Query);
                ViewData["Title"] = "Equipment";
                return View("Index", vm);
            }

            var equipment = new Equipment
            {
                Name = vm.Name.Trim(),
                Type = string.IsNullOrWhiteSpace(vm.Type) ? null : vm.Type.Trim(),
                Cost = vm.Cost,
                Weight = vm.Weight
            };

            _dbContext.Equipment.Add(equipment);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Created custom equipment {EquipmentId} {EquipmentName}", equipment.EquipmentId, equipment.Name);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var equipment = await _dbContext.Equipment.FirstOrDefaultAsync(item => item.EquipmentId == id && item.DeletedAt == null);
            if (equipment == null)
            {
                return NotFound();
            }

            if (equipment.GetType() != typeof(Equipment))
            {
                return Forbid();
            }

            equipment.DeletedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deleted custom equipment {EquipmentId} {EquipmentName}", equipment.EquipmentId, equipment.Name);

            return RedirectToAction(nameof(Index));
        }

        private List<Equipment> LoadEquipment(string? query)
        {
            var equipment = _dbContext.Equipment
                .Where(item => item.DeletedAt == null)
                .AsNoTracking()
                .ToList();

            if (!string.IsNullOrWhiteSpace(query))
            {
                equipment = equipment
                    .Where(item =>
                        item.EquipmentId.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        item.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        (item.Type ?? string.Empty).Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        item.Cost.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        item.Weight.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return equipment
                .OrderBy(item => item.Name)
                .ThenBy(item => item.EquipmentId)
                .ToList();
        }
    }
}