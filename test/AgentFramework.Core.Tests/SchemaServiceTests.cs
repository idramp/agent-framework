using System;
using System.Net.Http;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Runtime;
using AgentFramework.TestHarness.Utils;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.PoolApi;
using Hyperledger.Indy.WalletApi;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class SchemaServiceTests : IAsyncLifetime
    {
        private readonly ISchemaService _schemaService;

        private readonly string _issuerConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";

        private Pool _pool;
        private Wallet _issuerWallet;
        private IAgentContext _agentContext;

        public SchemaServiceTests()
        {
            var walletService = new DefaultWalletRecordService();
            var ledgerService = new DefaultLedgerService();
            var tailsService = new DefaultTailsService(ledgerService, new HttpClientHandler());

            var provisioningMock = new Mock<IProvisioningService>();
            _schemaService = new DefaultSchemaService(provisioningMock.Object, walletService, ledgerService, tailsService);
        }

        [Fact]
        public async Task CanCreateAndResolveSchema()
        {
            var issuer = await Did.CreateAndStoreMyDidAsync(_issuerWallet,
                new { seed = "000000000000000000000000Steward1" }.ToJson());

            var schemaName = $"Test-Schema-{Guid.NewGuid().ToString("N")}";
            var schemaVersion = "1.0";
            var schemaAttrNames = new[] {"test_attr_1", "test_attr_2"};

            //Create a dummy schema
            var schemaId = await _schemaService.CreateSchemaAsync(_agentContext, issuer.Did, schemaName, schemaVersion,
                schemaAttrNames);

            //Resolve it from the ledger with its identifier
            var resultSchema = await _schemaService.GetSchemaAsync(_agentContext, schemaId);

            var resultSchemaName = resultSchema.Name;
            var resultSchemaVersion = resultSchema.Version;
            var sequenceId = resultSchema.SequenceNumber??0;

            Assert.Equal(schemaName, resultSchemaName);
            Assert.Equal(schemaVersion, resultSchemaVersion);

            //Resolve it from the ledger with its sequence Id
            var secondResultSchema = await _schemaService.LookupSchemaAsync(_pool, sequenceId);

            var secondResultSchemaName = secondResultSchema.Name;
            var secondResultSchemaVersion = secondResultSchema.Version;

            Assert.Equal(schemaName, secondResultSchemaName);
            Assert.Equal(schemaVersion, secondResultSchemaVersion);
        }

        [Fact]
        public async Task CanCreateAndResolveCredentialDefinitionAndSchema()
        {
            var issuer = await Did.CreateAndStoreMyDidAsync(_issuerWallet,
                new { seed = "000000000000000000000000Steward1" }.ToJson());

            var schemaName = $"Test-Schema-{Guid.NewGuid().ToString()}";
            var schemaVersion = "1.0";
            var schemaAttrNames = new[] { "test_attr_1", "test_attr_2" };

            //Create a dummy schema
            var schemaId = await _schemaService.CreateSchemaAsync(_agentContext, issuer.Did, schemaName, schemaVersion,
                schemaAttrNames);

            var credId = await _schemaService.CreateCredentialDefinitionAsync(_agentContext, schemaId, issuer.Did, "Tag", false, 100, new Uri("http://mock/tails"));

            var credDef =
                await _schemaService.GetCredentialDefinitionAsync(_agentContext, credId, checkLedgerIfNotFound:true);

            var resultCredId = credDef.Id;

            Assert.Equal(credId, resultCredId);

            var result = await _schemaService.LookupSchemaFromCredentialDefinitionAsync(_agentContext, credId);

            var resultSchemaName = result.Name;
            var resultSchemaVersion = result.Version;

            Assert.Equal(schemaName, resultSchemaName);
            Assert.Equal(schemaVersion, resultSchemaVersion);

            var recordResult = await _schemaService.GetCredentialDefinitionAsync(_agentContext, credId);

            Assert.Equal(schemaId, recordResult.SchemaId);
        }

        public async Task InitializeAsync()
        {
            try
            {
                await Wallet.CreateWalletAsync(_issuerConfig, Credentials);
            }
            catch (WalletExistsException)
            {
                // OK
            }

            _issuerWallet = await Wallet.OpenWalletAsync(_issuerConfig, Credentials);

            _pool = await PoolUtils.GetPoolAsync();

            _agentContext = new Handlers.AgentContext
            {
                Wallet = _issuerWallet,
                Pool = Models.PoolAwaitable.FromPool(_pool)
            };
        }

        public async Task DisposeAsync()
        {
            if (_issuerWallet != null) await _issuerWallet.CloseAsync();

            _issuerWallet?.Dispose();

            await Wallet.DeleteWalletAsync(_issuerConfig, Credentials);
        }
    }
}
