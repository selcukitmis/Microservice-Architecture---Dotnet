using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;
using Play.Common;
using Play.Inventory.Service.Clients;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        private readonly IRepository<InventoryItem> repository;
        private readonly CatalogClient _catalogClient;

        public ItemsController(IRepository<InventoryItem> repository, CatalogClient catalogClient)
        {
            this.repository = repository;
            _catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await _catalogClient.GetCatalogItemAsync();
            var inventoryItemEntities = await repository.GetAllAsync(item => item.UserId == userId);

            var inventoryItemDtos = inventoryItemEntities.Select(inventoryItemEntity =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItemEntity.CatalogItemId);
                return inventoryItemEntity.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await repository.GetAsync(
                item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem != null)
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await repository.UpdateAsync(inventoryItem);
            }
            else
            {
                inventoryItem = new InventoryItem
                {
                    UserId = grantItemsDto.UserId,
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    Quantity = grantItemsDto.Quantity,
                    AcquireDate = DateTimeOffset.UtcNow
                };

                await repository.CreateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}