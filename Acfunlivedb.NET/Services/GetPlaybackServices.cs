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
using Acfunlivedb.NET.DAL;
using Microsoft.EntityFrameworkCore;

namespace Acfunlivedb.NET.Services
{
    public class GetPlaybackServices
    {
        private Request _request { get; set; }
        private AcfunlivedbDbContext _dbContext { get; set; }

        public GetPlaybackServices(Request request, AcfunlivedbDbContext dbContext)
        {
            _request = request;
            _dbContext = dbContext;
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

                //爱咔剪辑
                string uid = _dbContext.Lives.Where(x => x.liveId == liveId).First().uid.ToString();
                string aikaUrl = $"https://live.acfun.cn/rest/pc-direct/live/getLiveCutInfo?authorId={uid}&liveId={liveId}";

                var responseAika = await _request.GetNoSignNoLogin(aikaUrl);
                var resultAika = JsonConvert.DeserializeObject<JToken>(responseAika);
                if (resultAika["liveCutStatus"].ToString() == "1" && resultAika["liveCutUrl"] != null)
                {
                    resultList.Add($"爱咔链接：{resultAika["liveCutUrl"]}");
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