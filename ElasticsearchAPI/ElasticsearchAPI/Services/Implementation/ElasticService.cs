using ElasticsearchAPI.Model;
using Nest;

namespace ElasticsearchAPI.Services.Implementation;

public class ElasticService : IElasticService
{
    private readonly ElasticClient _client;
    private static string _indexName = "movie-db-index";
    private static string? _cloudUrl;
    private static string? _username;
    private static string? _password;
    private Dictionary<string, long>? Indices { get; set; }

    #region Constructor

    private static readonly Lazy<ElasticService> _instance = new(() => new ElasticService());

    public static ElasticService Instance => _instance.Value;

    private ElasticService()
    {
        InitializeElastiClientData();
        var settings = new ConnectionSettings(new Uri(_cloudUrl!))
            .DefaultIndex(_indexName)
            .BasicAuthentication(_username, _password);
        _client = new ElasticClient(settings);
        Indices = new Dictionary<string, long>();
        if (_client.Indices.Exists(_indexName).Exists)
        {
            var dbIndices = _client.Cat.Indices().Records
                .Where(r => r.Index.Contains("-db-index"))
                .Select(r => r.Index).ToList();
            foreach (var index in dbIndices)
            {
                Indices.Add(index, _client.Count<object>(c => c.Index(index)).Count);
            }

            return;
        }

        var createIndexResponse = _client.Indices.Create(_indexName, c => c
            .Map<Movie>(m => m.AutoMap())
        );

        if (!createIndexResponse.IsValid)
        {
            throw new InvalidOperationException();
        }
    }

    #endregion

    public async Task<IEnumerable<object>> GetAllData(string type)
    {
        ChangeIndex(type);

        if (Indices == null) throw new InvalidOperationException();
        var searchResponse = await _client.SearchAsync<object>(s => s
            .Index(_indexName)
            .Query(q => q.MatchAll())
            .From(0)
            .Size((int)Indices[_indexName])
        );

        return searchResponse.Documents;
    }

    public async Task<IEnumerable<object>> GetSnippetData(string type)
    {
        ChangeIndex(type);

        if (Indices == null) throw new InvalidOperationException();
        var rnd = new Random().Next(1, (int)(Indices[_indexName] - 11));
        var searchResponse = await _client.SearchAsync<object>(s => s
            .Index(_indexName)
            .Query(q => q.MatchAll())
            .From(rnd)
            .Size(10)
        );

        return searchResponse.Documents;
    }

    public async Task<Dictionary<string, string>> GetAllIndicesProperties()
    {
        if (Indices == null) throw new InvalidOperationException();
        var tasks = Indices.Keys.Select(async index =>
        {
            if (Indices[index] == 0) return new KeyValuePair<string, string>(index, "no-mapping");
            var mappingResponse = await _client.Indices.GetMappingAsync(new GetMappingRequest(index));
            var properties = mappingResponse.Indices[index].Mappings.Properties;
            var propString = string.Join(",", properties.Keys);
            return new KeyValuePair<string, string>(index, propString);
        });

        var results = await Task.WhenAll(tasks);

        return results.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private void ChangeIndex(string newIndexName)
    {
        var auxIndex = newIndexName + "-db-index";
        if (Indices != null && !Indices.ContainsKey(auxIndex))
        {
            throw new InvalidOperationException();
        }

        _indexName = auxIndex;
    }

    private static void InitializeElastiClientData()
    {
        var clientData = File.ReadAllLines("elastic_client_connection_data");
        _cloudUrl = clientData[0];
        _username = clientData[1];
        _password = clientData[2];
    }
    
    
    //------------------------------------------------------------------------------------------------------------
    //Look up Interface for description
    //------------------------------------------------------------------------------------------------------------
    //
    // public async Task PopulateMovieDb()
    // {
    //     var json = await File.ReadAllTextAsync("C:\\Compendium\\ProiectePROG\\MoviesDataSets\\archive\\moviesDB.json");
    //     var moviesArray = JArray.Parse(json);
    //
    //     foreach (var mvs in moviesArray)
    //     {
    //         var indexResponse = await _client.IndexAsync(mvs.ToObject<Movie>(), i => i.Index(IndexName));
    //         
    //         if (!indexResponse.IsValid)
    //         {
    //             throw new InvalidOperationException();
    //         }
    //     }
    // }
    //------------------------------------------------------------------------------------------------------------
}