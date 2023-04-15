# Acfunlivedb.NET
[https://github.com/orzogc/acfunlivedb]acfunlivedb 的.NET实现
用于保存Acfun直播数据

## 配置
第一次运行会自动生成appsettings.json配置文件

```

{
  "ConnectionStrings": {
    "sqlite": "Data Source=./acfunlive.db;"//数据库地址
  },
  "Queryinterval": 75000 //查询间隔，默认为75秒

}

```  