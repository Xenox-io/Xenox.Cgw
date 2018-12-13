using System.Threading.Tasks;

namespace Xenox.Cgw.Services {
	public interface IMessageRoutedQueueService {
		Task Queue(string routingKey, object message);
	}
}
