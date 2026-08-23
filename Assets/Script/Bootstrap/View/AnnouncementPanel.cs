using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameUI
{
    public class AnnouncementPanel : CommonPopUpPanel
    {
        [SerializeField]
        private Transform annoucementAnchor;
        [SerializeField]
        private OneAnnouncement oneAnnoucement;
        [SerializeField]
        private Text currentContent;
        [SerializeField]
        private GameObject emptyPart;

        //private List<AnnouncementConfig> announcements = new List<AnnouncementConfig>();

        private void Awake()
        {
            emptyPart.SetActive(true);
            Show();
        }

        private void OnSelectAnnouncement(string content)
        {
            this.currentContent.text = content;
        }

        public override void Hide()
        {
            tweenAlpha.SetOnCompleteCB(() => Destroy(this.gameObject));
            base.Hide();
        }
    }
}