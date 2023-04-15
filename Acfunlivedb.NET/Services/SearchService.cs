using Acfunlivedb.NET.DAL;
using Acfunlivedb.NET.DAL.Entity;
using Acfunlivedb.NET.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Acfunlivedb.NET.Services
{
    public class SearchService
    {
        private readonly AcfunlivedbDbContext _dbContext;

        public SearchService(AcfunlivedbDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<string> GetLiveStringByName(string name)
        {
            return OutPut(GetLiveByName(name));
        }

        public List<string> GetLiveStringByNameDefault(string name)
        {
            return OutPut(GetLiveByNameDefault(name));
        }

        public List<string> GetLiveStringByUid(string uid)
        {
            if (int.TryParse(uid, out int uidint))
            {
                return OutPut(GetLiveByUid(uidint));
            }
            return new List<string>() { "异常：uid不为数字" };
        }

        public List<string> GetLiveStringByUidDefault(string uid)
        {
            if (int.TryParse(uid, out int uidint))
            {
                return OutPut(GetLiveByUidDefault(uidint));
            }
            return new List<string>() { "异常：uid不为数字" };
        }

        private IEnumerable<Live> GetLiveByName(string name)
        {
            return _dbContext.Lives.Where(x => x.name.Contains(name)).OrderByDescending(x => x.startTime);
        }

        private IEnumerable<Live> GetLiveByNameDefault(string name)
        {
            return GetLiveByName(name).Take(10);
        }

        private IEnumerable<Live> GetLiveByUid(int uid)
        {
            return _dbContext.Lives.Where(x => x.uid == uid).OrderByDescending(x => x.startTime);
        }

        private IEnumerable<Live> GetLiveByUidDefault(int uid)
        {
            return GetLiveByUid(uid).Take(10);
        }

        public List<string> OutPut(IEnumerable<Live> lives)
        {
            return lives.Select(x => { return $"直播时间：{GetTimeStamp.GetDateTimeByMiliSecond(x.startTime)}标题： {x.title} UP主：{x.name} 录播ID：{x.liveId}"; }).ToList();
        }
    }
}