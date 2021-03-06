﻿using System;
using System.Collections.Generic;

namespace AgentFramework.Core.Models.Wallets
{
    /// <summary>
    /// A configuration object for controlling the provisioning of a new agent.
    /// </summary>
    public class ProvisioningConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the owner of the agent
        /// </summary>
        /// <value>
        /// The agent owner name 
        /// </value>
        public string OwnerName { get; set; }

        /// <summary>
        /// Gets or sets the imageUrl of the owner of the agent
        /// </summary>
        /// <value>
        /// The agent owner image url
        /// </value>
        public string OwnerImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the agent seed used to generate deterministic DID and Verkey. (32 characters)
        /// <remarks>Leave <c>null</c> to generate random agent did and verkey</remarks>
        /// </summary>
        /// <value>
        /// The agent seed.
        /// </value>
        public string AgentSeed { get; set; }

        /// <summary>
        /// Gets or sets the agent did.
        /// </summary>
        /// <value>
        /// The agent did.
        /// </value>
        public string AgentDid { get; set; }

        /// <summary>
        /// Gets or sets the agent verkey.
        /// </summary>
        /// <value>
        /// The agent verkey.
        /// </value>
        public string AgentVerkey { get; set; }

        /// <summary>
        /// Gets or sets the endpoint URI that this agent will receive Sovrin messages
        /// </summary>
        /// <value>
        /// The endpoint URI.
        /// </value>
        public Uri EndpointUri { get; set; }

        /// <summary>
        /// Gets or sets the issuer seed used to generate deterministic DID and Verkey. (32 characters)
        /// <remarks>Leave <c>null</c> to generate random issuer did and verkey</remarks>
        /// </summary>
        /// <value>
        /// The issuer seed.
        /// </value>
        public string IssuerSeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an issuer did and verkey should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create issuer]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateIssuer { get; set; }

        /// <summary>
        /// Gets or sets the tails service base URI.
        /// </summary>
        /// <value>The tails base URI.</value>
        public Uri TailsBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the wallet configuration.
        /// </summary>
        /// <value>
        /// The wallet configuration.
        /// </value>
        public WalletConfiguration WalletConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the wallet credentials.
        /// </summary>
        /// <value>
        /// The wallet credentials.
        /// </value>
        public WalletCredentials WalletCredentials { get; set; }

        /// <summary>
        /// Initial set of tags to populate the provisioning record
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public Dictionary<string, string> Tags { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"OwnerName={OwnerName}, " +
            $"OwnerImageUrl={OwnerImageUrl}, " +
            $"AgentSeed={(AgentSeed?.Length > 0 ? "[hidden]" : null)}, " +
            $"AgentDid={AgentDid}, " +
            $"AgentVerkey={(AgentVerkey?.Length > 0 ? "[hidden]" : null)}, " +
            $"EndpointUri={EndpointUri}, " +
            $"IssuerSeed={(IssuerSeed?.Length > 0 ? "[hidden]" : null)}, " +
            $"CreateIssuer={CreateIssuer}, " +
            $"TailsBaseUri={TailsBaseUri}, " +
            $"WalletConfiguration={WalletConfiguration}, " +
            $"WalletCredentials={WalletCredentials}";
    }
}
