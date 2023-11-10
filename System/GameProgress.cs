using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AncientGod.System
{
    // Acts as a container for "downed boss" flags.
    // Set a flag like this in your bosses OnKill hook:
    //    NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

    // Saving and loading these flags requires TagCompounds, a guide exists on the wiki: https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
    public class GameProgress : ModSystem
    {
        //简单来说，tmodloader里面的额外世界数据都是由一个又一个的tag来保存的。
        //数据结构类似于一个字典：
        // tag ： 数据
        // public static bool downedOtherBoss = false;

        //游戏进度用一个又一个布尔值来表示，比如firstshadow变量保存玩家有没有造出第一个影子来
        //这里默认是false
        public static bool FirstShadow = false;
        public override void ClearWorld()//这个函数保证生成世界时，初始值是啥样子
        {
            FirstShadow = false;
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.
        public override void SaveWorldData(TagCompound tag)//存档的时候往tag里面写数据
        {
            if (FirstShadow)
            {
                tag["FirstShadow"] = FirstShadow;//往tag里面写数据
            }
            // if (downedOtherBoss) {
            //	tag["downedOtherBoss"] = true;
            // }
        }

        public override void LoadWorldData(TagCompound tag)
        {//读档的时候从tag里读取数据
            FirstShadow = tag.ContainsKey("FirstShadow");
            // downedOtherBoss = tag.ContainsKey("downedOtherBoss");
        }


        //下面两个是网络相关的代码，但是我们目前的mod还不支持联机，所以先不用管，我全注释掉了
        public override void NetSend(BinaryWriter writer)
        {
            // Order of operations is important and has to match that of NetReceive
            //var flags = new BitsByte();
            //
            //flags[0] = FirstShadow;
            // flags[1] = downedOtherBoss;
            //writer.Write(flags);

            /*
			Remember that Bytes/BitsByte only have up to 8 entries. If you have more than 8 flags you want to sync, use multiple BitsByte:
				This is wrong:
			flags[8] = downed9thBoss; // an index of 8 is nonsense.
				This is correct:
			flags[7] = downed8thBoss;
			writer.Write(flags);
			BitsByte flags2 = new BitsByte(); // create another BitsByte
			flags2[0] = downed9thBoss; // start again from 0
			// up to 7 more flags here
			writer.Write(flags2); // write this byte
			*/

            // If you prefer, you can use the BitsByte constructor approach as well.
            // BitsByte flags = new BitsByte(downedMinionBoss, downedOtherBoss);
            // writer.Write(flags);

            // This is another way to do the same thing, but with bitmasks and the bitwise OR assignment operator (the |=)
            // Note that 1 and 2 here are bit masks. The next values in the pattern are 4,8,16,32,64,128. If you require more than 8 flags, make another byte.
            // byte flags = 0;
            // if (downedMinionBoss)
            // {
            //	flags |= 1;
            // }
            // if (downedOtherBoss)
            // {
            //	flags |= 2;
            // }
            // writer.Write(flags);

            // If you plan on having more than 8 of these flags and don't want to use multiple BitsByte, an alternative is using a System.Collections.BitArray
            /*
			bool[] flags = new bool[] {
				downedMinionBoss,
				downedOtherBoss,
			};
			BitArray bitArray = new BitArray(flags);
			byte[] bytes = new byte[(bitArray.Length - 1) / 8 + 1]; // Calculation for correct length of the byte array
			bitArray.CopyTo(bytes, 0);

			writer.Write(bytes.Length);
			writer.Write(bytes);
			*/
        }

        public override void NetReceive(BinaryReader reader)
        {
            // Order of operations is important and has to match that of NetSend
            //BitsByte flags = reader.ReadByte();
            //FirstShadow = flags[0];
            // downedOtherBoss = flags[1];

            // As mentioned in NetSend, BitBytes can contain up to 8 values. If you have more, be sure to read the additional data:
            // BitsByte flags2 = reader.ReadByte();
            // downed9thBoss = flags2[0];

            // System.Collections.BitArray approach:
            /*
			int length = reader.ReadInt32();
			byte[] bytes = reader.ReadBytes(length);

			BitArray bitArray = new BitArray(bytes);
			downedMinionBoss = bitArray[0];
			downedOtherBoss = bitArray[1];
			*/
        }
    }
}
