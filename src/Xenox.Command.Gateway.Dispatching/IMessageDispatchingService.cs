using System.Threading.Tasks;
using Xenox.Messaging;

namespace Xenox.Command.Gateway.Dispatching {
	public interface IMessageDispatchingService {
		Task Dispatch(Message message);
	}
}
