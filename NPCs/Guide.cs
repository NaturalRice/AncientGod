using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using AncientGod.System;
using Terraria.Localization;
using Terraria.Utilities;
namespace AncientGod.NPCs
{
    public class Guide : GlobalNPC
    {
        public override void SetStaticDefaults()
        {
            //SetSataicDefaults里面如果对Language类进行过注册，那么相应的值就会出现在Localization/en-US.hjson 和 Localization/zh-Hans.hjson里面
            string text_root = "Mods.AncientGod.Dialogue.Guide.";
            //这里的逻辑是：
            //所有对话文本放在 Mods.AncientGod.Dialogue这个类里面，然后 这个类的子类Guide就是向导的对话
            //具体的对话还要取名字
            //一定要在这里先注册，注册的方法就是下面用get or register（字符串）去注册一个key，然后编辑hjson文件添加对话
            _ = Language.GetOrRegister("Mods.AncientGod.Dialogue.Guide.AfterSaving1");
            _ = Language.GetOrRegister("Mods.AncientGod.Dialogue.Guide.AfterSaving2");
            _ = Language.GetOrRegister("Mods.AncientGod.Dialogue.Guide.AfterSaving3");
            string[] text = { "base","FirstShadow","FirstBoss" };
            foreach(var s in text)
            {
                _ = Language.GetOrRegister(text_root + s + "." + "text1");
                _ = Language.GetOrRegister(text_root + s + "." + "text2");
                _ = Language.GetOrRegister(text_root + s + "." + "text3");
            }

        }
        public override bool? CanChat(NPC npc)
        {
            return true;
        }
        public override void GetChat(NPC npc, ref string chat)//这个就是用来改变对话的函数，具体来说，就是让chat变量等于你想要的字符串。
        {
            string text_root = "Mods.AncientGod.Dialogue.Guide.";
            WeightedRandom<string> Chat = new WeightedRandom<string>();

            if (!GameProgress.FirstShadow)
            {
                //这里就是在读取之前注册的字符串
                Chat.Add(Language.GetText(text_root + "AfterSaving1").Value);
                Chat.Add(Language.GetText(text_root + "AfterSaving3").Value);
                Chat.Add(Language.GetText(text_root + "AfterSaving2").Value);
                chat = Chat;
                return;
            }

            if(GameProgress.FirstShadow)
            {

            }
            chat = Chat;
        }
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return npc.type == NPCID.Guide;
        }
        public override ITownNPCProfile ModifyTownNPCProfile(NPC npc)
        {
            //这两行的意思就是，有一个叫Guide的tag用于保存世界上向导是否存在
            //详见System/ActiveNPC.cs
            if (ActiveNPC.Guide)
            {
                //如果这个tag是真，那么就激活向导NPC
                npc.townNPC = true;
                npc.active = true;
                return null;
            }
            //如果为假，就设置向导不存在
            npc.townNPC = false;
            npc.active = false;
            return base.ModifyTownNPCProfile(npc);
        }
        public override void AI(NPC npc)
        {
        }
    }
}
