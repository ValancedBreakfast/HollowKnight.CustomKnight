using static Satchel.FsmUtil;
using static Satchel.GameObjectUtils;

namespace CustomKnight
{
    internal class DungRecharge : Skinable_Multiple
    {
        private Color DungColor = new Color(0.6706f, 0.4275f, 0, 1);
        public static string NAME = "DungRecharge";
        public DungRecharge() : base(NAME) { }
        public override List<Material> GetMaterials()
        {
            var HC = HeroController.instance.gameObject;
            var Dung = HC.FindGameObjectInChildren("Dung");
            return new List<Material>{
                Dung.FindGameObjectInChildren("Particle 1").GetComponent<ParticleSystemRenderer>().material,
                HC.FindGameObjectInChildren("Dung Recharge").GetComponent<ParticleSystemRenderer>().material
            };
        }
        public override void SaveDefaultTexture()
        {
            if (materials != null && materials[0].mainTexture != null)
            {
                ckTex.defaultTex = materials[0].mainTexture as Texture2D;
            }
            else
            {
                CustomKnight.Instance.Log($"skinable {name} : material is null");
            }
        }

        public override void ApplyTexture(Texture2D tex)
        {
            var HC = HeroController.instance.gameObject;
            var Dung = HC.FindGameObjectInChildren("Dung");
            var enableFilter = true;
            var skin = SkinManager.GetCurrentSkin() as ISupportsConfig;
            if (skin != null)
            {
                enableFilter = skin.GetConfig().dungFilter;
            }
#pragma warning disable CS0618 // Type or member is obsolete
            Dung.FindGameObjectInChildren("Particle 1").GetComponent<ParticleSystem>().startColor = enableFilter ? DungColor : new Color(1, 1, 1, 1);
            HC.FindGameObjectInChildren("Dung Recharge").GetComponent<ParticleSystem>().startColor = enableFilter ? DungColor : new Color(1, 1, 1, 1);
#pragma warning restore CS0618 // Type or member is obsolete
            foreach (var mat in materials)
            {
                mat.mainTexture = tex;
            }
            // basic dung trail
            var action = HC.FindGameObjectInChildren("Dung").LocateMyFSM("Control").GetAction<SpawnObjectFromGlobalPoolOverTime>("Equipped", 0);
            var prefab = action.gameObject.Value;
            UnityEngine.Object.DontDestroyOnLoad(prefab);
            prefab.SetActive(false);
#pragma warning disable CS0618 // Type or member is obsolete
            prefab.FindGameObjectInChildren("Pt Normal").GetComponent<ParticleSystem>().startColor = enableFilter ? DungColor : new Color(1, 1, 1, 1);
#pragma warning restore CS0618 // Type or member is obsolete
            prefab.FindGameObjectInChildren("Pt Normal").GetComponent<ParticleSystemRenderer>().material.mainTexture = tex;
            action.gameObject.Value = prefab;

            // dung cloud for spore shroom
            var action2 = HC.LocateMyFSM("Spell Control").GetAction<SpawnObjectFromGlobalPool>("Dung Cloud", 0);
            var prefab2 = action2.gameObject.Value;
            UnityEngine.Object.DontDestroyOnLoad(prefab2);
            prefab2.SetActive(false);
#pragma warning disable CS0618 // Type or member is obsolete
            prefab2.FindGameObjectInChildren("Pt Deep").GetComponent<ParticleSystem>().startColor = enableFilter ? DungColor : new Color(1, 1, 1, 1);
#pragma warning restore CS0618 // Type or member is obsolete
            prefab2.FindGameObjectInChildren("Pt Deep").GetComponent<ParticleSystemRenderer>().material.mainTexture = tex;
#pragma warning disable CS0618 // Type or member is obsolete
            prefab2.FindGameObjectInChildren("Pt Normal").GetComponent<ParticleSystem>().startColor = enableFilter ? DungColor : new Color(1, 1, 1, 1);
#pragma warning restore CS0618 // Type or member is obsolete
            prefab2.FindGameObjectInChildren("Pt Normal").GetComponent<ParticleSystemRenderer>().material.mainTexture = tex;
            action2.gameObject.Value = prefab2;
            // dung cloud for spore shroom unn
            var action3 = HC.LocateMyFSM("Spell Control").GetAction<SpawnObjectFromGlobalPool>("Dung Cloud 2", 0);
            action3.gameObject.Value = prefab2;

        }
    }
}