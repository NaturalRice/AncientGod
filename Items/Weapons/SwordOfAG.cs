using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AncientGod.Items.Weapons
{
    public class SwordOfAG : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.AncientGod.hjson file.

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 1;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()//������,������ʱע���
        {
            Recipe recipe = CreateRecipe();//����һ���䷽
            recipe.AddIngredient(ItemID.Torch, 1);//������ϣ�1��ѣ�
            recipe.AddTile(TileID.Campfire);//����ϳ�վ������Ϊ����Ȥ�Ҹĳ�������
            recipe.Register();
        }
    }
}