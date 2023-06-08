using System.Text;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

        var db = redis.GetDatabase();
        Console.WriteLine("Try some redis commands!");

        db.StringSet("foo2",9000,TimeSpan.FromSeconds(2),When.NotExists);
        db.StringSet("foo", "bar");
        
        Console.WriteLine(db.StringGet("foo")); // prints bar
        Console.WriteLine(db.StringGet("foo2")); // prints bar

        var hash = new HashEntry[] { 
        new HashEntry("name", "John"), 
        new HashEntry("surname", "Smith"),
        new HashEntry("company", "Redis"),
        new HashEntry("age", "29"),
        };
        db.HashSet("user-session:123", hash);

        var hashFields = db.HashGetAll("user-session:123");
        Console.WriteLine(String.Join("; ", hashFields));

        db.HashIncrement("user-session:123","age",1);
        Console.WriteLine("Update age via increment to 30");
        Console.WriteLine(String.Join("; ", db.HashGetAll("user-session:123")));

        var ft = db.FT();
        var json = db.JSON();

        var user1 = new {
            name = "Paul John",
            email = "paul.john@example.com",
            age = 42,
            city = "London"
        };

        var user2 = new {
            name = "Eden Zamir",
            email = "eden.zamir@example.com",
            age = 29,
            city = "Tel Aviv"
        };

        var user3 = new {
            name = "Paul Zamir",
            email = "paul.zamir@example.com",
            age = 35,
            city = "Tel Aviv"
        };

        // Set records to path
        json.Set("user:1", "$", user1);
        json.Set("user:2", "$", user2);
        json.Set("user:3", "$", user3);

        // Get json record
        System.Console.WriteLine("Get user:1 json");
        System.Console.WriteLine(json.Get("user:1",path: "$"));

        // Search module isn't in Elasticache cluster...
        
        // var schema = new Schema()
        //     .AddTextField(new FieldName("$.name", "name"))
        //     .AddTagField(new FieldName("$.city", "city"))
        //     .AddNumericField(new FieldName("$.age", "age"));

        // try {
        //     ft.DropIndex("idx:users");
        //     ft.Create(
        //         "idx:users",
        //         new FTCreateParams().On(IndexDataType.JSON).Prefix("user:"),
        //         schema);
        // } catch (Exception) {
        //     System.Console.WriteLine($"Index already exists.");
        // }

        // // Search on age range for Pauls
        // var res = ft.Search("idx:users", new Query("Paul @age:[40 45]"))
        //     .Documents.Select(x => x["json"]);
        // Console.WriteLine(string.Join("\n", res)); 

        // // return only cities
        // var res_cities = ft.Search("idx:users", new Query("Paul")
        //     .ReturnFields(new FieldName("$.city", "city"))).Documents.Select(x => x["city"]);
        // Console.WriteLine(string.Join(", ", res_cities)); 

        // // Count all users in same city
        // var request = new AggregationRequest("*")
        //     .GroupBy("@city", Reducers.Count().As("count"));
        // var result = ft.Aggregate("idx:users", request);

        // for (var i=0; i<result.TotalResults; i++)
        // {
        //     var row = result.GetRow(i);
        //     Console.WriteLine($"{row["city"]} - {row["count"]}");
        // }

    }
}
