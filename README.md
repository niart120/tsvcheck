# tsvcheckGUI
 
## なにこれ
第七世代(SM/USUM)におけるTSV特定ツールのGUI版です.

ろいしん氏の[【USM対応】トレーナーIDからTSVを求めるツール](https://blastoise-x.hatenablog.com/entry/TID-to-TSV)が公開停止となっているため, 同等の機能を有するツールを独自に作成しました.

トレーナーカードに表示されているID(g7tid), 御三家の個体値及び性格からTSV, TRV(おまけでTID, SID etc...)を特定します.

32bit全探索する兼ね合いで長時間PCを占有することになります. ご容赦ください.

(参考:Ryzen5 5600Xで最大約30分程度掛かります)

TSV特定にあたって,
- ゲームを最初から始めたときにトレーナーの名前入力確認画面でいいえを選択していない
- ゲーム開始からリセットせずに御三家を受け取っている
必要があります.

## 使い方
各項目を入力後`Search`ボタンをクリックすると検索を開始します. 

### G7TID
トレーナーカードに記載されている6桁のIDを入力してください.

### ROM
TSVを特定したいROMのバージョンを選択してください.

### 個体値
TSVを特定したいROMで受け取った御三家の個体値を特定してツールに入力してください.

### 性格
TSVを特定したいROMで受け取った御三家の性格を選択してください.

### 検索範囲
御三家を受け取った際の, 初期seedからの消費数の下限と上限です.

注:検索時間の大半は32bit全探索の部分が大半を占めているため, 検索範囲を減らしても全体の計算時間に及ぼす影響は極めて小さいです.


## 使用ライブラリ
- [PokemonStandardLibrary](https://github.com/yatsuna827/PokemonStandardLibrary)
- [PokemonPRNG](https://github.com/yatsuna827/PokemonPRNG)

## おわりに
いくつかのテストケースに関して確認をしておりますが, 実行結果に関する保障は致しかねます.

不具合, バグなどの申し立てが御座いましたらニアト(https://twitter.com/21i10r29) までご連絡願います.