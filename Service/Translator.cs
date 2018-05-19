using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WishHand.Service
{
   
    public class Translator
    {
        private  string key = "AIzaSyDcoyfhaVxkCPwnCjwT-QI_l3B7lJ3mFR4";

        private  string baseUrl = "https://translation.googleapis.com/language/translate/v2?key=";



        public  async Task<IList<string>> Translateasync(string source, string code, IList<string> text)
        {
     
          IList<KeyValuePair<string, string>>  keyValuePair =  new List<KeyValuePair<string, string>>();

            keyValuePair.Add(new KeyValuePair<string, string>("source", source));
            keyValuePair.Add(new KeyValuePair<string, string>("target", code));
            keyValuePair.Add(new KeyValuePair<string, string>("format", "text"));


            foreach (var item in text)
            {
                keyValuePair.Add(new KeyValuePair<string, string>("q",item));
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (var item in keyValuePair)
            {
                stringBuilder.Append($"\"{item.Key}\":\"{item.Value}\",");
            }
            stringBuilder.Append("}");



            return SerializaResponse( await   DoPostExcuteAsync(stringBuilder.ToString()));

        }


        private IList<string> SerializaResponse(string str) {
            JToken jToken = JToken.Parse(str);
            JToken data = jToken.SelectToken("data");
            IList<string> list = new List<string>();
            if (data!=null)
            {
              JToken  arr=  data.SelectToken("translations");
                if (arr!=null)
                {
                   var result= arr.ToObject<IList<TranslateResponse>>();
                    foreach (var item in result)
                    {
                        list.Add(item.translatedText);
                    }
                }
            }
            
            return list;
        }
        private  async Task<string> DoPostExcuteAsync(string  postBody)
        {
            try
            {
                HttpWebRequest httpWebRequest = WebRequest.CreateHttp(baseUrl+ key);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json;charset=UTF-8";
                string str = "";


                Console.WriteLine(baseUrl + key);
                Console.WriteLine(postBody);

                var requestAsy = await httpWebRequest.GetRequestStreamAsync();

                using (Stream requestStream = requestAsy)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(postBody);
                    await requestStream.WriteAsync(bytes, 0, bytes.Length);
                    requestStream.Close();
                }

                var response =await httpWebRequest.GetResponseAsync();
                using (StreamReader streamReader = new StreamReader((response as HttpWebResponse).GetResponseStream(), Encoding.UTF8))
                {
                    str = await streamReader.ReadToEndAsync();
                    streamReader.Close();
                }
                return str;
            }

            catch (Exception ex1)
            {
                throw ex1;
            }


        }

        public class TranslateResponse
        {
            public string translatedText { get; set; }
        }
    }
}