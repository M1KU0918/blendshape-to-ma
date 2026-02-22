# BlendShape to MA ShapeChanger

SkinnedMeshRenderer の BlendShape 値を Modular Avatar Shape Changer に  
ワンクリックで一括登録できる Unity エディター拡張ツールです。

## VCC でインストール

[![Add to VCC](https://img.shields.io/badge/Add_to_VCC-5865F2?style=for-the-badge)](https://M1KU0918.github.io/blendshape-to-ma)

VCC の Settings → Packages → Add Repository に以下のURLを追加してください。

```
https://M1KU0918.github.io/blendshape-to-ma/index.json
```

## 使い方

1. Unity メニューの `Tools > BlendShape to MA ShapeChanger` を開く
2. **Skinned Mesh Renderer** に対象レンダラーをセット
3. **MA Shape Changer** に対象コンポーネントをセット
4. 「MA Shape Changer に登録」ボタンを押す

## オプション

| オプション | 説明 |
|---|---|
| 0 以外のみ登録 | ON のとき値が 0 の BlendShape をスキップ（デフォルト ON） |
| 適用前にリストをクリア | ON のとき既存エントリを全削除してから登録 |

## 必要環境

- Unity 2019.4 以上
- Modular Avatar 1.9.0 以上

## ライセンス

MIT License
