#if UNITY_ANDROID

using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;

public class ZorunluGuncelleme : MonoBehaviour
{
    AppUpdateManager mgr;

    IEnumerator Start()
    {
#if UNITY_EDITOR
        yield break; // Editor'da JNI yok → çık
#endif
        if (Application.platform != RuntimePlatform.Android)
            yield break; // Sadece Android

        yield return null; // Activity hazır olsun (bir frame bekle)

        try { mgr = new AppUpdateManager(); }
        catch { yield break; } // Activity bulunamadıysa sessizce çık

        var infoOp = mgr.GetAppUpdateInfo();
        yield return infoOp;

        if (infoOp.Error != AppUpdateErrorCode.NoError)
        {
            PlayStoreAc(); // Hata → mağazaya yönlendir
            yield break;
        }

        var info = infoOp.GetResult();
        var opts = AppUpdateOptions.ImmediateAppUpdateOptions(false); // immediate akış

        if (info.UpdateAvailability == UpdateAvailability.UpdateAvailable &&
            info.IsUpdateTypeAllowed(opts))
        {
            var updateOp = mgr.StartUpdate(info, opts);
            yield return updateOp;

            // Kullanıcı iptal/hata → mağaza sayfasını aç
            if (updateOp.Error != AppUpdateErrorCode.NoError)
                PlayStoreAc();
        }
        // Güncelleme yoksa oyuna normal devam
    }

    void PlayStoreAc()
    {
        string pkg = Application.identifier; // com.AgalarGamingStudios.AngryInsects
        try { Application.OpenURL("market://details?id=" + pkg); }
        catch { Application.OpenURL("https://play.google.com/store/apps/details?id=" + pkg); }
    }
}
#endif