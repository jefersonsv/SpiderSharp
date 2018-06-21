# SpiderSharp
Web Crawling and Scraping Framework

## Features
* Cache web pages with Redis
* Export the results automatically to MongoDB

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
> Call SafeUrlsPipeline(result, prefixUrl, fieldArgs[])

This pipeline is used to add a http prefix to any scraped link data because sometimes the href attributes says only the path and hide the schema://host:port like:

```C#
"href="image.jpg" => "http://user-domain.com/image.jpg"
```

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

## Scrap Shell

This application can be used to inspect and debug a spider