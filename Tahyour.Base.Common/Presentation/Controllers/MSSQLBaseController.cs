﻿namespace Tahyour.Base.Common.Presentation;

[Route("api/[controller]")]
[ApiController]
public class MSSQLBaseController<TEntity, TResponse, TId> : ControllerBase
{
    private readonly IMSSQLBaseService<TEntity, TId> _service;

    public MSSQLBaseController(IMSSQLBaseService<TEntity, TId> service)
    {
        _service = service;
    }

    [HttpGet]
    public virtual async Task<ActionResult> GetAllAsync(
        [FromQuery] string search = null,   // Generic search across all properties
        [FromQuery] string filter = null,  // Specific filtering (e.g., "Department=Accounting")
        [FromQuery] int page = 1,           // Page number
        [FromQuery] int pageSize = 100,      // Items per page
        [FromQuery] string select = null)
    {
        var result = new Result<dynamic>();
        result.RequestTime = DateTime.UtcNow;

        var response = await _service.GetAllAsync<TResponse>(search, filter, page, pageSize, select);

        result = response;
        result.ResponseTime = DateTime.UtcNow;
        return Ok(result);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual async Task<ActionResult> GetByIdAsync(TId id)
    {
        var result = new Result<TResponse>();
        result.RequestTime = DateTime.UtcNow;

        var response = await _service.GetByIdAsync<TResponse>(id);

        result = response;
        result.ResponseTime = DateTime.UtcNow;
        return Ok(result);
    }

    protected async Task<Result<TResponse>> CreateAsync<TRequest>([FromBody] TRequest request)
    {
        var result = new Result<TResponse>();
        result.RequestTime = DateTime.UtcNow;

        var response = await _service.CreateAsync<TResponse, TRequest>(request);

        result = response;
        result.ResponseTime = DateTime.UtcNow;
        return result;
    }

    protected async Task<Result<bool>> UpdateAsync<TRequest>(TId id, [FromBody] TRequest request)
    {
        var result = new Result<bool>();
        result.RequestTime = DateTime.UtcNow;

        var response = await _service.UpdateAsync(id, request);

        result = response;
        result.ResponseTime = DateTime.UtcNow;
        return result;
    }


    protected async Task<Result<bool>> RemoveAsync(TId id)
    {
        var result = new Result<bool>();
        result.RequestTime = DateTime.UtcNow;

        var response = await _service.RemoveAsync(id);

        result = response;
        result.ResponseTime = DateTime.UtcNow;
        return result;
    }

    protected async Task<ActionResult> ImportAsync<TRequest>([FromBody] TRequest[] requests)
    {
        var result = new Result<string>();
        result.RequestTime = DateTime.UtcNow;

        var response = await _service.ImportAsync(requests);

        result = response;
        result.ResponseTime = DateTime.UtcNow;
        return Ok(result);
    }
}