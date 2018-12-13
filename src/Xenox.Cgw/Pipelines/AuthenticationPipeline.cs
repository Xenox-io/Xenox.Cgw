using System;
using System.Threading.Tasks;
using Xenox.Auth.Client.AuthorizationTokenClientProvider;
using Xenox.Auth.Models;
using Xenox.Messaging;
using Xenox.Serialization;

namespace Xenox.Cgw.Pipelines {
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
			Header authorizationTokenHeader = commandMessage.Headers.GetHeader(HeaderNames.AuthorizationToken);
			AuthorizationToken authorizationToken;
			try {
				authorizationToken = await _authorizationTokenClientProvider.ParseAuthorizationToken(new SerializedAuthorizationToken(authorizationTokenHeader.Value));
			} catch {
				throw new Exception("Authorization Token is not valid.");
			}
			commandMessage.Headers.AddHeader(new Header(HeaderNames.AuthorizationContext, _serializationService.SerializeToString(authorizationToken)));
			return commandMessage;
		}
	}
}
