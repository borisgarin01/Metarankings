using API.Models.RequestsModels.Games.Publishers;
using AutoMapper;
using Data.Repositories.Interfaces;
using Domain.Games;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Games;

[ApiController]
[Route("api/[controller]")]
public sealed class PublishersController : ControllerBase
{
    private readonly IMapper _mapper;

    private readonly IRepository<Publisher> _publishersRepository;

    private readonly IValidator<AddPublisherModel> _addPublisherValidator;
    private readonly IValidator<UpdatePublisherModel> _updatePublisherValidator;

    public PublishersController(IMapper mapper, IRepository<Publisher> publishersRepository, IValidator<AddPublisherModel> addPublisherValidator, IValidator<UpdatePublisherModel> pdatePublisherValidator)
    {
        _mapper = mapper;
        _publishersRepository = publishersRepository;
        _addPublisherValidator = addPublisherValidator;
        _updatePublisherValidator = pdatePublisherValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Publisher>>> GetAllAsync()
    {
        var publishers = await _publishersRepository.GetAllAsync();

        return Ok(publishers);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<Publisher>> AddAsync(AddPublisherModel addPublisherModel)
    {
        var validationResult = _addPublisherValidator.Validate(addPublisherModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var publisher = _mapper.Map<Publisher>(addPublisherModel);

        var insertedPublisherId = await _publishersRepository.AddAsync(publisher);

        publisher.Id = insertedPublisherId;

        return Created($"api/publishers/{publisher.Id}", publisher);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Publisher>> GetAsync(long id)
    {
        var publisher = await _publishersRepository.GetAsync(id);
        if (publisher is null)
            return NotFound();
        else
            return Ok(publisher);
    }

    [HttpDelete("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
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

    [HttpPut("{id:long}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<Publisher>> UpdateAsync(long id, UpdatePublisherModel updatePublisherModel)
    {
        var validationResult = _updatePublisherValidator.Validate(updatePublisherModel);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }

        var publisherToUpdate = await _publishersRepository.GetAsync(id);
        if (publisherToUpdate is null)
            return NotFound();

        var publisherToGetAfterUpdate = _mapper.Map<Publisher>(updatePublisherModel);

        var updatePublisher = await _publishersRepository.UpdateAsync(publisherToGetAfterUpdate, id);

        return Ok(updatePublisher);
    }
}
