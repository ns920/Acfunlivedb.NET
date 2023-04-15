using AcfunApi;
using Acfunlivedb.NET.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Acfunlivedb.NET.DAL.Entity;

namespace Acfunlivedb.NET.Services
{
    public class GetPlaybackServices
    {
        private Request _request { get; set; }

        public GetPlaybackServices(Request request)
        {
            _request = request;
        }

        public async Task<List<string>> Get(string liveId)
        {
            try
            {
                var form = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("liveId", liveId) };
                var requesturl = "https://api.kuaishouzt.com/rest/zt/live/playBack/startPlay?subBiz=mainApp&kpn=ACFUN_APP&kpf=PC_WEB";
                var response = await _request.Post(requesturl, form);
                var result = JsonConvert.DeserializeObject<JToken>(response)?["result"]?.ToString();
                if (result is null || result != "1")
                {
                    Console.WriteLine("接口调用失败");
                    return new List<string>();
                }
                List<string> resultList = new();
                var responseObject = JsonConvert.DeserializeObject<GetPlaybackResponse>(response);
                var adaptiveManifest = JsonConvert.DeserializeObject<AdaptiveManifest>(responseObject.data.adaptiveManifest);

                foreach (var adaption in adaptiveManifest.adaptationSet)
                {
                    foreach (var singleRepresentation in adaption.representation)
                    {
                        resultList.Add($"录播链接：{singleRepresentation.url}");
                        foreach (var singleBackupUrl in singleRepresentation.backupUrl)
                        {
                            resultList.Add($"备用录播链接：{singleBackupUrl}");
                        }
                    }
                }
                return resultList;
            }
            catch (Exception)
            {
                Console.WriteLine("接口调用失败");
                return new List<string>();
            }
        }
    }
}