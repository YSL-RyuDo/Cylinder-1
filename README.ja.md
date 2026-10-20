# Cylinder

[English](README.md), [Korean](README.ko.md), [Japanese](README.ja.md)

チーム制作のナラティブゲーム。分岐会話、インベントリ、人格UI、ポータルとパズルを担当しました。

| プロジェクト情報 | 内容 |
|---|---|
| 開発状況 | チーム開発での担当 |
| 使用技術 | C#, Unity, CSV |

[開発事例を見る](https://ysl-ryudo.github.io/ja/work/cylinder/), [Portfolio](https://ysl-ryudo.github.io/ja/)

## ゲーム全体で担当した実装

初期JSON会話ローダーに加え、YSLブランチでCSV会話、インベントリ、人格・メニューUI、ポータル・カメラ遷移、パズルを担当しました。移動・会話の共同ファイルには他メンバーの変更もあります。小さいmain版だけでなく、YSLブランチの担当を明示します。

**プレイの流れ**

探索 → 操作と会話 → 選択とフラグ変更 → アイテム取得・登録 → パズルと経路解放

ソースリンクは確認した版に固定し、担当リンクは自分のマージ以外の変更を示します。共同ファイルは単独著作を意味しません。非公開ソースには閲覧権限が必要です。

### 分岐会話とシステム命令

選択肢、条件、フラグ、システム命令を扱う会話ノード実行器を実装・拡張しました。CSV読込で内容と実行を分け、DialogueUIが文字送り・立ち絵・選択肢を表示します。

[DialogueManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueManager.cs), [担当変更 f2bade0](https://github.com/YSL-RyuDo/Cylinder-1/commit/f2bade0f41e38fe1d6b07cd38d9aff677152782d), [DialogueCSVLoader.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueCSVLoader.cs), [担当変更 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5)

### インベントリと登録アイテム

アイテム定義・取得・除去・登録・使用とインベントリUIを担当しました。登録アイテムをポータル解放などで使い、メニュー選択と進行をつなぎました。

[InventoryManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Item/InventoryManager.cs), [担当変更 6ed8ded](https://github.com/YSL-RyuDo/Cylinder-1/commit/6ed8ded8f321ed7da22453f874316442aa2da7d3), [InventoryUI.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/InventoryUI.cs), [担当変更 b21ce48](https://github.com/YSL-RyuDo/Cylinder-1/commit/b21ce4867f5cff4459ed22a8f97ef87d1c70e54c)

### ポータル、フェードとカメラ遷移

フェードとワープを連携するポータル移動、アイテム条件と即時の往復を防ぐ準備状態を実装しました。カメラ追従と外部制御の切替も担当し、部屋やパズルが一時的に視点を制御できます。

[PortalTrigger.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Portal/PortalTrigger.cs), [担当変更 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [CameraFollow.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Map/CameraFollow.cs), [担当変更 b5e01aa](https://github.com/YSL-RyuDo/Cylinder-1/commit/b5e01aa3498a702fa05e44b18782b3c70da0042a)

### 操作対象とウィンドウ制御

プレイヤー操作対象・案内表示とIUIWindowベースのメニュー制御を担当しました。メニューでゲーム時間を停止し、閉じると戻します。人格選択、独白、ナレーションも別の表示処理として実装しました。

[UIManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/UIManager.cs), [担当変更 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [PersonalityUI.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/UI/PersonalityUI/PersonalityUI.cs), [担当変更 89a3080](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5), [PlayerInteraction.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Player/PlayerInteraction.cs), [担当変更 1cb584e](https://github.com/YSL-RyuDo/Cylinder-1/commit/1cb584e0032c653a1c14e1166f76505ddf6ae5f4)

### 図書館とスライドパズル

図書館の封印グループ操作・正解判定と、データから構成するスライド盤面・UIを実装しました。会話表示とは別のプレイルールと操作画面です。全パズルを自作扱いせず、リンク先で確認できる範囲を示します。

[LibraryPuzzlePanel.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Puzzle/LibraryPuzzle/LibraryPuzzlePanel.cs), [担当変更 24497c4](https://github.com/YSL-RyuDo/Cylinder-1/commit/24497c4aed3f61d3eafcdef1937fcc29ddaae436), [SliderPuzzleManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Puzzle/SliderPuzzleManager/SliderPuzzleManager.cs), [担当変更 24497c4](https://github.com/YSL-RyuDo/Cylinder-1/commit/24497c4aed3f61d3eafcdef1937fcc29ddaae436)

## 担当した内容

CSV読み込み、会話ノード・UI、立ち絵、フラグ、操作トリガーを結ぶ会話システムを改修しました。

会話・感情状態の変化、チュートリアル進行、カットシーン連携を担当しました。

インベントリ、ポータル、操作UIを担当しました。YSLブランチにも他のメンバーのコミットが含まれ、名前だけで担当者を判断しません。

## 貢献の根拠

[Dialogue rework / 대화 개편 / 会話改修](https://github.com/YSL-RyuDo/Cylinder-1/commit/89a3080fa996dd7b30180d51d87a1db21f7e3ec5)

[Emotion state / 감정 상태 / 感情状態](https://github.com/YSL-RyuDo/Cylinder-1/commit/719fe90233e191f8d4df5fe5b05bb0143525edcb)

[Working branch / 작업 브랜치 / 作業ブランチ](https://github.com/YSL-RyuDo/Cylinder-1/tree/YSL)

コミットは記載した変更の根拠であり、貢献率の点数ではありません。外部素材・チームメンバーの貢献は区別します。

## 実装と設計

進行状況、感情状態、インタラクション条件に応じて会話を変え、肖像とUIを更新する必要があります。

CSV読み込み、会話ノード・UI、肖像、フラグ、トリガーを改修し、チュートリアル・カットシーン進行を連携しました。共同変更を含むYSLブランチでは会話・感情状態のコミットで担当を示します。

## コードの動作と実装構造の理由

確認した実装が扱う制約と利点・代償を説明します。設計効果の説明はコード分析であり、比較する全代案を当時試作したという意味ではありません。担当リンクは共同制作内の変更を示し、ソースの閲覧権限は維持します。

### 会話命令が後から完了する場合

**実装の流れ**

YSLのDialogueManagerはデータノードとシステム命令を実行します。即時命令は続行し、fade_wait・walk・delayは即座には続けずコールバックやコルーチンから次ノードへ進みます。後述のJSONローダーは別の初期実装です。

**この構造が必要な理由**

会話イベントは移動やフェード終了を待って次の行を出す場合があります。即時実行と完了待ちを分けることで、画面動作より先に物語が進むことを防ぎます。

**代案と現在の実装の折合い**

シーン別コルーチンは直接的ですが順序制御が重複します。視覚的タイムラインには選択肢・フラグ連携が必要です。現在のノード命令方式は順序を集約する代わりに、命令体系と参照の妥当性を管理します。

**利点・代償と検証範囲**

文字列命令やシーン検索は実行時に失敗します。例えばfade_waitでFadeManagerがない場合、継続を予約せず返ります。これは具体的な検証対象であり、全命令列の安全性を示すものではありません。

[Cylinder-1/DialogueManager.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueManager.cs), [Cylinder-1/DialogueCSVLoader.cs (YSL)](https://github.com/YSL-RyuDo/Cylinder-1/blob/b5de69a235c4548e93b4dc8438efea1701fa5d63/Assets/Scripts/YSL/Dialogue/DialogueCSVLoader.cs)

### 初期版のJSON会話読込

**実装の流れ**

DialogueLoaderはJSON台詞を読み、IDで検索できるデータにします。DialogueManagerはプレビューと確定台詞を選び、キーで表情画像を検索し、前の文字送りコルーチンを停止して実時間で新しい台詞を表示します。

**この構造が必要な理由**

内容と表示を分けると、台詞ごとにUI制御へ文章と表情の分岐を埋め込まずに済みます。前のコルーチンを停止するのは、同じテキスト欄に二つの処理が同時に書き込むのを防ぐためです。実時間の待機はゲーム時間倍率と文字送り速度を分離します。

**代案と現在の実装の折合い**

直接コードに書く方式は道具が少なく済みますが内容変更がコードに散ります。ScriptableObjectはUnityインスペクター編集、JSONは専用ローダーによるテキスト管理に向きます。現在のJSON方式では形式だけで保証されないスキーマとリソースキーの検証が必要です。

**利点・代償と検証範囲**

文字列IDとリソース名の規則は検証が必要なデータ契約です。データ分離だけで多言語制作基盤が完成するわけではなく、文字送り時の文字列追加には割当てコストも残ります。これらはローダーと表示の分離とは別の課題です。

[Cylinder-1/DialogueLoader.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueLoader.cs), [Cylinder-1/DialogueManager.cs](https://github.com/YSL-RyuDo/Cylinder-1/blob/e6b28c873b11bd9daa1421780e0608a057b40cb4/Assets/Scripts/YSL/Dialogue/DialogueManager.cs)

## 実行・コードの確認

[ProjectVersion.txt](ProjectSettings/ProjectVersion.txt)記載のUnity版で開いてください。パッケージロックとシリアライズ済み素材を維持し、Unityを開く前に対象ブランチを選んでください。

YSLブランチのシーンとBuild Settingsを確認してください。未確認の起動シーンは指定していません。

## 開発・検証範囲

YSLブランチにはmain未反映の作業とチーム共有の変更があります。引用コミットで担当範囲を区別し、Assets/Scripts/YSL内の全機能を単独制作したとは主張しません。デモ可能な状態は対象ブランチで確認してください。

## 詳細資料

[既存の詳細開発・利用・検証資料（韓国語）](docs/DEVELOPMENT_REFERENCE.ko.md)

## 関連リポジトリ

[Cylinder-1](https://github.com/YSL-RyuDo/Cylinder-1), 非公開・アクセス権が必要
