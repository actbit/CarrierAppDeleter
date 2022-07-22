# CareerAppDeleter
CareerAppDeleterは[竹林人間](https://github.com/Aoi-Developer/)さんの[remove-career-app](https://github.com/Aoi-Developer/remove-career-app)の機能をWindowsのGUIで実装し機能を追加したものです。
## 対応環境
Windows8以上の.NET Framework 4.7.2がインストールされている環境。
### テスト環境
|項目|値|
|----|--|
|OS|Windows 10|
|CPU|Ryzen 5 1600|
|RAM|DDR4 24GB|
|SSD|500 GB|

## 注意事項
指定したアプリのデータがすべて削除されます。
アプリのデータが削除されることによりキャリアからのサポートを受けることができなくなる可能性があります。
一度削除したアプリはバックアップソフトでバックアップを取った場合をのぞき復元することができません。削除するアプリをよく確認し実行してください。
なお、本ソフトで発生した故障や損害につきましては開発者は責任を負いかねます。

## 実行方法
まず、[こちら](https://github.com/binary-number/CarrierAppDeleter/releases)のリンクにアクセスしCarrierAppDeleter.zipの最新版をダウンロードします。
ダウンロードしたCarrierAppDeleter.zipを解凍し、フォルダ内にあるCarrierAppDeleter.exeファイルを実行してください。
本ソフトは署名がないため以下のような画面が表示されますが、詳細を押すと実行というボタンが表示されるのでそちらを押して実行してください。
![署名なし](https://github.com/binary-number/CarrierAppDeleter/REAEME_Image/Image1.ong)

## Build方法
Visual Studioで.slnファイルを開きBuildしてください。
