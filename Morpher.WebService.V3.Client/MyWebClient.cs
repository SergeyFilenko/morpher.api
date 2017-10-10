﻿using System;
using System.IO;
using System.Net;
using System.Text;

namespace Morpher.WebService.V3
{
    using System.Collections.Specialized;
    using Newtonsoft.Json;

    public class MyWebClient : IDisposable
    {
        readonly string _baseUrl;
        IWebClient _webClient;

        public MyWebClient(Guid? token, string baseUrl)
        {
            _baseUrl = baseUrl;
            if (token != null)
            {
                AddParam("token", token.ToString());
            }

            AddParam("format", "json");

        }

        public IWebClient WebClient
        {
            get { return _webClient ?? (_webClient = new MorpherWebClient() {Encoding = Encoding.UTF8}); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _webClient = value;
            }
        }

        public void AddParam(string name, string value)
        {
            WebClient.QueryString.Add(name, value);
        }

        public T GetObject<T>(string relativeUrl)
        {
            try
            {
                string response = WebClient.DownloadString(_baseUrl + relativeUrl);
                return Deserialize<T>(response);
            }
            catch (WebException exc)
            {
                throw HandleException(exc);
            }
        }

        public void UploadValues(string relativeUrl, NameValueCollection collection)
        {
            try
            {
                WebClient.UploadValues(_baseUrl + relativeUrl, collection);
            }
            catch (WebException exc)
            {
                throw HandleException(exc);
            }
        }

        public T DeleteRequest<T>(string relativeUrl)
        {
            try
            {
                var response = WebClient.UploadValues(_baseUrl + relativeUrl, "DELETE", new NameValueCollection());
                return Deserialize<T>(response);
            }
            catch (WebException exc)
            {
                throw HandleException(exc);
            }
        }

        static T Deserialize<T>(byte[] response)
        {
            using (MemoryStream memoryStream = new MemoryStream(response))
            using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                var serializer = new JsonSerializer();
                return (T)serializer.Deserialize(reader, typeof(T));
            }
        }

        static T Deserialize<T>(string response)
        {
            return Deserialize<T>(Encoding.UTF8.GetBytes(response));
        }

        static string GetResponseText(WebException exception)
        {
            Stream responseStream = exception.Response?.GetResponseStream();

            if (responseStream == null) return null;

            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        private MorpherWebServiceException HandleException(WebException exc)
        {
            var httpWebResponse = exc.Response as HttpWebResponse;
            if (httpWebResponse != null
                &&
                httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
                throw exc;

            string response = GetResponseText(exc);
            if (response == null) throw exc;
            var error = Deserialize<ServiceErrorMessage>(response);
            return new MorpherWebServiceException(error.Message, error.Code);
        }
 
        public void Dispose()
        {
            WebClient.Dispose();
        }
    }
}
