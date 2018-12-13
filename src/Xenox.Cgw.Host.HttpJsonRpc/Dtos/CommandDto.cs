using Newtonsoft.Json.Linq;

namespace Xenox.Cgw.Host.HttpJsonRpc.Dtos {
	public class CommandDto {
		public string Name { get; set; }
		public JObject Data { get; set; }
	}
}
