using System.Threading.Tasks;

namespace Xenox.Command.Gateway.Dispatching {
	public interface IMessageRoutedQueueService {
		Task Queue(string routingKey, object message);
	}
}
