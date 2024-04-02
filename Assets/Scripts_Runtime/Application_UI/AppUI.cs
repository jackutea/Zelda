using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zelda {

    public class AppUI {

        ModuleAssets assets;

        Canvas screenCanvas;
        Canvas worldCanvas;

        Panel_Login login; // Unique 唯一
        Dictionary<int /*EntityID*/, HUD_HpBar> hpBars;

        public Action Login_OnStartHandle;

        public AppUI() {
            hpBars = new Dictionary<int, HUD_HpBar>();
        }

        public void Inject(ModuleAssets assets, Canvas screenCanvas, Canvas worldCanvas) {
            this.assets = assets;
            this.screenCanvas = screenCanvas;
            this.worldCanvas = worldCanvas;
        }

        // - Login
        #region Panel_Login
        public void Login_Open() {
            GameObject go = Open(nameof(Panel_Login), screenCanvas);
            login = go.GetComponent<Panel_Login>();
            login.Ctor();

            login.OnStartHandle = () => {
                Login_OnStartHandle.Invoke();
            };
        }

        public void Login_Close() {
            GameObject.Destroy(login.gameObject);
            login = null;
        }
        #endregion

        // - HpBar
        #region HUD_HpBar
        public void HpBar_Open(int id, float hp, float hpMax) {
            GameObject go = Open(nameof(HUD_HpBar), worldCanvas);
            HUD_HpBar hpBar = go.GetComponent<HUD_HpBar>();
            hpBar.Ctor();
            hpBar.SetHp(hp, hpMax);
            hpBars.Add(id, hpBar);
        }

        public void HpBar_UpdatePosition(int id, Vector3 position, Vector3 cameraForward) {
            hpBars.TryGetValue(id, out HUD_HpBar hpBar);
            hpBar.SetPosition(position, cameraForward);
        }
        #endregion

        // 打开 UI
        GameObject Open(string uiName, Canvas canvas) {
            bool has = assets.TryGetUIPrefab(uiName, out GameObject prefab);
            if (!has) {
                Debug.LogError($"UI: {uiName} not found.");
                return null;
            }
            GameObject go = GameObject.Instantiate(prefab, canvas.transform);
            return go;
        }

    }

}