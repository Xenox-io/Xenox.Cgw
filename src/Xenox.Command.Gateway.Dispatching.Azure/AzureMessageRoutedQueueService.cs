using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Xenox.Serialization;

namespace Xenox.Command.Gateway.Dispatching.Azure {
	public class AzureMessageRoutedQueueService : IMessageRoutedQueueService {
		private readonly ISerializationService _serializationService;
		private readonly CloudQueueClient _azureQueueClient;

		public AzureMessageRoutedQueueService(
			ISerializationService serializationService,
			CloudQueueClient azureQueueClient
		) {
			_azureQueueClient = azureQueueClient;
			_serializationService = serializationService;
		}

		public async Task Queue(string routingKey, object message) {
			CloudQueue azureQueue = _azureQueueClient.GetQueueReference(routingKey.ToLower());
			await azureQueue.AddMessageAsync(
				CloudQueueMessage.CreateCloudQueueMessageFromByteArray(_serializationService.SerializeToBytes(message))
			);
		}
	}
}
