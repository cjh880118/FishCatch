using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JHchoi.UI.Event
{
    public class LoadImageChangeMsg : Message
    {
    }
}

public class ImageChange : MonoBehaviour
{
    public Image image;
    public List<Sprite> imageList = new List<Sprite>();

    private void Awake()
    {
        Message.AddListener<JHchoi.UI.Event.LoadImageChangeMsg>(Change);
    }

    private void OnDestroy()
    {
        Message.RemoveListener<JHchoi.UI.Event.LoadImageChangeMsg>(Change);
    }

    void Change(JHchoi.UI.Event.LoadImageChangeMsg msg)
    {
        image.sprite = imageList[(int)Random.Range(0, imageList.Count)];
    }
}
