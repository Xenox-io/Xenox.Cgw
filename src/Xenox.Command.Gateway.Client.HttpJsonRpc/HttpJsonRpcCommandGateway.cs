using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xenox.Command.Messaging;
using Xenox.Serialization;

namespace Xenox.Command.Gateway.Client.HttpJsonRpc {
	public class HttpJsonRpcCommandGateway : ICommandGateway {
		private readonly ISerializationService _serializationService;
		private readonly JsonRpcCommandGatewayConfiguration _configuration;

		public HttpJsonRpcCommandGateway(
			ISerializationService serializationService,
			JsonRpcCommandGatewayConfiguration configuration
		) {
			_serializationService = serializationService;
			_configuration = configuration;
		}

		public async Task Send(CommandMessage commandMessage) {
			string correlationId = "";
			try {
				correlationId = commandMessage.HeaderCollection.GetHeader(CommandMessageHeaderNames.CorrelationId).Value;
			} catch {
				correlationId = Guid.NewGuid().ToString();
			}
			JsonRpcRequest jsonRpcRequest = new JsonRpcRequest() {
				id = 1,
				jsonrpc = "2.0",
				method = "ExecuteWithCorrelationAndHeaders",
				Params = new JsonRpcGatewayExecuteParams() {
					routingKey = commandMessage.HeaderCollection.GetHeader(CommandMessageHeaderNames.RoutingKey).Value,
					command = new JsonRpcGatewayCommand() {
						name = commandMessage.CommandMessageBody.CommandName,
						data = commandMessage.CommandMessageBody.Command
					},
					authorizationToken = commandMessage.HeaderCollection.GetHeader(CommandMessageHeaderNames.AuthorizationToken).Value,
					correlationId = correlationId,
					headers = commandMessage.HeaderCollection.ToDictionary(h => h.Name, h => h.Value)
				}
			};
			using (HttpClient httpClient = new HttpClient()) {
				HttpResponseMessage httpResponse = await httpClient.PostAsync(
					_configuration.GatewayUrl + "/api/rpc.json",
					new StringContent(_serializationService.SerializeToString(jsonRpcRequest), Encoding.UTF8, "application/json")
				);
				httpResponse.EnsureSuccessStatusCode();
				JsonRpcResponse jsonRpcResponse = _serializationService.DeserializeFromString<JsonRpcResponse>(
					await httpResponse.Content.ReadAsStringAsync()
				);
				if (jsonRpcResponse.error != null) {
					throw new Exception($"JSON RPC Error {jsonRpcResponse.error.code}: {jsonRpcResponse.error.message}");
				}
			}
		}
	}

	public class JsonRpcRequest {
		public int id { get; set; }
		public string jsonrpc { get; set; }
		public string method { get; set; }
		public JsonRpcGatewayExecuteParams Params { get; set; }
	}

	public class JsonRpcGatewayExecuteParams {
		public string routingKey { get; set; }
		public JsonRpcGatewayCommand command { get; set; }
		public string authorizationToken { get; set; }
		public string correlationId { get; set; }
		public Dictionary<string, string> headers { get; set; }
	}

	public class JsonRpcGatewayCommand {
		public string name { get; set; }
		public object data { get; set; }
	}

	public class JsonRpcResponse {
		public string id { get; set; }
		public string jsonrpc { get; set; }
		public JsonRpcGatewayExecuteResult result { get; set; }
		public JsonRpcError error { get; set; }
	}

	public class JsonRpcError {
		public string code { get; set; }
		public string message { get; set; }
		public object data { get; set; }
	}

	public class JsonRpcGatewayExecuteResult {
		public string correlationId { get; set; }
	}

	public class JsonRpcCommandGatewayConfiguration {
		public string GatewayUrl { get; set; }
	}
}
