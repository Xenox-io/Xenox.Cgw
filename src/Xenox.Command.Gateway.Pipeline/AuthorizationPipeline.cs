using System;
using System.Linq;
using System.Threading.Tasks;
using Xenox.Auth.Models;
using Xenox.Command.Messaging;
using Xenox.Messaging;
using Xenox.Serialization;

namespace Xenox.Command.Gateway.Pipeline {
	public class AuthorizationPipeline : IAuthorizationPipeline {
		private readonly ISerializationService _serializationService;

		public AuthorizationPipeline(ISerializationService serializationService) {
			_serializationService = serializationService;
		}

		public Task<CommandMessage> Send(CommandMessage commandMessage) {
			Header authorizationContextHeader = commandMessage.HeaderCollection.GetHeader(CommandMessageHeaderNames.AuthorizationContext);
			Header routingKeyHeader = commandMessage.HeaderCollection.GetHeader(CommandMessageHeaderNames.RoutingKey);
			AuthorizationToken authorizationToken = _serializationService.DeserializeFromString<AuthorizationToken>(authorizationContextHeader.Value);
			bool isAuthorized = authorizationToken.Permissions.Any(p =>
				p.RoutingKey == routingKeyHeader.Value
				&& p.CommandName == commandMessage.CommandMessageBody.CommandName
			);
			commandMessage.HeaderCollection.AddHeader(new Header(CommandMessageHeaderNames.IsAuthorized, isAuthorized.ToString()));
			if (!isAuthorized) {
				throw new Exception($"User {authorizationToken.User.Username} is not authorized to execute {commandMessage.CommandMessageBody.CommandName} on {routingKeyHeader.Value}.");
			}
			return Task.FromResult(commandMessage);
		}
	}
}
