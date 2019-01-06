using Newtonsoft.Json.Linq;

namespace Xenox.Command.Gateway.Host.HttpJsonRpc.Dtos {
	public class CommandDto {
		public string Name { get; set; }
		public JObject Data { get; set; }
	}
}
