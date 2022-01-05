# Tool-Monitor

後継は[こちら](https://github.com/usausa/munin-node-alternative)？

## これはなんぞ？

* Windowsスタンドアロンで使用できるRRDToolを使った監視ツールです
* DB等の設定は不要でXCopyで使えます(でも公開用には別途Webサーバとかは必要)

## なにができるん？

* パフォーマンスカウンタの値を出力できます
* ディスク容量とプロセス/スレッド数は専用のプロバイダがあります
* Open Hardware Monitorのライブラリを使った、CPUのクロック数、温度やマザーボードのFAN回転数、HDDの温度なんかも出力できます
* redis-client infoによるRedisの情報を出力できます
