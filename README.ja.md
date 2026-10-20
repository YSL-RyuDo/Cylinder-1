# Cylinder

[English](README.md), [Korean](README.ko.md), [Japanese](README.ja.md)

会話、ポートレート、感情状態、進行条件を扱うナラティブゲームのシステム開発事例です。

| プロジェクト情報 | 内容 |
|---|---|
| 開発状況 | チーム開発での担当 |
| 使用技術 | C#, Unity, CSV |

[開発事例を見る](https://ysl-ryudo.github.io/ja/work/cylinder/), [Portfolio](https://ysl-ryudo.github.io/ja/)

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

| 領域 | 実装 |
|---|---|
| 制作データ | CSV読み込みと会話ノード |
| 進行条件 | フラグ・感情状態・インタラクショントリガー |
| 画面表示 | 会話UI・ポートレート・チュートリアル・カットシーン連携 |

## コードの動作と実装構造の理由

確認した実装が扱う制約と利点・代償を説明します。設計効果の説明はコード分析であり、比較する全代案を当時試作したという意味ではありません。担当リンクは共同制作内の変更を示し、ソースの閲覧権限は維持します。

### 台詞データと表示処理を分ける理由

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
