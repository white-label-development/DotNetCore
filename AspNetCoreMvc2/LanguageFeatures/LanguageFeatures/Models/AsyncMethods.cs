using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LanguageFeatures.Models
{
    public class AsyncMethods
    {
        //Before async and await
        public static Task<long?> GetPageLength()
        {
            HttpClient client = new HttpClient();
            var httpTask = client.GetAsync("http://apress.com");
            return httpTask.ContinueWith((Task<HttpResponseMessage> antecedent) => { return antecedent.Result.Content.Headers.ContentLength; });

            // the continuation is the mechanism by which you specify what you want to happen when the background task is complete. Note TWO returns
        }

        public static async Task<long?> GetPageLengthAsync()
        {
            HttpClient client = new HttpClient();
            var httpMessage = await client.GetAsync("http://apress.com"); //wait for the result of the Task that the GetAsync method returns, then continue
            return httpMessage.Content.Headers.ContentLength;
        }
    }
}
