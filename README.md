# UnityVFXEditor

動画の上に **物理ベースの3Dアニメーション** を合成し、透過付きで書き出すための Unity ツール（制作中）です。

MVPプリセット: **GlassBreak**（位置/奥行き、破壊起点、散布方向、破片数、力、重力スケール など）

## 機能（MVP）
- **MP4(H.264)** の読み込み → 3D空間の平面に貼り付けてプレビュー
- タイムラインの **トリガーマーカー**（特定時刻にプリセットを発火）
- Inspector でプリセットパラメータ編集（拡張しやすい設計を想定）
- 固定デルタタイム + 乱数シードで **再現性のある再生/書き出し**
- **PNG連番（アルファ対応）** などの書き出し（環境依存で動画書き出しも検討）

## 動作環境 / 前提
- Unity: **2022.3 LTS** 系を想定（同一バージョン推奨）
- Project Settings（推奨）
  - Version Control: **Visible Meta Files**
  - Asset Serialization: **Force Text**

## セットアップ（初回）
### 1) クローン
```bash
git clone https://github.com/B21020/UnityVFXEditor.git
cd UnityVFXEditor
```

### 2) Git LFS（必須）
このリポジトリには 100MB を超える動画ファイルが含まれるため、**Git LFS が必要**です。

```bash
git lfs install
git lfs pull
```

※ `git lfs` が見つからない場合は、Git LFS をインストールしてください。

### 3) Unityで開く
Unity Hub で本リポジトリを追加して開きます。
必要に応じて、上記の Project Settings（Visible Meta Files / Force Text）を確認してください。

## Unity Smart Merge（任意）
シーンやPrefabなどの YAML アセットを安全にマージしたい場合、UnityYAMLMerge を Git の merge driver として設定できます。

Windows（`<VERSION>` を Unity のバージョンに置換）:
```powershell
git config merge.unityyamlmerge.name "Unity SmartMerge (UnityYAMLMerge)"
git config merge.unityyamlmerge.driver "\"C:/Program Files/Unity/Hub/Editor/<VERSION>/Editor/Data/Tools/UnityYAMLMerge.exe\" merge -p %O %A %B %L"
```

macOS（`<VERSION>` を Unity のバージョンに置換）:
```bash
git config merge.unityyamlmerge.name "Unity SmartMerge (UnityYAMLMerge)"
git config merge.unityyamlmerge.driver "/Applications/Unity/Hub/Editor/<VERSION>/Unity.app/Contents/Tools/UnityYAMLMerge merge -p %O %A %B %L"
```

## クイックスタート（MVP）
1. `Assets/Scenes/Editor.unity` を開く
2. 動画を `Assets/Video/` に置く（サンプルが入っている場合はそれを使用）
3. タイムラインでマーカーを追加して、GlassBreak の各種パラメータを設定
4. プレビュー再生 → 必要なら書き出し

## バージョン管理メモ
- Unity のキャッシュ（`Library/` など）はコミットしません（.gitignore 済み）
- 動画などの大きいバイナリは GitHub の制限回避のため **Git LFS** を使います（このリポジトリでは `*.mp4` を LFS 追跡済み）

## ライセンス
TBD
