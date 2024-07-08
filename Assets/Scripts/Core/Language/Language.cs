using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class Language
{
    private string languagefolderpath = $"{Application.dataPath}/Language/";
    [SerializeField] List<LanguageString> localizedText;
    private string langfile;
    public Language(string langfile)
    {
        localizedText = new List<LanguageString>();
        this.langfile = langfile;
        LoadLanguage();
    }

    public string GetString(string id)
    {
        var foundText = localizedText.FirstOrDefault(x => x.id == id);
        if (foundText == null)
        {
            Debug.LogWarning($"Text with id {id} is not localized");
            return id;
        }
        else
        {

        }
        string text = localizedText.Find(x => x.id == id).text;
        return text;
    }

    private void LoadLanguage()
    {
        string filename = $"{langfile}.json";
        string fullPath = Path.Combine(languagefolderpath, filename);

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = File.ReadAllText(fullPath);

                Language loadData = JsonUtility.FromJson<Language>(dataToLoad);
                localizedText = loadData.localizedText;

                Debug.Log($"Language {filename} succesfully loaded");
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred when trying to load language from file: " + fullPath + "\n" + e);
            }
        }
        else
        {
            Debug.Log($"There is no {filename} language file, setting default data");
            WriteDefaultsToDisk();
            SaveLanguage();
        }
    }

    private void SaveLanguage()
    {
        string filename = $"{langfile}.json";
        string fullPath = Path.Combine(languagefolderpath, filename);

        try
        {
            string dataToStore = JsonUtility.ToJson(this, true);
            Debug.Log(fullPath);
            File.WriteAllText(fullPath, dataToStore);
            Debug.Log($"Language {filename} succesfully saved");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error occurred when trying to save language {filename} to file: " + fullPath + "\n" + e);
        }
    }

    public void WriteDefaultsToDisk()
    {
        localizedText.Add(new LanguageString("mainmenu.start", "START GAME"));
        localizedText.Add(new LanguageString("mainmenu.continue", "CONTINUE"));
        localizedText.Add(new LanguageString("mainmenu.options", "OPTIONS"));
        localizedText.Add(new LanguageString("mainmenu.exit", "EXIT"));
    }
}