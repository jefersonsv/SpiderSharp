# SpiderSharp

Web Crawling and Scraping Framework

# Content
<!-- TOC -->

- [SpiderSharp](#spidersharp)
- [Content](#content)
    - [Features](#features)
    - [Regular Expression Library](#regular-expression-library)
    - [Http Requester Providers](#http-requester-providers)
    - [Pipelines](#pipelines)
        - [Dasherize Pipeline](#dasherize-pipeline)
        - [Print to Console Pipeline](#print-to-console-pipeline)
        - [Safe Urls Pipeline](#safe-urls-pipeline)
        - [Youtube Detail Metadata Extractor](#youtube-detail-metadata-extractor)
        - [Save to MongoDB Pipeline](#save-to-mongodb-pipeline)
        - [Save to ElasticSearch](#save-to-elasticsearch)
        - [Run Html Decode](#run-html-decode)
    - [Getting started](#getting-started)
    - [To run the spider](#to-run-the-spider)
    - [Sample](#sample)
    - [Scrap Shell](#scrap-shell)
    - [Thanks to](#thanks-to)

<!-- /TOC -->

## Features

- Cache web pages with Redis
- Export the results automatically to MongoDB
- Manny http request providers
- Helper to ignore invalid ssl certificate
- Helper to rename json property name token
- Helper to get or delete N itens of collection
- Common regular expression to grab data
- Extract youtube video metadata

## Regular Expression Library
There are some **Regex** done to grab data

- Cookies: Can slice each part of cookies

## Http Requester Providers
Each provider has your own features. You can choose anyone to get a resource

- **HttpClient**: Provides a class for sending HTTP requests and receiving HTTP responses from a resource.
- **AngleSharp**: WebRequester for navigation like a browser.
- **WebClient**: Provides common methods for sending data to and receiving data from a resource.
- **CookieWebClient**: Extension of WebClient including Cookies of transaction.
- **BetterWebClient**: Extension of WebClient including session through with cookie container, GZip header, HTTP status code.
- **Chrome Headless**: Request pages interpreting javascript. _You must have Google Chrome instaled_

## Pipelines

Pipeline get a data and transform sending the result to next pipeline. Use can use how much pipelines you want.

### Dasherize Pipeline
> Call .RunDasherizePipeline()

This pipeline is used to normalize an object transforming the json key fields like:

```C#
"some_title" => "some-title"
"someTitle" => "some-title"
```

### Print to Console Pipeline

> Call .RunPrintToConsolePipeline(**fields**)

This pipeline is used to print some scraped data to stdout. The **fields** parameter is optional

### Safe Urls Pipeline

> Call .RunSafeUrlsPipeline(result, prefixUrl, fieldArgs[])

This pipeline is used to add a http prefix to any scraped link data because sometimes the href attributes says only the path and hide the schema://host:port like:

```C#
"href="image.jpg" => "http://user-domain.com/image.jpg"
```

### Youtube Detail Metadata Extractor

> Call .RunYoutubeDetailPipeline()

This pipeline is used to extract information of metadata from youtube video url. The attributes like: Id, Author, UploadDate, Title, Description, Thumbnails, Duration, Keywords, Statistics, StreamMediaInfo and ClosedCaptionInfo can be extract from youtube video url.

- Retrieves information about videos, playlists, channels, media streams and closed caption tracks
- Handles all types of videos, including legacy, signed, restricted, non-embeddable and unlisted videos
- Works with media streams of all types -- muxed, embedded adaptive, dash adaptive
- Parses and downloads closed caption tracks
- Provides static methods to validate IDs and to parse IDs from URLs

__You must specify the field name that will receive the metadata information__

### Save to MongoDB Pipeline

> Call .RunSaveToMongoAsyncPipeline(**collection**, **unique-id-filed**);

This pipeline is used to do **UpSert** data on MongoDB. The **collection** parameter define the collection name to save and **unique-id-field** define the primary key of document to choose if the operation will be Insert or Update

### Save to ElasticSearch

> Call .RunSaveToElasticSearchPipeline(**type**, **unique-id-filed**);

This pipeline is used to **Index** data on ElasticSearch. The **type** parameter define the type name to save and **unique-id-field** define the primary key of document.

The field **unique-id-field** is optional, its will be generated if not specified.

### Run Html Decode

> Call .RunHtmlDecode();

This pipeline converts a string that has been HTML-encoded for HTTP transmission into a decoded string

## Getting started 

1. Start a new **console** project and add Nuget Reference
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
                yield return this.Fetch(() => {
                    ct.Data.text = item.GetInnerText("span.text");
                    ct.Data.author = item.GetInnerText("small.author");
                    ct.Data.tags = item.SelectInnerText("div.tags > a.tag");
                });
            }
        }

		protected override void SuccessPipeline(SpiderContext context)
        {
            context.RunPrintToConsolePipeline();
        }

        protected override void ErrorPipeline(SpiderContext context)
        {
            context.RunEmbedMetadata();
            context.RunPrintToConsolePipeline();
        }
    }

```

## To run the spider

1. Create a instance of spider
2. Choose almost one pipeline to return
3. Run the spider

```c#
ScrapQuotesSpider spider = new ScrapQuotesSpider();
spider.Run();
```

## Sample

The project include a simple quotes scrap of http://quotes.toscrape.com

![Sample running](https://github.com/jefersonsv/SpiderSharp/raw/master/sample-running.gif)

## Scrap Shell

This application can be used to inspect and debug a spider with the bellow features

* set => set httpclient or anglesharp driver to request
* load => load url or local file
* save => save content to local file
* innertext => select innertext using css selector
* attribute => select attribute value using css selector
* innerhtml => select innerhtml using css selector
* outerhtml => select outerhtml using css selector
* links => select links using css selector
* json => select links using css selector
* path => select links using css selector and print json path
* cls => clear screen
* browse => open browser with content
* notepad => open notepad with content
* help => show all commands
* quit => exit program

### How to use Scrap Shell

1. Use the __set__ command to choose what kind of driver to request
2. Load any url or local file with a page content
3. If you want use the command __browse__ to see the page content inside the browser
4. Try to get data with any css selector with the commands: __innertext__, __attribute__, __innerhtml__, __outerhtml__, __links__, __json__ or __path__
5. You will see the result on console

** You can use the command __notead__ to see the result of last css selector command on text editor or use the command __save__ to write content to a file **

## Thanks to

- [AngleSharp](https://github.com/AngleSharp/AngleSharp) - The ultimate angle brackets parser library parsing HTML5, MathML, SVG and CSS to construct a DOM based on the official W3C specifications
- [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) - The ultimate dirty YouTube library