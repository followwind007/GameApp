using System.Collections.Generic;
using System.IO;
using GameApp.Serialize;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Tools.Table
{
    [CreateAssetMenu(fileName = "GuideConfigAsset", menuName = "Table/Create GuideConfigAsset")]
    public class GuideConfigAsset : TableScriptableObject
    {
        private const string ViewTip = 
@"MainPanel, //主界面
Store, //抽卡
StorePackInfoPanel, //抽卡信息
ShowDrawCardPanel, //抽卡结果
LadderInfoPanel, //天梯匹配
SeasonPassMainPanel, //季票
Hero, //英雄系统
PanelTroopCollection, //解锁收集
ChessTroopPanel, //卡组
PVEEntryPanel, //试炼PVE入口
HTMapPanel, //试炼
MSMapPanel, //PVE
CardDetailInfoView, //卡牌详情
BattleArenaPanel, //竞技场
BattleOptionPanel, //战斗选项
PanelArenaMain, //竞技场主界面
ChessShopPanel //商店"; 
        
        [EnumValue(EnumSaveType.String)]
        public enum View
        {
            MainPanel, //主界面
            Store, //抽卡
            StorePackInfoPanel, //抽卡信息
            ShowDrawCardPanel, //抽卡结果
            LadderInfoPanel, //天梯匹配
            SeasonPassMainPanel, //季票
            Hero, //英雄系统
            PanelTroopCollection, //解锁收集
            ChessTroopPanel, //卡组
            PVEEntryPanel, //试炼PVE入口
            HTMapPanel, //试炼
            MSMapPanel, //PVE
            CardDetailInfoView, //卡牌详情
            BattleArenaPanel, //竞技场
            BattleOptionPanel, //战斗选项
            PanelArenaMain, //竞技场主界面
            ChessShopPanel //商店
        }

        private const string ModuleTip = 
@"Ladder = 20, //天梯引导1
Ladder2 = 26, //天梯引导2
LadderMatch = 21, //天梯匹配

Store = 30, //抽卡主界面
StoreChoose = 31, //卡包选择
StoreBuy = 32, //卡包购买
StoreGoTroop = 35, //前往组卡

TroopCard1 = 41, //卡组卡牌
TroopCard2 = 42, 
TroopCard3 = 43,
TroopCard4 = 44,
TroopArrow = 4001, //组卡箭头
TroopReturn = 4002, //组卡返回
TroopEditSkill = 4003, //编辑技能

SeasonAwardGet = 55, //季票奖励

CardDetailTip = 60, //卡牌详情

HeroUnlock = 72, //英雄解锁
HeroUnlockCards = 73, //解锁卡牌
HeroUnlockCards2 = 74,
HeroUnlockCards3 = 75,

PVETrain = 80, //试炼入口
PVETrain1 = 81, //试炼第一关
PVESel = 82, //试炼主界面入口
PVEReset = 88, //重置PVE

ArenaSelect = 90, //竞技场选择
ArenaTroop = 91, //竞技场卡组
ArenaTroopClick = 92, //竞技场卡组点击
ArenaReward = 93, //竞技场奖励

Abundance = 111, //放弃竞技场";
        
        [EnumValue(EnumSaveType.Int)]
        public enum Module
        {
            Ladder = 20, //天梯引导1
            Ladder2 = 26, //天梯引导2
            LadderMatch = 21, //天梯匹配

            Store = 30, //抽卡主界面
            StoreChoose = 31, //卡包选择
            StoreBuy = 32, //卡包购买
            StoreGoTroop = 35, //前往组卡
            
            TroopCard1 = 41, //卡组卡牌
            TroopCard2 = 42, 
            TroopCard3 = 43,
            TroopCard4 = 44,
            TroopArrow = 4001, //组卡箭头
            TroopReturn = 4002, //组卡返回
            TroopEditSkill = 4003, //编辑技能

            SeasonAwardGet = 55, //季票奖励

            CardDetailTip = 60, //卡牌详情
            
            HeroUnlock = 72, //英雄解锁
            HeroUnlockCards = 73, //解锁卡牌
            HeroUnlockCards2 = 74,
            HeroUnlockCards3 = 75,

            PVETrain = 80, //试炼入口
            PVETrain1 = 81, //试炼第一关
            PVESel = 82, //试炼主界面入口
            PVEReset = 88, //重置PVE

            ArenaSelect = 90, //竞技场选择
            ArenaTroop = 91, //竞技场卡组
            ArenaTroopClick = 92, //竞技场卡组点击
            ArenaReward = 93, //竞技场奖励

            Abundance = 111, //放弃竞技场
        }
        private const string FlagTip = 
@"SeasonAward = 5005, //季票奖励
HeroUnlockClick = 7003, //英雄解锁
HeroUnlockCardsClick = 7004, //英雄解锁卡牌点击
StoreGoTroopClick = 2006, //点击前往组卡
TroopDragDone = 3002, //组卡拖拽结束
PVETrainStarted = 4009, //试炼开始
CardDetailInfoShown = 6003, //卡牌详情已展示";
        
        [EnumValue(EnumSaveType.Int)]
        public enum Flag
        {
            SeasonAward = 5005, //季票奖励
            HeroUnlockClick = 7003, //英雄解锁
            HeroUnlockCardsClick = 7004, //英雄解锁卡牌点击
            StoreGoTroopClick = 2006, //点击前往组卡
            TroopDragDone = 3002, //组卡拖拽结束
            PVETrainStarted = 4009, //试炼开始
            CardDetailInfoShown = 6003, //卡牌详情已展示
        }
        
        private const string SysTip = 
@"";
        
        [EnumValue(EnumSaveType.Int)]
        public enum Sys
        {
            Season = 1001,
            Shop = 1002,
            Activity = 1003,
            Mail = 1004,
            Friend = 1005,
            Chat = 1006,
            Arena = 1007,
            PVE = 1008,
            LadderUpgrade = 1009,
            LadderRank = 1010,
            MLadderBox = 1011,
            LLadderBox = 1012,
        }

        [System.Serializable][CustomDesc]
        public class Config
        {
            [CustomName("模块"), Tooltip(ModuleTip)]
            public Module module;
            [CustomName("界面"), Tooltip(ViewTip)]
            public View view;
            [CustomName("强制")]
            public bool force;
            [CustomName("标志"), NotExportEmpty, Tooltip(FlagTip)]
            public List<Flag> flags;
            [CustomName("禁用标志"), NotExportEmpty, Tooltip(FlagTip)]
            public List<Flag> notFlags;
            [CustomName("等级"), NotExportEmpty]
            public List<int> level;
            [CustomName("任意等级"), NotExportEmpty]
            public List<int> anyLevel;
            [CustomName("完成模块"), NotExportEmpty, Tooltip(ModuleTip)]
            public List<Module> mDone;
            [CustomName("全屏点击")]
            public bool clickPass;
            [CustomName("系统解锁"), NotExportEmpty, Tooltip(SysTip)]
            public List<Sys> sysUnlock;
            [CustomName("手动触发")]
            public bool manual;
            [CustomName("仅显示一次")]
            public bool showOnce;
            [CustomName("显示直至通过")]
            public bool untilPass;
            [CustomName("提示")]
            public string text;

            public override string ToString()
            {
                return $"界面:{view} 模块:{module} 强制:{force}";
            }
        }
        
        [System.Serializable]
        public class Configs
        {
            [ListItem(useSearch = true), CustomName("引导配置")]
            public List<Config> guides;
        }

        public string configPath = "Assets/GuideConfigs.lua";
        public Configs configs;

        [ExposedFunc(displayName = "保存Lua", priority = 101)]
        public void SaveToLua()
        {
            var lua = configs?.EncodeToLua();
            File.WriteAllText(configPath, $"local content = {lua}\nreturn content");
            Debug.Log($"save config to: {configPath}");
        }
    }
}