# LineNotify登録試験用サーバ(LINREG)

## 参考情報源

| サイト | 参考内容 |
| --- | --- |
| [公式](https://notify-bot.line.me/doc/ja/) |  基本的な情報 |
| [実装の参考サイト](https://nomurabbit.hatenablog.jp/entry/20211207/1638803200) |  Lambda実装、遷移の実画面等が紹介されている(API1,API2) |
| [serverless+C# の参考サイト](https://medium.com/geekculture/get-started-with-aws-sam-and-asp-net-core-6d4eddddbb93) |  lambda実装以外にもAPIGatewayとかの設定が必要なのでserverlessを利用する。 donet lambda deploy-serverless　の情報 |

## 実際にデプロイするもの

以下を参考にする。デプロイはAPI1, API2が同時にアクセスされる

1. 注意 AWS Credential は　~/.aws/credentials ファイルに書き出す。（環境変数が貫通しなかった）
1. 注意 -rs True オプションで serverless用の標準S3バケットが生成される
1. 注意 -sn スタック名では基本自由
1. 注意 --template-parameters は Lineのコンソールから取得したclientid,clientsecretを代入する(クレデンシャル)

```bash
$ cd 20211129_my_api_line_notify_token_src/src/20211129_my_api_line_notify_token_src
$ dotnet lambda deploy-serverless -rs True -sn Yikes230131 --template-parameters 'LineClientId=arginjectid;LineClientSecret=arginjectsecret'
```

## ソース
| ソース | 目的 | URL例 |
| --- | --- | --- |
| [API1](20211129_my_api_line_notify_token_src/src/20211129_my_api_line_notify_token_src/Function.cs) | NotifyAPIのToken取得API１段目 | https://53gfzu4qnd.execute-api.ap-northeast-1.amazonaws.com/Prod/api1 |
| [API2](20211129_my_api_line_notify_token_src/src/20211129_my_api_line_notify_token_src/DSTFunction.cs) | NotifyAPIのToken取得API2段目(Tokenを得る) | https://53gfzu4qnd.execute-api.ap-northeast-1.amazonaws.com/Prod/api2 |


## メモ
### Lambda 実装 -> SAMへの移行方法
実体の実装は変わらず、基本殆ど代える必要はない
プロジェクトを初期化する際に、
```bash
dotnet new serverless.EmptyServerless --name 20211129_my_api_line_notify_token_src --force
```
を用いると大体うまくいく。
(20211129_my_api_line_notify_token_src)はフォルダ名



