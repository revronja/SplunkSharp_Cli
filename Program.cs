using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Test1
{
	public class MainClass
	{
		
		public static string token = null;
		public static string sId = null;
		public static void Main(string[] args)
		{
			if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter a search string argument.");
                System.Console.WriteLine("Usage: Search <string>");
                
            }

			ServicePointManager.ServerCertificateValidationCallback =
(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true;
                     
            

			Stream x = login(Config.user, Config.password).Result;
			Decoder(x);
			Console.WriteLine(token);
			var a = Job().Result;
			Decoder2(a);
			Console.WriteLine(sId);
			//var b = JobInfo().Result;
			//Console.WriteLine(b);
			var c = ResultsPreview().Result;
			Console.WriteLine(c);

		}

		[XmlType("response")]
        public class tokenClass1
        {
            [XmlElement("sessionKey")]
            public string sessionKey { get; set; }
        }

        // sid

        [XmlType("response")]
        public class Sid1
        {
            [XmlElement("sid")]
            public string sid { get; set; }
        }

		public static async Task<Stream> login(string username, string password)
		{
			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://10.0.0.62:8089/services/auth/login");
				client.DefaultRequestHeaders.Add("Authorization", "Basic");
				var postData = new List<KeyValuePair<string, string>>();
				postData.Add(new KeyValuePair<string, string>("username", username));
				postData.Add(new KeyValuePair<string, string>("password", password));

				HttpContent body = new FormUrlEncodedContent(postData);
				HttpResponseMessage response = await client.PostAsync(client.BaseAddress, body);
				HttpContent content = response.Content;
                
				var x = await content.ReadAsStreamAsync();
                
				// lambda here
				string result = await content.ReadAsStringAsync();
				return x;
				//return x;
			}
		}
        // search passed as string arg
		public static async Task<Stream> Job()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://10.0.0.62:8089/services/search/jobs");
				client.DefaultRequestHeaders.Add("Authorization", String.Format("Splunk {0}", token));
                var postData = new List<KeyValuePair<string, string>>();
                //postData.Add(new KeyValuePair<string, string>("username", username));
                //postData.Add(new KeyValuePair<string, string>("password", password));
				postData.Add(new KeyValuePair<string, string>("search", "search *"));

                HttpContent body = new FormUrlEncodedContent(postData);
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, body);
                HttpContent content = response.Content;

                var x = await content.ReadAsStreamAsync();

                // lambda here
                string result = await content.ReadAsStringAsync();
                return x;
                //return result;
            }
        }
		public static async Task<string> JobInfo()
        {
            using (HttpClient client = new HttpClient())
            {
				client.BaseAddress = new Uri(string.Format("https://10.0.0.62:8089/services/search/jobs/{0}",sId));
                
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Splunk {0}", token));
                
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                HttpContent content = response.Content;

                var x = await content.ReadAsStreamAsync();

                // lambda here
                string result = await content.ReadAsStringAsync();
                //return x;
                return result;
            }
        }

		public static async Task<string> ResultsPreview()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(string.Format("https://10.0.0.62:8089/services/search/jobs/{0}/results_preview", sId));

                client.DefaultRequestHeaders.Add("Authorization", String.Format("Splunk {0}", token));

                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                HttpContent content = response.Content;

                var x = await content.ReadAsStreamAsync();

                // lambda here
                string result = await content.ReadAsStringAsync();
                //return x;
                return result;
            }
        }

		public static void Decoder(Stream stream){
			XmlSerializer serializer = new
       XmlSerializer(typeof(tokenClass1));
			tokenClass1 i;
			i = (tokenClass1)serializer.Deserialize(stream);
			token = i.sessionKey;
		}
		public static void Decoder2(Stream stream)
        {
            XmlSerializer serializer = new
       XmlSerializer(typeof(Sid1));
            Sid1 i;
            i = (Sid1)serializer.Deserialize(stream);
			sId = i.sid;
        }
        
	}
}
