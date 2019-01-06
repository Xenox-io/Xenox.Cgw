using System;
using System.Threading.Tasks;
using Xenox.Command.Messaging;

namespace Xenox.Command.Gateway.Client.HttpJsonRpc {
	public class HttpJsonRpcCommandGateway : ICommandGateway {
		private readonly HttpJsonRpcCommandGatewayConfiguration _configuration;

		public HttpJsonRpcCommandGateway(
			HttpJsonRpcCommandGatewayConfiguration configuration
		) {
			_configuration = configuration;
		}

		public Task Send(CommandMessage commandMessage) {
			//TODO Actually call HTTP JSON RPC call to the URL with the command data.
			throw new NotImplementedException();
		}
	}

	public class HttpJsonRpcCommandGatewayConfiguration {
		public string GatewayUrl { get; set; }
	}
}
