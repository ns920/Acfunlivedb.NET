﻿using Acfunlivedb.NET.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acfunlivedb.NET.DAL.Mapper
{
    public static class LiveMapper
    {
        public static Live LivelistToLive(Livelist livelist)
        {
            if (!int.TryParse(livelist.href, out int uid))
            {
                uid = 0;
            }
            return new Live()
            {
                liveId = livelist.liveId,
                name = livelist.user.name,
                startTime = livelist.createTime.Value,
                streamName = livelist.streamName,
                title = livelist.title,
                uid = uid
            };
        }
    }
}