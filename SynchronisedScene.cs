
    internal class SynchronisedScene
    {
        internal Dictionary<string, int> Entities;
        internal List<string> SoundBanks;
        internal int Scene;
        internal string _AnimationDict;
        internal delegate void SynchronisedScenePhaseChange(float Phase);
        internal bool SceneRunning = false;
        internal bool UseDebugText;
        internal bool _Looped;

        public SynchronisedScene(int coreEntity, string animationDict, string coreEntityAnimationString = "", int rotationOrder = 2, bool holdLastFrame = true, bool looped = false, float animTime = 0.0f, float animSpeed = 1.0f, bool debugText = false)
        {
            Entities = new();
            SoundBanks = new();

            Vector3 coreEntityPosition = API.GetEntityCoords(coreEntity, false);
            Vector3 coreEntityRotation = API.GetEntityRotation(coreEntity, 2);

            Scene = API.NetworkCreateSynchronisedScene(coreEntityPosition.X, coreEntityPosition.Y, coreEntityPosition.Z, coreEntityRotation.X, coreEntityRotation.Y, coreEntityRotation.Z, rotationOrder, holdLastFrame, looped, 1.0f, animTime, animSpeed);

            _AnimationDict = animationDict;
            UseDebugText = debugText;
            _Looped = looped;

            if (this.UseDebugText) Debug.WriteLine("DEBUG: Scene: " + Scene.ToString() + " was successfully added.");

            if (coreEntityAnimationString == "" || coreEntityAnimationString == null) return;

            this.AddPremadeEntityToScene(coreEntity, coreEntityAnimationString);
        }

        internal void AddPedToScene(Ped ped, string pedAnimation, float blendIn = 7.0f, float blendOut = 7.0f, int duration = 0, int flag = 0, float playBackRate = 1000.0f)
        {
            API.NetworkAddPedToSynchronisedScene(ped.Handle, this.Scene, this._AnimationDict, pedAnimation, blendIn, blendOut, duration, flag, playBackRate, 0);
            if (this.UseDebugText) Debug.WriteLine("DEBUG: Ped: " + ped.Handle.ToString() + " was successfully added to the scene.");
        }

        internal void AddEntityToScene(string modelHash, string animationName, float speed = 8.0f, float speedMulti = 8.0f, int flag = 0)
        {
            int HashKey = API.GetHashKey(modelHash);
            int CreatedObject = API.CreateObject(HashKey, 0.0f, 0.0f, 0.0f, true, false, false);
            Entities.Add(modelHash, CreatedObject);
            if (this.UseDebugText) Debug.WriteLine("DEBUG: Entity: " + modelHash.ToString() + " was successfully added to the scene.");
            if (animationName == null || animationName == "") return;
            API.NetworkAddEntityToSynchronisedScene(CreatedObject, this.Scene, this._AnimationDict, animationName, speed, speedMulti, flag);
        }

        internal void AddPremadeEntityToScene(int entity, string animationName, float speed = 8.0f, float speedMulti = 8.0f, int flag = 0)
        {
            API.NetworkAddEntityToSynchronisedScene(entity, this.Scene, this._AnimationDict, animationName, speed, speedMulti, flag);

            if (this.UseDebugText) Debug.WriteLine("DEBUG: Premade Entity: " + Scene.ToString() + " was successfully added and had animation: " + animationName);
        }

        internal void LoadSoundBankForScene(string soundBank)
        {
            API.RequestScriptAudioBank(soundBank, false);

            this.SoundBanks.Add(soundBank);

            if (this.UseDebugText) Debug.WriteLine("DEBUG: Loaded: " + soundBank.ToString() + " SoundBank to scene successfully.");
        }

        internal void UnloadSoundBanksForScene()
        {
            foreach (string soundBank in this.SoundBanks)
            {
                API.ReleaseNamedScriptAudioBank(soundBank);
                if (this.UseDebugText) Debug.WriteLine("DEBUG: Unloaded: " + soundBank.ToString() + " SoundBank from scene successfully.");
            }

            SoundBanks.Clear();
        }

        internal void StartScene()
        {
            API.NetworkStartSynchronisedScene(this.Scene);
            if (this.UseDebugText) Debug.WriteLine("DEBUG: Scene started successfully.");

            this.PhaseChangeTick();
        }

        internal void DisposeScene()
        {
            API.DisposeSynchronizedScene(this.Scene);

            foreach (var Entity in Entities)
            {
                int entityVal = Entity.Value;
                if (this.UseDebugText) Debug.WriteLine("DEBUG: Deleted: " + entityVal.ToString() + " object successfully.");
                API.DeleteObject(ref entityVal);
            }

            Entities.Clear();

            if (this.UseDebugText) Debug.WriteLine("DEBUG: Scene disposed successfully.");

            this.UnloadSoundBanksForScene();
            this.SceneRunning = false;
        }

        internal async void PhaseChangeTick()
        {
            this.SceneRunning = true;
            double prevValue = 0.00f;

            while (SceneRunning)
            {
                int LocalScene = API.NetworkGetLocalSceneFromNetworkId(this.Scene);
                float Phase = API.GetSynchronizedScenePhase(LocalScene);

                double roundedWithMod = Math.Round((Phase % 0.01), 2);
                double roundedWithoutMod = Math.Round(Phase, 2);
                if (roundedWithMod == 0 && prevValue != roundedWithoutMod)
                {
                    OnPhaseChange?.Invoke((float)Math.Round(Phase, 2));
                    prevValue = roundedWithoutMod;
                }

                if (this.SceneRunning && Phase == 1.0f && !this._Looped)
                {
                    this.DisposeScene();
                    OnPhaseChange?.Invoke(1.0f);
                    return;
                }

                await BaseScript.Delay(1);
            }
        }

        internal void HaltPhaseChangeAndDispose()
        {
            this.DisposeScene();
            this.UnloadSoundBanksForScene();
            this.SceneRunning = false;
        }

        internal event SynchronisedScenePhaseChange OnPhaseChange;
    }
