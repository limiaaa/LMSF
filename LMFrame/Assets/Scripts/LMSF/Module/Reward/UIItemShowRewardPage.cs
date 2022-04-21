//using DG.Tweening;
//using SG.EventSystem.EventDispatcher;
//using SG.UI;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class RewardItem
//{
//    public GameObject obj;
//    public Text textRoot;
//    public Image Img;
//    public string ItemName="";
//    public string ItemType="";
//    public int ItemId=1;
//    public int ItemNumber =0;
//}



//[UIResPath(Global.PFB_UI_ROOT_PATH + "UI_ItemShowReward.prefab", PageType.PopBox)]
//public class UIItemShowRewardPage : ABaseUI, IMsgSender
//{
//    [UIBinder("Tip")]
//    Text Tip;
//    [UIBinder("ShowBox")]
//    Transform ShowBox;
//    [UIBinder("RewardItemBg")]
//    Transform RewardItemBg;
//    [UIBinder("Mask")]
//    Image Mask;

//    Button ThisButton;
//    Action ShowFunc;
//    Action ClickFunc;
//    //处理后的奖励数据列表
//    List<RewardItem> RewardItemList = new List<RewardItem>();

//    Vector3 StartFlyCoinPos;
//    Vector3 EndFlyCoinPos;
//    string ActName;
//    protected override void OnInit(params object[] parmas)
//    {
//        base.OnInit(parmas);
//        InitButton();
//        InitUI();
//    }
//    void InitButton()
//    {
//        ThisButton = this.transform.GetComponent<Button>();
//        ThisButton.onClick.AddListener(() =>
//        {
//            ThisButton.enabled = false;
//            ClickShow();
//        });
//    }
//    void InitUI() { 
//        InitMaskAndTipText();
//    }
//    void InitMaskAndTipText()
//    {
//        var MaskColor = Mask.color;
//        MaskColor = new Color(MaskColor.r, MaskColor.g, MaskColor.b, 0);
//        Mask.color = MaskColor;
//        var TipColor = Tip.color;
//        TipColor = new Color(TipColor.r, TipColor.g, TipColor.b, 0);
//        Tip.color = TipColor;
//        ShowBox.localScale = Vector3.zero;
//        RewardItemBg.localScale = Vector3.zero;
//    }
//    float MaskComeTime;
//    float RewardPopTime;
//    public void StartShowItemReward(string[] itemList, Action showFunc, Action clickFunc, string actName, Vector3 StartFlyPos, Vector3 EndFlyPos, float maskComeTime = 0.8f, float rewardPopTime = 3.2f)
//    {
//        ShowFunc = showFunc;
//        MaskComeTime = maskComeTime;
//        RewardPopTime = rewardPopTime;
//        ClickFunc = clickFunc;
//        StartFlyCoinPos = StartFlyPos;
//        EndFlyCoinPos = EndFlyPos;
//        ActName = actName;
//        if (StartFlyCoinPos == Vector3.zero)
//            StartFlyCoinPos = this.transform.position;
//        if (EndFlyCoinPos == Vector3.zero)
//            EndFlyCoinPos = UITopManager.Instance.GetTopCoinText().transform.position;
//        InitData(itemList);
//    }


//    List<Vector3> posList = new List<Vector3>();
//    void mathPos(int Length)
//    {
//        posList.Clear();
//        if (Length <= 4)
//        {
//            if (Length == 1)
//            {
//                posList.Add(new Vector3(0, 0, 0));
//            }
//            else if (Length == 2)
//            {
//                posList.Add(new Vector3(-95, 0, 0));
//                posList.Add(new Vector3(95, 0, 0));
//            }
//            else if (Length == 3)
//            {
//                posList.Add(new Vector3(-190, 0, 0));
//                posList.Add(new Vector3(0, 0, 0));
//                posList.Add(new Vector3(190, 0, 0));
//            }
//            else if (Length == 4)
//            {
//                posList.Add(new Vector3(-285, 0, 0));
//                posList.Add(new Vector3(-95, 0, 0));
//                posList.Add(new Vector3(95, 0, 0));
//                posList.Add(new Vector3(285, 0, 0));
//            }
//        }
//        else
//        {
//            posList.Add(new Vector3(-285,100,0));
//            posList.Add(new Vector3(-95,100,0));
//            posList.Add(new Vector3(95,100,0));
//            posList.Add(new Vector3(285,100,0));
//            if (Length == 5)
//            {
//                posList.Add(new Vector3(0,-100,0));
//            }
//            else if (Length == 6)
//            {
//                posList.Add(new Vector3(-95,-100,0));
//                posList.Add(new Vector3(95,-100,0));
//            }
//            else if (Length == 7)
//            {
//                posList.Add(new Vector3(-190,-100,0));
//                posList.Add(new Vector3(0,-100,0));
//                posList.Add(new Vector3(190,-100,0));
//            }
//            else if (Length == 8)
//            {
//                posList.Add(new Vector3(-285,-100,0));
//                posList.Add(new Vector3(-95,-100,0));
//                posList.Add(new Vector3(95,-100,0));
//                posList.Add(new Vector3(285,-100,0));
//            }
//        }
//    }
//    private void InitData(string[] itemList)
//    {
//        RewardItemList.Clear();
//        mathPos(itemList.Length);
//        for (int i = 0; i <= itemList.Length-1; i++)
//        {
//            Debug.Log("Show Reward__" + itemList[i]);
//            string[] ItemInfo = itemList[i].Split('|');
//            int itemId = int.Parse(ItemInfo[0]);
//            int itemNum = int.Parse(ItemInfo[1]);
//            if (itemId != 0)
//            {
//                RewardItem rewarditem = new RewardItem();
//                rewarditem.obj = ShowBox.GetChild(i).gameObject;
//                rewarditem.textRoot = ShowBox.GetChild(i).Find("Text").GetComponent<Text>();
//                rewarditem.Img = ShowBox.GetChild(i).GetComponent<Image>();
//                rewarditem.obj.gameObject.SetActive(false);
//                rewarditem.obj.transform.localPosition= posList[i];
//                rewarditem.ItemId = itemId;
//                rewarditem.ItemNumber = itemNum;
//                rewarditem.ItemName = ItemManager.Instance.GetItemNameById(itemId);
//                rewarditem.ItemType = ItemManager.Instance.GetItemData(itemId).Type;
                
