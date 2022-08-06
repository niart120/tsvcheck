# tsvcheck
 
## なにこれ
第七世代(SM/USUM)におけるTSV特定ツールです.

ろいしん氏の[【USM対応】トレーナーIDからTSVを求めるツール](https://blastoise-x.hatenablog.com/entry/TID-to-TSV)が公開停止となっているため, 同等の機能を有するツールを独自に作成しました.

トレーナーカードに表示されているID(g7tid), 御三家の個体値及び性格からTSV, TRV(おまけでTID, SID etc...)を特定します.

32bit全探索する兼ね合いで長時間PCを占有することになります. ご容赦ください.

TSV特定にあたって,
- ゲームを最初から始めたときにトレーナーの名前入力確認画面でいいえを選択していない
- ゲーム開始からリセットせずに御三家を受け取っている
必要があります.

## 実行例
例えば, 使用ROMがサン(またはムーン), トレーナーIDが`259742`, 受け取った御三家の個体値が`H:5 A:24 B:29 C:20 D:31 S:24`, 性格が`いじっぱり`
の場合, 

`tsvcheck.exe`が入っているフォルダでコマンドプロンプト(またはWindows Terminal)を開いて次のコマンドを実行してください.

```
./tsvcheck.exe -t 259742 -i 5 24 29 20 31 24 -n いじっぱり
```

使用ROMがウルトラサン, ウルトラムーンの場合は, コマンドの末尾に`-u`をつけてください. 

具体的には, トレーナーIDが`747259`, 受け取った御三家の個体値が`H:25 A:17 B:23 C:14 D:14 S:11`, 性格が`なまいき`の場合, 

```
./tsvcheck.exe -t 747259 -i 25 17 23 14 14 11 -n なまいき -u
```
として実行してください.

暫くすると,

```
Calculating... (will take a few hours) 11/4096
TSV:3107, TRV:0, TID:16763, SID:33611, SEED:0xBADBAD, advance:17343
```
といった形で結果が出力されます.

## 使用ライブラリ
- [PokemonStandardLibrary](https://github.com/yatsuna827/PokemonStandardLibrary)
- [PokemonPRNG](https://github.com/yatsuna827/PokemonPRNG)
- [Command Line Parser](https://github.com/commandlineparser/commandline)

## おわりに
いくつかのテストケースに関して確認をしておりますが, 実行結果に関する保障は致しかねます.

不具合, バグなどの申し立てが御座いましたらニアト(https://twitter.com/21i10r29) までご連絡願います.