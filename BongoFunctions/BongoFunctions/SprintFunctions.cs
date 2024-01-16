using AutoMapper;
using BongoApplication.Handlers.GetSprint;
using BongoApplication.Handlers.GetSprints;
using BongoApplication.Handlers.SprintTaskUpdateAction;
using BongoDomain.Api;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace BongoFunctions
{
    public class SprintFunctions
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SprintFunctions(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [Function("SubmitActions")]
        public async Task<IActionResult> SubmitActions([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, CancellationToken cancellationToken)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
            var data = JsonConvert.DeserializeObject<SprintTaskUpdateActionRequest>(requestBody);
            var cmd = _mapper.Map<SprintTaskUpdateActionCommand>(data);
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        [Function("Sprints")]
        public async Task<IActionResult> GetSprints([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
            var data = JsonConvert.DeserializeObject<GetSprintsRequest>(requestBody);
            var cmd = _mapper.Map<GetSprintsCommand>(data ?? new GetSprintsRequest());
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        /// <summary>
        /// e.g. http://localhost:7020/api/sprint/d6f0aa6f-983e-4e59-8ab2-53d64d48bb97
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Function("GetSprint")]
        public async Task<IActionResult> GetSprint(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "sprint/{id}")] HttpRequestData req,
            Guid id,
            CancellationToken cancellationToken)
        {
            var cmd = new GetSprintCommand
            {
                SprintId = id
            };
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }

        [Function("CreateSprint")]
        public async Task<IActionResult> PostSprint(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sprint")] HttpRequestData req,
            CancellationToken cancellationToken)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
            var data = JsonConvert.DeserializeObject<CreateSprintRequest>(requestBody);
            var cmd = _mapper.Map<GetSprintsCommand>(data ?? new CreateSprintRequest());
            var result = await _mediator.Send(cmd, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}