using System.Threading.Tasks;

namespace Xenox.Cgw {
	public interface ICommandGateway {
		Task Send(CommandMessage commandMessage);
	}
}
