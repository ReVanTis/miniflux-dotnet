# miniflux-dotnet
.Net library and a sample CLI client for [Miniflux 2](https://miniflux.app/) ([Github](https://github.com/miniflux/v2)) API based on [API Reference](https://miniflux.app/docs/api.html)

## To use the CLI client:

build miniflux-dotnet.cli with dotnet:
```
dotnet build miniflux-dotnet.cli
```

or publish as single file executable:
```
dotnet publish miniflux-dotnet.cli -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
```

Modify [miniflux-dotnet.cli/sample.miniflux_cli_config.json](miniflux-dotnet.cli/sample.miniflux_cli_config.json) based on your server's setting.

```
{
  "URL": "https://miniflux.example.com:10443",
  "Token": "SECRET_TOKEN",
  "Proxy": "socks5://127.0.0.1:1984"
}
```

Copy it to your home directory:

Linux: "~/.miniflux_cli_config.json" or "$HOME/.miniflux_cli_config.json"

Windows: "X:\Users\\\<username>\\.miniflux_cli_config.json" or "%HOMEDRIVE%%HOMEPATH%\\.miniflux_cli_config.json"

Config file also can be specified with argument of '--config="path/to/config.json"'

Run with:
```
dotnet run miniflux-dotnet.cli
```
or directly with the published executable.
