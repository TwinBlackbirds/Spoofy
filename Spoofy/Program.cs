
using Newtonsoft.Json.Linq;
using Spoofy;
using System.Text.RegularExpressions;

//await ytdlpHelper.Init();
//ytdlpHelper.ToMP3("https://www.youtube.com/watch?v=dQw4w9WgXcQ");


bool correct = false;
string query = "";
while (!correct)
{
    Console.Write("Enter your search query: ");
    query = Console.ReadLine();
    Console.Write ("Searching for: " + query + "? (Y/n): ");
    var confirmation = Console.ReadLine();
    if (confirmation.ToLower() != "n")
    {
        correct = true;
        Console.WriteLine();
    }
}

string url = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(query)}";

using HttpClient client = new HttpClient();
string html = await client.GetStringAsync(url);

// Find ytInitialData JSON block
var match = Regex.Match(html, @"var ytInitialData = ({.*?});</script>", RegexOptions.Singleline);
if (!match.Success)
{
    Console.WriteLine("Failed to find ytInitialData.");
    return;
}

string json = match.Groups[1].Value;
JObject data = JObject.Parse(json);

// Traverse the JSON to find the first videoRenderer
var contents = data["contents"]?["twoColumnSearchResultsRenderer"]?["primaryContents"]
    ?["sectionListRenderer"]?["contents"]?[0]?["itemSectionRenderer"]?["contents"];

var idx = 1;
foreach (var item in contents)
{
    if (idx > 5) break;
    var video = item["videoRenderer"];
    if (video != null)
    {
        string videoId = video["videoId"]?.ToString();
        string title = video["title"]?["runs"]?[0]?["text"]?.ToString();

        Console.WriteLine($"({idx}) \tTitle: {title}");
        Console.WriteLine($"\tID: {videoId}");
        Console.WriteLine();
        idx++;
    }
}
Console.Write("Please make a selection (1-5): ");
var selection = Console.ReadLine()!.Trim();