# QRReader
QRコードをスマホやタブレットで読み込み、その内容を表示（アドレスでない場合は検索）できるアプリを目指しています。

## Dependency
- Unity 2018.3.8f1
- [Zxing.Net](https://github.com/zxing/zxing)

## Setup
Unityから開いてください。Task(async/await)を使用しているためプレイヤー設定のスクリプティングランタイムバージョンが.NET 4.x相当になっていない場合は変更してください。

## Usage
現在UnityEditor上で再生及び実機へビルド等して起動後、アプリはすぐに読み取りを開始します。画面内にQRコードを収めることで読み取りを行ってください。成功すれば画面下部の帯にある「Scanning...」が読み取り結果に応じて変化しますので、読み取ったものを開いたり検索するか、別のものを読み取りたい場合はキャンセルを選んでください。開いたり検索する方を選択した場合は、ブラウザーが立ち上がります。キャンセルしたりブラウザーから戻ってきた場合は再び読み取りを開始します。終了したい時は左上の「終了する」を選択してください。

## Details
このアプリはfuzzy様のサイトにあるQRコードリーダーの記事「[Unityでバーコード・QRコードリーダーをつくる](http://fuzzy0.hatenablog.com/entry/2018/07/10/234630)」を基にして制作しています。

上記サイト様のスクリプトから指定の時間だけSleepを入れる処理を避けたく思い、別スレッドを立ち上げて処理するところをTaskとasync/awaitに置き換えています。またそれに合わせ、基のスクリプトでは2つのスクリプトで行っていた処理を1つにまとめています。
Unity Edetor上、iOS上では正常な動作を確認済みです。

## Problem
現在Androidでこのアプリの実機テストを行った際、挙動が不安定になる現象が確認されています。処理が重い・落ちる、コードが読み取れないといった問題が起きています。
Android Device Monitorでログを確認してみたところ、次のエラー及び警告が見受けられました。

`Tag: Unity`
```
W/Unity(8806): Camera2: only LEGACY hardware level is supported.
W/UnityHardwareCa(8806): type=1400 audit(0.0:1871): avc: denied { read } for name="u:object_r:camera_prop:s0" dev="tmpfs" ino=230 scontext=u:r:untrusted_app:s0:c512,c768 tcontext=u:object_r:camera_prop:s0 tclass=file permissive=0
```
`Tag: libc`
```
E/libc(1597): Access denied finding property "persist.camera.dumpmetadata"
E/libc(1597): Access denied finding property "persist.camera.ltm.filteroff"
E/libc(1597): Access denied finding property "persist.camera.isp.regdump"
E/libc(1597): Access denied finding property "persist.camera.isp.offregdump"
E/libc(1597): Access denied finding property "persist.camera.eztune.enable"
E/libc(1597): Access denied finding property "persist.camera.mobicat"
E/libc(1597): Access denied finding property "persist.camera.isp.dump"
E/libc(1597): Access denied finding property "persist.camera.dumpmetadata"
E/libc(1597): Access denied finding property "camera.cpp.dumppreviewpayload"
```

このエラーのうち、libcタグのエラーは毎フレームレベルでエラーが呼び出されており、その影響で他のログがあっという間に流されてしまうためデバッグにも苦労している状況です。

このAndroidだけQRが読み取れないことがあるバグの解決法が見つけられず、対処に苦慮しています。どなたかよい方法をご存知でしたら、教えていただけると幸いです。

## References
- [Unityでバーコード・QRコードリーダーをつくる](http://fuzzy0.hatenablog.com/entry/2018/07/10/234630)
- [Taskを極めろ！async/await完全攻略](https://qiita.com/acple@github/items/8f63aacb13de9954c5da)
- [初心者のためのTask.Run(), async/awaitの使い方](https://qiita.com/Alupaca1363Inew/items/0126270bca99883605de)
- [async/awaitのしくみ](https://www.youtube.com/watch?v=sT5kwDEb3xY&t=379s)
