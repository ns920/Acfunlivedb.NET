using AcfunApi;
using Acfunlivedb.NET.DAL;
using Acfunlivedb.NET.DAL.Entity;
using Acfunlivedb.NET.DAL.Mapper;
using Acfunlivedb.NET.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Acfunlivedb.NET.Services
{
    public class GetLiveListService
    {
        private readonly AcfunlivedbDbContext _dbContext;
        private string APIURL = "https://live.acfun.cn/api/channel/list?count=100000&pcursor=0";
        private Request _request;

        public GetLiveListService(AcfunlivedbDbContext dbContext, Request request)
        {
            _dbContext = dbContext;
            _request = request;
        }

        public async Task AddNewLive()
        {
            try
            {
                var recentLiveList = GetLiveDataRecent();
                var origindataFromApiString = await _request.GetNoSign(APIURL);
                var isError = JsonConvert.DeserializeObject<JToken>(origindataFromApiString)?["isError"];
                if (isError is not null && (bool)isError == true)
                {
                    ConsoleLog.WriteLine("API获取列表失败");
                    return;
                }
                //实际获取失败会无法反序列化抛出异常
                var origindataFromApi = JsonConvert.DeserializeObject<OriginalLiveData>(origindataFromApiString);
                var liveListFromApi = origindataFromApi?.liveList.ToList().Select(LiveMapper.LivelistToLive).ToList();
                var newLiveList = liveListFromApi?.Where(x => !recentLiveList.Select(x => x.liveId).ToList().Contains(x.liveId)).ToList() ?? new List<Live>();
                _dbContext.Lives.AddRange(newLiveList);
                _dbContext.SaveChanges();
                ConsoleLog.WriteLine("API获取列表成功");
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteLine("API获取列表失败");
            }
        }

        private List<Live> GetLiveDataRecent()
        {
            //获取2天内的直播
            var timestamp = GetTimeStamp.GetMiliSecond(DateTime.Now.AddDays(-2));
            return _dbContext.Lives.Where(x => x.startTime > timestamp).ToList();
        }

        private async Task<string> RequestAPIAsync()
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(APIURL);
            return await response.Content.ReadAsStringAsync();
        }
    }
}