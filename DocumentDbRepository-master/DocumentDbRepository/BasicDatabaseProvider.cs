using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Santhos.DocumentDb.Repository
{
    /// <inheritdoc />
    /// <summary>
    /// Provides DocumentDb database based on given database identifier
    /// </summary>
    public class BasicDatabaseProvider : IDatabaseProvider
    {
        private readonly DocumentClient _client;

        private readonly string _databaseId;

        /// <summary>Initializes a new instance of the <see cref="BasicDatabaseProvider"/> class.</summary>
        /// <param name="client">DocumentDb Document Client</param>
        /// <param name="databaseId">Database identifier</param>
        public BasicDatabaseProvider(DocumentClient client, string databaseId)
        {
            this._client = client;
            this._databaseId = databaseId;
        }

        /// <inheritdoc />
        /// <summary>
        /// Creates or gets the DocumentDb database. Queries the DocumentDb subscription 
        /// using the <see cref="T:Microsoft.Azure.Documents.Client.DocumentClient" /> specified in the constructor.
        /// If a database with the specified database id exists, returns the instance.
        /// If the database does not exist, creates a new instance and returns it.
        /// </summary>
        /// <returns>DocumentDb database</returns>
        public virtual async Task<Database> CreateOrGetDb()
        {
            Database db = this._client.CreateDatabaseQuery()
                .Where(d => d.Id == this._databaseId)
                .AsEnumerable()
                .FirstOrDefault();
            
            return db ?? await this._client.CreateDatabaseAsync(new Database { Id = this._databaseId });
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the DocumentDb database self link. Uses the <see cref="M:Santhos.DocumentDb.Repository.BasicDatabaseProvider.CreateOrGetDb" /> method to
        /// obtain the database instance.
        /// </summary>
        /// <returns>DocumentDb database self link</returns>
        public virtual async Task<string> GetDbSelfLink()
        {
            return (await this.CreateOrGetDb()).SelfLink;
        }
    }
}
