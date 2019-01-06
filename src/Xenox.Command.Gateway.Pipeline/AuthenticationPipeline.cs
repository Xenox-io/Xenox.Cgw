using System;
using System.Threading.Tasks;
using Xenox.Auth.Client.AuthorizationTokenClientProvider;
using Xenox.Auth.Models;
using Xenox.Command.Messaging;
using Xenox.Messaging;
using Xenox.Serialization;

namespace Xenox.Command.Gateway.Pipeline {
	public class AuthenticationPipeline : IAuthenticationPipeline {
		private readonly IAuthorizationTokenClientProvider _authorizationTokenClientProvider;
		private readonly ISerializationService _serializationService;

		public AuthenticationPipeline(
			IAuthorizationTokenClientProvider authorizationTokenClientProvider,
			ISerializationService serializationService
		) {
			_authorizationTokenClientProvider = authorizationTokenClientProvider;
			_serializationService = serializationService;
		}

		public async Task<CommandMessage> Send(CommandMessage commandMessage) {
			Header authorizationTokenHeader = commandMessage.HeaderCollection.GetHeader(CommandMessageHeaderNames.AuthorizationToken);
			AuthorizationToken authorizationToken;
			try {
				authorizationToken = await _authorizationTokenClientProvider.ParseAuthorizationToken(new SerializedAuthorizationToken(authorizationTokenHeader.Value));
			} catch {
				throw new Exception("Authorization Token is not valid.");
			}
			commandMessage.HeaderCollection.AddHeader(new Header(CommandMessageHeaderNames.AuthorizationContext, _serializationService.SerializeToString(authorizationToken)));
			return commandMessage;
		}
	}
}
