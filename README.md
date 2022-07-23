# CareerAppDeleter
CareerAppDeleterは[竹林人間](https://github.com/Aoi-Developer/)さんの[remove-career-app](https://github.com/Aoi-Developer/remove-career-app)の機能をWindowsのGUIで独自実装し機能を追加したものです。
## 対応環境
Windows8以上の.NET Framework 4.7.2がインストールされている環境。

ARM系CPUでの動作確認はしておりません。
### テスト環境
|項目|値|
|----|--|
|OS|Windows 10|
|CPU|Ryzen 5 1600|
|RAM|DDR4 24GB|
|SSD|500 GB|
|Android端末1|Redmi Note 9T|
|Android端末2|Xperia 8|
## 注意事項
指定したアプリのデータがすべて削除されます。

アプリのデータが削除されることによりキャリアからのサポートを受けることができなくなる可能性があります。

一度削除したアプリはバックアップソフトでバックアップを取った場合をのぞき復元することができません。

削除するアプリをよく確認し実行してください。

なお、本ソフトで発生した故障や損害につきましては開発者は責任を負いかねます。

あくまで **自己責任** でのご使用をお願いします。

## 実行方法
### 導入
まず、[こちら](https://github.com/binary-number/CarrierAppDeleter/releases)のリンクにアクセスしCarrierAppDeleter.zipの最新版をダウンロードします。

ダウンロードしたCarrierAppDeleter.zipを解凍してください。

### 実行
キャリアアプリを削除したいAndroid端末のUSBデバッグを有効にし本ソフトを導入したパソコンに接続します。

接続はファイル転送モードにします。


フォルダ内にあるCarrierAppDeleter.exeファイルを実行してください。

本ソフトは署名がないため初回起動時には以下のような画面が表示されますが、詳細を押すと実行というボタンが表示されるのでそちらを押して実行してください。


![署名なし](https://github.com/binary-number/CarrierAppDeleter/blob/master/REAEME_Image/Image1.png)


初回起動時はplatform-toolsの導入のため5秒ほど時間がかかります。

### 操作
起動すると以下のは画面表示されます。


![実行画面](https://github.com/binary-number/CarrierAppDeleter/blob/master/REAEME_Image/Image3.png)

#### デバイスの選択
まず接続しているデバイスの検索を行う必要があります。

(接続している端末が1台の場合はデバイスの選択を行う必要はありません)

デバイスと書いてある横に __検索__ と書かれたボタンがあります。

こちらのボタンを押すことで現在接続しているAndroid端末を検索することができます。

検索が終了するとデバイスが下のComboBox(ドロップダウンリスト)に追加されますので、任意の端末を選択してください。

#### 削除するアプリの選択
アプリと書かれた横にある __検索__ と書かれたボタンを押すことで選択されたデバイスのキャリアアプリを検索します。

今回キャリアアプリの定義としては以下の二点とします。

1. システムアプリに設定されている
2. package名にsoftbank,kddi,auone,docomo,ntt,rakuten,yahooのいずれかが含まれる


アプリの検索が終了するとアプリの文字の下にpackage名の書かれたチェックボックスのリストが表示されます。

![実行画面](https://github.com/binary-number/CarrierAppDeleter/blob/master/REAEME_Image/Image4.png)


初期状態はすべて選択されていますので、削除したくないアプリがありましたら、チェックボックスのチェックを外し除外してください。



また、Wi-Fiアプリ・災害系のアプリを除外するのボタンを押すことで該当のアプリの選択を解除することができます。
なお、2022年7月23日時点でのWi-Fiアプリ・災害系のアプリですので今後変更された場合には除外されない可能性がありますので、あくまで __目安__ としてご使用ください。

#### アプリの削除
削除を実行ボタンを押すことでチェックボックスのチェックのついているアプリが削除されます。

削除する前にもう一度削除して良いかダイアログが表示されますので確認の上実行してください。

アプリが削除が始まるとログが表示され、すべて削除されると削除された旨を伝えるダイアログが表示されます。


## Build方法
Visual Studioで.slnファイルを開きBuildしてください。

## 今後導入したい機能(不可能な可能性あり)

- APKバックアップ機能
- チェックボックスの表示をpackage名からアプリ名への変更
