﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ScreenMusic.Domain.ApiManagement;
using ScreenMusic.Domain.Interfaces.Service;
using ScreenMusic.Domain.Services;

namespace ScreenMusic.Api.Controllers;

[Authorize]
[ApiController]
public class BaseController<TIService, TInputCreate, TInputUpdate, TOutput, TInputIdentifier> : Controller
    where TIService : IBaseService<TInputCreate, TInputUpdate, TOutput, TInputIdentifier>
{
    protected readonly IApiDataService? _apiDataService;
    public Guid _guidApiDataRequest;
    public TIService? _service;
    public List<Notification> ListNotification = [];

    public BaseController(IApiDataService apiDataService, TIService service)
    {
        _apiDataService = apiDataService;
        _service = service;
    }

    public BaseController(IApiDataService apiDataService)
    {
        _apiDataService = apiDataService;
    }

    #region Read
    [HttpGet]
    public virtual async Task<ActionResult<BaseResponse<TOutput>>> GetAll()
    {
        try
        {
            return await ResponseAsync(_service!.GetAll());
        }
        catch (BaseResponseException ex)
        {
            return await BaseResponseExceptionAsync(ex);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<BaseResponse<TOutput>>> Get(long id)
    {
        try
        {
            return await ResponseAsync(_service!.Get(id));
        }
        catch (BaseResponseException ex)
        {
            return await BaseResponseExceptionAsync(ex);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }

    [HttpPost("GetByIdentifier")]
    public virtual async Task<ActionResult<BaseResponse<TOutput>>> GetByIdentifier(TInputIdentifier inputIdentifier)
    {
        try
        {
            return await ResponseAsync(_service!.GetByIdentifier(inputIdentifier));
        }
        catch (BaseResponseException ex)
        {
            return await BaseResponseExceptionAsync(ex);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
    #endregion

    #region Create
    [HttpPost]
    public virtual async Task<ActionResult<BaseResponse<string>>> Create(TInputCreate inputCreate)
    {
        try
        {
            return await ResponseAsync(_service?.Create(inputCreate), 201);
        }
        catch (BaseResponseException ex)
        {
            return await BaseResponseExceptionAsync(ex);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
    #endregion

    #region Update
    [HttpPut("{id}")]
    public virtual async Task<ActionResult<BaseResponse<string>>> Update(long id, TInputUpdate inputUpdate)
    {
        try
        {
            return await ResponseAsync(_service?.Update(id, inputUpdate));
        }
        catch (BaseResponseException ex)
        {
            return await BaseResponseExceptionAsync(ex);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
    #endregion

    #region Delete
    [HttpDelete]
    public virtual async Task<ActionResult<BaseResponse<bool>>> Delete(long id)
    {
        try
        {
            return await ResponseAsync(_service?.Delete(id));
        }
        catch (BaseResponseException ex)
        {
            return await BaseResponseExceptionAsync(ex);
        }
        catch (Exception ex)
        {
            return await ResponseExceptionAsync(ex);
        }
    }
    #endregion

    [NonAction]
    public async Task<ActionResult> ResponseAsync<ResponseType>(ResponseType result, int statusCode = 0)
    {
        if (_service != null)
            ListNotification.AddRange(_service?.ListNotification!);

        List<Notification> listNegativeNotification = (from i in ListNotification ?? [] where i.MessageType == EnumMessageType.Negative select i).ToList();

        if (listNegativeNotification.Count == 0)
        {
            try
            {
                return StatusCode(statusCode == 0 ? 200 : statusCode, new BaseResponseApi<ResponseType, string> { Value = new BaseResponseApiContent<ResponseType, string>() { Result = result } });
            }
            catch (BaseResponseException ex)
            {
                return await BaseResponseExceptionAsync(ex);
            }
            catch (Exception ex)
            {
                return BadRequest($"Houve um problema interno com o servidor. Entre em contato com o Administrador do sistema caso o problema persista. Erro interno: {ex.Message}");
            }
        }
        else
            return BadRequest(listNegativeNotification);
    }

    [NonAction]
    public async Task<ActionResult> BaseResponseExceptionAsync(BaseResponseException ex)
    {
        return await Task.FromResult(BadRequest(new { Value = new { Result = (object?)default, ListNotification = ex.Incidents } }));
    }

    [NonAction]
    public async Task<ActionResult> ResponseExceptionAsync(Exception ex)
    {
        return await Task.FromResult(BadRequest(new { Value = ex.Message }));
    }

    [NonAction]
    public void SetData()
    {
        Guid guidApiDataRequest = ApiData.CreateApiDataRequest();
        SetGuid(guidApiDataRequest);
    }

    [NonAction]
    public void SetGuid(Guid guidApiDataRequest)
    {
        _guidApiDataRequest = guidApiDataRequest;
        GenericModule.SetGuidApiDataRequest(this, guidApiDataRequest);
    }

    [NonAction]
    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            SetData();
        }
        catch (Exception ex)
        {
            context.Result = await ResponseExceptionAsync(ex);
        }
    }
}

#region TInputUpdate, TEntity, TInputIdentifier
public class BaseController_1<TIService>(IApiDataService apiDataService, TIService service) : BaseController<TIService, object, object, object, object>(apiDataService, service)
    where TIService : IBaseService_0
{ }
#endregion