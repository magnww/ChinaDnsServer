# ChinaDnsServer
根据gfwlist和ip地址，选择最佳DNS查询结果，在不破坏CDN加速的同时解决了DNS污染问题。

## 使用方法
需要配合代理使用

编辑appsettings.json，修改以下内容：
<pre><code>{
  "Forwarders": {
    "Protocol": "Udp",
    "NameServers": [ "202.106.0.20", "202.106.46.151" ], // 国内DNS服务器
    "Proxy": []
  },
  "ReliableForwarders": {
    "Protocol": "Tcp",
    "NameServers": [ "8.8.8.8", "8.8.4.4" ], // 国外DNS服务器
    "Proxy": [ // 代理配置，根据实际修改
      {
        "Type": "Socks5",
        "Address": "192.168.88.113",
        "Port": 1153
      },
      {
        "Type": "Socks5",
        "Address": "192.168.88.113",
        "Port": 1253
      }
    ]
  }
}</code></pre>

## 更新IPList.txt
<pre><code>curl 'http://ftp.apnic.net/apnic/stats/apnic/delegated-apnic-latest' | grep ipv4 | grep CN | awk -F\| '{ printf("%s/%d\n", $4, 32-log($5)/log(2)) }' > IPList.txt</code></pre>

## 更新GfwList.txt
下载替换：https://raw.githubusercontent.com/gfwlist/gfwlist/master/gfwlist.txt

## 关联项目
- [ChinaDNS](https://github.com/shadowsocks/ChinaDNS)
- [Technitium DNS Server](https://github.com/TechnitiumSoftware/DnsServer)
