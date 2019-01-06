using System.Threading.Tasks;
using Xenox.Command.Messaging;

namespace Xenox.Command.Gateway {
	public interface ICommandGateway {
		Task Send(CommandMessage commandMessage);
	}
}
