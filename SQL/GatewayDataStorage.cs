namespace sql_fetcher;
using enums;

public class GatewayDataStorage
{
    public List<AccesableData> Name { get; private set; }
    public List<List<double>> Data { get; private set; }
    public List<string> Query { get; private set; }
    public List<string> Location { get; private set; }
    public List<string> Gateways { get; private set; }
    
    private readonly DataFetcher _dataFetcher;
    
    public GatewayDataStorage(DataFetcher dataFetcher) //Initialize components
    {
        _dataFetcher = dataFetcher;
        Name = new List<AccesableData>();
        Location = new List<string>();
        Query = new List<string>();
        Data = new List<List<double>>();
        Gateways = new List<string>();
    }
    
    public void Add (AccesableData name, string location, string query) //This function adds a query to the list of queries to be fetched with corresponding name and fetches the data once.
    {
        try
        {
            for (int i = 0;
                 i < _dataFetcher.FetchInt($"SELECT COUNT(*) FROM gateway WHERE deviceID = '{location}'");
                 i++)
            {
                Name.Add(name);
                Location.Add(location);
                Gateways.Add(_dataFetcher.FetchString($"WITH OrderedRows AS " +
                                                    $"(SELECT *, ROW_NUMBER() OVER (ORDER BY Latitude) AS RowNum " +
                                                    $"FROM gateway " +
                                                    $"WHERE deviceID = '{location}')" +
                                                    $"SELECT gatewayID FROM OrderedRows WHERE RowNum = {i + 1};"));
                string query_ = query.Replace("gateway_input", Gateways.Last());
                Query.Add(query_);
                Data.Add(_dataFetcher.FetchData(query_));
            }
        }
        catch
        {
            throw new Exception($"Failed to fetch data for {query}");
        }
    }
    
    public void Reload() //Reloads the data for all the queries in the list
    {
        for (int i = 0; i < Query.Count; i++)
        {
            Data[i] = _dataFetcher.FetchData(Query[i]);
        }
    }
}