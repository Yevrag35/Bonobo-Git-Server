using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yev.Bonobo.Database;
using Yev.Bonobo.Git;
using Yev.Bonobo.Models.Api;

namespace Yev.Bonobo.Controllers.Api;

[ApiController]
[Route("api/git")]
public sealed class GitApiController : ControllerBase
{
    private readonly IRepoService _repoSvc;
    private readonly IExpressionStateFactory _stateFac;

    public GitApiController(IExpressionStateFactory stateFac, IRepoService repoSvc)
    {
        _repoSvc = repoSvc;
        _stateFac = stateFac;
    }

    public async IActionResult CreateRepo(
        [FromBody] CreateRepoModel request)
    {
        await using var state = _stateFac.Create(request, this.HttpContext);
        _repoSvc.

    }
}