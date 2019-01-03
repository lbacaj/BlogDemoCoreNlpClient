using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BlogDemoCoreNlpClient
{
    class Program
    {
        private const string CoreNlpServerUrl = "http://localhost:9000/";
        private const string coreNlpParams = "?properties={\"annotators\":\"tokenize,ssplit,pos,lemma,ner,depparse,coref,quote\",\"outputFormat\":\"json\",\"timeout\":300000}";


        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {

            try
            {
                //Read in the text from our sample.
                string textToAnnotate = File.ReadAllText(Directory.GetCurrentDirectory() + "\\SellingEnlightment.txt");

                // A simple REST call to the Core NLP Server
                var response = await PostCoreNLP(coreNlpParams, textToAnnotate);

                Console.WriteLine($"{response})");

                //Write out a summary file back to the current directory.
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\NlpSummary.txt", response, Encoding.UTF8);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static async Task<string> PostCoreNLP(string path, string data)
        {
            var result = String.Empty;
            var content = new StringContent(data);

            //we will give our client a nice long timeout since we know this could take a big to run, especially for a large data set.
            var client = new HttpClient()
            {
                Timeout = new TimeSpan(0, 5, 30),
                BaseAddress = new Uri(CoreNlpServerUrl)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync(path, content);

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }
    }
}
