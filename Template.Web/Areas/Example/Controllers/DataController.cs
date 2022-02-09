namespace Template.Web.Areas.Example.Controllers;

using AutoMapper;

using Microsoft.AspNetCore.Mvc;

using Template.Services;
using Template.Web.Areas.Example.Models;

public class DataController : BaseExampleController
{
    private const int PageSize = 15;

    private IMapper Mapper { get; }

    private DataService DataService { get; }

    public DataController(
        IMapper mapper,
        DataService dataService)
    {
        Mapper = mapper;
        DataService = dataService;
    }

    //--------------------------------------------------------------------------------
    // List
    //--------------------------------------------------------------------------------

    [HttpGet]
    public async ValueTask<IActionResult> List([FromQuery] DataListForm form)
    {
        if (ModelState.IsValid)
        {
            var parameter = Mapper.Map<DataSearchParameter>(form).SetSize(PageSize);
            ViewBag.Paged = await DataService.QueryAccountPagedAsync(parameter);
        }

        return View(form);
    }

    //--------------------------------------------------------------------------------
    // Import
    //--------------------------------------------------------------------------------

    // TODO

    //--------------------------------------------------------------------------------
    // Export
    //--------------------------------------------------------------------------------

    // TODO
}
