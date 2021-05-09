using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tools.ResCheck
{
    public class ResModelImporter : AssetPostprocessor
    {
        private void OnPreprocessModel()
        {
            if (ModelImportSetting.Instance == null || !ModelImportSetting.Instance.autoImport)
            {
                return;
            }
            var importer = assetImporter as ModelImporter;
            if (importer == null) return;
            var m = ModelImportSetting.GetSetting(importer.assetPath);
            if (m != null) { }
        }

        private void OnPostprocessModel(GameObject g)
        {
            if (ModelImportSetting.Instance == null ||!ModelImportSetting.Instance.autoImport)
            {
                return;
            }
            ConstructPrefab(assetPath, g);
        }

        [MenuItem("Assets/Import Model/Skin", true, 1100)]
        private static bool ValidateImportModelSkin()
        {
            var sel = Selection.activeObject;
            if (sel && sel is GameObject)
            {
                return ValidateImportModelSkin(sel);
            }

            return false;
        }

        private static bool ValidateImportModelSkin(Object go)
        {
            var ms = ModelImportSetting.GetSetting(AssetDatabase.GetAssetPath(go));
            return ms != null;
        }
        
        [MenuItem("Assets/Import Model/Skin", false, 1100)]
        public static void ImportModelSkin()
        {
            if (Selection.activeObject is GameObject mg)
            {
                ImportModelSkin(mg);
            }
        }
        
        public static void ImportModelSkin(GameObject go)
        {
            ConstructPrefab(AssetDatabase.GetAssetPath(go), go);
        }
        
        [MenuItem("Assets/Import Model/Animation", false, 1101)]
        public static void ImportModelAnimation()
        {
            var mgs = Selection.GetFiltered<GameObject>(SelectionMode.Assets);
            ImportModelAnimation(mgs);
        }
        
        public static void ImportModelAnimation(GameObject[] gos)
        {
            foreach (var mg in gos)
            {
                var assetPath = AssetDatabase.GetAssetPath(mg);
                
                var ms = ModelImportSetting.GetSetting(assetPath);
                var anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                if (ms == null || anim == null) continue;
                var exportPath = ms.GetAnimationAssetName(assetPath, anim.name);

                var clone = Object.Instantiate(anim);
                clone.name = anim.name;
                
                AssetUtil.CreateOrReplaceAsset(clone, exportPath);
                Debug.Log($"export animation at: {exportPath}");
            }
            AssetDatabase.SaveAssets();
        }
        
        private static void ConstructPrefab(string assetPath, GameObject mg)
        {
            var ms = ModelImportSetting.GetSetting(assetPath);
            if (mg == null || ms == null) return;

            var name = ms.GetPrefabNameFromModel(assetPath);
            var path = $"{ms.GetPrefabFolderFromModel(assetPath)}{name}.prefab";
            var prefab =  AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            //create new prefab
            if (prefab == null)
            {
                var model = Object.Instantiate(mg);
                prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(model, path, InteractionMode.AutomatedAction);
                Object.DestroyImmediate(model);
            }
            
            var ig = Object.Instantiate(prefab);
            //sync exist prefab
            SyncPrefab(mg, ig, ms);
            
            //sync skinned mesh render
            SyncSkinnedMeshRender(mg, ig);
            if (ms.extractMesh)
            {
                var imrs = ig.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in imrs)
                {
                    if (!r.sharedMesh) continue;
                    r.sharedMesh = CreateMeshAsset(r.sharedMesh, ms, assetPath);
                }

                var mfs = ig.GetComponentsInChildren<MeshFilter>();
                foreach (var m in mfs)
                {
                    if (!m.sharedMesh) continue;
                    m.sharedMesh = CreateMeshAsset(m.sharedMesh, ms, assetPath);
                }
            }
            
            //sync avatar
            if (ms.extractAvatar)
            {
                var animators = ig.GetComponentsInChildren<Animator>();
                foreach (var animator in animators)
                {
                    if (!animator.avatar) continue;
                    var avatar = Object.Instantiate(animator.avatar);
                    AssetDatabase.CreateAsset(avatar, ms.GetAvatarAssetName(assetPath, animator.avatar.name));
                    animator.avatar = avatar;
                }
            }
            
            PrefabUtility.SaveAsPrefabAssetAndConnect(ig, path, InteractionMode.AutomatedAction); 
            Object.DestroyImmediate(ig);
            
            //post process
            ProcessorUtil.ModelPostProcessors.ForEach(p => { if (p.Catagory == "Character") p.Process(path); });

            AssetDatabase.SaveAssets();
        }

        private static Mesh CreateMeshAsset(Mesh mesh, ModelSetting ms, string assetPath)
        {
            var assetMesh = Object.Instantiate(mesh);
            AssetDatabase.CreateAsset(assetMesh, ms.GetMeshAssetName(assetPath, mesh.name));
            return assetMesh;
        }

        private static void SyncPrefab(GameObject mg, GameObject ig, ModelSetting ms)
        {
            SyncGameObject(mg, ig);
            
            //sync childs
            for (var i = 0; i < mg.transform.childCount; i++)
            {
                var mSub = mg.transform.GetChild(i).gameObject;
                var iSubTrans = ig.transform.Find(mSub.name);
                GameObject iSub;
                if (iSubTrans != null)
                {
                    iSub = iSubTrans.gameObject;
                }
                else
                {
                    iSub = new GameObject(mSub.name);
                    iSub.transform.SetParent(ig.transform);
                }

                SyncPrefab(mSub, iSub, ms);
            }

            //delete redundant childs
            var reserves = new List<Transform>();
            if (ig.transform.childCount != mg.transform.childCount)
            {
                var redundant = new List<GameObject>();
                for (var i = 0; i < ig.transform.childCount; i++)
                {
                    var iSub = ig.transform.GetChild(i).gameObject;
                    var mTrans = mg.transform.Find(iSub.name);
                    if (mTrans == null)
                    {
                        if (!ms.IsReserve(iSub.name)) redundant.Add(iSub);
                        else reserves.Add(iSub.transform);
                    }
                }
                redundant.ForEach(Object.DestroyImmediate);
            }

            //sort childs
            var reservePres = new Dictionary<Transform, Transform>();
            reserves.ForEach(r =>
            {
                var rIndex = r.GetSiblingIndex();
                reservePres[r] = rIndex > 0 ? ig.transform.GetChild(rIndex - 1) : null;
            });
            reserves.ForEach(r => r.SetAsLastSibling());
            for (var i = 0; i < mg.transform.childCount; i++)
            {
                var mSubTrans = mg.transform.GetChild(i);
                var iSubTrans = ig.transform.Find(mSubTrans.gameObject.name);
                iSubTrans.SetSiblingIndex(i);
            }
            reserves.ForEach(r =>
            {
                var pre = reservePres[r];
                r.SetSiblingIndex(pre == null ? 0 : pre.GetSiblingIndex() + 1);
            });
        }

        private static void SyncGameObject(GameObject mg, GameObject ig)
        {
            var mgcs = mg.GetComponents<Component>();
            foreach (var mc in mgcs)
            {
                ComponentUtility.CopyComponent(mc);
                var ic = ig.GetComponent(mc.GetType());
                if (ic is Animator || ic is Renderer || ic is MeshFilter)
                {
                    Debug.Log($"deal exist component: {mg.name}({mc.GetType()})");
                }
                else
                {
                    if (!ic) ic = ig.AddComponent(mc.GetType());
                    ComponentUtility.PasteComponentValues(ic);
                }
            }
        }

        private static void SyncSkinnedMeshRender(GameObject mg, GameObject ig)
        {
            var mgs = mg.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var mr in mgs)
            {
                var ibones = new Transform[mr.bones.Length];
                for (var i = 0; i < mr.bones.Length; i++)
                {
                    var bp = GetPath(mr.bones[i], mg.transform);
                    ibones[i] = ig.transform.Find(bp);
                }
                var mp = GetPath(mr.transform, mg.transform);
                var ir = ig.transform.Find(mp).gameObject.GetOrAddComponent<SkinnedMeshRenderer>();
                //mesh
                ir.sharedMesh = mr.sharedMesh;
                //bones
                ir.bones = ibones;
                //root bone
                var mrp = GetPath(mr.rootBone, mg.transform);
                ir.rootBone = ig.transform.Find(mrp);
            }
        }

        private static string GetPath(Transform ct, Transform rt)
        {
            var sb = "";
            while (ct && ct != rt)
            {
                sb = $"{ct.gameObject.name}/{sb}";
                ct = ct.parent;
            }

            sb = sb.Remove(sb.Length - 1);
            return sb;
        }


    }
}