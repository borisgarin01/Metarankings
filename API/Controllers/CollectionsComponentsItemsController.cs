using Data.Repositories.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsComponentsItemsController : ControllerBase
{
    private readonly ICollectionsComponentsItemsRepository collectionsComponentsItemsRepository;
    private readonly IConfiguration configuration;

    public CollectionsComponentsItemsController(ICollectionsComponentsItemsRepository collectionsComponentsItemsRepository, IConfiguration configuration)
    {
        this.collectionsComponentsItemsRepository = collectionsComponentsItemsRepository;
        this.configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CollectionsComponentItem>>> GetAllAsync()
    {
        var collectionsComponentsItems = await collectionsComponentsItemsRepository.GetAllAsync();
        return Ok(collectionsComponentsItems);
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(CollectionsComponentItem collectionsComponentItem)
    {
        await collectionsComponentsItemsRepository.AddAsync(collectionsComponentItem);
        return Ok();
    }
}
