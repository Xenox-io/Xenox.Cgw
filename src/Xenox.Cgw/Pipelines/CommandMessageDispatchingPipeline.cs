using System.Threading.Tasks;
using Xenox.Cgw.Services;
using Xenox.Messaging;

namespace Xenox.Cgw.Pipelines {
	public class CommandMessageDispatchingPipeline : ICommandMessageDispatchingPipeline {
		private readonly IMessageDispatchingService _messageDispatchingService;

		public CommandMessageDispatchingPipeline(IMessageDispatchingService messageDispatchingService) {
			_messageDispatchingService = messageDispatchingService;
		}

		public async Task<CommandMessage> Send(CommandMessage commandMessage) {
			Message message = new Message(commandMessage.Command, commandMessage.Headers);
			await _messageDispatchingService.Dispatch(message);
			return commandMessage;
		}
	}
}
