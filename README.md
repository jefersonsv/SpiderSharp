# SpiderSharp

Web Crawling and Scraping Framework

# 2. Content
<!-- TOC -->

- [SpiderSharp](#spidersharp)
- [2. Content](#2-content)
    - [2.1. Features](#21-features)
    - [2.2. Regular Expression Library](#22-regular-expression-library)
    - [2.3. Http Requester Providers](#23-http-requester-providers)
    - [2.4. Pipelines](#24-pipelines)
        - [2.4.1. Dasherize Pipeline](#241-dasherize-pipeline)
        - [2.4.2. Print to Console Pipeline](#242-print-to-console-pipeline)
        - [2.4.3. Save to MongoDB Pipeline](#243-save-to-mongodb-pipeline)
        - [2.4.4. Safe Urls Pipeline](#244-safe-urls-pipeline)
        - [2.4.5. Youtube Detail Metadata Extractor](#245-youtube-detail-metadata-extractor)
        - [2.4.6. Custom Pipeline](#246-custom-pipeline)
		- [2.4.7. Save to ElasticSearch](#247-save-to-elasticsearch-pipeline)
    - [2.5. Getting started](#25-getting-started)
    - [2.6. To run the spider](#26-to-run-the-spider)
    - [2.7. Sample](#27-sample)
    - [2.8. Scrap Shell](#28-scrap-shell)
    - [2.9. Thanks to](#29-thanks-to)

<!-- /TOC -->

## 2.1. Features

- Cache web pages with Redis
- Export the results automatically to MongoDB
- Manny http request providers
- Helper to ignore invalid ssl certificate
- Helper to rename json property name token
- Helper to get or delete N itens of collection
- Common regular expression to grab data
- Extract youtube video metadata

## 2.2. Regular Expression Library
There are some **Regex** done to grab data

- Cookies: Can slice each part of cookies

## 2.3. Http Requester Providers
Each provider has your own features. You can choose anyone to get a resource

- **HttpClient**: Provides a class for sending HTTP requests and receiving HTTP responses from a resource.
- **AngleSharp**: WebRequester for navigation like a browser.
- **WebClient**: Provides common methods for sending data to and receiving data from a resource.
- **CookieWebClient**: Extension of WebClient including Cookies of transaction.
- **BetterWebClient**: Extension of WebClient including session through with cookie container, GZip header, HTTP status code.
- **Chrome Headless**: Request pages interpreting javascript. _You must have Google Chrome instaled_

## 2.4. Pipelines

Pipeline get a data and transform sending the result to next pipeline. Use can use how much pipelines you want.

### 2.4.1. Dasherize Pipeline 
> Call .RunDasherizePipeline() 

This pipeline is used to normalize an object transforming the json key fields like:

```C#
"some_title" => "some-title"
"someTitle" => "some-title"
```

### 2.4.2. Print to Console Pipeline

> Call .RunPrintToConsolePipeline(**fields**)

This pipeline is used to print some scraped data to stdout. The **fields** parameter is optional

### 2.4.3. Save to MongoDB Pipeline

> Call .RunSaveToMongoAsyncPipeline(**collection**, **unique-id-filed**);

This pipeline is used to do **UpSert** data on MongoDB. The **collection** parameter define the collection name to save and **unique-id-field** define the primary key of document to choose if the operation will be Insert or Update

### 2.4.4. Safe Urls Pipeline

> Call .RunSafeUrlsPipeline(result, prefixUrl, fieldArgs[])

This pipeline is used to add a http prefix to any scraped link data because sometimes the href attributes says only the path and hide the schema://host:port like:

```C#
"href="image.jpg" => "http://user-domain.com/image.jpg"
```

### 2.4.5. Youtube Detail Metadata Extractor

> Call .RunYoutubeDetailPipeline()

This pipeline is used to extract information of metadata from youtube video url. The attributes like: Id, Author, UploadDate, Title, Description, Thumbnails, Duration, Keywords, Statistics, StreamMediaInfo and ClosedCaptionInfo can be extract from youtube video url.

- Retrieves information about videos, playlists, channels, media streams and closed caption tracks
- Handles all types of videos, including legacy, signed, restricted, non-embeddable and unlisted videos
- Works with media streams of all types -- muxed, embedded adaptive, dash adaptive
- Parses and downloads closed caption tracks
- Provides static methods to validate IDs and to parse IDs from URLs

__You must specify the field name that will receive the metadata information__

### 2.4.6. Custom Pipeline

> Call .RunPipeline(**Action**)

This pipeline is used to do any custom steps like:

```C#
spider.RunPipeline(result =>
{
    result.scraped = DateTime.Now;
}
```

### 2.4.7. Save to ElasticSearch

> Call .RunSaveToElasticSearchPipeline(**type**, **unique-id-filed**);

This pipeline is used to **Index** data on ElasticSearch. The **type** parameter define the type name to save and **unique-id-field** define the primary key of document


## 2.5. Getting started 

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

## 2.6. To run the spider

1. Create a instance of spider
2. Choose almost one pipeline to return
3. Run the spider

```c#
ScrapQuotesSpider spider = new ScrapQuotesSpider();
spider.Run();
```

## 2.7. Sample

The project include a simple quotes scrap of http://quotes.toscrape.com

![Sample running](https://github.com/jefersonsv/SpiderSharp/raw/master/sample-running.gif)

## 2.8. Scrap Shell

This application can be used to inspect and debug a spider

## 2.9. Thanks to

- [AngleSharp](https://github.com/AngleSharp/AngleSharp) - The ultimate angle brackets parser library parsing HTML5, MathML, SVG and CSS to construct a DOM based on the official W3C specifications
- [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) - Html Agility Pack (HAP)
- [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) - The ultimate dirty YouTube library