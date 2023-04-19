# Acfunlivedb.NET
[acfunlivedb](https://github.com/orzogc/acfunlivedb) 的.NET实现
用于保存Acfun直播数据

## 依赖
[.NET Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
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
