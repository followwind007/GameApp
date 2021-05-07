using System.Collections.Generic;
using System.IO;
using GameApp.Serialize;
using UnityEngine;

namespace Tools.Table
{
    [CreateAssetMenu(fileName = "ChessPlayerAsset", menuName = "Table/Create ChessPlayerAsset")]
    public class ChessPlayerAsset : TableScriptableObject
    {
        public enum OperationType
        {
            None = 0,
            Reset = 1,
            Add = 2,
            Minus = 3
        }
        
        [System.Serializable][CustomDesc]
        public class RawCardInfo
        {
            public int id;
            public int atk = -1;
            public int armor = -1;
            public int star = -1;
            public override string ToString()
            {
                return $"id:{id} atk:{atk} armor:{armor} star:{star}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class CardInfo
        {
            public int id;
            public int atk = -1;
            public int armor = -1;
            public int star = -1;

            public SkillInfo skill;
            
            [ListItem(useSearch = true)]
            public List<RawCardInfo> equip;
            public override string ToString()
            {
                return $"id:{id} atk:{atk} armor:{armor} star:{star} equip:{equip.Count}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class SkillInfo
        {
            [CustomName("技能操作")]
            public OperationType skillOp = OperationType.None;
            [CustomName("技能列表")]
            public List<int> skills = new List<int>();

            public override string ToString()
            {
                var strs = "";
                foreach (var sk in skills)
                {
                    strs += $"{sk} ";
                }
                return $"{skillOp}: {strs}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class HeroInfo
        {
            public int id;
            [ListItem(useSearch = true)]
            public List<int> skills;
            public override string ToString()
            {
                return $"id:{id} skills:{skills.Count}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class TroopCard
        {
            [CustomName("卡牌id")]
            public int id;
            [CustomName("卡牌数量")]
            public int num;
            public override string ToString()
            {
                return $"[{id}:{num}]";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class TroopInfo
        {
            [CustomName("技能发现回合")]
            public int discoverSkillRound;
            [CustomName("英雄信息"), ListItem(useSearch = true)]
            public List<HeroInfo> heros;
            [CustomName("卡背")]
            public int cardBack;
            [ListItem, CustomName("卡组卡牌")]
            public List<TroopCard> cards;

            public override string ToString()
            {
                return $"discoverRound:{discoverSkillRound} heros:{heros} cards:{cards.Count}";
            }
        }

        [System.Serializable][CustomDesc]
        public class MatchInfo
        {
            public int round;
            public int opponentIndex;
            public override string ToString()
            {
                return $"round:{round} opponent:{opponentIndex}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class RoundAttack
        {
            [System.Serializable][CustomDesc]
            public class AttackInfo
            {
                public int playerIndex;
                public int attackerId;
                public int targetId;
                public int fightType;

                public override string ToString()
                {
                    return $"player:{playerIndex} attacker:{attackerId} target:{targetId} fightType:{fightType}";
                }
            }
            
            public int round;
            [ListItem(useSearch = true)]
            public List<AttackInfo> attackInfos;

            public override string ToString()
            {
                return $"round:{round} infos:{attackInfos.Count}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class CardGroup
        {
            [CustomName("刷出卡牌数量")]
            public int count;
            [ListItem(useSearch = true)][CustomName("卡牌列表")]
            public List<int> cards;

            public override string ToString()
            {
                return $"count:{count} cards:{cards.Count}";
            }
        }

        [System.Serializable][CustomDesc]
        public class ShopInfo
        {
            [CustomName("刷出卡牌使用卡池")]
            public int dropId;

            [CustomName("禁止商店刷出卡牌")]
            public bool disableShop;

            [CustomName("刷出卡牌数量")]
            public int count;
            
            [ListItem(useSearch = true)][CustomName("商店刷出设置")]
            public List<CardGroup> groups;

            public override string ToString()
            {
                return $"groups:{groups.Count}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class RoundInfo
        {
            public int round;
            [TabItem("战场")][CustomName("重置")]
            public bool resetBoard;
            [TabItem("战场")][ListItem(useSearch = true)][CustomName("列表")]
            public List<CardInfo> board;
            
            [TabItem("手牌")][CustomName("操作")]
            public OperationType handOp;
            [TabItem("手牌")][ListItem(useSearch = true)][CustomName("列表")]
            public List<CardInfo> hand;

            [TabItem("陷阱")][CustomName("重置")]
            public bool resetTrap;
            [TabItem("陷阱")][ListItem(useSearch = true)][CustomName("列表")]
            public List<RawCardInfo> trap;

            [TabItem("商店")][CustomName("重置")]
            public bool resetShop;
            [TabItem("商店")][CustomName("详情")]
            public ShopInfo shop;

            [TabItem("玩家")][LineItem("金币")]
            public bool resetGold;
            [TabItem("玩家")][LineItem("金币")]
            public int gold;

            [TabItem("玩家")][LineItem("科技等级")]
            public bool resetTech;
            [TabItem("玩家")][LineItem("科技等级")]
            public int tech;

            [TabItem("玩家")][LineItem("科技点")]
            public bool resetTechPoint;
            [TabItem("玩家")][LineItem("科技点")]
            public int techPoint;

            [TabItem("玩家")][LineItem("最大金币")]
            public bool resetMaxGold;
            [TabItem("玩家")][LineItem("最大金币")]
            public int maxGold;

            [TabItem("玩家")][LineItem("HP")]
            public bool resetHp;
            [TabItem("玩家")][LineItem("HP")]
            public int hp;

            [TabItem("玩家")][LineItem("最大HP")]
            public bool resetMaxHp;
            [TabItem("玩家")][LineItem("最大HP")]
            public int maxHp;

            [TabItem("玩家")]
            public SkillInfo skill;

            public override string ToString()
            {
                return $"round:{round} hp:{hp} gold:{gold} board:{board.Count}";
            }
        }
        
        [System.Serializable][CustomDesc(showIndex = true, indexBase = 1)]
        public class PlayerInfo
        {
            [CustomName("备注")]
            public string tip;
            [CustomName("Skin Id")]
            public int id;
            [CustomName("卡组")]
            public TroopInfo troop;
            [CustomName("玩家配置")]
            public PlayerEnv env;
            [CustomName("行为树")]
            public List<string> behaviourTrees;
            [CustomName("回合信息")][ListItem(useSearch = true)]
            public List<RoundInfo> infos;
            
            public void ShowDetail()
            {
                Debug.Log(this);
            }
            
            public override string ToString()
            {
                return $"id:{id} infos:{infos.Count}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class DropInfo
        {
            public int id;
            [ListItem(useSearch = true)]
            public List<int> dropCards;

            public override string ToString()
            {
                return $"id:{id} dropCards:{dropCards.Count}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class RoundDrop
        {
            public int round;
            public int dropId;
            public override string ToString()
            {
                return $"round:{round} dropId:{dropId}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class RoundDrops
        {
            [ListItem(useSearch = true)]
            public List<DropInfo> drops;
            [ListItem(useSearch = true)]
            public List<RoundDrop> rounds;

            public override string ToString()
            {
                return $"drops:{drops.Count} rounds:{rounds.Count}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class CardPair
        {
            public int card1;
            public int card2;
            public override string ToString()
            {
                return $"card1:{card1} card2:{card2}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class RoundDiscover
        {
            public int round;
            [ListItem(useSearch = true)]
            public List<CardPair> cardGroups;

            public override string ToString()
            {
                return $"round:{round} groups:{cardGroups.Count}";
            }
        }
        
        [System.Serializable][CustomDesc]
        public class ChessEnv
        {
            [CustomName("升级消耗(长度必须是5)")]
            public List<int> upgradeCost;

            [CustomName("NPC跳过战斗阶段")]
            public bool ignoreNpcBattle;

            [CustomName("禁用公共卡池(仅使用卡组内卡牌)")]
            public bool ignoreCommonPool;

            public override string ToString()
            {
                return $"costs:{upgradeCost.Count}";
            }
        }
        
        [System.Serializable, CustomDesc]
        public class PlayerEnv
        {
            [CustomName("禁用公共卡池(仅使用卡组内卡牌)")]
            public bool ignoreCommonPool;
        }

        [System.Serializable]
        public class PlayerInfos
        {
            public int stageId;
            public string bgm;
            [ListItem(useSearch = true)][CustomName("行为树")]
            public List<string> behaviourTrees;
            [TabItem("玩家信息")][ListItem(useSearch = true)]
            public List<PlayerInfo> players;
            [TabItem("我方回合")][ListItem(useSearch = true)]
            public List<RoundInfo> myList;
            [TabItem("对手回合")][ListItem(useSearch = true)]
            public List<RoundInfo> opponentList;
            [TabItem("回合掉落")][ListItem(useSearch = true)]
            public RoundDrops roundDrops;
            [TabItem("回合发现")][ListItem(useSearch = true)]
            public List<RoundDiscover> roundDiscovers;
            [TabItem("配置信息"), ListItem(useSearch = true), CustomName("匹配")]
            public List<MatchInfo> matchInfos;
            [TabItem("配置信息"), CustomName("全局配置")]
            public ChessEnv env;
            [TabItem("其他")][ListItem(useSearch = true)]
            public List<RoundAttack> roundAttacks;
            [TabItem("其他")][ListItem(useSearch = true)]
            public List<DropInfo> dropPools;
            [TabItem("其他")][CustomName("关卡流程类型")]
            public int processType;
            [TabItem("其他")][CustomName("使用玩家信息")]
            public bool usePlayerInfo;
            [TabItem("其他")][CustomName("自定义加载界面")]
            public int customLoading;
        }

        public PlayerInfos playerInfos;

        private string ConfigPath => $"Assets/Lua/app/RookieConfig/{playerInfos.stageId}.lua";
        
        [ExposedFunc(displayName = "保存Lua", priority = 101)]
        public void SaveToLua()
        {
            var lua = playerInfos?.EncodeToLua();
            File.WriteAllText(ConfigPath, $"local content = {lua}\nreturn content");
            Debug.Log($"save config to: {ConfigPath}");
        }

        [ExposedFunc(displayName = "删除Lua", priority = 102)]
        public void DeleteLua()
        {
            File.Delete(ConfigPath);
            Debug.Log($"delete config from: {ConfigPath}");
        }

    }
}