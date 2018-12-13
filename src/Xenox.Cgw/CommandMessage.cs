using System.Collections.Generic;
using Xenox.Command;
using Xenox.Messaging;

namespace Xenox.Cgw {
	public class CommandMessage {
		public CommandInfo Command { get; }
		public HeaderCollection Headers { get; }

		public CommandMessage(CommandInfo command) {
			Command = command;
			Headers = new HeaderCollection();
		}

		public CommandMessage(CommandInfo command, HeaderCollection headers) {
			Command = command;
			Headers = headers;
		}

		public CommandMessage(CommandInfo command, IEnumerable<Header> headers) {
			Command = command;
			Headers = new HeaderCollection(headers);
		}

		public CommandMessage(CommandInfo command, IEnumerable<KeyValuePair<string, string>> headers) {
			Command = command;
			Headers = new HeaderCollection(headers);
		}
	}
}
