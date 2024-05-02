using MediatR;
using Microsoft.AspNetCore.Mvc;
using Challenge02.Domain.Commands;
using Challenge02.Domain.Queries;

namespace Challenge02.Api.Controllers
{
    [ApiController]
    [Route("dev")]
    public class DevController : ControllerBase
    {
        public DevController()
        {
        }

        /// <summary>
        /// Normaliza os e-mails dos desenvolvedores para o domínio @challenge.
        /// </summary>
        /// <returns> Retorna uma mensagem de sucesso. </returns>
        [HttpPost("normalize-emails")]
        public async Task<IActionResult> NormalizeEmails([FromServices] IMediator mediator)
        {
            await mediator.Send(new NormalizeDevEmailDomainsCommand());
            return Ok("E-mails dos desenvolvedores normalizados com sucesso.");
        }

        /// <summary>
        ///  Atualiza o cadastro de um desenvolvedor.
        /// </summary>
        /// <param name="command"></param>
        /// <returns> Retorna uma mensagem de sucesso. </returns>
        [HttpPut("update-dev")]
        public async Task<IActionResult> UpdateDevAsync([FromServices] IMediator mediator, [FromBody] UpdateDevCommand command)
        {
            await mediator.Send(command);
            return Ok();
        }

        /// <summary>
        /// Adiciona um novo desenvolvedor.
        /// </summary>
        /// <param name="command"></param>
        /// <returns> Retorna uma mensagem de sucesso. </returns>
        [HttpPost("add-dev")]
        public async Task<IActionResult> AddDevAsync([FromServices] IMediator mediator, [FromBody] AddDevCommand command)
        {
            await mediator.Send(command);
            return Ok();
        }

        /// <summary>
        /// Recupera todos os desenvolvedores cadastrados.
        /// </summary>
        /// <returns> Retorna uma lista de desenvolvedores. </returns>
        [HttpGet("get-all-devs")]
        public async Task<IActionResult> GetAllDevsAsync([FromServices] IMediator mediator)
        {
            var devs = await mediator.Send(new GetAllDevsQuery());
            return Ok(devs);
        }

        /// <summary>
        /// Recupera um desenvolvedor pelo seu Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Retorna um desenvolvedor. </returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDevByIdAsync([FromServices] IMediator mediator, [FromRoute] int id)
        {
            var command = new GetDevByIdQuery() { Id = id };
            var dev = await mediator.Send(command);

            if (dev == null)
                return NotFound();

            return Ok(dev);
        }
    }
}
