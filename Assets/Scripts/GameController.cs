using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.IO;

[System.Serializable]
public class Topic
{
    public int id;
    public string subject;
    public string grade;
    public int mastery;
    public string domainid;
    public string domain;
    public string cluster;
    public string standardid;
    public string standarddescription;
}

public static class JsonHelper
{
    private static string fixJson(string value){
        value = "{\"Items\":" + value + "}";
        return value;
    }

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(fixJson(json));
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

public class GameController : MonoBehaviour
{
    public GameObject StoneBlock, WoodenBlock, GlassBlock;
    public AudioSource AssembleSound;
    public AudioSource PullSound;
    private Topic[] topics;
    public List<Topic> t6, t7, t8;
    private List<GameObject> b6, b7, b8;

    void Start(){
        Camera.main.transform.position = new Vector3(0f, 15f, -10f);
        string json = readDataFile();
        topics = JsonHelper.FromJson<Topic>(json);
        SortTopics();
        SplitTopics();
        BuildStack(1);
        BuildStack(2);
        BuildStack(3);
    }

    private string readDataFile()
    {
        string s = Resources.Load<TextAsset>("Data").text;
        return s;
    }

    public void Fetch(){
        t6 = new List<Topic>();
        t7 = new List<Topic>();
        t8 = new List<Topic>();
        Debug.Log(GameObject.Find("URL"));
        Debug.Log(GameObject.Find("URL").GetComponent<TMP_InputField>());
        StartCoroutine(GetRequest(GameObject.Find("URL").GetComponent<TMP_InputField>().text));
    }

    private IEnumerator GetRequest(string uri){
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)){
            yield return webRequest.SendWebRequest();
            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    break;
                case UnityWebRequest.Result.Success:
                    string json = webRequest.downloadHandler.text;
                    topics = JsonHelper.FromJson<Topic>(json);
                    SortTopics();
                    SplitTopics();
                    BuildStack(1);
                    BuildStack(2);
                    BuildStack(3);
                    break;
            }
        }
    }

    private void SortTopics(){
        Array.Sort(topics, (t1, t2) =>
        {
            int result = t1.domain.CompareTo(t2.domain);
            if (result == 0)
            {
                result = t1.cluster.CompareTo(t2.cluster);
                if (result == 0)
                {
                    result = t1.id.CompareTo(t2.id);
                }
            }
            return result;
        });
    }

    private void SplitTopics() { 
        for (int i = 0; i<topics.Length; i++){
            switch (topics[i].grade) {
                       case "6th Grade": t6.Add(topics[i]);
                break; case "7th Grade": t7.Add(topics[i]);
                break; case "8th Grade": t8.Add(topics[i]);
                break;
            }
        }                        
    }

    public void BuildStack(int stack)
    {
        AssembleSound.GetComponent<AudioSource>().Play();
        List<GameObject> b = new List<GameObject>();
        GameObject stackGO = GameObject.Find("Stack " + stack);
        List<Topic> stopics = (stack == 1) ? t6 : (stack == 2) ? t7 : t8;
        destroyStackBlocks(stackGO);
        for (int i = 0; i < stopics.Count; i++)
        {
            GameObject go = AddBlock(stackGO, stopics[i], i);
            b.Add(go);
        }
        if (stack == 1) b6 = b;
        else if (stack == 2) b7 = b;
        else if (stack == 3) b8 = b;
    }

    private void destroyStackBlocks(GameObject stackGO) {
        Transform transform = stackGO.transform;
        while (transform.childCount > 0){
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    private GameObject AddBlock (GameObject sgo, Topic t, int i){
        GameObject go = Instantiate((t.mastery == 0) ? GlassBlock : (t.mastery == 1) ? WoodenBlock : StoneBlock, sgo.transform);
        go.GetComponent<Block>().i = i;
        go.transform.Find("Front Text").GetComponent<TextMeshPro>().text = 
            go.transform.Find("Back Text").GetComponent<TextMeshPro>().text = t.standardid;
        int level = Mathf.FloorToInt(i / 3f);
        Vector3 sp = sgo.transform.position;
        if (level % 2 == 0)
            go.transform.position = new Vector3(sp.x, 
                                                level * 0.8f + 0.4f, 
                                                (i % 3 == 0) ? sp.z - 1.5f:(i % 3 == 1) ? sp.z : sp.z + 1.5f);
        else{
            go.transform.position = new Vector3((i % 3 == 0) ? sp.x - 1.5f : (i % 3 == 1) ? sp.x : sp.x + 1.5f, 
                                                level * 0.8f + 0.4f, 
                                                sp.z);
            go.transform.rotation = Quaternion.Euler(0, 90f, 0);
        }
        go.transform.parent = sgo.transform;
        return go;
    }

    public void PullGlassBlocks(int s){
        PullSound.GetComponent<AudioSource>().Play();
        StartCoroutine(PullGlassBlocksC(s));
    }

    private IEnumerator PullGlassBlocksC(int s){
        List<GameObject> sblocks = (s == 1) ? b6 : (s == 2) ? b7 : b8;
        for (int i = 0; i < sblocks.Count; i++)
        {
            if (sblocks[i] && sblocks[i].name.Contains("Glass"))
            {
                sblocks[i].GetComponent<Block>().push = true;
                yield return new WaitForSeconds(0.3f);

            }
        }
    }
}