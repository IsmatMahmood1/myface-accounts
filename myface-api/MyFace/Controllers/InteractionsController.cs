using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;

namespace MyFace.Controllers
{
    public class InteractionsController
    {
        [ApiController]
        [Route("/interactions")]
        public class UsersController : ControllerBase
        {
            private readonly IInteractionsRepo _interactions;
            private readonly IUsersRepo _users;


            public UsersController(IInteractionsRepo interactions, IUsersRepo users)
            {
                _interactions = interactions;
                _users = users;
            }
        
            [HttpGet("")]
            public ActionResult<ListResponse<InteractionResponse>> Search([FromQuery] SearchRequest search)
            {
                var interactions = _interactions.Search(search);
                var interactionCount = _interactions.Count(search);
                return InteractionListResponse.Create(search, interactions, interactionCount);
            }

            [HttpGet("{id}")]
            public ActionResult<InteractionResponse> GetById([FromRoute] int id)
            {
                 var authHeader = HttpContext.Request.Headers["Authorization"];

                if (!_users.IsAuthorized(authHeader))
                {
                    return Unauthorized();
                }
                var interaction = _interactions.GetById(id);
                return new InteractionResponse(interaction);
            }

            [HttpPost("create")]
            public IActionResult Create([FromBody] CreateInteractionRequest newUser)
            {
                var authHeader = HttpContext.Request.Headers["Authorization"];

                if (!_users.IsAuthorized(authHeader))
                {
                    return Unauthorized();
                }
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            
                var interaction = _interactions.Create(newUser);

                var url = Url.Action("GetById", new { id = interaction.Id });
                var responseViewModel = new InteractionResponse(interaction);
                return Created(url, responseViewModel);
            }

            [HttpDelete("{id}")]
            public IActionResult Delete([FromRoute] int id)
            {
                 var authHeader = HttpContext.Request.Headers["Authorization"];

                if (!_users.IsAuthorized(authHeader))
                {
                    return Unauthorized();
                }
                _interactions.Delete(id);
                return Ok();
            }
        }
    }
}