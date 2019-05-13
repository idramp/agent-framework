﻿using System.Collections.Generic;
using AgentFramework.AspNetCore.Middleware;
using AgentFramework.Core.Handlers;

namespace AgentFramework.AspNetCore
{
    public interface IAgentFactory
    {
        T Create<T>(object param) where T : IAgent;

        IDictionary<object,object> Properties { get; set; }
    }
}
