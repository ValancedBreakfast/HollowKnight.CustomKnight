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

        private bool enableFilter = true;
        private Texture2D dungTex = null;
        public override void ApplyTexture(Texture2D tex)
        {
            var HC = HeroController.instance.gameObject;
            var Dung = HC.FindGameObjectInChildren("Dung");
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

            dungTex = tex;
            // apply tex and color to basic dung trail & dung cloud for spore shroom
            On.HutongGames.PlayMaker.FsmState.OnEnter += ApplyTexToDungCloud;
        }

        private void ApplyTexToDungCloud(On.HutongGames.PlayMaker.FsmState.orig_OnEnter orig, HutongGames.PlayMaker.FsmState self)
        {
            if ((self.Fsm.GameObject.name.StartsWith("Knight Dung Trail") && self.Fsm.Name == "Control" && self.Name == "Init")
                || (self.Fsm.GameObject.name.StartsWith("Knight Dung Cloud") && self.Fsm.Name == "Control" && self.Name == "Normal"))
            {
                var pt = self.Fsm.GetFsmGameObject("Pt Normal").Value;
                pt.GetComponent<ParticleSystem>().startColor = enableFilter ? DungColor : new Color(1, 1, 1, 1);
                pt.GetComponent<ParticleSystemRenderer>().material.mainTexture = dungTex;
            }
            else if (self.Fsm.GameObject.name.StartsWith("Knight Dung Cloud") && self.Fsm.Name == "Control" && self.Name == "Deep")
            {
                var pt = self.Fsm.GetFsmGameObject("Pt Deep").Value;
                pt.GetComponent<ParticleSystem>().startColor = enableFilter ? DungColor : new Color(1, 1, 1, 1);
                pt.GetComponent<ParticleSystemRenderer>().material.mainTexture = dungTex;
            }

            orig(self);
        }
    }
}
