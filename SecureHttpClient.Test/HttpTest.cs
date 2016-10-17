﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace SecureHttpClient.Test
{
    public class HttpTest
    {
        [Fact]
        public async Task HttpTest_Get()
        {
            const string page = @"https://httpbin.org/get";
            var result = await GetPageAsync(page).ConfigureAwait(false);
            var json = JToken.Parse(result);
            var url = json["url"].ToString();
            Assert.Equal(page, url);
        }

        [Fact]
        public async Task HttpTest_Gzip()
        {
            const string page = @"https://httpbin.org/gzip";
            var result = await GetPageAsync(page).ConfigureAwait(false);
            var json = JToken.Parse(result);
            var url = json["gzipped"].ToString();
            Assert.Equal("True", url);
        }

        /*
        [Fact]
        public async Task HttpTest_Deflate()
        {
            const string page = @"https://httpbin.org/deflate";
            var result = await GetPageAsync(page).ConfigureAwait(false);
            var json = JToken.Parse(result);
            var url = json["deflated"].ToString();
            Assert.Equal("True", url);
        }
        */

        [Fact]
        public async Task HttpTest_Utf8()
        {
            const string page = @"https://httpbin.org/encoding/utf8";
            var result = await GetPageAsync(page).ConfigureAwait(false);
            Assert.Contains("∮ E⋅da = Q,  n → ∞, ∑ f(i) = ∏ g(i)", result);
        }

        [Fact]
        public async Task HttpTest_Redirect()
        {
            const string page = @"https://httpbin.org/redirect/5";
            var result = await GetPageAsync(page).ConfigureAwait(false);
            var json = JToken.Parse(result);
            var url = json["url"].ToString();
            Assert.Equal(@"https://httpbin.org/get", url);
        }

        [Fact]
        public async Task HttpTest_Delay()
        {
            const string page = @"https://httpbin.org/delay/5";
            var result = await GetPageAsync(page).ConfigureAwait(false);
            var json = JToken.Parse(result);
            var url = json["url"].ToString();
            Assert.Equal(page, url);
        }

        [Fact]
        public async Task HttpTest_Stream()
        {
            const string page = @"https://httpbin.org/stream/50";
            var result = await GetPageAsync(page).ConfigureAwait(false);
            var nbLines = result.Split('\n').Length - 1;
            Assert.Equal(50, nbLines);
        }

        [Fact]
        public async Task HttpTest_Bytes()
        {
            const string page = @"https://httpbin.org/bytes/1024";
            var result = await GetBytesAsync(page).ConfigureAwait(false);
            Assert.Equal(1024, result.Length);
        }

        [Fact]
        public async Task HttpTest_StreamBytes()
        {
            const string page = @"https://httpbin.org/stream-bytes/1024";
            var result = await GetBytesAsync(page).ConfigureAwait(false);
            Assert.Equal(1024, result.Length);
        }

        [Fact]
        public async Task HttpTest_SetCookie()
        {
            const string page = @"https://httpbin.org/cookies/set?k1=v1";
            var result = await GetPageAsync(page).ConfigureAwait(false);
            var json = JToken.Parse(result);
            var cookies = json["cookies"].ToObject<Dictionary<string, string>>();
            Assert.Contains(new KeyValuePair<string, string>("k1", "v1"), cookies);
        }

        private static async Task<string> GetPageAsync(string page)
        {
            string result;
            var secureHttpClientHandler = new SecureHttpClientHandler();
            using (var httpClient = new HttpClient(secureHttpClientHandler))
            using (var response = await httpClient.GetAsync(page).ConfigureAwait(false))
            {
                result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return result;
        }

        private static async Task<byte[]> GetBytesAsync(string page)
        {
            byte[] result;
            var secureHttpClientHandler = new SecureHttpClientHandler();
            using (var httpClient = new HttpClient(secureHttpClientHandler))
            using (var response = await httpClient.GetAsync(page).ConfigureAwait(false))
            {
                result = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            return result;
        }
    }
}
