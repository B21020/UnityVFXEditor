# UnityVFXEditor (Working Title)
A standalone Unity tool that composites **physics-driven 3D animations** over a pre-made video and exports the result (alpha-capable).  
MVP preset: **GlassBreak** (position/depth, break origin, scatter direction, shard count, force, gravity scale).

## ✅ Features (MVP)
- Input **video (MP4/H.264)** → place in 3D space as a textured plane
- Timeline with **trigger markers** (fire presets at exact timestamps)
- Inspector-driven **preset parameters** (schema-based for extensibility)
- Deterministic recording (fixed timestep + seeded randomness)
- Export **PNG image sequence (with alpha)** or ProRes 4444 (if supported)

## 🧩 Tech/Versions
- Unity **2022.3 LTS** (recommend fixed editor version for all collaborators)
- Serialization: **Force Text**, Version Control: **Visible Meta Files**
- IDE: **VS Code** with OmniSharp C# extension

## 📦 Project Setup
1. Clone the repo:
   ```bash
   git clone https://github.com/<yourname>/UnityVFXEditor.git
   cd UnityVFXEditor
   ```

2. (First time) Install LFS & track heavy assets (optional but recommended):
    ```
    git lfs install
    git lfs track "*.psd" "*.fbx" "*.mp4" "*.mov" "*.wav" "*.ogg" "*.ttf" "*.otf" "*.zip"
    git add .gitattributes .gitattributes.lock .gitattributes && git add .gitattributes
    ```
3. Open in Unity 2022.3 LTS.
   In Edit → Project Settings → Editor:
    * Version Control: Visible Meta Files
    * Asset Serialization: Force Text

🔀 Smart Merge (UnityYAMLMerge)
For safer merges on scenes/prefabs/animators, configure the merge driver once in this repo:
```Windows
git config merge.unityyamlmerge.name "Unity SmartMerge (UnityYAMLMerge)"
git config merge.unityyamlmerge.driver "\"C:/Program Files/Unity/Hub/Editor/<VERSION>/Editor/Data/Tools/UnityYAMLMerge.exe\" merge -p %O %A %B %L"
```

```macOS
git config merge.unityyamlmerge.name "Unity SmartMerge (UnityYAMLMerge)"
git config merge.unityyamlmerge.driver "/Applications/Unity/Hub/Editor/<VERSION>/Unity.app/Contents/Tools/UnityYAMLMerge merge -p %O %A %B %L"
```

📁 Repo Layout (planned)
```
Assets/
  Scripts/
    Core/        # TimeController, EffectPlayer, ProjectManager
    UI/          # TimelineView, Inspector, Overlay Gizmos
    Effects/     # IEffect, EffectBase, GlassBreakEffect, params
  Resources/
  Presets/       # preset JSON schema (future)
ProjectSettings/
Packages/
```

🌿 Git Branching
* main — stable (demo/submit)
* dev — active integration
* feature/ui-layout — UI wireframe work (this sprint)
* feature/effect-system — physics/preset engine

▶︎ Quick Start (MVP path)
* Import sample video into Assets/Video.
* Open Scenes/Editor.unity.
* Add a marker at t=1.2s, set GlassBreak params (Z, origin, force, shards).
* Play preview → export PNG sequence with alpha.

🧪 QA Checklist
* Same seed/params → identical shards (deterministic)
* Timeline seek fires exactly once per marker
* Exported frames match preview frames (1 / middle / last)

📝 License
TBD (MIT or private for coursework).

---

# Copilot (Agent) prompt
Paste this into Copilot Chat in VS Code (Agent mode). It will create the files at the repo **root** with the exact contents above and commit them.

```text
You are operating in my VS Code workspace at the root of a Unity project.

TASK:
1) Create three files at the repository root with the following exact contents:
   - .gitignore  → (paste the .gitignore block from the chat)
   - .gitattributes → (paste the .gitattributes block from the chat)
   - README.md → (paste the README.md block from the chat)

2) Save all files with UTF-8 (no BOM), LF line endings.

3) Stage and commit:
   git add .gitignore .gitattributes README.md
   git commit -m "chore(repo): add Unity .gitignore/.gitattributes and project README"

4) (Optional) If Git LFS is installed, run:
   git lfs install
   git lfs track "*.psd" "*.fbx" "*.mp4" "*.mov" "*.wav" "*.ogg" "*.ttf" "*.otf" "*.zip"
   git add .gitattributes
   git commit -m "chore(repo): track large binary asset types via Git LFS"

5) Open README.md for me to review.

IMPORTANT:
- Do NOT change any other files.
- Place the files at the repository root.
- Preserve the exact content and code fences.

上記のプロンプトの回答は日本語で行ってください。
```
