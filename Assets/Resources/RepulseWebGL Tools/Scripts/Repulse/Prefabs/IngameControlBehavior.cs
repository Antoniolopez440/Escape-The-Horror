using Resources.Scripts.Repulse.WEBGL;
using UnityEngine;

namespace Resources.RepulseWebGL_Tools.Scripts.Repulse.Prefabs
{
    public class IngameControlBehavior : MonoBehaviour
    {
        public void Minimize()
        {
            Screen.fullScreen=false;
           if(this.gameObject.activeSelf)this.gameObject.SetActive(!gameObject.activeSelf);
        }

        public void Maximize()
        {
            Screen.fullScreen = true;
        }
    }
}
