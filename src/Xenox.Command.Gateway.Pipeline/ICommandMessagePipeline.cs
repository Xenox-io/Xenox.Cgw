using Xenox.Command.Messaging;
using Xenox.Pipeline;

namespace Xenox.Command.Gateway.Pipeline {
	public interface ICommandMessagePipeline : IPipeline<CommandMessage, CommandMessage> {
	}
}
