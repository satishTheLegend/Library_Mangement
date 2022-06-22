using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse
{
    //public class BooksJsonData
    //{
    //    public string author { get; set; }
    //    public string country { get; set; }
    //    public string imageLink { get; set; }
    //    public string language { get; set; }
    //    public string link { get; set; }
    //    public int pages { get; set; }
    //    public string Title { get; set; }
    //    public int year { get; set; }
    //}

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class PublishedDate
    {
        [JsonProperty("$date")]
        public DateTime Date { get; set; }
    }

    public class BooksJsonData
    {
        public string title { get; set; }
        public string isbn { get; set; }
        public int pageCount { get; set; }
        public PublishedDate publishedDate { get; set; }
        public string thumbnailUrl { get; set; }
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public string status { get; set; }
        public List<string> authors { get; set; }
        public List<string> categories { get; set; }

        public string FilePath { get; set; }
    }
}
