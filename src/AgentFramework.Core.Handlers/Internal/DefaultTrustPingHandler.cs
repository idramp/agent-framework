﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Decorators.Threading;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.Common;
using AgentFramework.Core.Models.Events;

namespace AgentFramework.Core.Handlers.Internal
{
    /// <summary>
    /// Default trust ping message handler.
    /// </summary>
    public class DefaultTrustPingMessageHandler : IMessageHandler
    {
        /// <summary>
        /// The event aggregator.
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// The message service.
        /// </summary>
        private readonly IMessageService _messageService;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:AgentFramework.Core.Handlers.Internal.DefaultTrustPingMessageHandler"/> class.
        /// </summary>
        /// <param name="eventAggregator">Event aggregator.</param>
        /// <param name="messageService">Message service.</param>
        public DefaultTrustPingMessageHandler(IEventAggregator eventAggregator, IMessageService messageService)
        {
            _eventAggregator = eventAggregator;
            _messageService = messageService;
        }

        /// <summary>
        /// Gets the supported message types.
        /// </summary>
        /// <value>
        /// The supported message types.
        /// </value>
        public IEnumerable<string> SupportedMessageTypes => new[]
        {
            MessageTypes.TrustPingMessageType,
            MessageTypes.TrustPingResponseMessageType
        };

        /// <summary>
        /// Processes the agent message
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="messageContext">The agent message agentContext.</param>
        /// <param name="serviceProvider">The service provider in the request context.</param>
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, MessageContext messageContext, System.IServiceProvider serviceProvider)
        {
            await Task.Yield();

            switch (messageContext.GetMessageType())
            {
                case MessageTypes.TrustPingMessageType:
                    {
                        var pingMessage = messageContext.GetMessage<TrustPingMessage>();

                        _eventAggregator.Publish(new ServiceMessageProcessingEvent
                        {
                            MessageType = MessageTypes.TrustPingMessageType,
                            ThreadId = pingMessage.FindDecorator<ThreadDecorator>("thread")?.ThreadId
                        });

                        if (pingMessage.ResponseRequested)
                        {
                            return pingMessage.CreateThreadedReply<TrustPingResponseMessage>();
                        }
                        break;
                    }
                case MessageTypes.TrustPingResponseMessageType:
                    {
                        var pingMessage = messageContext.GetMessage<TrustPingMessage>();

                        _eventAggregator.Publish(new ServiceMessageProcessingEvent
                        {
                            MessageType = MessageTypes.TrustPingResponseMessageType,
                            ThreadId = pingMessage.FindDecorator<ThreadDecorator>("thread")?.ThreadId
                        });
                        break;
                    }
            }
            return null;
        }
    }
}
