using System.Collections.Generic;
using Xenox.Command.Serialization;
using Xenox.Messaging;

namespace Xenox.Cgw {
	public class CommandMessage {
		public SerializableCommand Command { get; }
		public HeaderCollection Headers { get; }

		public CommandMessage(SerializableCommand command) {
			Command = command;
			Headers = new HeaderCollection();
		}

		public CommandMessage(SerializableCommand command, HeaderCollection headers) {
			Command = command;
			Headers = headers;
		}

		public CommandMessage(SerializableCommand command, IEnumerable<Header> headers) {
			Command = command;
			Headers = new HeaderCollection(headers);
		}

		public CommandMessage(SerializableCommand command, IEnumerable<KeyValuePair<string, string>> headers) {
			Command = command;
			Headers = new HeaderCollection(headers);
		}
	}
}
