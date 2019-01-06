using System;
using System.Threading.Tasks;
using Xenox.Command.Gateway.Dispatching;
using Xenox.Command.Messaging;
using Xenox.Messaging;

namespace Xenox.Command.Gateway.Pipeline {
	public class CommandMessageDispatchingPipeline : ICommandMessageDispatchingPipeline {
		private readonly IMessageDispatchingService _messageDispatchingService;

		public CommandMessageDispatchingPipeline(IMessageDispatchingService messageDispatchingService) {
			_messageDispatchingService = messageDispatchingService;
		}

		public async Task<CommandMessage> Send(CommandMessage commandMessage) {
			Message message = new Message(
				Guid.NewGuid().ToString(),
				commandMessage.CommandMessageBody,
				commandMessage.HeaderCollection
			);
			await _messageDispatchingService.Dispatch(message);
			return commandMessage;
		}
	}
}
