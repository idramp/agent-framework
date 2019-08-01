using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Credentials
{
    /// <summary>
    /// A credential offer content message.
    /// </summary>
    public class CredentialRejectMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialRejectMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.CredentialReject;
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"Type={Type}";
    }
}
