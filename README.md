# SpiderSharp
Web Crawling and Scraping Framework

## Features
* Cache web pages with Redis
* Export the results automatically to MongoDB
* Manny http request providers
* Helper to ignore invalid ssl certificate
* Helper to rename json property name token
* Helper to get or delete N itens of collection
* Common regular expression to grab data
* Extract youtube video metadata

## Regular Expression Library
There are some **Regex** done to grab data

* Cookies: Can slice each part of cookies

## Http Requester Providers
Each provider has your own features. You can choose anyone to get a resource

* HttpClient: Provides a class for sending HTTP requests and receiving HTTP responses from a resource.
* AngleSharp: WebRequester for navigation like a browser.
* WebClient: Provides common methods for sending data to and receiving data from a resource.
* CookieWebClient: Extension of WebClient including Cookies of transaction.
* BetterWebClient: Extension of WebClient including session through with cookie container, GZip header, HTTP status code.

## Pipelines
Pipeline get a data and transform sending the result to next pipeline. Use can use how much pipelines you want.

### Dasherize Pipeline 
> Call .AddDasherizePipeline() 

This pipeline is used to normalize an object transforming the json key fields like:

```C#
"some_title" => "some-title"
"someTitle" => "some-title"
```

### Print to Console Pipeline
> Call .AddPrintToConsolePipeline(**fields**)

This pipeline is used to print some scraped data to stdout. The **fields** parameter is optional

### Save to Mongo Pipeline
> Call .AddSaveToMongoAsyncPipeline(**collection**, **unique-id-filed**);

This pipeline is used to do **UpSert** data on MongoDB. The **collection** parameter define the collection name to save and **unique-id-field** define the primary key of document to choose if the operation will be Insert or Update

### Safe Urls Pipeline
> Call AddSafeUrlsPipeline(result, prefixUrl, fieldArgs[])

This pipeline is used to add a http prefix to any scraped link data because sometimes the href attributes says only the path and hide the schema://host:port like:

```C#
"href="image.jpg" => "http://user-domain.com/image.jpg"
```

### Youtube Detail Metadata Extractor 
> Call .AddYoutubeDetailPipeline() 

This pipeline is used to extract information of metadata from youtube video url. The attributes like: Id, Author, UploadDate, Title, Description, Thumbnails, Duration, Keywords, Statistics, StreamMediaInfo and ClosedCaptionInfo can be extract from youtube video url.

* Retrieves information about videos, playlists, channels, media streams and closed caption tracks
* Handles all types of videos, including legacy, signed, restricted, non-embeddable and unlisted videos
* Works with media streams of all types -- muxed, embedded adaptive, dash adaptive
* Parses and downloads closed caption tracks
* Provides static methods to validate IDs and to parse IDs from URLs

__You must specify the field name that will receive the metadata information__

### Custom Pipeline
> Call .AddPipeline(**Action**)

This pipeline is used to do any custom steps like:
```C#
spider.AddPipeline(result =>
{
    result.scraped = DateTime.Now;
}
```
## Getting started 

1 Start a new **console** project and add Nuget Reference
2. PM> ` Install-Package SpiderSharp `
3. Define a spider class inherit from **SpiderEngine, ISpiderEngine**
4. Set the first url on constructor
5. Set the link for next page on override FollowPage method
6. Implement whats you want to scrap on override OnRun method

> TIP: you can use **this.node** to view the html object content

```c#
public class ScrapQuotesSpider : SpiderEngine, ISpiderEngine
    {
        public ScrapQuotesSpider()
        {
            this.SetUrl("http://quotes.toscrape.com");
        }

        protected override string FollowPage()
        {
            return $"http://quotes.toscrape.com{this.node.GetHref("ul > li > a")}";
        }

        protected override IEnumerable<dynamic> OnRun()
        {
            dynamic json = new ExpandoObject();

            var quotes = this.node.SelectNodes("div.quote");
            foreach (var item in quotes)
            {
                json.text = item.GetInnerText("span.text");
                json.author = item.GetInnerText("small.author");
                json.tags = item.SelectInnerText("div.tags > a.tag");

                yield return json;
            }
        }
    }

```

## To run the spider

1. Create a instance of spider
2. Choose almost one pipeline to return
3. Run the spider

```c#
ScrapQuotesSpider spider = new ScrapQuotesSpider();
spider.AddPrintToConsolePipeline();
spider.Run();
```

## Sample

The project include a simple quotes scrap of http://quotes.toscrape.com

![Sample running](https://github.com/jefersonsv/SpiderSharp/raw/master/sample-running.gif)

## Scrap Shell

This application can be used to inspect and debug a spider

## Thanks to

* [AngleSharp](https://github.com/AngleSharp/AngleSharp) - The ultimate angle brackets parser library parsing HTML5, MathML, SVG and CSS to construct a DOM based on the official W3C specifications
* [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) - Html Agility Pack (HAP)
* [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) - The ultimate dirty YouTube library