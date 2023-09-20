internal class Example
{
  public Example() 
  {
    // Example #1: Fingerprint Scanner Hack
    int prop = API.GetHashKey("ch_prop_fingerprint_scanner_01d");
    int propObject = API.CreateObject(prop, 100.0f, 80.0f, 180.0f, true, false, true);

    SynchronisedScene initScene = new(propObject, "anim_heist@hs3f@ig1_hack_keypad@male@", "", debugText: true);

    initScene.AddPedToScene(Game.PlayerPed, "action_var_01");
    initScene.AddEntityToScene("ch_prop_ch_usb_drive01x", "action_var_01_ch_prop_ch_usb_drive01x");
    initScene.AddEntityToScene("ch_prop_ch_phone_ing_01a", "action_var_01_prop_phone_ing");

    initScene.StartScene();

    initScene.OnPhaseChange += async (float phase) =>
    {
     Debug.WriteLine(phase.ToString());
     if (phase > 0.98f)
     {
    	SynchronisedScene loopScene = new(propObject, "anim_heist@hs3f@ig1_hack_keypad@male@", "", debugText: true, holdLastFrame: false, looped: true);

        loopScene.AddPedToScene(Game.PlayerPed, "hack_loop_var_01");
        loopScene.AddEntityToScene("ch_prop_ch_usb_drive01x", "hack_loop_var_01_ch_prop_ch_usb_drive01x");
        loopScene.AddEntityToScene("ch_prop_ch_phone_ing_01a", "hack_loop_var_01_prop_phone_ing");

        loopScene.StartScene();

        await BaseScript.Delay(5000);
	 
        loopScene.DisposeScene();

        SynchronisedScene successScene = new(propObject, "anim_heist@hs3f@ig1_hack_keypad@male@", "", debugText: true);

        successScene.AddPedToScene(Game.PlayerPed, "success_react_exit_var_01");
        successScene.AddEntityToScene("ch_prop_ch_usb_drive01x", "success_react_exit_var_01_ch_prop_ch_usb_drive01x");
        successScene.AddEntityToScene("ch_prop_ch_phone_ing_01a", "success_react_exit_var_01_prop_phone_ing");

        successScene.StartScene();
      }
    };

    //Example 2: Container Lock Cut

    int prop = API.GetHashKey("tr_prop_tr_container_01a");
    int propObject = API.CreateObject(prop, 100.0f, 80.0f, 180.0f, true, false, true);

    SynchronisedScene initScene = new(propObject, "anim@scripted@player@mission@tunf_train_ig1_container_p1@male@", "action_container", debugText: true);

    initScene.AddPedToScene(Game.PlayerPed, "action");
    initScene.AddEntityToScene("ch_p_m_bag_var04_arm_s", "action_bag");
    initScene.AddEntityToScene("tr_prop_tr_lock_01a", "action_lock");
    initScene.AddEntityToScene("tr_prop_tr_grinder_01a", "action_angle_grinder");

    initScene.LoadSoundBankForScene("dlc_tuner/dlc_tuner_generic");

    initScene.StartScene();

    bool HasParticleFXBeenRequested = false;
    int ParticleFXLoop = 0;

    initScene.OnPhaseChange += (float Phase) =>
    {
    	if (Phase >= 0.35f && Phase <= 0.45f)
    	{
        if (!HasParticleFXBeenRequested)
        {
          API.RequestNamedPtfxAsset("scr_tn_tr");
          if (API.HasNamedPtfxAssetLoaded("scr_tn_tr"))
          {
            HasParticleFXBeenRequested = true;
            if (!API.DoesParticleFxLoopedExist(ParticleFXLoop))
            {
              API.UseParticleFxAsset("scr_tn_tr");
              ParticleFXLoop = API.StartNetworkedParticleFxLoopedOnEntity("scr_tn_tr_angle_grinder_sparks", initScene.Entities["tr_prop_tr_grinder_01a"], 0f, 0.25f, 0f, 0f, 0f, 0f, 1f, false, false, false);
            }
          }
        }
      }
      else
      {
      	if (HasParticleFXBeenRequested)
        {
          if (API.DoesParticleFxLoopedExist(ParticleFXLoop))
          {
            API.StopParticleFxLooped(ParticleFXLoop, false);
            ParticleFXLoop = 0;
            API.RemoveNamedPtfxAsset("scr_tn_tr");
            API.ReleaseNamedScriptAudioBank("dlc_tuner/dlc_tuner_generic");
            HasParticleFXBeenRequested = false;
          }
        }
      }
    };
  }
}
