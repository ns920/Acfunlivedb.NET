using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acfunlivedb.NET.DAL.Entity
{
    [Index(nameof(uid))]
    [Index(nameof(startTime))]
    public class Live
    {
        /// <summary>
        /// 直播Id
        /// </summary>
        [Key]
        public string liveId { get; set; }

        /// <summary>
        /// up主uid
        /// </summary>
        public int uid { get; set; }

        /// <summary>
        /// up昵称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 获取录播用的剪辑id
        /// </summary>
        public string streamName { get; set; }

        /// <summary>
        /// 直播标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 直播开始时间
        /// </summary>
        public long startTime { get; set; }
    }
}