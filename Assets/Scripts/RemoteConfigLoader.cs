using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class WeaponConfig
{
    public string id;
    public int damage;
    public float cooldown;
}

[Serializable]
public class WeaponConfigList
{
    public WeaponConfig[] weapons;
}

public class RemoteConfigLoader : MonoBehaviour
{
    public string url = "https://ssakuray.github.io/Unity-Remote-Config/docs/weapons.json";
    public Weapon weapon;
    private string cachePath;

    private IEnumerator Start()
    {
        cachePath = Path.Combine(Application.persistentDataPath, "weapons.json");
        if (weapon == null)
        {
            weapon = GetComponent<Weapon>();
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                if (ApplyConfig(json))
                {
                    try
                    {
                        File.WriteAllText(cachePath, json);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Не удалось сохранить локальную копию: " + e.Message);
                    }
                    yield break;
                }
            }
        }

        try
        {
            if (File.Exists(cachePath) && ApplyConfig(File.ReadAllText(cachePath)))
            {
                yield break;
            }
        }
        catch (Exception)
        {
           
        }

        weapon.Apply(10, 0.5f);
        Debug.LogError("Рабочего конфига нет. Применены дефолты.");
    }

    private bool ApplyConfig(string json)
    {
        try
        {
            WeaponConfigList config = JsonUtility.FromJson<WeaponConfigList>(json);
            if (config == null || config.weapons == null)
            {
                return false;
            }

            WeaponConfig selected = null;
            foreach (WeaponConfig item in config.weapons)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.id) ||
                    item.damage < 0 || !(item.cooldown > 0) || float.IsInfinity(item.cooldown))
                {
                    return false;
                }
                if (item.id == weapon.id)
                {
                    selected = item;
                }
            }
            if (selected == null)
            {
                return false;
            }

            weapon.Apply(selected.damage, selected.cooldown);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
