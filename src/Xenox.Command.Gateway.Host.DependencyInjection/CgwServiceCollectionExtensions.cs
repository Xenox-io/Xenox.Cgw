using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Xenox.Auth.Client.DependencyInjection;
using Xenox.Command.Gateway.Dispatching;
using Xenox.Command.Gateway.Dispatching.Azure;
using Xenox.Command.Gateway.Pipeline;
using Xenox.Encryption.DependencyInjection;
using Xenox.Encryption.SuperBasicEncryption;
using Xenox.Serialization.JsonNet.DependencyInjection;

namespace Xenox.Command.Gateway.Host.DependencyInjection {
	public static class CgwServiceCollectionExtensions {
		public static IServiceCollection AddCgw(this IServiceCollection serviceCollection, IConfiguration configuration) {
			if (serviceCollection == null) {
				throw new ArgumentNullException(nameof(serviceCollection));
			}
			return serviceCollection
				.AddSingleton<ICommandGateway, PipelineCommandGateway>()
				.AddSingleton<ICommandMessageDispatchingPipeline, CommandMessageDispatchingPipeline>()
				.AddSingleton<IAuthenticationPipeline, AuthenticationPipeline>()
				.AddSingleton<IAuthorizationPipeline, AuthorizationPipeline>()
				.AddMessageDispatchingService(configuration)
				.AddJsonSuperBasicEncryption(configuration)
				.AddAzureMessaging(configuration)
			;
		}

		private static IServiceCollection AddMessageDispatchingService(this IServiceCollection serviceCollection, IConfiguration configuration) {
			if (serviceCollection == null) {
				throw new ArgumentNullException(nameof(serviceCollection));
			}
			return serviceCollection
				.AddSingleton<IMessageDispatchingService, MessageDispatchingRoutedQueueService>()
			;
		}

		private static IServiceCollection AddJsonSuperBasicEncryption(this IServiceCollection serviceCollection, IConfiguration configuration) {
			if (serviceCollection == null) {
				throw new ArgumentNullException(nameof(serviceCollection));
			}
			return serviceCollection
				.AddJsonNetSerialization()
				.AddEncryption()
				.AddSuperBasicEncryption()
				.AddAesPlusHmacAuthorizationTokenClient()
			;
		}

		private static IServiceCollection AddAzureMessaging(this IServiceCollection serviceCollection, IConfiguration configuration) {
			if (serviceCollection == null) {
				throw new ArgumentNullException(nameof(serviceCollection));
			}
			return serviceCollection
				.AddTransient<IMessageRoutedQueueService, AzureMessageRoutedQueueService>()
				.AddSingleton<CloudStorageAccount>(sp => CloudStorageAccount.Parse(configuration["App:AzureStorageConnectionString"]))
				.AddTransient<CloudQueueClient>(sp => {
					CloudStorageAccount azureAccount = sp.GetService<CloudStorageAccount>();
					return azureAccount.CreateCloudQueueClient();
				})
			;
		}
	}
}
