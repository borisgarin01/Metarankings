using API.Models.RequestsModels.Publishers;
using AutoMapper;
using Data.Repositories.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PublishersController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IRepository<Publisher> _publishersRepository;

    public PublishersController(IMapper mapper, IRepository<Publisher> publishersRepository)
    {
        _mapper = mapper;
        _publishersRepository = publishersRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Publisher>>> GetAllAsync()
    {
        var publishers = await _publishersRepository.GetAllAsync();

        return Ok(publishers);
    }

    [HttpPost]
    public async Task<ActionResult<Publisher>> AddAsync(AddPublisherModel addPublisherModel)
    {
        var publisher = _mapper.Map<Publisher>(addPublisherModel);

        var insertedPublisherId = await _publishersRepository.AddAsync(publisher);

        publisher.Id = insertedPublisherId;

        return Created($"api/publishers/{publisher.Id}", publisher);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Publisher>> GetAsync(long id)
    {
        var publisher = await _publishersRepository.GetAsync(id);
        if (publisher is null)
            return NotFound();
        else
            return Ok(publisher);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var publisher = await _publishersRepository.GetAsync(id);
        if (publisher is null)
            return NotFound();
        else
        {
            try
            {
                await _publishersRepository.RemoveAsync(publisher.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Publisher>> UpdateAsync(long id, UpdatePublisherModel updatePublisherModel)
    {
        var publisherToUpdate = await _publishersRepository.GetAsync(id);
        if (publisherToUpdate is null)
            return NotFound();

        var publisherToGetAfterUpdate = _mapper.Map<Publisher>(updatePublisherModel);

        var updatePublisher = await _publishersRepository.UpdateAsync(publisherToGetAfterUpdate, id);

        return Ok(updatePublisher);
    }
}