//                RewardItemList.Add(rewarditem);
//            }
//        }
//        InitItemUI();
//        StartShowAni();
//    }
//    private void InitItemUI()
//    {
//        foreach(var item in RewardItemList)
//        {
//            item.Img.sprite = GetSpriteByName(item);
//            item.textRoot.text = "+" + item.ItemNumber;
//            item.obj.gameObject.SetActive(true);
//        }
//    }
//    void StartShowAni()
//    {
//        ThisButton.enabled = false;
//        SoundMgr.Instance.PlaySoundEffect(SoundTypes.resource_get.ToString());
//        GetHowDayBonus(RewardItemList);
//        ShowFunc?.Invoke();
//        DoTweenManager.Instance.DoFade(Mask, 0.9f, 0.2f);
//        ShowRewardBoxAndTip(1f);
//    }

//    void ShowRewardBoxAndTip(float time)
//    {
//        ShowBox.gameObject.SetActive(true);
//        RewardItemBg.gameObject.SetActive(true);
//        ShowBox.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
//        RewardItemBg.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
//        DelayTimeManager.Instance.delay_time_run(time, () =>
//        {
//            ThisButton.enabled = true;
//            DoTweenManager.Instance.DoFade(Tip, 1f, 0.2f);
//        });
//    }
//    Text RunText=null;
//    public void SetRunText(Text text)
//    {
//        RunText = text;
//    }
//    void ClickShow()
//    {
//        bool HasCoinReward = false;
//        foreach(var item in RewardItemList)
//        {
//            if (item.ItemName == "Coin")
//            {
//                HasCoinReward = true;
//                break;
//            }
//        }
//        if (HasCoinReward)
//        {
//            FlyCoinManager.Instance.FlyCoinWithRunNumber(StartFlyCoinPos,EndFlyCoinPos, 10, coinNumber, RunText, ()=> {
//                RunText = null;
//            });
//        }
//        if (ClickFunc != null)
//        {
//            ClickFunc.Invoke();
//        }
//        this.SendMsg(GameEventType.RefeshCoin, null);
//        CloseSelf(true);
//    }
   
//    //奖励发放
//    int coinNumber = 0;
//    public void GetHowDayBonus(List<RewardItem> rewardItemList)
//    {
//        foreach (var item in rewardItemList)
//        {
//            Debug.Log("Reward Id__" + item.ItemId+"__"+item.ItemName + "__" + item.ItemNumber);
//            switch (item.ItemName)
//            {
//                case "Coin":
//                    coinNumber += item.ItemNumber;
//                    ItemManager.Instance.AddItemNum(GameItemType.Coin, item.ItemNumber, ActName);
//                    break;
//                case "Star":
//                    ItemManager.Instance.AddItemNum(GameItemType.Star, item.ItemNumber, ActName);
//                    break;
//                case "Grenade"://手榴弹(全屏)
//                    ItemManager.Instance.AddItemNum(GameItemType.Grenade, item.ItemNumber, ActName);
//                    break;
//                case "AddBalls"://加球
//                    ItemManager.Instance.AddItemNum(GameItemType.AddBalls, item.ItemNumber, ActName);
//                    break;
//                case "Hammer"://锤子3*3
//                    ItemManager.Instance.AddItemNum(GameItemType.Hammer, item.ItemNumber, ActName);
//                    break;
//                case "Boxing"://拳套横向
//                    ItemManager.Instance.AddItemNum(GameItemType.Boxing, item.ItemNumber, ActName);
//                    break;
//                case "Weight"://秤砣竖向
//                    ItemManager.Instance.AddItemNum(GameItemType.Weight, item.ItemNumber, ActName);
//                    break;
//                default:
//                    break;
//            }

//            if (item.ItemId>=200&item.ItemId<300)
//            {
//                SkinManager.Instance.GetNewSkinById(item.ItemId);
//            }
//        }
//        ItemManager.Instance.LookItem();
//    }
    







//    Sprite GetSpriteByName(RewardItem item)
//    {
//        if (item.ItemType == "BallSkin")
//        {
//            return SkinManager.Instance.GetSpriteByKey(item.ItemId);
//        }
//        else
//        {
//            return ItemManager.Instance.GetItemSpriteByKey(item.ItemName);
//        }
//    }
//    protected override void OnOpenPage(params object[] parmas)
//    {
//        base.OnOpenPage(parmas);
//    }
//    public override void OnDestroy()
//    {
//        base.OnDestroy();
//    }

//    private void OnDisable()
//    {

//    }
//}
