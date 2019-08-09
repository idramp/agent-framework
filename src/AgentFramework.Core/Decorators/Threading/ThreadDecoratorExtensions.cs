using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Messages;

namespace AgentFramework.Core.Decorators.Threading
{
    /// <summary>
    /// Message threading extensions.
    /// </summary>
    public static class ThreadDecoratorExtensions
    {
        /// <summary>
        /// Threading decorator extension.
        /// </summary>
        public static string DecoratorIdentifier => "thread";

        /// <summary>
        /// Created a new threaded message response
        /// </summary>
        /// <param name="message">The message to thread from.</param>
        public static T CreateThreadedReply<T>(this AgentMessage message) where T : AgentMessage, new ()
        {
            var newMsg = new T();
            newMsg.ThreadMessage(message);
            return newMsg;
        }

        /// <summary>
        /// Threads the current message from a previous message.
        /// </summary>
        /// <param name="message">The message to add threading to.</param>
        /// <param name="previousMessage">The message to thread from.</param>
        public static void ThreadFrom(this AgentMessage message, AgentMessage previousMessage)
        {
            bool hasThreadBlock = false;
            try
            {
                message.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
                hasThreadBlock = true;
            }
            catch (AgentFrameworkException) { }

            if (hasThreadBlock)
                throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Cannot thread message when it already has a valid thread decorator");

            message.ThreadMessage(previousMessage);
        }

        /// <summary>
        /// Gets the current messages thread id.
        /// </summary>
        /// <param name="message">Message to extract the thread id from.</param>
        /// <returns>Thread id of the message.</returns>
        public static string GetThreadId(this AgentMessage message)
        {
            string threadId = null;
            try
            {
                var threadBlock = message.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
                threadId = threadBlock.ThreadId;
            }
            catch (Exception)
            {
                // ignored
            }

            if (string.IsNullOrEmpty(threadId))
                threadId = message.Id;

            return threadId;
        }

        /// <summary>
        /// Threads the current message.
        /// </summary>
        /// <param name="messageToThread">Message to thread.</param>
        /// <param name="threadId">Thread id to thread the message with.</param>
        public static void ThreadFrom(this AgentMessage messageToThread, string threadId)
        {
            var currentThreadContext = new ThreadDecorator
            {
                ThreadId = threadId
            };
            messageToThread.AddDecorator(currentThreadContext, DecoratorIdentifier);
        }

        /// <summary>
        /// Created a new threaded message response
        /// </summary>
        /// <param name="message">The parent message to thread from.</param>
        public static T CreateChildThreadedReply<T>(this AgentMessage message) where T : AgentMessage, new()
        {
            var newMsg = new T();
            newMsg.ThreadChildMessage(message);
            return newMsg;
        }

        /// <summary>
        /// Threads the current message from a parent message.
        /// </summary>
        /// <param name="message">The message to add threading to.</param>
        /// <param name="previousMessage">The parent message to thread from.</param>
        public static void ThreadFromParent(this AgentMessage message, AgentMessage previousMessage)
        {
            string parentThreadId = previousMessage.GetThreadId();

            message.ThreadFromParent(parentThreadId);
        }

        /// <summary>
        /// Gets the current messages parent thread id.
        /// </summary>
        /// <param name="message">Message to extract the thread id from.</param>
        /// <returns>Thread id of the message.</returns>
        public static string GetParentThreadId(this AgentMessage message)
        {
            string threadId = null;
            try
            {
                var threadBlock = message.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
                threadId = threadBlock?.ParentThreadId;
            }
            catch (Exception)
            {
                // ignored
            }

            return threadId;
        }

        /// <summary>
        /// Threads the current message.
        /// </summary>
        /// <param name="messageToThread">Message to thread.</param>
        /// <param name="threadId">Thread id to thread the message with.</param>
        /// <param name="parentThreadId">The thread Id of the parent message thread.</param>
        public static void ThreadFrom(this AgentMessage messageToThread, string threadId, string parentThreadId)
        {
            var currentThreadContext = new ThreadDecorator
            {
                ThreadId = threadId,
                ParentThreadId = parentThreadId
            };
            messageToThread.AddDecorator(currentThreadContext, DecoratorIdentifier);
        }



        /// <summary>
        /// Threads the current message.
        /// </summary>
        /// <param name="message">Message to thread.</param>
        /// <param name="parentThreadId">The thread Id of the parent message thread.</param>
        public static void ThreadFromParent(this AgentMessage message, string parentThreadId)
        {
            ThreadDecorator threadBlock = null;
            try
            {
                threadBlock = message.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
            }
            catch (AgentFrameworkException) { }

            if (threadBlock == null)
            {
                var currentThreadContext = new ThreadDecorator
                {
                    ThreadId = message.Id,
                    ParentThreadId = parentThreadId
                };
                message.AddDecorator(currentThreadContext, DecoratorIdentifier);
            }
            else
            {
                if(!string.IsNullOrEmpty(threadBlock.ParentThreadId))
                    throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Cannot add parent thread when it already has one");

                threadBlock.ParentThreadId = parentThreadId;
                message.SetDecorator(threadBlock, DecoratorIdentifier); 
            }
        }

        private static void ThreadMessage(this AgentMessage messageToThread, AgentMessage messageToThreadFrom)
        {
            ThreadDecorator previousMessageThreadContext = null;
            try
            {
                previousMessageThreadContext = messageToThreadFrom.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
            }
            catch (AgentFrameworkException) { }

            ThreadDecorator currentThreadContext;
            if (previousMessageThreadContext != null)
            {
                currentThreadContext = new ThreadDecorator
                {
                    ParentThreadId = previousMessageThreadContext.ParentThreadId,
                    ThreadId = previousMessageThreadContext.ThreadId
                };
            }
            else
            {
                currentThreadContext = new ThreadDecorator
                {
                    ThreadId = messageToThreadFrom.Id
                };
            }


            messageToThread.AddDecorator(currentThreadContext, DecoratorIdentifier);
        }

        private static void ThreadChildMessage(this AgentMessage messageToThread, AgentMessage messageToThreadFrom)
        {
            ThreadDecorator previousMessageThreadContext = null;
            try
            {
                previousMessageThreadContext = messageToThreadFrom.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
            }
            catch (AgentFrameworkException) { }

            ThreadDecorator currentThreadContext;
            if (previousMessageThreadContext != null)
            {
                currentThreadContext = new ThreadDecorator
                {
                    ParentThreadId = previousMessageThreadContext.ThreadId,
                    ThreadId = messageToThreadFrom.Id
                };
            }
            else
            {
                currentThreadContext = new ThreadDecorator
                {
                    ThreadId = messageToThreadFrom.Id
                };
            }


            messageToThread.AddDecorator(currentThreadContext, DecoratorIdentifier);
        }
    }
}
