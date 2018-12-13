using System;
using System.Linq;
using System.Threading.Tasks;
using Xenox.Messaging;

namespace Xenox.Cgw.Services {
	public class MessageDispatchingRoutedQueueService : IMessageDispatchingService {
		private readonly IMessageRoutedQueueService _messageRoutedQueueService;

		public MessageDispatchingRoutedQueueService(IMessageRoutedQueueService messageRoutedQueueService) {
			_messageRoutedQueueService = messageRoutedQueueService;
		}

		public Task Dispatch(Message message) {
			Header routingKeyHeader = message.Headers.FirstOrDefault(h => h.Name == HeaderNames.RoutingKey);
			if (routingKeyHeader == null) {
				throw new Exception("No Routing Key Header exists.");
			}
			return _messageRoutedQueueService.Queue(routingKeyHeader.Value, message);
		}
	}
}
