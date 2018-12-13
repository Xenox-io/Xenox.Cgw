using System;
using System.Linq;
using System.Threading.Tasks;
using Xenox.Auth.Models;
using Xenox.Messaging;
using Xenox.Serialization;

namespace Xenox.Cgw.Pipelines {
	public class AuthorizationPipeline : IAuthorizationPipeline {
		private readonly ISerializationService _serializationService;

		public AuthorizationPipeline(ISerializationService serializationService) {
			_serializationService = serializationService;
		}

		public Task<CommandMessage> Send(CommandMessage commandMessage) {
			Header authorizationContextHeader = commandMessage.Headers.GetHeader(HeaderNames.AuthorizationContext);
			Header routingKeyHeader = commandMessage.Headers.GetHeader(HeaderNames.RoutingKey);
			AuthorizationToken authorizationToken = _serializationService.DeserializeFromString<AuthorizationToken>(authorizationContextHeader.Value);
			bool isAuthorized = authorizationToken.Permissions.Any(p =>
				p.RoutingKey == routingKeyHeader.Value
				&& p.CommandName == commandMessage.Command.Name
			);
			commandMessage.Headers.AddHeader(new Header(HeaderNames.IsAuthorized, isAuthorized.ToString()));
			if (!isAuthorized) {
				throw new Exception($"User {authorizationToken.User.Username} is not authorized to execute {commandMessage.Command.Name} on {routingKeyHeader.Value}.");
			}
			return Task.FromResult(commandMessage);
		}
	}
}
