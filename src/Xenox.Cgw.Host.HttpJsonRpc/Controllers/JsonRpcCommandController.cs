using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdjCase.JsonRpc.Core;
using EdjCase.JsonRpc.Router;
using EdjCase.JsonRpc.Router.Abstractions;
using EdjCase.JsonRpc.Router.Defaults;
using Newtonsoft.Json;
using Xenox.Cgw.Host.HttpJsonRpc.Dtos;
using Xenox.Command.Serialization;

namespace Xenox.Cgw.Host.HttpJsonRpc.Controllers {
	[RpcRoute("api/rpc.json")]
	public class JsonRpcCommandController : RpcController {
		private readonly ICommandGateway _commandGateway;

		public JsonRpcCommandController(ICommandGateway commandGateway) {
			_commandGateway = commandGateway;
		}

		public async Task<IRpcMethodResult> ExecuteWithCorrelationAndHeaders(
			string routingKey,
			CommandDto command,
			string authorizationToken,
			string correlationId,
			Dictionary<string, string> headers
		) {
			if (string.IsNullOrWhiteSpace(routingKey)) {
				return new RpcMethodErrorResult((int)RpcErrorCode.InvalidParams, "Routing Key must be provided.");
			}
			if (command == null) {
				return new RpcMethodErrorResult((int)RpcErrorCode.InvalidParams, "Command must be provided.");
			}
			if (string.IsNullOrWhiteSpace(command.Name)) {
				return new RpcMethodErrorResult((int)RpcErrorCode.InvalidParams, "Command Name must be provided.");
			}
			if (command.Data == null) {
				return new RpcMethodErrorResult((int)RpcErrorCode.InvalidParams, "Command Data must be provided.");
			}
			if (string.IsNullOrWhiteSpace(authorizationToken)) {
				return new RpcMethodErrorResult((int)RpcErrorCode.InvalidParams, "Authorization Token must be provided.");
			}
			if (string.IsNullOrWhiteSpace(correlationId)) {
				return new RpcMethodErrorResult((int)RpcErrorCode.InvalidParams, "Correlation ID must be provided.");
			}
			if (headers == null) {
				return new RpcMethodErrorResult((int)RpcErrorCode.InvalidParams, "Headers must be provided.");
			}
			headers[HeaderNames.RoutingKey] = routingKey;
			headers[HeaderNames.AuthorizationToken] = authorizationToken;
			headers[HeaderNames.CorrelationId] = correlationId;
			CommandMessage commandMessage = new CommandMessage(
				new SerializableCommand() {
					Name = command.Name,
					Data = command.Data.ToString(Formatting.None)
				},
				headers
			);
			try {
				await _commandGateway.Send(commandMessage);
			} catch (Exception exception) {
				return new RpcMethodErrorResult((int)RpcErrorCode.InternalError, exception.Message);
			}
			return new RpcMethodSuccessResult(new ExecuteResponseDto() {
				CorrelationId = correlationId
			});
		}

		public Task<IRpcMethodResult> ExecuteWithCorrelation(
			string routingKey,
			CommandDto command,
			string authorizationToken,
			string correlationId
		) {
			return ExecuteWithCorrelationAndHeaders(
				routingKey,
				command,
				authorizationToken,
				Guid.NewGuid().ToString(),
				new Dictionary<string, string>()
			);
		}

		public Task<IRpcMethodResult> ExecuteWithHeaders(
			string routingKey,
			CommandDto command,
			string authorizationToken,
			Dictionary<string, string> headers
		) {
			return ExecuteWithCorrelationAndHeaders(
				routingKey,
				command,
				authorizationToken,
				Guid.NewGuid().ToString(),
				headers
			);
		}

		public Task<IRpcMethodResult> Execute(
			string routingKey,
			CommandDto command,
			string authorizationToken
		) {
			return ExecuteWithCorrelationAndHeaders(
				routingKey,
				command,
				authorizationToken,
				Guid.NewGuid().ToString(),
				new Dictionary<string, string>()
			);
		}
	}
}
