using System.Collections;
using Microsoft.Win32;
using UnityEngine;

namespace MultiDisplay.Scripts
{
    public class MultiDisplay : MonoBehaviour
    {
        private const string _fullscreenMode = "Screenmanager Fullscreen mode Default_h401710285";

        // Start is called before the first frame update
        void Awake()
        {
            Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemHeight, FullScreenMode.FullScreenWindow);
            
            foreach (var display in Display.displays)
            {
                display.Activate();
            }
            
            StartCoroutine(DeleteFullscreenMode());

        }


        private IEnumerator DeleteFullscreenMode()
        {
            yield return new WaitForSeconds(0.5f);
            DeleteFullscreenModeValue();
        }

        private static bool DeleteFullscreenModeValue()
        {
            RegistryKey kml      = Registry.CurrentUser;
            RegistryKey software = kml.OpenSubKey("SOFTWARE", true);
            if (software == null)
            {
                return false;
            }
                
            RegistryKey company = software.OpenSubKey(Application.companyName, true);
            if (company == null)
            {
                return false;
            }
                
            RegistryKey product = company.OpenSubKey(Application.productName, true);
            if (product == null)
                return false;
            product.DeleteValue(_fullscreenMode);
            return true;
        }
    }
}