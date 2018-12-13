using System.Threading.Tasks;
using Xenox.Messaging;

namespace Xenox.Cgw.Services {
	public interface IMessageDispatchingService {
		Task Dispatch(Message message);
	}
}
