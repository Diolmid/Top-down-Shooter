using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;

    private Dictionary<string, AudioClip[]> _groupDictionary = new Dictionary<string, AudioClip[]>();

    private void Awake()
    {
        foreach(SoundGroup soundGroup in soundGroups)
            _groupDictionary.Add(soundGroup.groupID, soundGroup.group);  
    }

    public AudioClip GetClipFromName(string name)
    {
        if (_groupDictionary.ContainsKey(name))
        {
            var sounds = _groupDictionary[name];
            return sounds[Random.Range(0, sounds.Length)];
        }

        return null;
    }

    [System.Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip[] group;
    }
}
